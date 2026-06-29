using System.Reflection;
using FluentValidation;
using LexManager.Api.Modules;
using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Exceptions;
using LexManager.Infrastructure.Modules;
using Mediarq.Extensions;
using Mediarq.FluentValidation;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IReadOnlyList<IModule> modules = ModuleRegistry.Modules;

// --- Cross-cutting services ---------------------------------------------------
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

builder.Services.AddHttpContextAccessor();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Mediarq (CQRS) across every module + host assembly -----------------------
Assembly[] applicationAssemblies = modules
    .SelectMany(module => module.Assemblies)
    .Append(typeof(Program).Assembly)
    .Distinct()
    .ToArray();

builder.Services.AddMediarq(isHttp: true, applicationAssemblies);
builder.Services.AddMediarqRequestLogging();
builder.Services.AddMediarqPerformanceTracking();

// Syntactic validation: FluentValidation validators run inside the Mediarq pipeline and
// short-circuit with a failed Result before the handler executes (Normes §3.1).
foreach (Assembly assembly in applicationAssemblies)
{
    builder.Services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
}

builder.Services.AddMediarqFluentValidation();

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

// Vertical-slice endpoints (one IEndpoint per feature) are discovered and mapped once.
apiGroup.MapRegisteredEndpoints();

// Modules may add bespoke routes beyond the discovered endpoints.
foreach (IModule module in modules)
{
    module.MapEndpoints(apiGroup);
}

app.Run();

/// <summary>Exposed so integration tests can use <c>WebApplicationFactory&lt;Program&gt;</c>.</summary>
public partial class Program;
