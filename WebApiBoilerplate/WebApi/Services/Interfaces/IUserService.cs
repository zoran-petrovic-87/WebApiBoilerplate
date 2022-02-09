using System;
using System.Threading.Tasks;
using WebApi.Helpers.Pagination;
using WebApi.Services.DataTransferObjects.UserService;

namespace WebApi.Services.Interfaces;

/// <summary>
/// The user service interface.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Authenticates the specified user.
    /// </summary>
    /// <param name="dto">The <see cref="AuthenticateAsyncReqDto"/> data transfer object.</param>
    /// <returns>The user info with token.</returns>
    Task<AuthenticateAsyncResDto> AuthenticateAsync(AuthenticateAsyncReqDto dto);

    /// <summary>
    /// Authenticates the external user.
    /// </summary>
    /// <param name="externalIdentityProvider">The external identity provider.</param>
    /// <param name="externalId">The identifier used by external identity provider.</param>
    /// <param name="email">The email.</param>
    /// <param name="givenName">The given name.</param>
    /// <param name="familyName">The family name.</param>
    /// <returns>The user info with token.</returns>
    Task<AuthenticateAsyncResDto> AuthenticateExternalAsync(string externalIdentityProvider,
        string externalId, string email, string givenName, string familyName);

    /// <summary>
    /// Registers the new user.
    /// </summary>
    /// <param name="dto">The <see cref="RegisterAsyncReqDto"/> data transfer object.</param>
    /// <returns>The user details.</returns>
    Task<GetDetailsAsyncResDto> RegisterAsync(RegisterAsyncReqDto dto);

    /// <summary>
    /// Creates the new user.
    /// </summary>
    /// <param name="userId">The identifier of the user that made the request.</param>
    /// <param name="dto">The <see cref="RegisterAsyncReqDto"/> data transfer object.</param>
    /// <returns>The user details.</returns>
    Task<GetDetailsAsyncResDto> CreateAsync(Guid userId, RegisterAsyncReqDto dto);

    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <param name="paginationFilter">The pagination filter.</param>
    /// <returns>The list of users.</returns>
    Task<PagedResult<GetAllAsyncResDto>> GetAllAsync(PaginationFilter paginationFilter);

    /// <summary>
    /// Gets the user by identifier.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <returns>The user details.</returns>
    Task<GetDetailsAsyncResDto> GetDetailsAsync(Guid id);

    /// <summary>
    /// Updates the specified user.
    /// </summary>
    /// <param name="id">The identifier of the user that should be updated.</param>
    /// <param name="userId">The identifier of the user that made update request.</param>
    /// <param name="dto">The <see cref="UpdateAsyncReqDto"/> data transfer object.</param>
    /// <returns>The user details.</returns>
    Task<GetDetailsAsyncResDto> UpdateAsync(Guid id, Guid userId, UpdateAsyncReqDto dto);

    /// <summary>
    /// Deletes the specified user.
    /// </summary>
    /// <param name="id">The identifier of the user that should be deleted.</param>
    /// <param name="userId">The identifier of the user that made the delete request.</param>
    /// <returns>Task.</returns>
    Task DeleteAsync(Guid id, Guid userId);

    /// <summary>
    /// Confirms the email.
    /// </summary>
    /// <param name="code">The code that will be used to validate email address.</param>
    /// <returns>Task.</returns>
    Task ConfirmEmailAsync(string code);

    /// <summary>
    /// Resets the password by sending an email to the user.
    /// </summary>
    /// <param name="dto">The <see cref="PasswordResetAsyncReqDto"/> data transfer object.</param>
    /// <returns>Task.</returns>
    Task PasswordResetAsync(PasswordResetAsyncReqDto dto);

    /// <summary>
    /// Confirms the reset password.
    /// </summary>
    /// <param name="code">The code that will be used to validate reset password request.</param>
    /// <param name="email">The email address of the user.</param>
    /// <returns>Task</returns>
    Task<ConfirmResetPasswordAsyncResDto> ConfirmResetPasswordAsync(string code, string email);
}