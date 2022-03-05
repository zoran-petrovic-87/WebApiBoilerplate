using WebApi.Services.Interfaces;

namespace WebApi.Services.DataTransferObjects.UserService;

/// <summary>
/// Data transfer object for the <see cref="IUserService.UpdateAsync"/> request.
/// </summary>
public class UpdateReqDto
{
    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    /// <value>
    /// The username.
    /// </value>
    public UpdateStringField Username { get; set; }

    /// <summary>
    /// Gets or sets the given name.
    /// </summary>
    /// <value>
    /// The given name.
    /// </value>
    public UpdateStringField GivenName { get; set; }

    /// <summary>
    /// Gets or sets the family name.
    /// </summary>
    /// <value>
    /// The family name.
    /// </value>
    public UpdateStringField FamilyName { get; set; }

    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    /// <value>
    /// The email.
    /// </value>
    public UpdateStringField Email { get; set; }

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    /// <value>
    /// The password.
    /// </value>
    public UpdateStringField Password { get; set; }
}