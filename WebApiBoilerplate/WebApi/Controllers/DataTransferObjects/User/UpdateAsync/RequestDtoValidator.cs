using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApi.Resources.Localization;

namespace WebApi.Controllers.DataTransferObjects.User.UpdateAsync
{
    /// <summary>
    /// Validator for the <seealso cref="RequestDto" />.
    /// </summary>
    public class UserUpdateRequestDtoValidator : AbstractValidator<RequestDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserUpdateRequestDtoValidator"/> class.
        /// </summary>
        /// <param name="l">The localizer.</param>
        public UserUpdateRequestDtoValidator(IStringLocalizer<Translation> l)
        {
            RuleFor(x => x.Username.NewValue).Username(l).When(y => y.Username != null);
            RuleFor(x => x.GivenName.NewValue).GivenName(l).When(y => y.GivenName != null);
            RuleFor(x => x.FamilyName.NewValue).FamilyName(l).When(y => y.FamilyName != null);
            RuleFor(x => x.Email.NewValue).Email(l).When(y => y.Email != null);
            RuleFor(x => x.Password.NewValue).Password(l).When(y => y.Password != null);
        }
    }
}