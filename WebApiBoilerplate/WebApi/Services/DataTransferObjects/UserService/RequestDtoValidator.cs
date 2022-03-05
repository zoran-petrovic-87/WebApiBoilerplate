using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApi.Resources.Localization;

namespace WebApi.Services.DataTransferObjects.UserService;

/// <summary>
/// Validator for the <see cref="UpdateReqDto" />.
/// </summary>
public class UpdateAsyncReqDtoValidator : AbstractValidator<UpdateReqDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateAsyncReqDtoValidator"/> class.
    /// </summary>
    /// <param name="l">The localizer.</param>
    public UpdateAsyncReqDtoValidator(IStringLocalizer<Translation> l)
    {
        RuleFor(x => x.Username.NewValue).Username(l).When(y => y.Username != null);
        RuleFor(x => x.GivenName.NewValue).GivenName(l).When(y => y.GivenName != null);
        RuleFor(x => x.FamilyName.NewValue).FamilyName(l).When(y => y.FamilyName != null);
        RuleFor(x => x.Email.NewValue).Email(l).When(y => y.Email != null);
        RuleFor(x => x.Password.NewValue).Password(l).When(y => y.Password != null);
    }
}