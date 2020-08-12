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
        /// <param name="dto">The <see cref="WebApi.Controllers.DataTransferObjects.User.AuthenticateAsync.RequestDto"/> data transfer object.</param>
        /// <returns>The user.</returns>
        Task<DTO.AuthenticateAsync.ResponseDto> AuthenticateAsync(DTO.AuthenticateAsync.RequestDto dto);

        /// <summary>
        /// Creates the new user.
        /// </summary>
        /// <param name="dto">The <see cref="WebApi.Controllers.DataTransferObjects.User.RegisterAsync.RequestDto"/> data transfer object.</param>
        /// <returns>The user details.</returns>
        Task<DTO.GetDetailsAsync.ResponseDto> RegisterAsync(DTO.RegisterAsync.RequestDto dto);

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
        Task<DTO.GetDetailsAsync.ResponseDto> GetDetailsAsync(Guid id);

        /// <summary>
        /// Updates the specified user.
        /// </summary>
        /// <param name="requestedByUserId">The identifier of the user that made update request.</param>
        /// <param name="userId">The identifier of the user that should be updated.</param>
        /// <param name="dto">The <see cref="WebApi.Controllers.DataTransferObjects.User.UpdateAsync.RequestDto"/> data transfer object.</param>
        /// <returns>The user details.</returns>
        Task<DTO.GetDetailsAsync.ResponseDto> UpdateAsync(Guid requestedByUserId, Guid userId, DTO.UpdateAsync.RequestDto dto);

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
        /// <param name="dto">The <see cref="WebApi.Controllers.DataTransferObjects.User.PasswordResetAsync.RequestDto"/> data transfer object.</param>
        /// <returns>Task.</returns>
        Task PasswordResetAsync(DTO.PasswordResetAsync.RequestDto dto);

        /// <summary>
        /// Confirms the reset password.
        /// </summary>
        /// <param name="code">The code that will be used to validate reset password request.</param>
        /// <param name="email">The email address of the user.</param>
        /// <returns>Task</returns>
        Task<DTO.ConfirmResetPasswordAsync.ResponseDto> ConfirmResetPasswordAsync(string code, string email);
    }
}