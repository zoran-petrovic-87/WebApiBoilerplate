using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApi.Resources.Localization;

namespace WebApi.DataTransferObjects.User
{
    /// <summary>
    /// Validator for the <seealso cref="UserCreateRequestDto" />.
    /// </summary>
    public class UserCreateRequestDtoValidator : AbstractValidator<UserCreateRequestDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserCreateRequestDtoValidator"/> class.
        /// </summary>
        /// <param name="l">The localizer.</param>
        public UserCreateRequestDtoValidator(IStringLocalizer<Translation> l)
        {
            RuleFor(x => x.Username).Username(l);
            RuleFor(x => x.FirstName).FirstName(l);
            RuleFor(x => x.LastName).LastName(l);
            RuleFor(x => x.Email).Email(l);
            RuleFor(x => x.Password).Password(l);
        }
    }
}