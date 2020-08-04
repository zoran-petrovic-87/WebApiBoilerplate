using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using WebApi.Data;
using WebApi.DataTransferObjects;
using WebApi.DataTransferObjects.User;
using WebApi.Helpers;
using WebApi.Helpers.Exceptions;
using WebApi.Helpers.Pagination;
using WebApi.IServices;
using WebApi.Models;
using WebApi.Resources.Localization;
using WebApi.Services;
using WebApi.Test.Helpers;
using Xunit;

namespace WebApi.Test.Tests.Services
{
    public class UserServiceTest : IDisposable
    {
        private readonly Factory _factory;
        private readonly AppDbContext _db;
        private readonly UserService _service;
        private readonly IPasswordHelper _passwordHelper;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly PaginationFilter _paginationFilter;
        private readonly IStringLocalizer<Translation> _stringLocalizer;
        private readonly IStringLocalizer<LocalizedResource> _resourceLocalizer;

        /// <summary>
        /// Constructor that is called for each test.
        /// </summary>
        public UserServiceTest()
        {
            _stringLocalizer = Substitute.For<IStringLocalizer<Translation>>();
            _stringLocalizer[Arg.Any<string>()].Returns(p => new LocalizedString((string) p[0], (string) p[0]));

            _resourceLocalizer = Substitute.For<IStringLocalizer<LocalizedResource>>();
            _resourceLocalizer["RESOURCE:Email_ConfirmEmail.html"]
                .Returns(new LocalizedString("RESOURCE:Email_ConfirmEmail.html", "Email_ConfirmEmail_en.html"));
            _resourceLocalizer["RESOURCE:Email_PasswordReset.html"]
                .Returns(new LocalizedString("RESOURCE:Email_PasswordReset.html", "Email_PasswordReset_en.html"));

            var passwordHelperLocalizer = Substitute.For<IStringLocalizer<PasswordHelper>>();
            passwordHelperLocalizer[Arg.Any<string>()].Returns(p => new LocalizedString((string) p[0], (string) p[0]));

            _passwordHelper = new PasswordHelper(passwordHelperLocalizer);

            var emailService = Substitute.For<IEmailService>();
            emailService
                .SendAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
                    Arg.Any<string>(), Arg.Any<string>())
                .Returns(callInfo => Task.FromResult(true));

            var dataHelper = new DataHelper();
            _db = dataHelper.CreateDbContext(_passwordHelper);
            _factory = dataHelper.Factory;
            _appSettings = Options.Create(new AppSettings {Secret = "Secret key, must be at least 16 chars long."});
            _service = new UserService(null, _db, _appSettings, _stringLocalizer, _resourceLocalizer, emailService,
                _passwordHelper);
            _paginationFilter = new PaginationFilter(int.MaxValue);
        }

        /// <summary>
        /// Disposes objects after each test.
        /// </summary>
        public void Dispose()
        {
            _db.Dispose();
        }

        #region Create

        /// <summary>
        /// Tests creating a user.
        /// It should create a user.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task Create_WhenCalled_CreatesUser()
        {
            // Arrange.
            var dto = new UserCreateRequestDto
            {
                Username = "test_username",
                FirstName = "My First Name",
                LastName = "My Last Name",
                Email = "myemail@example.com",
                Password = "MyPassword123"
            };

            // Act.
            var response = await _service.Create(dto);

            // Assert.
            Assert.Equal(dto.Username, response.Username);
            Assert.Equal(dto.FirstName, response.FirstName);
            Assert.Equal(dto.LastName, response.LastName);

            var user = _db.Users.Single(x => x.Username == dto.Username);
            Assert.Equal(dto.Username, user.Username);
            Assert.Equal(dto.FirstName, user.FirstName);
            Assert.Equal(dto.LastName, user.LastName);
            Assert.Equal(dto.Email, user.UnconfirmedEmail);
            Assert.Null(user.Email);
            Assert.True(_passwordHelper.VerifyHash(dto.Password, user.PasswordHash, user.PasswordSalt));
            Assert.False(user.IsActive);
        }

