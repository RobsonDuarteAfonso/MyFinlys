using MyFinlys.Infrastructure.IoC; 
using MyFinlys.Infrastructure.Repositories;
using MyFinlys.Infrastructure.Services;
using MyFinlys.Application.Services;
using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Application.Validators;
using MyFinlys.Domain.Repositories;
using MyFinlys.Api.Services;
using FluentValidation;

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
            services.AddScoped<IEventQuarterlyRepository, EventQuarterlyRepository>();
            services.AddScoped<IEventSemiAnnualRepository, EventSemiAnnualRepository>();
            services.AddScoped<IEventAnnualRepository, EventAnnualRepository>();
            services.AddScoped<IBalanceRepository, BalanceRepository>();

            // Email service
            services.Configure<EmailSettings>(config.GetSection("EmailSettings"));
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Serviços de aplicação
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IBankService, BankService>();
            services.AddScoped<IBalanceService, BalanceService>();
            services.AddScoped<IRegisterService, RegisterService>();
            services.AddScoped<ICardPurchaseService, CardPurchaseService>();
            services.AddScoped<ICreditCardService, CreditCardService>();
            services.AddScoped<IEventWeeklyService, EventWeeklyService>();
            services.AddScoped<IEventMonthlyService, EventMonthlyService>();
            services.AddScoped<IEventBiweeklyService, EventBiweeklyService>();
            services.AddScoped<IEventQuarterlyService, EventQuarterlyService>();
            services.AddScoped<IEventSemiAnnualService, EventSemiAnnualService>();
            services.AddScoped<IEventAnnualService, EventAnnualService>();
            services.AddScoped<IAccountPermissionService, AccountPermissionService>();
            services.AddScoped<ICardPlanService, CardPlanService>();

            // AuthService
            services.AddScoped<JwtAuthService>();

            // FluentValidation
            services.AddValidatorsFromAssemblyContaining<LoginRequestDtoValidator>();

            return services;
        }
    }
}
