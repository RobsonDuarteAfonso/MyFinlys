using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using MyFinlys.Api.Extensions;

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

// 5) Controllers
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
    .AddNewtonsoftJson();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
