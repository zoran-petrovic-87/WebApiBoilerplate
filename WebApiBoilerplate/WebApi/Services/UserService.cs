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
using WebApi.Helpers;
using WebApi.Helpers.Exceptions;
using WebApi.Helpers.Pagination;
using WebApi.Models;
using WebApi.Resources.Localization;
using WebApi.Services.DataTransferObjects.UserService;
using WebApi.Services.Interfaces;

namespace WebApi.Services;

/// <summary>
/// The user service.
/// </summary>
/// <seealso cref="IUserService" />
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
    public async Task<AuthenticateAsyncResDto> AuthenticateAsync(AuthenticateAsyncReqDto dto)
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

        return new AuthenticateAsyncResDto
        {
            Id = user.Id,
            Username = user.Username,
            GivenName = user.GivenName,
            FamilyName = user.FamilyName,
            Email = user.Email,
            Token = CreateToken(user.Id.ToString())
        };
    }

    /// <inheritdoc />
    public async Task<AuthenticateAsyncResDto> AuthenticateExternalAsync(string externalIdentityProvider,
        string externalId, string email, string givenName, string familyName)
    {
        var user = await _db.Users.SingleOrDefaultAsync(x =>
            x.ExternalIdentityProvider == externalIdentityProvider && x.ExternalId == externalId);
        var now = DateTime.UtcNow;

        // Create a user if doesn't exist.
        if (user == null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                ExternalId = externalId,
                ExternalIdentityProvider = externalIdentityProvider,
                Username = $"{externalId}|{externalIdentityProvider}",
                CreatedAt = now,
                IsActive = true
            };
            await _db.Users.AddAsync(user);
        }

        user.GivenName = givenName;
        user.FamilyName = familyName;
        user.Email = email;
        user.UpdatedAt = now;

        // Authentication successful.
        user.LoginFailedCount = 0;
        user.LoginFailedAt = null;
        user.LastLoginAt = now;
        await _db.SaveChangesAsync();

        return new AuthenticateAsyncResDto
        {
            Id = user.Id,
            Username = user.Username,
            GivenName = user.GivenName,
            FamilyName = user.FamilyName,
            Email = user.Email,
            Token = CreateToken(user.Id.ToString())
        };
    }

    /// <inheritdoc />
    public async Task<GetDetailsAsyncResDto> RegisterAsync(RegisterAsyncReqDto dto)
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

        var user = new User
        {
            Id = Guid.NewGuid(),
            GivenName = dto.GivenName,
            FamilyName = dto.FamilyName,
            Username = dto.Username,
            IsActive = false
            // Email must be confirmed first.
        };
        var emailSuccess = await ChangeEmailAsync(user, dto.Email);
        user.CreatedAt = DateTime.UtcNow;
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        if (!emailSuccess) throw new EmailNotSentException(_l["Sending of confirmation email failed."]);

        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();

        return new GetDetailsAsyncResDto
        {
            Id = user.Id,
            Username = user.Username,
            GivenName = user.GivenName,
            FamilyName = user.FamilyName,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            LastLoginAt = user.LastLoginAt,
            IsActive = user.IsActive
        };
    }

    /// <inheritdoc />
    public async Task<GetDetailsAsyncResDto> CreateAsync(Guid userId, RegisterAsyncReqDto dto)
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

        var user = new User
        {
            Id = Guid.NewGuid(),
            GivenName = dto.GivenName,
            FamilyName = dto.FamilyName,
            Username = dto.Username,
            IsActive = true,
            Email = dto.Email,
            CreatedById = userId,
            CreatedAt = DateTime.UtcNow,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();

        return new GetDetailsAsyncResDto
        {
            Id = user.Id,
            Username = user.Username,
            GivenName = user.GivenName,
            FamilyName = user.FamilyName,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            LastLoginAt = user.LastLoginAt,
            IsActive = user.IsActive
        };
    }

    /// <inheritdoc />
    public async Task<PagedResult<GetAllAsyncResDto>> GetAllAsync(PaginationFilter paginationFilter)
    {
        return await _db.Users.AsNoTracking()
            .Include(x => x.Role)
            .Where(x => x.IsActive)
            .Select(x => new GetAllAsyncResDto
            {
                Id = x.Id,
                FirstName = x.GivenName,
                LastName = x.FamilyName,
                Username = x.Username,
                Email = x.Email,
                Role = x.Role == null
                    ? null
                    : new RoleResDto {Id = x.Role.Id, Name = x.Role.Name}
            })
            .GetPagedAsync(paginationFilter);
    }

    /// <inheritdoc />
    public async Task<GetDetailsAsyncResDto> GetDetailsAsync(Guid id)
    {
        var user = await _db.Users.AsNoTracking()
            .Include(x => x.Role)
            .Where(x => x.Id == id)
            .Select(x => new GetDetailsAsyncResDto
            {
                Id = x.Id,
                Username = x.Username,
                GivenName = x.GivenName,
                FamilyName = x.FamilyName,
                Email = x.Email,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                LastLoginAt = x.LastLoginAt,
                IsActive = x.IsActive,
                Role = x.Role == null
                    ? null
                    : new RoleResDto
                    {
                        Id = x.Role.Id, Name = x.Role.Name
                    }
            })
            .FirstOrDefaultAsync();
        if (user == null) throw new EntityNotFoundException(_l["User not found."]);
        return user;
    }

    /// <inheritdoc />
    public async Task<GetDetailsAsyncResDto> UpdateAsync(Guid id, Guid userId,
        UpdateAsyncReqDto dto)
    {
        if (userId != id) throw new ForbiddenException();

        var user = await _db.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Id == id);
        if (user == null) throw new EntityNotFoundException(_l["User not found."]);
        var isExternalUser = user.ExternalId != null;

        // Update username if it has changed.
        if (dto.Username != null && dto.Username.NewValue != user.Username)
        {
            if (isExternalUser) throw new UpdateReadOnlyPropertyException(_l["Cannot update username."]);
            // Throw error if the new username is already taken.
            if (_db.Users.Any(x => x.Username == dto.Username.NewValue))
                throw new UsernameTakenException(string.Format(
                    _l["Username '{0}' is already taken."], dto.Username.NewValue));
            user.Username = dto.Username.NewValue;
        }

        // Update user properties if provided.
        if (dto.GivenName != null)
        {
            if (isExternalUser) throw new UpdateReadOnlyPropertyException(_l["Cannot update given name."]);
            user.GivenName = dto.GivenName.NewValue;
        }

        if (dto.FamilyName != null)
        {
            if (isExternalUser) throw new UpdateReadOnlyPropertyException(_l["Cannot update family name."]);
            user.FamilyName = dto.FamilyName.NewValue;
        }

        // Update password if provided.
        if (dto.Password != null)
        {
            if (isExternalUser) throw new UpdateReadOnlyPropertyException(_l["Cannot update password."]);
            var (passwordHash, passwordSalt) = _passwordHelper.CreateHash(dto.Password.NewValue);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
        }

        // Update email if provided.
        if (dto.Email != null)
        {
            if (isExternalUser) throw new UpdateReadOnlyPropertyException(_l["Cannot update email."]);
            var emailSuccess = await ChangeEmailAsync(user, dto.Email.NewValue);
            if (!emailSuccess) throw new EmailNotSentException(_l["Sending of confirmation email failed."]);
        }

        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedById = userId;

        await _db.SaveChangesAsync();

        return new GetDetailsAsyncResDto
        {
            Id = user.Id,
            Username = user.Username,
            GivenName = user.GivenName,
            FamilyName = user.FamilyName,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            LastLoginAt = user.LastLoginAt,
            IsActive = user.IsActive,
            Role = user.Role == null
                ? null
                : new RoleResDto
                {
                    Id = user.Role.Id, Name = user.Role.Name
                }
        };
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, Guid userId)
    {
        if (userId != id) throw new ForbiddenException();

        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user != null)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }
    }

    /// <inheritdoc />
    public async Task ConfirmEmailAsync(string code)
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
    public async Task PasswordResetAsync(PasswordResetAsyncReqDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == dto.Email && x.ExternalId == null);
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
    public async Task<ConfirmResetPasswordAsyncResDto> ConfirmResetPasswordAsync(string code,
        string email)
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

        return new ConfirmResetPasswordAsyncResDto
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

    /// <summary>
    /// Creates authentication token.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>The authentication token.</returns>
    private string CreateToken(string userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] {new Claim(ClaimTypes.Name, userId)}),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    #endregion
}