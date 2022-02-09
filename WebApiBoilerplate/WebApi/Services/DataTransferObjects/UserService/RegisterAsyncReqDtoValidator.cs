using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApi.Resources.Localization;

namespace WebApi.Services.DataTransferObjects.UserService;

/// <summary>
/// Validator for the <see cref="RegisterAsyncReqDto" />.
/// </summary>
public class RegisterAsyncReqDtoValidator : AbstractValidator<RegisterAsyncReqDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterAsyncReqDtoValidator"/> class.
    /// </summary>
    /// <param name="l">The localizer.</param>
    public RegisterAsyncReqDtoValidator(IStringLocalizer<Translation> l)
    {
        RuleFor(x => x.Username).Username(l);
        RuleFor(x => x.GivenName).GivenName(l);
        RuleFor(x => x.FamilyName).FamilyName(l);
        RuleFor(x => x.Email).Email(l);
        RuleFor(x => x.Password).Password(l);
    }
}