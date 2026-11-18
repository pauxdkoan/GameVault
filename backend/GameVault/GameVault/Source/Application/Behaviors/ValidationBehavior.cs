using FluentValidation;
using MediatR;
using System.Text.RegularExpressions;

namespace GameVault.Source.Application.Behaviors
{
    public sealed class ValidationBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {

        private readonly IEnumerable<IValidator<TRequest>> _validators;
        
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) 
        {
            _validators = validators;
        }
        
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {

            if (!_validators.Any()) 
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll
                (_validators.Select(validator =>
                validator.ValidateAsync(context, cancellationToken)));

            var errors = validationResults
                .SelectMany(result => result.Errors)
                .Where(failure => failure is not null)
                .GroupBy(failure => failure.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(failure => failure.ErrorMessage).Distinct().ToArray());

            if (errors.Any())
            {
                throw new Exceptions.ValidationException(errors);
            }

            return await next();
        }
    }
}
