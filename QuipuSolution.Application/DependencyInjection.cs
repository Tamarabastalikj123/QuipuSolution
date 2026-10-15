using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using QuipuSolution.Application.Common.Behaviors;
using QuipuSolution.Application.Features.Books.Mapping;

namespace QuipuSolution.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            // CQRS: every command/query handler, with validation running before each handler
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            // Every AbstractValidator<T> in this assembly
            services.AddValidatorsFromAssembly(assembly);

            services.AddSingleton<IBookMapper, BookMapper>();

            return services;
        }
    }
}
