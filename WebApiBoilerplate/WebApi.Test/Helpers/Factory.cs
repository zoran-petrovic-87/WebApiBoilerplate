using System;
using System.Collections.Generic;
using WebApi.Data;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Test.Helpers;

/// <summary>
/// Class used to generate fixtures.
/// </summary>
public class Factory
{
    /// <summary>
    /// Gets or sets the context.
    /// </summary>
    public AppDbContext Context { get; set; }

    private readonly IPasswordHelper _passwordHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="Factory"/> class.
    /// It will execute the <see cref="InitializeDatabase"/> method.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="passwordHelper">The password helper.</param>
    public Factory(AppDbContext context, IPasswordHelper passwordHelper)
    {
        Context = context;
        _passwordHelper = passwordHelper;
        InitializeDatabase();
        Context.SaveChanges();
    }

    /// <summary>
    /// Creates list of users.
    /// </summary>
    /// <param name="count">The number of users to create.</param>
    /// <param name="password">The password that will be set for every user.</param>
    /// <returns>The list of users.</returns>
    public List<User> CreateUsers(int count, string password)
    {
        var users = new List<User>();
        for (var i = 0; i < count; i++)
        {
            var (passwordHash, passwordSalt) = _passwordHelper.CreateHash(password);

            users.Add(new User
            {
                Id = Guid.NewGuid(),
                Username = "user" + i,
                GivenName = "First name " + i,
                FamilyName = "Last name " + i,
                Email = $"email{i}@example.com",
                CreatedAt = DateTime.UtcNow,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                IsActive = true
            });
        }

        Context.Users.AddRange(users);
        Context.SaveChanges();
        return users;
    }

    /// <summary>
    /// Generates 
    /// </summary>
    private void InitializeDatabase()
    {
        // Here we can add data to the database that is common for each test case.
    }
}