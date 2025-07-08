using MyFinlys.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 1) JWT
builder.Services.AddJwtAuthentication(builder.Configuration);

// 2) Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3) Infraestrutura & Repositórios
builder.Services.AddInfrastructureServices(builder.Configuration);

// 4) Serviços de aplicação + AuthService
builder.Services.AddApplicationServices();

// 5) Controllers
builder.Services
    .AddControllers()
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
