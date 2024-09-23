﻿using MediatR;
using FluentValidation;
using BuildingBlocks.CQRS;

namespace BuildingBlocks.Behaviours
{
    public class ValidatorBehaviour<TRequest, TResponse>
        (IEnumerable<IValidator<TRequest>> validators)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommand<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TRequest>(request);

            var ValidationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        
            var failures = 
                ValidationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Any())
                {
                throw new ValidationException(failures);
            }

            return await next();
        }
    }
}
