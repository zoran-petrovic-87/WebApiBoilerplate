using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helpers.Exceptions;

namespace WebApi.Helpers;

/// <summary>
/// The authentication helper interface.
/// </summary>
public interface IAuthHelper
{
    /// <summary>
    /// Gets the identifier of the user that made current request.
    /// It will use the first "Name" claim as identifier and cast it to GUID.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <returns>The identifier of the user that made current request.</returns>
    Guid GetUserId(ControllerBase controller);
}

/// <inheritdoc />
public class AuthHelper : IAuthHelper
{
    /// <inheritdoc />
    public Guid GetUserId(ControllerBase controller)
    {
        if (controller == null) throw new ArgumentNullException(nameof(controller));
        var identity = controller.HttpContext.User.Identity as ClaimsIdentity;
        if (identity == null) throw new UnauthorizedException();
        var nameClaim = identity.FindFirst(ClaimTypes.Name);
        if (nameClaim == null) throw new ApplicationException("Cannot get claim 'Name'.");
        return Guid.Parse(nameClaim.Value);
    }
}