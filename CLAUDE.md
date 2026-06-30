# CLAUDE.md — LexManager

Conventions for working in this repo. Read the two spec files at the root before non-trivial work:
`Cahier_des_Charges_LexManager_V5.md` (SRD) and `Normes_Techniques_et_Regles_de_Tests_LexManager.md`.

## Stack & versions
- .NET 10 (`net10.0`), C# latest. Central package management in `Directory.Packages.props` — add
  package **versions there**, reference without `Version=` in csproj. Shared settings in `Directory.Build.props`
  (nullable, ImplicitUsings, `TreatWarningsAsErrors=true`).
- **CQRS = Mediarq** (NOT MediatR). Package family `Mediarq.*` (MIT, by the repo owner). Key namespaces:
  - Requests: `Mediarq.Core.Common.Requests.Command` (`ICommand`, `ICommand<T>`, `ICommandHandler<T>`,
    `ICommandHandler<T,R>`), `...Requests.Query` (`IQuery<T>`, `IQueryHandler<T,R>`),
    `...Requests.Notifications` (`INotification`, `INotificationHandler<T>`).
  - Results: `Mediarq.Core.Common.Results` (`Result`, `Result<T>`, `ResultError`, `ErrorType`,
    `ValidationError`). Use `Result.Success(...)`, `Result.Failure(ResultError.NotFound(code,msg))`, etc.
    Do **not** reintroduce a homegrown `Result`/`Error`.
  - Pipeline behaviors: `Mediarq.Core.Common.Pipeline.IPipelineBehavior<TRequest,TResponse>` where
    `TRequest : ICommandOrQuery<TResponse>`; `Handle(IMutableRequestContext, Func<Task<TResponse>>, ct)`.
  - DI: `services.AddMediarq(isHttp: true, assemblies)` + `AddMediarqRequestLogging()` /
    `AddMediarqPerformanceTracking()`. Validation via `Mediarq.FluentValidation` (write FluentValidation
    validators; they run in the pipeline and short-circuit with a failed `Result`).
  - Unit of work: `Mediarq.UnitOfWork.IUnitOfWork` (`Task<int> SaveChangesAsync(ct)`). Because each module
    has its OWN DbContext, do NOT register the bare `IUnitOfWork` (the single-resolver `UnitOfWorkBehavior`
    would commit the wrong context). Instead each module defines a marker `I<M>UnitOfWork : IUnitOfWork`
    in its Application layer; the module's DbContext implements it; handlers depend on the marker and call
    `SaveChangesAsync` explicitly. Domain events are published by the DbContext's `SaveChangesAsync` override.

## Architecture rules (enforced by tests/LexManager.ArchitectureTests with NetArchTest)
- Per module: `LexManager.Modules.<M>.Domain | .Application | .Infrastructure | .Contracts`.
- Domain depends on nothing but SharedKernel. Infrastructure/Application depend on Domain, never reverse.
- No cross-module DB access; modules talk only via `Contracts` (`IModuleApi`) or integration events.
- DTOs/Commands/Queries/Events are `record`s. Use primary constructors for DI. No try-catch for normal flow;
  expected errors → `Result`; invariant breaches → `BusinessRuleValidationException` (mapped to ProblemDetails).
- Each module ships a `<M>Module : IModule` (in its Application assembly) registered in
  `src/Bootstrapper/LexManager.Api/Modules/ModuleRegistry.cs`. Endpoints implement `IEndpoint` (one per feature)
  and are discovered via `AddEndpointsFrom(assembly)` / `MapRegisteredEndpoints()`.

## Adding a feature (vertical slice)
Create one folder under the module's `Application/Features/<FeatureName>/` containing:
`<Feature>Command`/`Query` (record), `<Feature>Handler`, `<Feature>Validator` (FluentValidation),
`<Feature>Endpoint` (`IEndpoint`). Map results to HTTP with `result.ToApiResult(...)` (see
`LexManager.Infrastructure.Results.ApiResults`).

## Commands
- Build: `dotnet build LexManager.slnx`
- Test + coverage: `dotnet test LexManager.slnx --collect:"XPlat Code Coverage"` (CI gate: ≥ 90% lines)
- Run API: `dotnet run --project src/Bootstrapper/LexManager.Api`

## Tooling
- **Code search: use the `/graphify` skill** for questions about this codebase's structure/relationships.
- Claude Code plugins to enable (interactive `/plugin`): `claude-dev-toolkit` (Angular) and
  `dotnet-skills` (ASP.NET).
