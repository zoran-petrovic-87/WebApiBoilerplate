using System;
using System.Threading.Tasks;
using WebApi.Helpers.Pagination;
using DTO = WebApi.Controllers.DataTransferObjects.User;

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
        /// <param name="dto">The <see cref="DTO.Authenticate.RequestDto"/> data transfer object.</param>
        /// <returns>The user.</returns>
        Task<DTO.Authenticate.ResponseDto> AuthenticateAsync(DTO.Authenticate.RequestDto dto);

        /// <summary>
        /// Creates the new user.
        /// </summary>
        /// <param name="dto">The <see cref="WebApi.Controllers.DataTransferObjects.User.Register.RequestDto"/> data transfer object.</param>
        /// <returns>The user details.</returns>
        Task<DTO.Details.ResponseDto> RegisterAsync(DTO.Register.RequestDto dto);

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <param name="paginationFilter">The pagination filter.</param>
        /// <returns>The list of users.</returns>
        Task<PagedResult<DTO.GetAll.ResponseDto>> GetAllAsync(PaginationFilter paginationFilter);

        /// <summary>
        /// Gets the user by identifier.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>The user details.</returns>
        Task<DTO.Details.ResponseDto> GetDetailsAsync(Guid id);

        /// <summary>
        /// Updates the specified user.
        /// </summary>
        /// <param name="requestedByUserId">The identifier of the user that made update request.</param>
        /// <param name="userId">The identifier of the user that should be updated.</param>
        /// <param name="dto">The <see cref="DTO.Update.RequestDto"/> data transfer object.</param>
        /// <returns>The user details.</returns>
        Task<DTO.Details.ResponseDto> UpdateAsync(Guid requestedByUserId, Guid userId, DTO.Update.RequestDto dto);

        /// <summary>
        /// Deletes the specified user.
        /// </summary>
        /// <param name="requestedByUserId">The identifier of the user that made the delete request.</param>
        /// <param name="id">The identifier of the user that should be deleted.</param>
        /// <returns>Task.</returns>
        Task DeleteAsync(Guid requestedByUserId, Guid id);

        /// <summary>
        /// Confirms the email.
        /// </summary>
        /// <param name="code">The code that will be used to validate email address.</param>
        /// <returns>Task.</returns>
        Task ConfirmEmailAsync(string code);

        /// <summary>
        /// Resets te password by sending an email to the user.
        /// </summary>
        /// <param name="dto">The <see cref="DTO.PasswordReset.RequestDto"/> data transfer object.</param>
        /// <returns>Task.</returns>
        Task PasswordResetAsync(DTO.PasswordReset.RequestDto dto);

        /// <summary>
        /// Confirms the reset password.
        /// </summary>
        /// <param name="code">The code that will be used to validate reset password request.</param>
        /// <param name="email">The email address of the user.</param>
        /// <returns>Task</returns>
        Task<DTO.ConfirmResetPassword.ResponseDto> ConfirmResetPasswordAsync(string code, string email);
    }
}