using FluentValidation;
using Microsoft.Extensions.Localization;

namespace WebApi.Services.DataTransferObjects;

/// <summary>
/// The extension methods for reusable validation rules.
/// </summary>
public static class ValidatorExtensions
{
    /// <summary>
    /// Validates the username.
    /// </summary>
    /// <typeparam name="T">Type of the object.</typeparam>
    /// <param name="ruleBuilderOptions">The rule builder options.</param>
    /// <param name="l">The localizer.</param>
    /// <returns>The rule builder options.</returns>
    public static IRuleBuilderOptions<T, string> Username<T>(this IRuleBuilder<T, string> ruleBuilderOptions,
        IStringLocalizer l)
    {
        return ruleBuilderOptions
            .NotEmpty().WithMessage(l["Username cannot be empty."])
            .MinimumLength(3)
            .WithMessage(string.Format(l["Username must be at least {0} characters long."], 3))
            .MaximumLength(20)
            .WithMessage(string.Format(l["Username cannot be longer than {0} characters."], 20));
    }

    /// <summary>
    /// Validates the password.
    /// </summary>
    /// <typeparam name="T">Type of the object.</typeparam>
    /// <param name="ruleBuilderOptions">The rule builder options.</param>
    /// <param name="l">The localizer.</param>
    /// <returns>The rule builder options.</returns>
    public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilderOptions,
        IStringLocalizer l)
    {
        return ruleBuilderOptions
            .NotEmpty().WithMessage(l["Password cannot be empty."])
            .MinimumLength(6)
            .WithMessage(string.Format(l["Password must be at least {0} characters long."], 6))
            .Matches("[A-Z]").WithMessage(l["Password must contain at least one uppercase letter."])
            .Matches("[a-z]").WithMessage(l["Password must contain at least one lowercase letter."])
            .Matches("[0-9]").WithMessage(l["Password must contain at least one digit."]);
    }

    /// <summary>
    /// Validates the given name.
    /// </summary>
    /// <typeparam name="T">Type of the object.</typeparam>
    /// <param name="ruleBuilderOptions">The rule builder options.</param>
    /// <param name="l">The localizer.</param>
    /// <returns>The rule builder options.</returns>
    public static IRuleBuilderOptions<T, string> GivenName<T>(this IRuleBuilder<T, string> ruleBuilderOptions,
        IStringLocalizer l)
    {
        return ruleBuilderOptions
            .NotEmpty().WithMessage(l["Given name cannot be empty."])
            .MinimumLength(2)
            .WithMessage(string.Format(l["Given name must be at least {0} characters long."], 2))
            .MaximumLength(30)
            .WithMessage(string.Format(l["Given name cannot be longer than {0} characters."], 30));
    }

    /// <summary>
    /// Validates the family name.
    /// </summary>
    /// <typeparam name="T">The rule builder options.</typeparam>
    /// <param name="ruleBuilderOptions">The rule builder options.</param>
    /// <param name="l">The localizer.</param>
    /// <returns>The rule builder options.</returns>
    public static IRuleBuilderOptions<T, string> FamilyName<T>(this IRuleBuilder<T, string> ruleBuilderOptions,
        IStringLocalizer l)
    {
        return ruleBuilderOptions
            .NotEmpty().WithMessage(l["Family name cannot be empty."])
            .MinimumLength(2)
            .WithMessage(string.Format(l["Family name must be at least {0} characters long."], 2))
            .MaximumLength(30)
            .WithMessage(string.Format(l["Family name cannot be longer than {0} characters."], 30));
    }

    /// <summary>
    /// Validates the email.
    /// </summary>
    /// <typeparam name="T">The rule builder options.</typeparam>
    /// <param name="ruleBuilderOptions">The rule builder options.</param>
    /// <param name="l">The localizer.</param>
    /// <returns>The rule builder options.</returns>
    public static IRuleBuilderOptions<T, string> Email<T>(this IRuleBuilder<T, string> ruleBuilderOptions,
        IStringLocalizer l)
    {
        return ruleBuilderOptions.EmailAddress().WithMessage(l["Invalid email address."]);
    }
}