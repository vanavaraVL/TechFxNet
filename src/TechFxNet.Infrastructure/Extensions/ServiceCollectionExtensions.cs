using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using TechFxNet.Infrastructure.Repositories;

namespace TechFxNet.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TechDbContext");

        services.AddDbContext<TechDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString);
        });

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IJournalRepository, JournalRepository>();
        services.AddScoped<ITreeNodeRepository, TreeNodeRepository>();

        return services;
    }
}