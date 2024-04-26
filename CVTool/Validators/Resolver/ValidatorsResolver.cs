using FluentValidation;

namespace CVTool.Validators.Resolver
{
    public class ValidatorsResolver: IValidatorsResolver
    {
        private readonly IServiceProvider serviceProvider;

        public ValidatorsResolver(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IValidator<T> Resolve<T>()
        {
            return serviceProvider.GetRequiredService<IValidator<T>>();
        }
    }
}
