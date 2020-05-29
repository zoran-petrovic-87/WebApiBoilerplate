using System;
using System.Threading.Tasks;
using WebApi.DataTransferObjects.User;
using WebApi.Helpers.Pagination;

namespace WebApi.IServices
{
    /// <summary>
    /// The user service interface.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Authenticates the specified user.
        /// </summary>
        /// <param name="dto">The <see cref="AuthenticateRequestDto"/> data transfer object.</param>
        /// <returns>The user.</returns>
        Task<AuthenticateResponseDto> Authenticate(AuthenticateRequestDto dto);

        /// <summary>
        /// Creates the new user.
        /// </summary>
        /// <param name="dto">The <see cref="UserCreateRequestDto"/> data transfer object.</param>
        /// <returns>The user details.</returns>
        Task<UserDetailsResponseDto> Create(UserCreateRequestDto dto);

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <param name="paginationFilter">The pagination filter.</param>
        /// <returns>The list of users.</returns>
        Task<PagedResult<UserSummaryResponseDto>> GetAll(PaginationFilter paginationFilter);

        /// <summary>
        /// Gets the user by identifier.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>The user details.</returns>
        Task<UserDetailsResponseDto> GetById(Guid id);

        /// <summary>
        /// Updates the specified user.
        /// </summary>
        /// <param name="requestedByUserId">The identifier of the user that made update request.</param>
        /// <param name="userId">The identifier of the user that should be updated.</param>
        /// <param name="dto">The <see cref="UserUpdateRequestDto"/> data transfer object.</param>
        /// <returns>The user details.</returns>
        Task<UserDetailsResponseDto> Update(Guid requestedByUserId, Guid userId, UserUpdateRequestDto dto);

        /// <summary>
        /// Deletes the specified user.
        /// </summary>
        /// <param name="requestedByUserId">The identifier of the user that made the delete request.</param>
        /// <param name="id">The identifier of the user that should be deleted.</param>
        /// <returns>Task.</returns>
        Task Delete(Guid requestedByUserId, Guid id);

        /// <summary>
        /// Confirms the email.
        /// </summary>
        /// <param name="code">The code that will be used to validate email address.</param>
        /// <returns>Task.</returns>
        Task ConfirmEmail(string code);

        /// <summary>
        /// Resets te password by sending an email to the user.
        /// </summary>
        /// <param name="dto">The <see cref="PasswordResetRequestDto"/> data transfer object.</param>
        /// <returns>Task.</returns>
        Task PasswordReset(PasswordResetRequestDto dto);

        /// <summary>
        /// Confirms the reset password.
        /// </summary>
        /// <param name="code">The code that will be used to validate reset password request.</param>
        /// <param name="email">The email address of the user.</param>
        /// <returns>Task</returns>
        Task<UserResetPasswordResponseDto> ConfirmResetPassword(string code, string email);
    }
}