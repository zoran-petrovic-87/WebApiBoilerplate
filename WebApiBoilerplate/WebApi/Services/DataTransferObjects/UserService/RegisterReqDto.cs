using WebApi.Services.Interfaces;

namespace WebApi.Services.DataTransferObjects.UserService;

/// <summary>
/// Data transfer object for the <see cref="IUserService.RegisterAsync"/> request.
/// </summary>
public class RegisterReqDto
{
    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    /// <value>
    /// The username.
    /// </value>
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the given name.
    /// </summary>
    /// <value>
    /// The given name.
    /// </value>
    public string GivenName { get; set; }

    /// <summary>
    /// Gets or sets the family name.
    /// </summary>
    /// <value>
    /// The family name.
    /// </value>
    public string FamilyName { get; set; }

    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    /// <value>
    /// The email.
    /// </value>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    /// <value>
    /// The password.
    /// </value>
    public string Password { get; set; }
}