        /// <summary>
        /// Tests creating a user when password is <c>null</c> or white space.
        /// It should throw <exception cref="InvalidPasswordException">InvalidPasswordException</exception>.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>The task.</returns>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Create_PasswordIsNullOrWhiteSpace_ThrowsInvalidPasswordException(string password)
        {
            // Arrange.
            var dto = new UserCreateRequestDto
            {
                Username = "test_username",
                FirstName = "My First Name",
                LastName = "My Last Name",
                Email = "myemail@example.com",
                Password = password
            };

            // Act.
            Task Act() => _service.Create(dto);

            // Assert.
            await Assert.ThrowsAsync<InvalidPasswordException>(Act);
        }

        /// <summary>
        /// Tests creating a user when username is taken.
        /// It should throw <exception cref="UsernameTakenException">UsernameTakenException</exception>.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task Create_UsernameIsTaken_ThrowsUsernameTakenException()
        {
            // Arrange.
            var user = _factory.CreateUsers(1, "AbcAbc123")[0];
            var dto = new UserCreateRequestDto
            {
                Username = user.Username,
                FirstName = "My First Name",
                LastName = "My Last Name",
                Email = "myemail@example.com",
                Password = "MyPassword123"
            };

            // Act.
            Task Act() => _service.Create(dto);

            // Assert.
            await Assert.ThrowsAsync<UsernameTakenException>(Act);
        }

        /// <summary>
        /// Tests creating a user when email is taken.
        /// It should throw <exception cref="EmailTakenException">EmailTakenException</exception>.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task Create_EmailIsTaken_ThrowsEmailTakenException()
        {
            // Arrange.
            var user = _factory.CreateUsers(1, "AbcAbc123")[0];
            var dto = new UserCreateRequestDto
            {
                Username = "test_username",
                FirstName = "My First Name",
                LastName = "My Last Name",
                Email = user.Email,
                Password = "MyPassword123"
            };

            // Act.
            Task Act() => _service.Create(dto);

            // Assert.
            await Assert.ThrowsAsync<EmailTakenException>(Act);
        }

        /// <summary>
        /// Tests creating a user when sending of activation email fails.
        /// It should throw <exception cref="EmailNotSentException">EmailNotSentException</exception>.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task Create_EmailNotSent_EmailNotSentException()
        {
            // Arrange.
            var dto = new UserCreateRequestDto
            {
                Username = "test_username",
                FirstName = "My First Name",
                LastName = "My Last Name",
                Email = "myemail@example.com",
                Password = "MyPassword123"
            };
            var emailService = Substitute.For<IEmailService>();
            emailService
                .SendAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
                    Arg.Any<string>(), Arg.Any<string>())
                .Returns(callInfo => Task.FromResult(false));

            var service = new UserService(null, _db, _appSettings, _stringLocalizer, _resourceLocalizer, emailService,
                _passwordHelper);

            // Act.
            Task Act() => service.Create(dto);

