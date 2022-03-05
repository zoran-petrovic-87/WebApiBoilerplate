using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApi.Resources.Localization;

namespace WebApi.Services.DataTransferObjects.UserService;

/// <summary>
/// Validator for the <see cref="AuthenticateReqDto" />.
/// </summary>
public class AuthenticateReqDtoValidator : AbstractValidator<AuthenticateReqDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticateReqDtoValidator"/> class.
    /// </summary>
    /// <param name="l">The localizer.</param>
    public AuthenticateReqDtoValidator(IStringLocalizer<Translation> l)
    {
        RuleFor(x => x.Username).Username(l);
        RuleFor(x => x.Password).Password(l);
    }
}