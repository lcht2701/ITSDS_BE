using FluentValidation;

namespace API.Validations
{
    public static class CustomValidations
    {
        public static IRuleBuilderOptions<T, string> UniqueEmail<T>(this IRuleBuilder<T, string> ruleBuilder, Func<string, bool> isUnique)
        {
            return ruleBuilder.Must(email => isUnique(email))
                              .WithMessage("Email Address is already in use.");
        }
    }
}
