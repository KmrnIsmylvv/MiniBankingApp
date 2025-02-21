using Customers.Application.Validators.Customer;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace Customers.Application;

public static class ServiceRegistration
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();

        services.AddValidatorsFromAssembly(typeof(AddCustomerValidator).Assembly);

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }
}