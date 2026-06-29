# Guide des Normes Techniques, Pratiques de Code et Stratégie de Tests
## Projet : Plateforme de Gestion de Clientèle pour Avocats (LexManager)

---

## 1. Principes d'Architecture & Bonnes Pratiques C# / .NET

L'application est conçue selon le pattern **Monolithe Modulaire** combiné à la **Clean Architecture** et une approche **Feature-Driven (Vertical Slice Architecture)**. 

### 1.1 Règles d'Or du Code Clean & C# 12+
* **Utilisation des fonctionnalités modernes :** Recours systématique aux `primary constructors` pour l'injection de dépendances simplifiée, aux types `record` immuables pour les DTOs, Commands, Queries et Événements, et au `pattern matching` avancé pour la validation ou les règles déontologiques complexes.
* **Architecture Feature-Driven (Vertical Slice) :** Au lieu de séparer les fichiers par couche technique globale (Controllers, Services, Repositories), chaque fonctionnalité métier (ex: `CreateClient`) possède son propre répertoire contenant tout son écosystème : l'Endpoint, la Command/Query, le Validateur et le Handler de traitement.
* **Séparation Commandes/Requêtes (CQRS) :** Utilisation de `MediatR`. Les lectures (Queries) retournent directement des DTOs plats et optimisés (idéalement via Entity Framework avec `.AsNoTracking()` ou des requêtes brutes ultra-rapides avec `Dapper`). Les écritures (Commands) passent par le Domaine pour altérer l'état du système.
* **Pas de fuite du Domaine :** Les entités du domaine PostgreSQL ne doivent jamais être exposées aux contrôleurs REST de l'API. La sérialisation JSON transite obligatoirement par des DTOs stricts.

### 1.2 Gestion des Erreurs et Exceptions
* Interdiction d'utiliser les blocs `try-catch` pour contrôler le flux métier normal. 
* Utilisation d'un **Global Exception Middleware** convertissant automatiquement les exceptions non gérées du framework en réponses standardisées au format **RFC 7807 (Problem Details)**.
* Pour les erreurs métiers attendues (ex: "Conflit d'intérêt détecté lors de la création d'un client"), privilégier l'utilisation du pattern *Result* (`Result<T>`) ou lever des exceptions de domaine spécifiques (ex: `BusinessRuleValidationException`) interceptées par le middleware pour renvoyer un code HTTP `400 Bad Request` ou `422 Unprocessable Entity`.

---

## 2. Bonnes Pratiques Frontend (Angular 17+ & Angular Material)

### 2.1 Gestion du State et Performance
* **Recours aux Signals Angular :** Pour la gestion fine de la réactivité, le suivi du cycle de vie des données d'interface et l'optimisation des performances de rendu.
* **Stratégie OnPush :** Tous les composants de l'application doivent explicitement utiliser la stratégie de détection des changements `ChangeDetectionStrategy.OnPush` pour économiser les cycles CPU du navigateur.
* **Découpe Modulaire :** Bien que l'application soit un monolithe, le frontend Angular reflète l'organisation du backend. Chaque module métier possède sa propre structure de dossiers (Components, Services, State). Utilisation systématique du **Lazy Loading** au niveau du Routeur Angular pour n'allouer la mémoire que lorsque l'avocat accède à l'écran concerné.

### 2.2 Utilisation d'Angular Material et CSS
* **Encapsulation stricte :** Aucun style global "bricolé". Les thèmes de couleurs (adaptés à la charte sobre et professionnelle du monde juridique) doivent être configurés via les fichiers thématiques Sass (`.scss`) d'Angular Material.
* **Accessibilité (a11y) :** Les composants Material (`mat-table`, `mat-dialog`, etc.) doivent être exploités au maximum de leurs capacités natives en conservant les attributs ARIA et la navigation au clavier, indispensables pour la saisie rapide des feuilles de temps ou l'examen des dossiers d'audience.

---

## 3. Stratégie de Validation & Validation des Données

La validation s'effectue à deux niveaux hermétiques de l'application backend : la validation d'entrée (syntaxique) et la validation métier (sémantique).

### 3.1 Validation Syntaxique (Couche API / Application)
* Implémentée via la bibliothèque **FluentValidation**.
* Chaque commande reçue par l'API REST est automatiquement interceptée par un comportement de pipeline MediatR (`IPipelineBehavior`) qui exécute le validateur associé.
* *Exemple de règle syntaxique :* Vérifier qu'une adresse email client respecte le pattern regex, ou qu'un numéro SIRET possède exactement 14 chiffres. Si une règle échoue, une réponse `400` avec la liste des champs invalides est immédiatement renvoyée à l'application Angular sans solliciter le Domaine.

