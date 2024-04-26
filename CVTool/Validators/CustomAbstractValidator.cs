using FluentValidation;
using FluentValidation.Results;

namespace CVTool.Validators
{
    public class CustomAbstractValidator<T>: AbstractValidator<T>
    {
        protected override void RaiseValidationException(ValidationContext<T> context, ValidationResult result)
        {
            var firstError = result.Errors[0];

            var ex = new Exceptions.ValidationException(firstError.ErrorMessage);

            throw ex;
        }
    }
}
