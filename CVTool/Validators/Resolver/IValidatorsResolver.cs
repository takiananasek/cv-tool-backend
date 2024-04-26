using FluentValidation;

namespace CVTool.Validators.Resolver
{
    public interface IValidatorsResolver
    {
        IValidator<T> Resolve<T>();
    }
}