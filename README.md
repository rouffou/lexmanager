# LexManager

Plateforme de gestion de clientèle pour cabinets d'avocats. Monolithe modulaire
(**Clean Architecture** + **Vertical Slice**) — backend **ASP.NET Core / .NET 10**,
frontend **Angular**, base **PostgreSQL** (EF Core).

> Spécifications : [`Cahier_des_Charges_LexManager_V5.md`](Cahier_des_Charges_LexManager_V5.md)
> et [`Normes_Techniques_et_Regles_de_Tests_LexManager.md`](Normes_Techniques_et_Regles_de_Tests_LexManager.md).

## Architecture

```
src/
  Bootstrapper/LexManager.Api        # Hôte web : compose les modules, pipeline HTTP
  BuildingBlocks/
    LexManager.SharedKernel          # Primitives domaine (Entity, AggregateRoot, ValueObject, exceptions)
    LexManager.Application.Abstractions  # Pagination, contrats inter-modules
    LexManager.Infrastructure        # IModule, IEndpoint, ProblemDetails, Result→IResult
  Modules/<Module>/                  # 1 module = Domain / Application / Infrastructure / Contracts
tests/
  LexManager.ArchitectureTests       # Règles de dépendance (NetArchTest)
  <Module>.UnitTests / .IntegrationTests
frontend/                            # Application Angular (à venir)
```

**CQRS via [Mediarq](https://github.com/rouffou/mediarq)** (MIT) : `ICommand`/`IQuery`,
`ICommandHandler`/`IQueryHandler`, behaviors de pipeline, et son type `Result`/`ResultError`
utilisé de bout en bout. Validation syntaxique via **FluentValidation** (`Mediarq.FluentValidation`),
validation sémantique dans le domaine. Erreurs renvoyées au format **RFC 7807 ProblemDetails**.

### Règles de dépendance (vérifiées par les tests d'architecture)
- Le **Domaine** ne dépend de rien (hors SharedKernel).
- **Pas d'accès BDD inter-modules** : communication via `Contracts` (`IModuleApi`) ou événements MediatR/Mediarq.
- Les entités du domaine ne franchissent jamais la frontière API (DTOs uniquement).

## Démarrer

```bash
dotnet restore LexManager.slnx
dotnet build   LexManager.slnx
dotnet run --project src/Bootstrapper/LexManager.Api   # Swagger sur /swagger, santé sur /health
```

## Tests (objectif ≥ 90 % de couverture)

```bash
dotnet test LexManager.slnx --collect:"XPlat Code Coverage"
```

Pyramide : unitaires (xUnit, FluentAssertions, NSubstitute), intégration (Testcontainers
PostgreSQL), architecture (NetArchTest). La CI échoue sous 90 % de couverture de lignes.

## Docker

```bash
docker build -t lexmanager .
docker run -p 8080:8080 lexmanager
```

Image multi-stage : build Angular → build/publish .NET → runtime `aspnet` léger.
