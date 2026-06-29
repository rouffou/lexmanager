using System.Reflection;
using LexManager.Api.Modules;
using LexManager.Infrastructure.Exceptions;
using LexManager.Infrastructure.Modules;
using Mediarq.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IReadOnlyList<IModule> modules = ModuleRegistry.Modules;

// --- Cross-cutting services ---------------------------------------------------
builder.Services.AddHttpContextAccessor();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Mediarq (CQRS) across every module + host assembly -----------------------
Assembly[] applicationAssemblies = modules
    .Select(module => module.GetType().Assembly)
    .Append(typeof(Program).Assembly)
    .Distinct()
    .ToArray();

builder.Services.AddMediarq(isHttp: true, applicationAssemblies);
builder.Services.AddMediarqRequestLogging();
builder.Services.AddMediarqPerformanceTracking();

// --- Module registration (isolated per module) --------------------------------
foreach (IModule module in modules)
{
    module.RegisterModule(builder.Services, builder.Configuration);
}

WebApplication app = builder.Build();

// --- Pipeline ----------------------------------------------------------------
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
   .WithName("HealthCheck")
   .WithTags("Diagnostics");

RouteGroupBuilder apiGroup = app.MapGroup("/api");
foreach (IModule module in modules)
{
    module.MapEndpoints(apiGroup);
}

app.Run();

/// <summary>Exposed so integration tests can use <c>WebApplicationFactory&lt;Program&gt;</c>.</summary>
public partial class Program;
