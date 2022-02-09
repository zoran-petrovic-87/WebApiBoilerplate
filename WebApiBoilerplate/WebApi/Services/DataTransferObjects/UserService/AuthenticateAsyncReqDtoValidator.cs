using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApi.Resources.Localization;

namespace WebApi.Services.DataTransferObjects.UserService;

/// <summary>
/// Validator for the <see cref="AuthenticateAsyncReqDto" />.
/// </summary>
public class AuthenticateAsyncReqDtoValidator : AbstractValidator<AuthenticateAsyncReqDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticateAsyncReqDtoValidator"/> class.
    /// </summary>
    /// <param name="l">The localizer.</param>
    public AuthenticateAsyncReqDtoValidator(IStringLocalizer<Translation> l)
    {
        RuleFor(x => x.Username).Username(l);
        RuleFor(x => x.Password).Password(l);
    }
}