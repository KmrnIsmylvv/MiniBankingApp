using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Transactions.Application.Validators;

namespace Transactions.Application;

public static class ServiceRegistration
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();

        services.AddValidatorsFromAssembly(typeof(AddTransactionValidator).Assembly);

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }
}