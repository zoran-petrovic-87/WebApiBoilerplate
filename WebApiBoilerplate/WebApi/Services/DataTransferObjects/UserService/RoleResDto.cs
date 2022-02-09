using System;
using WebApi.Models;

namespace WebApi.Services.DataTransferObjects.UserService;

/// <summary>
/// Data transfer object for the <see cref="Role"/> response.
/// </summary>
public class RoleResDto
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name { get; set; }
}