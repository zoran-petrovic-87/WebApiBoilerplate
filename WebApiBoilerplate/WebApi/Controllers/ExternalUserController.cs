using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services.DataTransferObjects.UserService;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers;

/// <summary>
/// The controller for handling external user related request.
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
[ApiController]
[Route("[controller]")]
public class ExternalUserController : ControllerBase
{
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalUserController"/> class.
    /// </summary>
    /// <param name="userService">The user service.</param>
    public ExternalUserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Authenticates the user with Google account. Must be called from web browser!
    /// </summary>
    /// <returns></returns>
    [HttpGet("loginWithGoogle")]
    public async Task LoginWithGoogle()
    {
        await HttpContext.ChallengeAsync("oidc-google", new AuthenticationProperties
        {
            RedirectUri = "/ExternalUser/loginWithGoogleToken"
        });
    }

    /// <summary>
    /// Returns new authentication token if user is successfully authenticated with Google.
    /// </summary>
    /// <returns>The authentication token.</returns>
    [Authorize(AuthenticationSchemes = "oidc-google")]
    [HttpGet("loginWithGoogleToken")]
    public async Task<ActionResult<AuthenticateResDto>> LoginWithGoogleToken()
    {
        if (User.Identity is not { IsAuthenticated: true }) return BadRequest();

        var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var givenName = User.FindFirst(ClaimTypes.GivenName)?.Value;
        var familyName = User.FindFirst(ClaimTypes.Surname)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var dto = await _userService.AuthenticateExternalAsync("Google", nameIdentifier, email, givenName,
            familyName);
        return Ok(dto);
    }
}