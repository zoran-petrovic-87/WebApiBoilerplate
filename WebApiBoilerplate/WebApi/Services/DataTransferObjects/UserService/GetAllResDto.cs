using System;
using WebApi.Services.Interfaces;

// ReSharper disable MemberCanBePrivate.Global

namespace WebApi.Services.DataTransferObjects.UserService;

/// <summary>
/// Data transfer object for the <see cref="IUserService.GetAllAsync"/> response.
/// </summary>
public class GetAllResDto
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    /// <value>
    /// The first name.
    /// </value>
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    /// <value>
    /// The last name.
    /// </value>
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    /// <value>
    /// The username.
    /// </value>
    public string Username { get; set; }

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
}