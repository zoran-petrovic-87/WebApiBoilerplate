using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helpers;
using WebApi.Helpers.Exceptions;
using WebApi.Helpers.Pagination;
using WebApi.Services.DataTransferObjects.UserService;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers;

/// <summary>
/// The controller for handling user related requests.
/// <c>CreatedAtAction</c> will not work if you use "Async" suffix in controller action name.
/// You must specify <c>ActionName</c> attribute to fix this known issue.
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
[Authorize]
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthHelper _authHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserController"/> class.
    /// </summary>
    /// <param name="userService">The user service.</param>
    /// <param name="authHelper">The authentication helper.</param>
    public UserController(IUserService userService, IAuthHelper authHelper)
    {
        _userService = userService;
        _authHelper = authHelper;
    }

    /// <summary>
    /// Authenticates the user.
    /// </summary>
    /// <param name="dto">The request data.</param>
    /// <returns>The result containing user info and authorization token, if authentication was successful.
    /// </returns>
    [AllowAnonymous]
    [HttpPost("authenticate")]
    [ActionName(nameof(AuthenticateAsync))]
    public async Task<ActionResult<AuthenticateResDto>> AuthenticateAsync([FromBody] AuthenticateReqDto dto)
    {
        try
        {
            return Ok(await _userService.AuthenticateAsync(dto));
        }
        catch (AppException ex)
        {
            return BadRequest(new ResponseMessage {Message = ex.Message});
        }
    }

    /// <summary>
    /// Registers new user.
    /// </summary>
    /// <param name="dto">The request data.</param>
    /// <returns>The HTTP response indicating if this request was successful or not.</returns>
    [AllowAnonymous]
    [HttpPost("register")]
    [ActionName(nameof(RegisterAsync))]
    public async Task<ActionResult> RegisterAsync([FromBody] RegisterReqDto dto)
    {
        try
        {
            var user = await _userService.RegisterAsync(dto);
            return CreatedAtAction(nameof(GetDetailsAsync), new {id = user.Id}, user);
        }
        catch (EmailNotSentException ex)
        {
            return StatusCode((int) HttpStatusCode.BadGateway, new ResponseMessage {Message = ex.Message});
        }
        catch (AppException ex)
        {
            return BadRequest(new ResponseMessage {Message = ex.Message});
        }
    }

    /// <summary>
    /// Creates new user. Only users with admin role can access this endpoint.
    /// </summary>
    /// <param name="dto">The request data.</param>
    /// <returns>The HTTP response indicating if this request was successful or not.</returns>
    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost]
    [ActionName(nameof(CreateAsync))]
    public async Task<ActionResult> CreateAsync([FromBody] RegisterReqDto dto)
    {
        try
        {
            var user = await _userService.CreateAsync(_authHelper.GetUserId(this), dto);
            return CreatedAtAction(nameof(GetDetailsAsync), new {id = user.Id}, user);
        }
        catch (EmailNotSentException ex)
        {
            return StatusCode((int) HttpStatusCode.BadGateway, new ResponseMessage {Message = ex.Message});
        }
        catch (AppException ex)
        {
            return BadRequest(new ResponseMessage {Message = ex.Message});
        }
    }

    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <param name="paginationFilter">The pagination filter.</param>
    /// <returns>The paginated HTTP response with list of users.</returns>
    [HttpGet]
    [ActionName(nameof(GetAllAsync))]
    public async Task<ActionResult<PagedResult<GetAllResDto>>> GetAllAsync(
        [FromQuery] PaginationFilter paginationFilter)
    {
        return Ok(await _userService.GetAllAsync(paginationFilter));
    }

    /// <summary>
    /// Gets the user details.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>The user details.</returns>
    [HttpGet("{id}")]
    [ActionName(nameof(GetDetailsAsync))]
    public async Task<ActionResult<GetDetailsResDto>> GetDetailsAsync(Guid id)
    {
        try
        {
            return Ok(await _userService.GetDetailsAsync(id));
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new ResponseMessage {Message = ex.Message});
        }
        catch (AppException ex)
        {
            return BadRequest(new ResponseMessage {Message = ex.Message});
        }
    }

    /// <summary>
    /// Updates the specified user.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="dto">The request data.</param>
    /// <returns>HTTP response indicating if this request was successful or not.</returns>
    [HttpPatch("{id}")]
    [ActionName(nameof(UpdateAsync))]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateReqDto dto)
    {
        try
        {
            var user = await _userService.UpdateAsync(id, _authHelper.GetUserId(this), dto);
            return CreatedAtAction(nameof(GetDetailsAsync), new {id = user.Id}, user);
        }
        catch (ForbiddenException ex)
        {
            return StatusCode((int) HttpStatusCode.Forbidden, new ResponseMessage {Message = ex.Message});
        }
        catch (EmailNotSentException ex)
        {
            return StatusCode((int) HttpStatusCode.BadGateway, new ResponseMessage {Message = ex.Message});
        }
        catch (AppException ex)
        {
            return BadRequest(new ResponseMessage {Message = ex.Message});
        }
    }

    /// <summary>
    /// Deletes the specified user.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <returns>HTTP response indicating if this request was successful or not.</returns>
    [HttpDelete("{id}")]
    [ActionName(nameof(DeleteAsync))]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        try
        {
            await _userService.DeleteAsync(id, _authHelper.GetUserId(this));
            return NoContent();
        }
        catch (ForbiddenException ex)
        {
            return StatusCode((int) HttpStatusCode.Forbidden, new ResponseMessage {Message = ex.Message});
        }
    }

    /// <summary>
    /// Confirms email address.
    /// </summary>
    /// <param name="code">The confirmation code.</param>
    /// <returns>HTTP response indicating if this request was successful or not.</returns>
    [AllowAnonymous]
    [HttpGet("ConfirmEmail")]
    [ActionName(nameof(ConfirmEmailAsync))]
    public async Task<ActionResult> ConfirmEmailAsync([FromQuery] string code)
    {
        try
        {
            await _userService.ConfirmEmailAsync(code);
            return NoContent();
        }
        catch (AppException ex)
        {
            return BadRequest(new ResponseMessage {Message = ex.Message});
        }
    }

    /// <summary>
    /// Resets the password by sending email.
    /// </summary>
    /// <param name="dto">The request data.</param>
    /// <returns>HTTP response indicating if this request was successful or not.</returns>
    [AllowAnonymous]
    [HttpPost("PasswordReset")]
    [ActionName(nameof(PasswordResetAsync))]
    public async Task<ActionResult> PasswordResetAsync([FromBody] PasswordResetReqDto dto)
    {
        try
        {
            await _userService.PasswordResetAsync(dto);
            return NoContent();
        }
        catch (EmailNotSentException ex)
        {
            return StatusCode((int) HttpStatusCode.BadGateway, new ResponseMessage {Message = ex.Message});
        }
        catch (AppException ex)
        {
            return BadRequest(new ResponseMessage {Message = ex.Message});
        }
    }

    /// <summary>
    /// Confirms password reset.
    /// </summary>
    /// <param name="code">The confirmation code.</param>
    /// <param name="email">The email address of the user that made this request.</param>
    /// <returns>HTTP response indicating if this request was successful or not.</returns>
    [AllowAnonymous]
    [HttpGet("ConfirmPasswordReset")]
    [ActionName(nameof(ConfirmPasswordResetAsync))]
    public async Task<ActionResult> ConfirmPasswordResetAsync([FromQuery] string code, [FromQuery] string email)
    {
        try
        {
            return Ok(await _userService.ConfirmResetPasswordAsync(code, HttpUtility.UrlDecode(email)));
        }
        catch (AppException ex)
        {
            return BadRequest(new ResponseMessage {Message = ex.Message});
        }
    }
}