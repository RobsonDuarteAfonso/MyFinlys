using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using MyFinlys.Api.Extensions;
using MyFinlys.Api.Middleware;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

// Fix for Npgsql v6+: DateTime without Kind=Utc causes 0001-01-01 on 'timestamp with time zone' columns.
// This switch restores the pre-v6 behavior so all DateTime values are accepted without requiring UTC kind.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// 1) JWT
builder.Services.AddJwtAuthentication(builder.Configuration);

// 2) Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3) Infrastruture & Repositories
builder.Services.AddInfrastructureServices(builder.Configuration);

// 4) Servies and application + AuthService
builder.Services.AddApplicationServices();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// 5) FluentValidation
builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();

// 6) Controllers with authorization
builder.Services
    .AddControllers(
    options =>
    {
        // Creates a policy that requires an authenticated user
        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
        // Applies this policy to **all** actions/controllers
        options.Filters.Add(new AuthorizeFilter(policy));
    })
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
        options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
    });

var app = builder.Build();

// Error handling middleware
app.UseMiddleware<ErrorHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAngularApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Update existing registers to correct calendar-based week numbers and recalculate all balances
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<MyFinlys.Infrastructure.Context.MyFinlysDbContext>();


        var registers = dbContext.Registers.Where(r => !r.IsDeleted).ToList();
        var events = dbContext.Events.ToList();
        int updatedCount = 0;
        int updatedInstallments = 0;
        foreach (var reg in registers)
        {
            var firstOfMonth = new DateTime(reg.Due.Year, reg.Due.Month, 1);
            int firstDayOfWeek = (int)firstOfMonth.DayOfWeek;
            int correctWeek = (reg.Due.Day + firstDayOfWeek - 1) / 7 + 1;
            if (reg.Week != correctWeek)
            {
                var prop = typeof(MyFinlys.Domain.Entities.Register).GetProperty(nameof(MyFinlys.Domain.Entities.Register.Week));
                if (prop != null)
                {
                    prop.SetValue(reg, correctWeek);
                    dbContext.Entry(reg).Property(r => r.Week).IsModified = true;
                    updatedCount++;
                }
            }

            if (reg.EventId.HasValue)
            {
                var ev = events.FirstOrDefault(e => e.Id == reg.EventId.Value);
                if (ev != null && ev.Installment != null)
                {
                    int correctInstallment = reg.InstallmentCurrent;
                    if (ev.Period == MyFinlys.Domain.Enums.EventPeriod.Weekly)
                    {
                        if (ev.Installment.DateInitial.HasValue && reg.Due.Date >= ev.Installment.DateInitial.Value.Date)
                        {
                            int weeksDiff = (int)((reg.Due.Date - ev.Installment.DateInitial.Value.Date).TotalDays / 7);
                            correctInstallment = ev.Installment.InstallmentCurrent + weeksDiff;
                        }
                    }
                    else if (ev.Period == MyFinlys.Domain.Enums.EventPeriod.Biweekly)
                    {
                        var startDate = ev.Installment.DateInitial ?? (ev as MyFinlys.Domain.Entities.EventBiweekly)?.StartDate;
                        if (startDate.HasValue && reg.Due.Date >= startDate.Value.Date)
                        {
                            int biweeksDiff = (int)((reg.Due.Date - startDate.Value.Date).TotalDays / 14);
                            correctInstallment = ev.Installment.InstallmentCurrent + biweeksDiff;
                        }
                    }

                    if (reg.InstallmentCurrent != correctInstallment)
                    {
                        var prop = typeof(MyFinlys.Domain.Entities.Register).GetProperty(nameof(MyFinlys.Domain.Entities.Register.InstallmentCurrent));
                        if (prop != null)
                        {
                            prop.SetValue(reg, correctInstallment);
                            dbContext.Entry(reg).Property(r => r.InstallmentCurrent).IsModified = true;
                            updatedInstallments++;
                        }
                    }
                }
            }
        }
        if (updatedCount > 0 || updatedInstallments > 0)
        {
            dbContext.SaveChanges();
            if (updatedCount > 0)
                Console.WriteLine($"Successfully updated {updatedCount} register weeks to calendar-based values.");
            if (updatedInstallments > 0)
                Console.WriteLine($"Successfully updated {updatedInstallments} register installments to correct values.");
        }
        else
        {
            Console.WriteLine("No register weeks or installments needed updating.");
        }

        // Recalculate all monthly balances in the database to sync with active registers
        var accountGroups = registers.GroupBy(r => new { r.AccountId, r.Month, r.Due.Year });
        int updatedBalances = 0;
        foreach (var group in accountGroups)
        {
            decimal balanceAmount = 0m;
            foreach (var reg in group)
            {
                if (reg.EventType == MyFinlys.Domain.Enums.EventType.Credit)
                    balanceAmount += reg.Value;
                else if (reg.EventType == MyFinlys.Domain.Enums.EventType.Debit)
                    balanceAmount -= reg.Value;
            }

            var yearVO = MyFinlys.Domain.ValueObjects.Year.Create(group.Key.Year);
            var balance = dbContext.Balances.FirstOrDefault(b => b.AccountId == group.Key.AccountId && b.Month == group.Key.Month && b.Year == yearVO);
            if (balance != null)
            {
                if (balance.Amount != balanceAmount)
                {
                    balance.UpdateAmount(balanceAmount);
                    dbContext.Entry(balance).Property(b => b.Amount).IsModified = true;
                    updatedBalances++;
                }
            }
            else
            {
                balance = MyFinlys.Domain.Entities.Balance.Create(group.Key.AccountId, group.Key.Year, group.Key.Month, balanceAmount);
                dbContext.Balances.Add(balance);
                updatedBalances++;
            }
        }
        if (updatedBalances > 0)
        {
            dbContext.SaveChanges();
            Console.WriteLine($"Successfully recalculated and updated {updatedBalances} monthly balance records.");
        }
        else
        {
            Console.WriteLine("All monthly balances are in sync.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error running database registers week and balance update: {ex.Message}");
    }
}

app.Run();