            // Assert.
            await Assert.ThrowsAsync<EmailNotSentException>(Act);
        }

        #endregion

        #region Authenticate

        /// <summary>
        /// Tests user authentication.
        /// It should authenticate a user.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task Authenticate_WhenCalled_AuthenticatesUser()
        {
            // Arrange.
            const string password = "AbcAbc123";
            var user = _factory.CreateUsers(2, password)[0];
            var dto = new AuthenticateRequestDto
            {
                Username = user.Username,
                Password = password
            };

            // Act.
            var response = await _service.Authenticate(dto);

            // Assert.
            Assert.Equal(dto.Username, response.Username);
            Assert.Equal(user.FirstName, response.FirstName);
            Assert.Equal(user.LastName, response.LastName);
            Assert.Equal(user.Email, response.Email);
            Assert.NotNull(user.LastLoginAt);
            Assert.True(user.IsActive);
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Value.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            tokenHandler.ValidateToken(response.Token, validationParameters, out _);
        }

        /// <summary>
        /// Tests user authentication when user doesn't exist.
        /// It should throw <exception cref="EntityNotFoundException">EntityNotFoundException</exception>.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task Authenticate_UserDoesNotExist_ThrowsEntityNotFoundException()
        {
            // Arrange.
            var dto = new AuthenticateRequestDto
            {
                Username = "test_username",
                Password = "AbcAbc123"
            };

            // Act.
            Task Act() => _service.Authenticate(dto);

            // Assert.
            await Assert.ThrowsAsync<EntityNotFoundException>(Act);
        }

        /// <summary>
        /// Tests user authentication when the password is incorrect.
        /// It should throw <exception cref="IncorrectPasswordException">IncorrectPasswordException</exception>.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task Authenticate_IncorrectPassword_ThrowsIncorrectPasswordException()
        {
            // Arrange.
            const string password = "AbcAbc123";
            var user = _factory.CreateUsers(2, password)[0];
            var dto = new AuthenticateRequestDto
            {
                Username = user.Username,
                Password = password + "A"
            };

            // Act.
            Task Act() => _service.Authenticate(dto);

            // Assert.
            await Assert.ThrowsAsync<IncorrectPasswordException>(Act);
        }

        /// <summary>
        /// Tests user authentication when there are too many failed login attempts.
        /// It should throw
        /// <exception cref="TooManyFailedLoginAttemptsException">TooManyFailedLoginAttemptsException</exception>.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task Authenticate_TooManyFailedLoginAttempts_ThrowsTooManyFailedLoginAttemptsException()
        {
            // Arrange.
            _appSettings.Value.MaxLoginFailedCount = 5;
            const string password = "AbcAbc123";
            var user = _factory.CreateUsers(2, password)[0];
            var dto = new AuthenticateRequestDto
            {
                Username = user.Username,
                Password = password + "A"
            };

            for (var i = 0; i < _appSettings.Value.MaxLoginFailedCount; i++)
            {
                try
                {
                    await _service.Authenticate(dto);
                }
                catch (IncorrectPasswordException)
                {
                }
            }

            // Act.
            Task Act() => _service.Authenticate(dto);

            // Assert.
            await Assert.ThrowsAsync<TooManyFailedLoginAttemptsException>(Act);
        }

        /// <summary>
        /// Tests user authentication when there are too many failed login attempts but waiting time has passed.
        /// It should authenticate the user.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task Authenticate_TooManyFailedLoginAttemptsButWaitingTimePassed_AuthenticatesUser()
        {
            // Arrange.
            _appSettings.Value.MaxLoginFailedCount = 5;
            const string password = "AbcAbc123";
            var user = _factory.CreateUsers(2, password)[0];
            var dto = new AuthenticateRequestDto
            {
                Username = user.Username,
                Password = password + "A"
            };

            for (var i = 0; i < _appSettings.Value.MaxLoginFailedCount; i++)
            {
                try
                {
                    await _service.Authenticate(dto);
                }
                catch (IncorrectPasswordException)
                {
                }
            }

            user.LoginFailedAt = DateTime.UtcNow.AddSeconds(-_appSettings.Value.LoginFailedWaitingTime - 1);
            await _db.SaveChangesAsync();

            dto.Password = password;

            // Act.
            var response = await _service.Authenticate(dto);

            // Assert.
            Assert.Equal(dto.Username, response.Username);
        }

        #endregion

        #region GetAll

        /// <summary>
        /// Tests getting list of users.
        /// It should return list of users.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task GetAll_WhenCalled_ReturnsListOfUsers()
        {
            // Arrange.
            var users = _factory.CreateUsers(10, "AbcAbc123");
            var deactivatedUser = users[0];
            deactivatedUser.IsActive = false;
            await _db.SaveChangesAsync();

            // Act.
            var response = await _service.GetAll(_paginationFilter);

            // Assert.
            Assert.Equal(9, response.Data.Count);

            var actualUsers = response.Data.OrderBy(x => x.Id).ToArray();
            var expectedUsers = users.Where(x => x.IsActive).OrderBy(x => x.Id).ToArray();

            Assert.Equal(expectedUsers.Count(), actualUsers.Count());

            for (var i = 0; i < actualUsers.Count(); i++)
            {
                Assert.Equal(expectedUsers[i].Id, actualUsers[i].Id);
                Assert.Equal(expectedUsers[i].Username, actualUsers[i].Username);
                Assert.Equal(expectedUsers[i].FirstName, actualUsers[i].FirstName);
                Assert.Equal(expectedUsers[i].LastName, actualUsers[i].LastName);
                Assert.Equal(expectedUsers[i].Email, actualUsers[i].Email);
                Assert.Equal(expectedUsers[i].Role, actualUsers[i].Role);
            }
        }

        #endregion

        #region GetById

        /// <summary>
        /// Tests getting user details.
        /// It should return user details.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task GetById_WhenCalled_ReturnsUserDetails()
        {
            // Arrange.
            var user = _factory.CreateUsers(2, "AbcAbc123")[0];

            // Act.
            var response = await _service.GetById(user.Id);

            // Assert.
            Assert.Equal(user.Id, response.Id);
            Assert.Equal(user.Username, response.Username);
            Assert.Equal(user.FirstName, response.FirstName);
            Assert.Equal(user.LastName, response.LastName);
            Assert.Equal(user.Email, response.Email);
            Assert.Equal(user.Role, response.Role);
            Assert.Equal(user.CreatedAt, response.CreatedAt);
            Assert.Equal(user.UpdatedAt, response.UpdatedAt);
            Assert.Equal(user.LastLoginAt, response.LastLoginAt);
            Assert.Equal(user.IsActive, response.IsActive);
        }

        /// <summary>
        /// Tests getting user details when user doesn't exist.
        /// It should throw <exception cref="EntityNotFoundException">EntityNotFoundException</exception>.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task GetById_UserDoesNotExist_ThrowsEntityNotFoundException()
        {
            // Arrange.
            _factory.CreateUsers(2, "AbcAbc123");

            // Act.
            Task Act() => _service.GetById(Guid.NewGuid());

            // Assert.
            await Assert.ThrowsAsync<EntityNotFoundException>(Act);
        }

        #endregion

        #region Update

        /// <summary>
        /// Tests updating user details.
        /// It should update user details.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task Update_WhenCalled_UpdatesUser()
        {
            // Arrange.
            var user = _factory.CreateUsers(2, "AbcAbc123")[0];

            var dto = new UserUpdateRequestDto
            {
                Username = new UpdateStringField {NewValue = Guid.NewGuid().ToString()},
                FirstName = new UpdateStringField {NewValue = Guid.NewGuid().ToString()},
                LastName = new UpdateStringField {NewValue = Guid.NewGuid().ToString()},
                Email = new UpdateStringField {NewValue = Guid.NewGuid() + "@example.com"},
                Password = new UpdateStringField {NewValue = Guid.NewGuid().ToString()}
            };

            // Act.
            await _service.Update(user.Id, user.Id, dto);

            // Assert.
            var actualUser = _db.Users.Single(x => x.Id == user.Id);

            Assert.Equal(dto.Username.NewValue, actualUser.Username);
            Assert.Equal(dto.FirstName.NewValue, actualUser.FirstName);
            Assert.Equal(dto.LastName.NewValue, actualUser.LastName);
            Assert.Equal(dto.Email.NewValue, actualUser.UnconfirmedEmail);
            Assert.NotNull(actualUser.UpdatedAt);
            Assert.True(
                _passwordHelper.VerifyHash(dto.Password.NewValue, actualUser.PasswordHash, actualUser.PasswordSalt));
        }

        /// <summary>
        /// Tests updating user details when nothing is provided.
        /// It user details should stay the same.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task Update_NoDataProvided_UserNotUpdated()
        {
            // Arrange.
            var user = _factory.CreateUsers(2, "AbcAbc123")[0];

            var dto = new UserUpdateRequestDto
            {
                Username = null,
                FirstName = null,
                LastName = null,
                Email = null,
                Password = null
            };

            // Act.
            await _service.Update(user.Id, user.Id, dto);

            // Assert.
            var actualUser = _db.Users.Single(x => x.Id == user.Id);

            Assert.Equal(user.Username, actualUser.Username);
            Assert.Equal(user.FirstName, actualUser.FirstName);
            Assert.Equal(user.LastName, actualUser.LastName);
            Assert.Equal(user.Email, actualUser.Email);
            Assert.NotNull(actualUser.UpdatedAt);
            Assert.Equal(user.PasswordHash, actualUser.PasswordHash);
            Assert.Equal(user.PasswordSalt, actualUser.PasswordSalt);
        }

        /// <summary>
        /// Tests updating the different user.
        /// It should throw <exception cref="ForbiddenException">ForbiddenException</exception>.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task Update_DifferentUser_ThrowsForbiddenException()
        {
            // Arrange.
            var users = _factory.CreateUsers(2, "AbcAbc123");
            var dto = new UserUpdateRequestDto();

            // Act.
            Task Act() => _service.Update(users[0].Id, users[1].Id, dto);

            // Assert.
            await Assert.ThrowsAsync<ForbiddenException>(Act);
        }

        /// <summary>
        /// Tests updating the non-existing user.
        /// It should throw <exception cref="EntityNotFoundException">EntityNotFoundException</exception>.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task Update_UserDoesNotExist_ThrowsEntityNotFoundException()
        {
            // Arrange.
            _factory.CreateUsers(2, "AbcAbc123");
            var userId = Guid.NewGuid();
            var dto = new UserUpdateRequestDto();

            // Act.
            Task Act() => _service.Update(userId, userId, dto);

            // Assert.
            await Assert.ThrowsAsync<EntityNotFoundException>(Act);
        }

        /// <summary>
        /// Tests updating when username is already taken.
        /// It should throw <exception cref="UsernameTakenException">UsernameTakenException</exception>.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task Update_UsernameTaken_ThrowsUsernameTakenException()
        {
            // Arrange.
            var users = _factory.CreateUsers(2, "AbcAbc123");
            var dto = new UserUpdateRequestDto
            {
                Username = new UpdateStringField {NewValue = users[1].Username}
            };

            // Act.
            Task Act() => _service.Update(users[0].Id, users[0].Id, dto);

            // Assert.
            await Assert.ThrowsAsync<UsernameTakenException>(Act);
        }

        /// <summary>
        /// Tests updating email when there are too many failed attempts.
        /// It should throw
        /// <exception cref="TooManyChangeEmailAttemptsException">TooManyChangeEmailAttemptsException</exception>.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task Update_TooManyFailedChangeEmailAttempts_ThrowsTooManyChangeEmailAttemptsException()
        {
            // Arrange.
            _appSettings.Value.MaxUnconfirmedEmailCount = 5;
            var user = _factory.CreateUsers(2, "AbcAbc123")[0];
            var dto = new UserUpdateRequestDto
            {
                Email = new UpdateStringField {NewValue = Guid.NewGuid() + "@example.com"}
            };

            for (var i = 0; i < _appSettings.Value.MaxUnconfirmedEmailCount; i++)
            {
                await _service.Update(user.Id, user.Id, dto);
            }

            // Act.
            Task Act() => _service.Update(user.Id, user.Id, dto);

            // Assert.
            await Assert.ThrowsAsync<TooManyChangeEmailAttemptsException>(Act);
        }

        /// <summary>
        /// Tests updating email when there are too many failed attempts but waiting time has passed.
        /// It should update email.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task Update_TooManyFailedChangeEmailAttemptsButWaitingTimePassed_AllowsEmailUpdate()
        {
            // Arrange.
            _appSettings.Value.MaxUnconfirmedEmailCount = 5;
            var user = _factory.CreateUsers(2, "AbcAbc123")[0];
            var dto = new UserUpdateRequestDto
            {
                Email = new UpdateStringField {NewValue = Guid.NewGuid() + "@example.com"}
            };

            for (var i = 0; i < _appSettings.Value.MaxUnconfirmedEmailCount; i++)
            {
                await _service.Update(user.Id, user.Id, dto);
            }

            user.UnconfirmedEmailCreatedAt =
                DateTime.UtcNow.AddSeconds(-_appSettings.Value.UnconfirmedEmailWaitingTime - 1);
            await _db.SaveChangesAsync();

            // Act.
            await _service.Update(user.Id, user.Id, dto);

            // Assert.
            Assert.Equal(dto.Email.NewValue, user.UnconfirmedEmail);
        }

        /// <summary>
        /// Tests updating email when the same email address is provided.
        /// It should not send confirmation email.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task Update_SameEmail_ConfirmationEmailNotSent()
        {
            // Arrange.
            var user = _factory.CreateUsers(2, "AbcAbc123")[0];
            var dto = new UserUpdateRequestDto
            {
                Email = new UpdateStringField {NewValue = user.Email}
            };

            // Act.
            await _service.Update(user.Id, user.Id, dto);

            // Assert.
            Assert.Null(user.UnconfirmedEmail);
            Assert.Equal(dto.Email.NewValue, user.Email);
        }

        /// <summary>
        /// Tests updating the email when email sending fails.
        /// It should throw <exception cref="EmailNotSentException">EmailNotSentException</exception>.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task Update_EmailNotSent_EmailNotSentException()
        {
            // Arrange.
            var users = _factory.CreateUsers(2, "AbcAbc123");
            var dto = new UserUpdateRequestDto
            {
                Email = new UpdateStringField {NewValue = Guid.NewGuid() + "@example.com"}
            };
            var emailService = Substitute.For<IEmailService>();
            emailService
                .SendAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
                    Arg.Any<string>(), Arg.Any<string>())
                .Returns(callInfo => Task.FromResult(false));

            var service = new UserService(null, _db, _appSettings, _stringLocalizer, _resourceLocalizer, emailService,
                _passwordHelper);

            // Act.
            Task Act() => service.Update(users[0].Id, users[0].Id, dto);

            // Assert.
            await Assert.ThrowsAsync<EmailNotSentException>(Act);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Tests deleting the user.
        /// It should delete the user.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task Delete_WhenCalled_DeletesUser()
        {
            // Arrange.
            var user = _factory.CreateUsers(2, "AbcAbc123")[0];

            // Act.
            await _service.Delete(user.Id, user.Id);

            // Assert.
            Assert.Null(_db.Users.FirstOrDefault(x => x.Id == user.Id));
        }

        /// <summary>
        /// Tests deleting the different user.
        /// It should throw <exception cref="ForbiddenException">ForbiddenException</exception>.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task Delete_DifferentUser_ThrowsForbiddenException()
        {
            // Arrange.
            var users = _factory.CreateUsers(2, "AbcAbc123");

            // Act.
            Task Act() => _service.Delete(users[0].Id, users[1].Id);

            // Assert.
            await Assert.ThrowsAsync<ForbiddenException>(Act);
        }

        #endregion

        #region ConfirmEmail

        /// <summary>
        /// Tests email confirmation.
        /// It should confirm email change.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task ConfirmEmail_WhenCalled_ChangesEmail()
        {
            // Arrange.
            var user = _factory.CreateUsers(2, "AbcAbc123")[0];
            var newEmail = $"{Guid.NewGuid()}@example.com";
            user.UnconfirmedEmail = newEmail;
            user.UnconfirmedEmailCreatedAt = DateTime.UtcNow;
            user.UnconfirmedEmailCode = _passwordHelper.GenerateRandomString(30) + Guid.NewGuid();
            user.UnconfirmedEmailCount = 1;
            user.IsActive = false;
            await _db.SaveChangesAsync();

            // Act.
            await _service.ConfirmEmail(user.UnconfirmedEmailCode);

            // Assert.
            Assert.Equal(0, user.UnconfirmedEmailCount);
            Assert.Null(user.UnconfirmedEmailCode);
            Assert.Null(user.UnconfirmedEmail);
            Assert.Equal(newEmail, user.Email);
            Assert.False(user.IsActive);
        }

        /// <summary>
        /// Tests the first email confirmation.
        /// It should activate the user.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task ConfirmEmail_FirstConfirmation_ActivatesUser()
        {
            // Arrange.
            var user = _factory.CreateUsers(2, "AbcAbc123")[0];
            var newEmail = $"{Guid.NewGuid()}@example.com";
            user.Email = null;
            user.UnconfirmedEmail = newEmail;
            user.UnconfirmedEmailCreatedAt = DateTime.UtcNow;
            user.UnconfirmedEmailCode = _passwordHelper.GenerateRandomString(30) + Guid.NewGuid();
            user.UnconfirmedEmailCount = 1;
            user.IsActive = false;
            await _db.SaveChangesAsync();

            // Act.
            await _service.ConfirmEmail(user.UnconfirmedEmailCode);

            // Assert.
            Assert.True(user.IsActive);
        }

        /// <summary>
        /// Tests email confirmation when confirmation code is invalid.
        /// It should throw <exception cref="EntityNotFoundException">EntityNotFoundException</exception>.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task ConfirmEmail_InvalidCode_ThrowsEntityNotFoundException()
        {
            // Arrange.
            var user = _factory.CreateUsers(2, "AbcAbc123")[0];
            var newEmail = $"{Guid.NewGuid()}@example.com";
            user.UnconfirmedEmail = newEmail;
            user.UnconfirmedEmailCreatedAt = DateTime.UtcNow;
            user.UnconfirmedEmailCode = _passwordHelper.GenerateRandomString(30) + Guid.NewGuid();
            user.UnconfirmedEmailCount = 1;
            user.IsActive = false;
            await _db.SaveChangesAsync();

            // Act.
            Task Act() => _service.ConfirmEmail(Guid.NewGuid().ToString());

            // Assert.
            await Assert.ThrowsAsync<EntityNotFoundException>(Act);
        }

        #endregion

        #region PasswordReset

        /// <summary>
        /// Tests password reset.
        /// It should send password reset email.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task PasswordReset_WhenCalled_SendsPasswordResetEmail()
        {
            // Arrange.
            var user = _factory.CreateUsers(2, "AbcAbc123")[0];
            var emailService = Substitute.For<IEmailService>();
            var toEmail = "";
            var dto = new PasswordResetRequestDto {Email = user.Email};
            emailService
                .SendAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
                    Arg.Any<string>(), Arg.Any<string>())
                .Returns(callInfo =>
                    {
                        toEmail = callInfo.ArgAt<string>(2);
                        return Task.FromResult(true);
                    }
                );

            var service = new UserService(null, _db, _appSettings, _stringLocalizer, _resourceLocalizer, emailService,
                _passwordHelper);

            // Act.
            await service.PasswordReset(dto);

            // Assert.
            Assert.NotNull(user.ResetPasswordCode);
            Assert.NotNull(user.ResetPasswordCreatedAt);
            Assert.Equal(1, user.ResetPasswordCount);
            Assert.Equal(toEmail, user.Email);
        }

        /// <summary>
        /// Tests password reset when user email is invalid.
        /// It should throw <exception cref="EntityNotFoundException">EntityNotFoundException</exception>.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task PasswordReset_InvalidEmail_ThrowsEntityNotFoundException()
        {
            // Arrange.
            _factory.CreateUsers(2, "AbcAbc123");
            var dto = new PasswordResetRequestDto {Email = Guid.NewGuid() + "@example.com"};

            // Act.
            Task Act() => _service.PasswordReset(dto);

            // Assert.
            await Assert.ThrowsAsync<EntityNotFoundException>(Act);
        }

        /// <summary>
        /// Tests password reset when there are too many attempts.
        /// It should throw
        /// <exception cref="TooManyResetPasswordAttemptsException">TooManyResetPasswordAttemptsException</exception>.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task PasswordReset_TooManyAttempts_ThrowsTooManyResetPasswordAttemptsException()
        {
            // Arrange.
            _appSettings.Value.MaxResetPasswordCount = 5;
            var user = _factory.CreateUsers(2, "AbcAbc123")[0];
            var dto = new PasswordResetRequestDto {Email = user.Email};

            for (var i = 0; i < _appSettings.Value.MaxResetPasswordCount; i++)
                await _service.PasswordReset(dto);

            // Act.
            Task Act() => _service.PasswordReset(dto);

            // Assert.
            await Assert.ThrowsAsync<TooManyResetPasswordAttemptsException>(Act);
        }

        /// <summary>
        /// Tests password reset when there are too many attempts but waiting time has passed.
        /// It should send password reset email.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task PasswordReset_TooManyAttemptsButWaitingTimePassed_SendsPasswordResetEmail()
        {
            // Arrange.
            _appSettings.Value.MaxResetPasswordCount = 5;
            var user = _factory.CreateUsers(2, "AbcAbc123")[0];
            var dto = new PasswordResetRequestDto {Email = user.Email};

            for (var i = 0; i < _appSettings.Value.MaxResetPasswordCount; i++)
                await _service.PasswordReset(dto);

            user.ResetPasswordCreatedAt = DateTime.UtcNow.AddSeconds(-_appSettings.Value.ResetPasswordWaitingTime - 1);
            await _db.SaveChangesAsync();

            // Act.
            await _service.PasswordReset(dto);

            // Assert.
            Assert.NotNull(user.ResetPasswordCode);
        }
        
        /// <summary>
        /// Tests password reset when email sending fails.
        /// It should throw <exception cref="EmailNotSentException">EmailNotSentException</exception>.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task PasswordReset_EmailNotSent_EmailNotSentException()
        {
            // Arrange.
            var user = _factory.CreateUsers(2, "AbcAbc123")[0];
            var dto = new PasswordResetRequestDto {Email = user.Email};
            var emailService = Substitute.For<IEmailService>();
            emailService
                .SendAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
                    Arg.Any<string>(), Arg.Any<string>())
                .Returns(callInfo => Task.FromResult(false));
            var service = new UserService(null, _db, _appSettings, _stringLocalizer, _resourceLocalizer, emailService,
                _passwordHelper);

            // Act.
            Task Act() => service.PasswordReset(dto);

            // Assert.
            await Assert.ThrowsAsync<EmailNotSentException>(Act);
        }

        #endregion

        #region ConfirmResetPassword

        /// <summary>
        /// Tests password reset confirmation.
        /// It should reset the password.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task ConfirmResetPassword_WhenCalled_ResetsPassword()
        {
            // Arrange.
            var user = _factory.CreateUsers(2, "AbcAbc123")[0];
            var passwordHash = user.PasswordHash;
            user.ResetPasswordCode = Guid.NewGuid().ToString();
            user.ResetPasswordCount = 1;
            user.ResetPasswordCreatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            // Act.
            var response = await _service.ConfirmResetPassword(user.ResetPasswordCode, user.Email);

            // Assert.
            Assert.Null(user.ResetPasswordCode);
            Assert.Null(user.ResetPasswordCreatedAt);
            Assert.Equal(0, user.ResetPasswordCount);
            Assert.NotEqual(passwordHash, user.PasswordHash);
        }

        /// <summary>
        /// Tests password reset confirmation when provided email is invalid.
        /// It should throw <exception cref="EntityNotFoundException">EntityNotFoundException</exception>.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task ConfirmResetPassword_InvalidEmail_ThrowsEntityNotFoundException()
        {
            // Arrange.
            var user = _factory.CreateUsers(2, "AbcAbc123")[0];
            user.ResetPasswordCode = Guid.NewGuid().ToString();
            user.ResetPasswordCount = 1;
            user.ResetPasswordCreatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            // Act.
            Task Act() => _service.ConfirmResetPassword(user.ResetPasswordCode, Guid.NewGuid() + "@example.com");

            // Assert.
            await Assert.ThrowsAsync<EntityNotFoundException>(Act);
        }

        /// <summary>
        /// Tests password reset confirmation when provided code is invalid.
        /// It should throw <exception cref="EntityNotFoundException">EntityNotFoundException</exception>.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task ConfirmResetPassword_InvalidCode_ThrowsEntityNotFoundException()
        {
            // Arrange.
            var user = _factory.CreateUsers(2, "AbcAbc123")[0];
            user.ResetPasswordCode = Guid.NewGuid().ToString();
            user.ResetPasswordCount = 1;
            user.ResetPasswordCreatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            // Act.
            Task Act() => _service.ConfirmResetPassword(Guid.NewGuid().ToString(), user.Email);

            // Assert.
            await Assert.ThrowsAsync<EntityNotFoundException>(Act);
        }

        /// <summary>
        /// Tests password reset confirmation when provided code has expired.
        /// It should throw <exception cref="AppException">AppException</exception>.
        /// </summary>
        /// <returns>The task.</returns>
        [Fact]
        public async Task ConfirmResetPassword_CodeExpired_ThrowsAppException()
        {
            // Arrange.
            _appSettings.Value.ResetPasswordValidTime = 10;
            var user = _factory.CreateUsers(2, "AbcAbc123")[0];
            user.ResetPasswordCode = Guid.NewGuid().ToString();
            user.ResetPasswordCount = 1;
            user.ResetPasswordCreatedAt = DateTime.UtcNow.AddSeconds(-_appSettings.Value.ResetPasswordValidTime - 1);
            await _db.SaveChangesAsync();

            // Act.
            Task Act() => _service.ConfirmResetPassword(user.ResetPasswordCode, user.Email);

            // Assert.
            await Assert.ThrowsAsync<AppException>(Act);
        }

        #endregion
    }
}