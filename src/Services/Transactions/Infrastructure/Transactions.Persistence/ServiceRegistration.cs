using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Transactions.Application.Repositories;
using Transactions.Application.Services.Persistence;
using Transactions.Persistence.Contexts;
using Transactions.Persistence.Repositories;
using Transactions.Persistence.Services;

namespace Transactions.Persistence;

public static class ServiceRegistration
{
    public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TransactionDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL"));
        });

        #region Repositories

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        #endregion

        #region Services

        services.AddScoped<ITransactionService, TransactionService>();

        #endregion
    }
}