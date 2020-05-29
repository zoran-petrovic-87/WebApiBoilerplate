using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DataTransferObjects.User;
using WebApi.Helpers;
using WebApi.Helpers.Exceptions;
using WebApi.Helpers.Pagination;
using WebApi.IServices;

namespace WebApi.Controllers
{
    /// <summary>
    /// The controller for handling user related request.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Authenticates the specified user.
        /// </summary>
        /// <param name="dto">The request data.</param>
        /// <returns>The result containing user info and authorization token, if authentication was successful.
        /// </returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthenticateResponseDto>> Authenticate([FromBody] AuthenticateRequestDto dto)
        {
            try
            {
                return Ok(await _userService.Authenticate(dto));
            }
            catch (AppException ex)
            {
                return BadRequest(new ResponseMessage {Message = ex.Message});
            }
        }

        /// <summary>
        /// Creates new user.
        /// </summary>
        /// <param name="dto">The request data.</param>
        /// <returns>The HTTP response indicating if this request was successful or not.</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] UserCreateRequestDto dto)
        {
            try
            {
                var user = await _userService.Create(dto);
                return CreatedAtAction(nameof(GetById), new {id = user.Id}, user);
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
        public async Task<ActionResult<PagedResult<UserSummaryResponseDto>>> GetAll(
            [FromQuery] PaginationFilter paginationFilter)
        {
            return Ok(await _userService.GetAll(paginationFilter));
        }

        /// <summary>
        /// Gets the user by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The user details.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDetailsResponseDto>> GetById(Guid id)
        {
            try
            {
                return Ok(await _userService.GetById(id));
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
        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateRequestDto dto)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var requestedByUserId = Guid.Parse(identity.FindFirst(ClaimTypes.Name).Value);

            try
            {
                var user = await _userService.Update(requestedByUserId, id, dto);
                return CreatedAtAction(nameof(GetById), new {id = user.Id}, user);
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
        public async Task<ActionResult> Delete(Guid id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var requestedByUserId = Guid.Parse(identity.FindFirst(ClaimTypes.Name).Value);
            try
            {
                await _userService.Delete(requestedByUserId, id);
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
        public async Task<ActionResult> ConfirmEmail([FromQuery] string code)
        {
            try
            {
                await _userService.ConfirmEmail(code);
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
        public async Task<ActionResult> PasswordReset([FromBody] PasswordResetRequestDto dto)
        {
            try
            {
                await _userService.PasswordReset(dto);
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
        public async Task<ActionResult> ConfirmPasswordReset([FromQuery] string code, [FromQuery] string email)
        {
            try
            {
                return Ok(await _userService.ConfirmResetPassword(code, HttpUtility.UrlDecode(email)));
            }
            catch (AppException ex)
            {
                return BadRequest(new ResponseMessage {Message = ex.Message});
            }
        }
    }
}