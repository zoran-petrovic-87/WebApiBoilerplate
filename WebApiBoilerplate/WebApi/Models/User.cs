using System;
using System.Collections.Generic;

namespace WebApi.Models;

/// <summary>
/// The user model.
/// </summary>
public class User
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier used by external identity provider.
    /// </summary>
    /// <value>
    /// The identifier used by external identity provider.
    /// </value>
    public string ExternalId { get; set; }

    /// <summary>
    /// Gets or sets the external identity provider.
    /// </summary>
    /// <value>
    /// The external identity provider.
    /// </value>
    public string ExternalIdentityProvider { get; set; }

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
    /// Gets or sets the date when the user account was created.
    /// </summary>
    /// <value>
    /// The date when the user account was created.
    /// </value>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user that created this user.
    /// </summary>
    /// <value>
    /// The identifier of the user that created this user.
    /// </value>
    public Guid? CreatedById { get; set; }

    /// <summary>
    /// Gets or sets the user that created this user.
    /// </summary>
    /// <value>
    /// The user that created this user.
    /// </value>
    public virtual User CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the date when the user account was updated.
    /// </summary>
    /// <value>
    /// The date when the user account was updated.
    /// </value>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user that updated this user.
    /// </summary>
    /// <value>
    /// The identifier of the user that updated this user.
    /// </value>
    public Guid? UpdatedById { get; set; }

    /// <summary>
    /// Gets or sets the user that updated this user.
    /// </summary>
    /// <value>
    /// The user that updated this user.
    /// </value>
    public virtual User UpdatedBy { get; set; }

    /// <summary>
    /// Gets or sets the role identifier.
    /// </summary>
    /// <value>
    /// The role identifier.
    /// </value>
    public Guid? RoleId { get; set; }

    /// <summary>
    /// Gets or sets the role.
    /// </summary>
    /// <value>
    /// The role.
    /// </value>
    public virtual Role Role { get; set; }

    /// <summary>
    /// Gets or sets the latest date when the user was successfully logged in.
    /// </summary>
    /// <value>
    /// The latest date when the user was successfully logged in.
    /// </value>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Gets or sets the date when the user login attempt failed.
    /// </summary>
    /// <value>
    /// The date when user login attempt failed.
    /// </value>
    public DateTime? LoginFailedAt { get; set; }

    /// <summary>
    /// Gets or sets the count of consecutive failed login attempts.
    /// </summary>
    /// <value>
    /// The count of consecutive failed login attempts.
    /// </value>
    public int LoginFailedCount { get; set; }

    /// <summary>
    /// Gets or sets the unconfirmed email.
    /// </summary>
    /// <value>
    /// The unconfirmed email.
    /// </value>
    public string UnconfirmedEmail { get; set; }

    /// <summary>
    /// Gets or sets the date when unconfirmed email was created.
    /// </summary>
    /// <value>
    /// The date when unconfirmed email was created.
    /// </value>
    public DateTime? UnconfirmedEmailCreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the unconfirmed email code that will be used to confirm this email.
    /// </summary>
    /// <value>
    /// The unconfirmed email code that will be used to confirm this email.
    /// </value>
    public string UnconfirmedEmailCode { get; set; }

    /// <summary>
    /// Gets or sets the count of consecutive failed email confirmations.
    /// </summary>
    /// <value>
    /// The count of consecutive failed email confirmations.
    /// </value>
    public int UnconfirmedEmailCount { get; set; }

    /// <summary>
    /// Gets or sets the date when the reset password was created.
    /// </summary>
    /// <value>
    /// The date when the reset password was created.
    /// </value>
    public DateTime? ResetPasswordCreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the count of consecutive failed password resets.
    /// </summary>
    /// <value>
    /// The count of consecutive failed password resets.
    /// </value>
    public int ResetPasswordCount { get; set; }

    /// <summary>
    /// Gets or sets the code that will be used to confirm password reset.
    /// </summary>
    /// <value>
    /// The code that will be used to confirm password reset.
    /// </value>
    public string ResetPasswordCode { get; set; }

    /// <summary>
    /// Gets or sets the password hash.
    /// </summary>
    /// <value>
    /// The password hash.
    /// </value>
    public byte[] PasswordHash { get; set; }

    /// <summary>
    /// Gets or sets the password salt.
    /// </summary>
    /// <value>
    /// The password salt.
    /// </value>
    public byte[] PasswordSalt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this user is active.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this user is active; otherwise, <c>false</c>.
    /// </value>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the users that were created by this user.
    /// </summary>
    /// <value>
    /// The users that were created by this user.
    /// </value>
    public virtual ICollection<User> CreatedUsers { get; set; }

    /// <summary>
    /// Gets or sets the users that were updated by this user.
    /// </summary>
    /// <value>
    /// The users that were updated by this user.
    /// </value>
    public virtual ICollection<User> UpdatedUsers { get; set; }

    /// <summary>
    /// Gets or sets the roles created by this user.
    /// </summary>
    /// <value>
    /// The roles created by this user.
    /// </value>
    public virtual ICollection<Role> CreatedRoles { get; set; }

    /// <summary>
    /// Gets or sets the roles updated by this user.
    /// </summary>
    /// <value>
    /// The roles updated by this user.
    /// </value>
    public virtual ICollection<Role> UpdatedRoles { get; set; }
}