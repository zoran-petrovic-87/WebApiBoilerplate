using System;
using System.Collections.Generic;

namespace WebApi.Models;

/// <summary>
/// The user role.
/// </summary>
public class Role
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the date when the role was created.
    /// </summary>
    /// <value>
    /// The date when the role was created.
    /// </value>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user that created this role.
    /// </summary>
    /// <value>
    /// The identifier of the user that created this role.
    /// </value>
    public Guid CreatedById { get; set; }

    /// <summary>
    /// Gets or sets the user that created this role.
    /// </summary>
    /// <value>
    /// The user that created this role.
    /// </value>
    public virtual User CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the date when the role was updated.
    /// </summary>
    /// <value>
    /// The date when the role was updated.
    /// </value>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user that updated this role.
    /// </summary>
    /// <value>
    /// The identifier of the user that updated this role.
    /// </value>
    public Guid? UpdatedById { get; set; }

    /// <summary>
    /// Gets or sets the user that updated this role.
    /// </summary>
    /// <value>
    /// The user that updated this role.
    /// </value>
    public virtual User UpdatedBy { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>
    /// The description.
    /// </value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the users assigned to this role.
    /// </summary>
    /// <value>
    /// The users assigned to this role.
    /// </value>
    public virtual ICollection<User> Users { get; set; }
}