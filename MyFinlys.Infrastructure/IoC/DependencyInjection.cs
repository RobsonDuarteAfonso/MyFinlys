using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyFinlys.Domain.Repositories;
using MyFinlys.Infrastructure.Context;
using MyFinlys.Infrastructure.Repositories;

namespace MyFinlys.Infrastructure.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<MyFinlysDbContext>(options =>
                options.UseNpgsql(
                        configuration.GetConnectionString("DefaultConnection"))
                       .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            );

            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBankRepository, BankRepository>();
            services.AddScoped<IRegisterRepository, RegisterRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IEventWeeklyRepository, EventWeeklyRepository>();
            services.AddScoped<IEventMonthlyRepository, EventMonthlyRepository>();
            services.AddScoped<IEventBiweeklyRepository, EventBiweeklyRepository>();
            services.AddScoped<IEventQuarterlyRepository, EventQuarterlyRepository>();
            services.AddScoped<IEventSemiAnnualRepository, EventSemiAnnualRepository>();
            services.AddScoped<IEventAnnualRepository, EventAnnualRepository>();
            services.AddScoped<IBalanceRepository, BalanceRepository>();
            services.AddScoped<ICardPurchaseRepository, CardPurchaseRepository>();
            services.AddScoped<ICardInstallmentRepository, CardInstallmentRepository>();
            services.AddScoped<ICreditCardRepository, CreditCardRepository>();
            services.AddScoped<ICardPlanRepository, CardPlanRepository>();

            return services;
        }
    }
}
