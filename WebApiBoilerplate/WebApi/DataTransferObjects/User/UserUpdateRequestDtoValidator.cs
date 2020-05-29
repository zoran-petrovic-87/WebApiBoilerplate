using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApi.Resources.Localization;

namespace WebApi.DataTransferObjects.User
{
    /// <summary>
    ///  Validator for the <seealso cref="UserUpdateRequestDto" />.
    /// </summary>
    public class UserUpdateRequestDtoValidator : AbstractValidator<UserUpdateRequestDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserUpdateRequestDtoValidator"/> class.
        /// </summary>
        /// <param name="l">The localizer.</param>
        public UserUpdateRequestDtoValidator(IStringLocalizer<Translation> l)
        {
            RuleFor(x => x.Username.NewValue).Username(l).When(y => y.Username != null);
            RuleFor(x => x.FirstName.NewValue).FirstName(l).When(y => y.FirstName != null);
            RuleFor(x => x.LastName.NewValue).LastName(l).When(y => y.LastName != null);
            RuleFor(x => x.Email.NewValue).Email(l).When(y => y.Email != null);
            RuleFor(x => x.Password.NewValue).Password(l).When(y => y.Password != null);
        }
    }
}