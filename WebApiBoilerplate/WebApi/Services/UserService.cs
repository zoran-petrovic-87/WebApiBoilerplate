using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApi.Data;
using WebApi.DataTransferObjects.User;
using WebApi.Helpers;
using WebApi.Helpers.Exceptions;
using WebApi.Helpers.Pagination;
using WebApi.IServices;
using WebApi.Models;
using WebApi.Resources.Localization;

namespace WebApi.Services
{
    /// <summary>
    /// The user service.
    /// </summary>
    /// <seealso cref="WebApi.IServices.IUserService" />
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly AppDbContext _db;
        private readonly AppSettings _appSettings;
        private readonly IStringLocalizer<Translation> _l;
        private readonly IStringLocalizer<LocalizedResource> _lr;
        private readonly IEmailService _emailService;
        private readonly IPasswordHelper _passwordHelper;
        private readonly EmbeddedFileProvider _embedded;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="db">The database context.</param>
        /// <param name="appSettings">The application settings.</param>
        /// <param name="l">The string localizer.</param>
        /// <param name="lr">The resource localizer.</param>
        /// <param name="emailService">The email service.</param>
        /// <param name="passwordHelper">The password helper.</param>
        public UserService(ILogger<UserService> logger, AppDbContext db, IOptions<AppSettings> appSettings,
            IStringLocalizer<Translation> l, IStringLocalizer<LocalizedResource> lr, IEmailService emailService,
            IPasswordHelper passwordHelper)
        {
            _logger = logger;
            _db = db;
            _appSettings = appSettings.Value;
            _l = l;
            _lr = lr;
            _emailService = emailService;
            _passwordHelper = passwordHelper;
            _embedded = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
        }

