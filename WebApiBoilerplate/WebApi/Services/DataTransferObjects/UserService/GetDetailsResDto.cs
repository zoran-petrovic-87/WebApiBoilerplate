using System;
using WebApi.Services.Interfaces;

// ReSharper disable MemberCanBePrivate.Global

namespace WebApi.Services.DataTransferObjects.UserService;

/// <summary>
/// Data transfer object for the <see cref="IUserService.GetDetailsAsync"/> response.
/// </summary>
public class GetDetailsResDto
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public Guid Id { get; set; }

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
    /// Gets or sets the role data transfer object.
    /// </summary>
    /// <value>
    /// The role data transfer object.
    /// </value>
    public RoleResDto Role { get; set; }

    /// <summary>
    /// Gets or sets the date when this user was created.
    /// </summary>
    /// <value>
    /// The date when this user was created.
    /// </value>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date when this user was updated.
    /// </summary>
    /// <value>
    /// The date when this user was updated.
    /// </value>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date of the last successful login.
    /// </summary>
    /// <value>
    /// The date of the last successful login.
    /// </value>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this user is active.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this user is active; otherwise, <c>false</c>.
    /// </value>
    public bool IsActive { get; set; }
}