using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyFinlys.Domain.Repositories;
using MyFinlys.Infrastructure.Context;
using MyFinlys.Infrastructure.Repositories;

namespace MyFinlys.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Registrar DbContext com Postgres (pegando connection string do appsettings.json)
            services.AddDbContext<MyFinlysDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                       .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            );

            // Registrar repositórios - exemplo para Account
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBankRepository, BankRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IRegisterRepository, RegisterRepository>();

            return services;
        }
    }
}