### 3.2 Validation Sémantique (Couche Domaine)
* Implémentée au cœur des entités du Domaine.
* Les entités interdisent les modifications d'état incohérentes en exposant uniquement des méthodes métiers explicites (ex: `.CloturerDossier(DateTime dateCloture)`) à la place de modificateurs publics (`set;`).
* *Exemple de règle sémantique :* Un dossier ne peut pas être clôturé s'il contient des factures émises dont le paiement est en attente ou s'il y a des audiences programmées dans le futur.

---

## 4. Guide Pratique des Tests Automatisés (Objectif >= 90%)

L'ensemble de la suite de tests utilise le framework **xUnit**, la bibliothèque d'assertions **FluentAssertions**, et les mocks via **NSubstitute** ou **Moq**.

### 4.1 Tests d'Architecture (Automatisés via NetArchTest)
Les tests d'architecture garantissent qu'aucune régression structurelle ne soit introduite par un développeur (ex: un couplage fort accidentel entre deux modules). Ils s'exécutent en moins d'une seconde dans le pipeline CI.

```csharp
[Fact]
public void Domain_Should_Not_HaveDependencyOn_Infrastructure()
{
    var result = Types.InAssembly(typeof(CaseManagement.Domain.AssemblyReference).Assembly)
        .ShouldNot()
        .HaveDependencyOn("CaseManagement.Infrastructure")
        .GetResult();

    result.IsSuccessful.Should().BeTrue();
}

[Fact]
public void Features_Should_Only_CommunicateWithOtherModules_ThroughContracts()
{
    var result = Types.InAssembly(typeof(CaseManagement.Application.AssemblyReference).Assembly)
        .ShouldNot()
        .HaveDependencyOn("Billing.Infrastructure")
        .GetResult();

    result.IsSuccessful.Should().BeTrue();
}
```
### 4.2 Tests Unitaires (Focus Règles Métiers & Validateurs)

Ils ciblent à 100% la logique pure du Domaine et les handlers de commandes sans aucune connexion à une base de données ou un service tiers.


```csharp

[Fact]
public async Task Handle_Should_ReturnError_WhenConflictOfInterestIsDetected()
{
    // Arrange
    var command = new CreateClientCommand("Jean", "Dupont", "AdversaireIdentifie");
    var conflictServiceMock = NSubstitute.Substitute.For<IConflictOfInterestChecker>();
    conflictServiceMock.HasConflictAsync(command.IdentityNumber).Returns(true);

    var handler = new CreateClientCommandHandler(conflictServiceMock, _clientRepositoryMock, _unitOfWorkMock);

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(ClientErrors.ConflictOfInterestDetected);
}
```
### 4.3 Tests d'Intégration (Via Testcontainers PostgreSQL)
Ces tests valident le comportement réel de l'application (requêtes EF Core, transactions, contraintes de clés étrangères) en démarrant un véritable conteneur éphémère Docker de base de données PostgreSQL lors de l'exécution de la suite de tests.


```csharp
public class CreateCaseIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    // Initialisation automatique du Testcontainer PostgreSQL ici

    [Fact]
    public async Task CreateCase_Should_InsertNewRecordInDatabase()
    {
        // Arrange
        var payload = new { Title = "Litige Commercial SNE", ClientId = Guid.NewGuid() };

        // Act
        var response = await _client.PostAsJsonAsync("/api/cases", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        // Vérification directe en base PostgreSQL
        var caseInDb = await GetCaseFromDbAsync(payload.Title);
        caseInDb.Should().NotBeNull();
        caseInDb.Status.Should().Be(CaseStatus.Opened);
    }
}

```
### 4.4 Tests d'Interface Frontend (Angular)

- **Tests Unitaires** : Utilisation de Jest pour valider la logique des contrôleurs de composants Angular et des services de communication HTTP en simulant les appels d'API.

- **Tests de flux bout-en-bout (E2E)** : Utilisation de Playwright ou Cypress pour tester les parcours utilisateurs clés (ex: "Se connecter, ouvrir une fiche client, enregistrer 30 minutes de temps passé et générer l'appel de provisions"). 2 à 3 scénarios majeurs suffisent à sécuriser l'expérience utilisateur globale.