        /// <inheritdoc />
        public async Task<AuthenticateResponseDto> Authenticate(AuthenticateRequestDto dto)
        {
            var user = await _db.Users.SingleOrDefaultAsync(x => x.Username == dto.Username && x.IsActive);

            // Check if the username exists.
            if (user == null) throw new EntityNotFoundException(_l["Username is incorrect."]);

            // Check for too many failed login attempts.
            if (user.LoginFailedAt != null)
            {
                var secondsPassed = DateTime.UtcNow.Subtract(user.LoginFailedAt.GetValueOrDefault()).Seconds;

                var isMaxCountExceeded = user.LoginFailedCount >= _appSettings.MaxLoginFailedCount;
                var isWaitingTimePassed = secondsPassed > _appSettings.LoginFailedWaitingTime;

                if (isMaxCountExceeded && !isWaitingTimePassed)
                {
                    var secondsToWait = _appSettings.LoginFailedWaitingTime - secondsPassed;
                    throw new TooManyFailedLoginAttemptsException(string.Format(
                        _l["You must wait for {0} seconds before you try to log in again."], secondsToWait));
                }
            }

            // Check if password is correct.
            if (!_passwordHelper.VerifyHash(dto.Password, user.PasswordHash, user.PasswordSalt))
            {
                user.LoginFailedCount += 1;
                user.LoginFailedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                throw new IncorrectPasswordException(_l["Password is incorrect."]);
            }

            // Authentication successful.
            user.LoginFailedCount = 0;
            user.LoginFailedAt = null;
            user.LastLoginAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            // Create token.
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {new Claim(ClaimTypes.Name, user.Id.ToString())}),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return new AuthenticateResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Token = tokenString
            };
        }

        /// <inheritdoc />
        public async Task<UserDetailsResponseDto> Create(UserCreateRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new InvalidPasswordException(_l["Password is required."]);

            var existingUser = await _db.Users.FirstOrDefaultAsync(
                x => x.Username == dto.Username || x.Email == dto.Email);

            if (existingUser?.Username == dto.Username)
                throw new UsernameTakenException(string.Format(_l["Username '{0}' is already taken."], dto.Username));

            if (existingUser?.Email == dto.Email)
                throw new EmailTakenException(string.Format(_l["Email '{0}' is already taken."], dto.Email));

            var (passwordHash, passwordSalt) = _passwordHelper.CreateHash(dto.Password);

            var user = dto.CreateUser();
            user.Email = null; // Email must be confirmed first.
            user.IsActive = false;
            var emailSuccess = await ChangeEmailAsync(user, dto.Email);
            user.CreatedAt = DateTime.UtcNow;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            if (!emailSuccess) throw new EmailNotSentException(_l["Sending of confirmation email failed."]);

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();

            return new UserDetailsResponseDto(user);
        }

        /// <inheritdoc />
        public async Task<PagedResult<UserSummaryResponseDto>> GetAll(PaginationFilter paginationFilter)
        {
            return await _db.Users.AsNoTracking()
                .Where(x => x.IsActive)
                .Select(x => new UserSummaryResponseDto(x))
                .GetPagedAsync(paginationFilter);
        }

        /// <inheritdoc />
        public async Task<UserDetailsResponseDto> GetById(Guid id)
        {
            var user = await _db.Users.AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new UserDetailsResponseDto(x))
                .FirstOrDefaultAsync();
            if (user == null) throw new EntityNotFoundException(_l["User not found."]);
            return user;
        }

        /// <inheritdoc />
        public async Task<UserDetailsResponseDto> Update(Guid requestedByUserId, Guid userId, UserUpdateRequestDto dto)
        {
            if (userId != requestedByUserId) throw new ForbiddenException();

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null) throw new EntityNotFoundException(_l["User not found."]);

            // Update username if it has changed.
            if (dto.Username != null && dto.Username.NewValue != user.Username)
            {
                // Throw error if the new username is already taken.
                if (_db.Users.Any(x => x.Username == dto.Username.NewValue))
                    throw new UsernameTakenException(string.Format(
                        _l["Username '{0}' is already taken."], dto.Username.NewValue));
                user.Username = dto.Username.NewValue;
            }

            // Update user properties if provided.
            if (dto.FirstName != null) user.FirstName = dto.FirstName.NewValue;
            if (dto.LastName != null) user.LastName = dto.LastName.NewValue;

            // Update password if provided.
            if (dto.Password != null)
            {
                var (passwordHash, passwordSalt) = _passwordHelper.CreateHash(dto.Password.NewValue);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            // Update email if provided.
            if (dto.Email != null)
            {
                var emailSuccess = await ChangeEmailAsync(user, dto.Email.NewValue);
                if (!emailSuccess) throw new EmailNotSentException(_l["Sending of confirmation email failed."]);
            }

            user.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return new UserDetailsResponseDto(user);
        }

        /// <inheritdoc />
        public async Task Delete(Guid requestedByUserId, Guid id)
        {
            if (requestedByUserId != id) throw new ForbiddenException();

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user != null)
            {
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
            }
        }

        /// <inheritdoc />
        public async Task ConfirmEmail(string code)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UnconfirmedEmailCode == code);
            if (user == null)
                throw new EntityNotFoundException(_l["Something went wrong... Please contact support."]);

            // Set user to active only if this is the first email confirmation.
            if (user.Email == null) user.IsActive = true;

            // Change email and reset unconfirmed fields.
            user.Email = user.UnconfirmedEmail;
            user.UnconfirmedEmail = null;
            user.UnconfirmedEmailCode = null;
            user.UnconfirmedEmailCount = 0;
            user.UnconfirmedEmailCreatedAt = null;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task PasswordReset(PasswordResetRequestDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);
            if (user == null)
                throw new EntityNotFoundException(_l["Something went wrong... Please contact support."]);

            var secondsPassed = DateTime.UtcNow.Subtract(user.ResetPasswordCreatedAt.GetValueOrDefault()).Seconds;

            var isMaxCountExceeded = user.ResetPasswordCount >= _appSettings.MaxResetPasswordCount;
            var isWaitingTimePassed = secondsPassed > _appSettings.ResetPasswordWaitingTime;

            if (isMaxCountExceeded && !isWaitingTimePassed)
            {
                var secondsToWait = _appSettings.ResetPasswordWaitingTime - secondsPassed;
                throw new TooManyResetPasswordAttemptsException(string.Format(
                    _l["You must wait for {0} seconds before you try to reset password again."], secondsToWait));
            }

            user.ResetPasswordCode = _passwordHelper.GenerateRandomString(30) + Guid.NewGuid();
            user.ResetPasswordCount += 1;
            user.ResetPasswordCreatedAt = DateTime.UtcNow;

            // Prepare email template.
            await using var stream = _embedded
                .GetFileInfo($"Resources/EmailTemplates/{_lr["RESOURCE:Email_PasswordReset.html"]}")
                .CreateReadStream();
            var encPasswordCode = HttpUtility.UrlEncode(user.ResetPasswordCode);
            var encEmail = HttpUtility.UrlEncode(user.Email);
            var emailBody = await new StreamReader(stream).ReadToEndAsync();
            emailBody = emailBody.Replace("{{APP_NAME}}", _appSettings.Name);
            emailBody = emailBody.Replace("{{PASSWORD_RESET_CONFIRM_URL}}",
                $"{_appSettings.ResetPasswordUrl}?code={encPasswordCode}&email={encEmail}");

            // Send an email.
            var emailSuccess = await _emailService.SendAsync(_appSettings.Email, _appSettings.EmailName, user.Email,
                _l["Reset your password"], emailBody, null);

            if (!emailSuccess) throw new EmailNotSentException(_l["Sending of email failed."]);

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<UserResetPasswordResponseDto> ConfirmResetPassword(string code, string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email && x.ResetPasswordCode == code);
            if (user == null) throw new EntityNotFoundException(_l["Invalid code."]);

            var secondsPassed = DateTime.UtcNow.Subtract(user.ResetPasswordCreatedAt.GetValueOrDefault()).Seconds;
            if (secondsPassed > _appSettings.ResetPasswordValidTime)
                throw new AppException(_l["This link has expired... Please try to reset password again."]);

            user.ResetPasswordCode = null;
            user.ResetPasswordCount = 0;
            user.ResetPasswordCreatedAt = null;
            var newPassword = _passwordHelper.GenerateRandomString(8);
            (user.PasswordHash, user.PasswordSalt) = _passwordHelper.CreateHash(newPassword);

            await _db.SaveChangesAsync();

            return new UserResetPasswordResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Password = newPassword
            };
        }

        #region Private helper methods

        /// <summary>
        /// Changes the email asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="newEmail">The new email.</param>
        /// <returns>Task.</returns>
        private async Task<bool> ChangeEmailAsync(User user, string newEmail)
        {
            if (newEmail == user.Email) return true;

            var secondsPassed = DateTime.UtcNow.Subtract(user.UnconfirmedEmailCreatedAt.GetValueOrDefault()).Seconds;

            var isMaxCountExceeded = user.UnconfirmedEmailCount >= _appSettings.MaxUnconfirmedEmailCount;
            var isWaitingTimePassed = secondsPassed > _appSettings.UnconfirmedEmailWaitingTime;

            if (isMaxCountExceeded && !isWaitingTimePassed)
            {
                var secondsToWait = _appSettings.UnconfirmedEmailWaitingTime - secondsPassed;
                throw new TooManyChangeEmailAttemptsException(string.Format(
                    _l["You must wait for {0} seconds before you try to change email again."], secondsToWait));
            }

            user.UnconfirmedEmail = newEmail;
            user.UnconfirmedEmailCode = _passwordHelper.GenerateRandomString(30) + Guid.NewGuid();
            user.UnconfirmedEmailCount += 1;
            user.UnconfirmedEmailCreatedAt = DateTime.UtcNow;

            // Prepare email template.
            await using var stream = _embedded
                .GetFileInfo($"Resources/EmailTemplates/{_lr["RESOURCE:Email_ConfirmEmail.html"]}")
                .CreateReadStream();
            var emailBody = await new StreamReader(stream).ReadToEndAsync();
            emailBody = emailBody.Replace("{{APP_NAME}}", _appSettings.Name);
            emailBody = emailBody.Replace("{{EMAIL_CONFIRM_URL}}",
                $"{_appSettings.ConfirmEmailUrl}?code={user.UnconfirmedEmailCode}");

            // Send an email.
            return await _emailService.SendAsync(_appSettings.Email, _appSettings.EmailName, newEmail,
                _l["Confirm your email"], emailBody, null);
        }

        #endregion
    }
}