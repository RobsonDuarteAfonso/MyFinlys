using MyFinlys.Application.Services;
using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Domain.Repositories;
using MyFinlys.Infrastructure;
using MyFinlys.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);

// Repositórios
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IBankRepository, BankRepository>();
builder.Services.AddScoped<IRegisterRepository, RegisterRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventWeeklyRepository, EventWeeklyRepository>();
builder.Services.AddScoped<IEventMonthlyRepository, EventMonthlyRepository>();
builder.Services.AddScoped<IEventBiweeklyRepository, EventBiweeklyRepository>();

// Serviços
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IBankService, BankService>();
builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<IEventWeeklyService, EventWeeklyService>();
builder.Services.AddScoped<IEventMonthlyService, EventMonthlyService>();
builder.Services.AddScoped<IEventBiweeklyService, EventBiweeklyService>();

// Controllers e Swagger
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

