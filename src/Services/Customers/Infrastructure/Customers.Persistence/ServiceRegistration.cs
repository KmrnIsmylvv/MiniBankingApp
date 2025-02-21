using Customers.Application.Repositories;
using Customers.Application.Services.Persistence;
using Customers.Persistence.Contexts;
using Customers.Persistence.Repositories;
using Customers.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Customers.Persistence;

public static class ServiceRegistration
{
    public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CustomerDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL"));
        });

        #region Repositories

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        #endregion

        #region Services

        services.AddScoped<ICustomerService, CustomerService>();

        #endregion
    }
}