using FluentValidation;
using Microsoft.Extensions.Localization;
using WebApi.Resources.Localization;

namespace WebApi.Controllers.DataTransferObjects.User.RegisterAsync
{
    /// <summary>
    /// Validator for the <seealso cref="RequestDto" />.
    /// </summary>
    public class RequestDtoValidator : AbstractValidator<RequestDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestDtoValidator"/> class.
        /// </summary>
        /// <param name="l">The localizer.</param>
        public RequestDtoValidator(IStringLocalizer<Translation> l)
        {
            RuleFor(x => x.Username).Username(l);
            RuleFor(x => x.GivenName).GivenName(l);
            RuleFor(x => x.FamilyName).FamilyName(l);
            RuleFor(x => x.Email).Email(l);
            RuleFor(x => x.Password).Password(l);
        }
    }
}