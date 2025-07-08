using MyFinlys.Infrastructure.IoC; 
using MyFinlys.Infrastructure.Repositories;
using MyFinlys.Application.Services;
using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Domain.Repositories;
using MyFinlys.Api.Services;

namespace MyFinlys.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddInfrastructure(config);

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IBankRepository, BankRepository>();
            services.AddScoped<IRegisterRepository, RegisterRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IEventWeeklyRepository, EventWeeklyRepository>();
            services.AddScoped<IEventMonthlyRepository, EventMonthlyRepository>();
            services.AddScoped<IEventBiweeklyRepository, EventBiweeklyRepository>();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Serviços de aplicação
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IBankService, BankService>();
            services.AddScoped<IRegisterService, RegisterService>();
            services.AddScoped<IEventWeeklyService, EventWeeklyService>();
            services.AddScoped<IEventMonthlyService, EventMonthlyService>();
            services.AddScoped<IEventBiweeklyService, EventBiweeklyService>();

            // AuthService
            services.AddScoped<JwtAuthService>();

            return services;
        }
    }
}
