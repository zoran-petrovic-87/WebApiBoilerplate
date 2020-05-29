using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApi.Resources.Localization;

namespace WebApi.DataTransferObjects.User
{
    /// <summary>
    /// Validator for the <seealso cref="AuthenticateRequestDto" />.
    /// </summary>
    public class AuthenticateRequestDtoValidator : AbstractValidator<AuthenticateRequestDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticateRequestDtoValidator"/> class.
        /// </summary>
        /// <param name="l">The localizer.</param>
        public AuthenticateRequestDtoValidator(IStringLocalizer<Translation> l)
        {
            RuleFor(x => x.Username).Username(l);
            RuleFor(x => x.Password).Password(l);
        }
    }
}