using System.Reflection;
using Microsoft.OpenApi.Models;
using TechFxNet.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using TechFxNet.Application.Extensions;
using TechFxNet.Infrastructure;
using TechFxNet.Web.Middleware;
using TechFxNet.Web.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddRepositories()
    .AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Swagger", Version = "0.0.1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    c.DocInclusionPredicate((name, api) => true);
});
builder.Services
    .AddHealthChecks()
    .AddCheck<ApplicationHealthCheck>("db", tags: new[] { "db" });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger v0.0.1");
        c.EnableFilter();
        c.DisplayRequestDuration();
        c.DocumentTitle = "TechFxNet.Web API Documentation";
    });
}

using var scope = app.Services.CreateScope();

var dbContext = scope.ServiceProvider.GetRequiredService<TechDbContext>();
await dbContext.Database.MigrateAsync();


// Configure the HTTP request pipeline.
app.UseMiddleware<ApplicationExceptionHandlerMiddleware>();

app.UseAuthorization();
app.MapControllers();

app.MapHealthChecks("HealthCheck", HealthCheckBuilder.CreateHealthCheck())
    .AllowAnonymous();

app.MapHealthChecks("HealthCheck/db",
        HealthCheckBuilder.CreateHealthCheck(r => r.Tags.Contains("db")))
    .AllowAnonymous();

app.Run();
