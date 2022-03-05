using WebApi.Services.Interfaces;

namespace WebApi.Services.DataTransferObjects.UserService;

/// <summary>
/// Data transfer object for the <see cref="IUserService.PasswordResetAsync"/> request.
/// </summary>
public class PasswordResetReqDto
{
    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    /// <value>
    /// The email.
    /// </value>
    public string Email { get; set; }
}