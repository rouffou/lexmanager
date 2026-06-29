# Cahier des Charges Fonctionnel et Technique (SRD)
## Projet : Plateforme de Gestion de Clientèle pour Avocats (LexManager)

---

## 1. Introduction et Objectifs

### 1.1 Contexte
Les cabinets d'avocats manipulent quotidiennement un volume important d'informations hautement confidentielles : données personnelles, procédures judiciaires, pièces de dossiers, et flux financiers. La gestion de cette clientèle nécessite une rigueur absolue, une traçabilité complète et le respect du secret professionnel.

### 1.2 Objectif du Projet
L'objectif est de concevoir une application web moderne, robuste et sécurisée permettant aux avocats et à leurs collaborateurs de gérer efficacement leur clientèle, leurs dossiers, leurs calendriers juridiques et leur facturation.

### 1.3 Principes Directeurs Spécifiés
* **Architecture :** Monolithe Modulaire guidé par la *Clean Architecture* avec une découpe par *Feature* (Vertical Slice / Feature Folder).
* **Qualité :** Couverture de tests automatisés minimale de **90%** (Tests unitaires, d'intégration et d'architecture).
* **Stack Technique :** Backend ASP.NET Core (REST API) & Frontend Angular.

---

## 2. Portée Fonctionnelle (Découpe par Feature)

Le système est découpé en modules fonctionnels autonomes. Chaque module encapsule ses propres règles métiers.

### Module 1 : Gestion des Clients & Contacts (`Identity & CRM`)
* **Création/Modification de fiches clients :** Personnes physiques (Nom, prénom, CNIE/Numéro d'identité, coordonnées) et Personnes morales (Raison sociale, SIRET/Registre, représentant légal).
* **Vérification des conflits d'intérêts :** Recherche globale obligatoire lors de la création d'un client pour s'assurer que le cabinet ne défend pas déjà la partie adverse.
* **Historique des interactions :** Journalisation des appels, rendez-vous et courriels échangés.

### Module 2 : Gestion des Dossiers (`Case Management`)
* **Cycle de vie du dossier :** Ouverture, instruction, archivage, clôture.
* **Rattachement automatique :** Liaison avec un ou plusieurs clients, et désignation des parties adverses et de leurs conseils (avocats adverses).
* **Suivi des instances :** Juridiction saisie (Tribunal judiciaire, Cour d'appel, etc.), numéro de RG (Répertoire Général), juge en charge.

### Module 3 : Gestion Documentaire (`Document Management System - DMS`)
* **Stockage sécurisé :** Dépôt de pièces de procédure, conclusions, et contrats (PDF, Word, Images).
* **Versionnage :** Suivi des modifications sur les documents de travail.
* **Génération de documents :** Création automatisée de courriers ou d'actes à partir de templates prédéfinis (publipostage).

### Module 4 : Agenda Juridique & Suivi du Temps (`Calendar & Time-Tracking`)
* **Agenda partagé :** Gestion des audiences, des délibérés, des rendez-vous clients et des échéances de procédure (calcul des délais légaux).
* **Synchronisation Agendas Professionnels (ex: Outlook/Office 365, Google Calendar) :**
    * Synchronisation bidirectionnelle en temps réel via API (Microsoft Graph API / Google Calendar API).
    * Gestion des conflits d'horaires et masquage automatique des détails personnels pour respecter la confidentialité des dossiers hors cabinet.
    * Webhooks pour capturer instantanément les modifications effectuées depuis l'extérieur de la plateforme.
* **Feuille de temps (Timesheet) :** Saisie du temps passé par dossier (au quart d'heure ou via un minuteur intégré) pour les prestations facturées au temps passé.

### Module 5 : Facturation & Comptabilité (`Billing & Finance`)
* **Modes de facturation :** Au forfait, au temps passé (basé sur la feuille de temps), ou honoraire de résultat.
* **Émission des documents :** Génération de factures, d'appels de provisions et de notes d'honoraires aux normes légales.
* **Suivi des règlements :** Statut des paiements (En attente, Payé, En retard), relances automatiques.

---

## 3. Architecture Technique & Choix Technologiques

### 3.1 Stack Technique
* **Backend :** .NET 8 / .NET 9 Core REST API (C#)
* **Frontend :** Angular 17+ (TypeScript, RxJS, NgRx ou Signals pour l'état, **Angular Material** pour la bibliothèque de composants UI/CSS)
* **Base de Données :** PostgreSQL (idéal pour la structure relationnelle des dossiers et le support JSONB)
* **ORM :** Entity Framework Core (EF Core)

### 3.2 Structure : Monolithe Modulaire + Clean Architecture
L'application est un monolithe unique au déploiement, mais segmentée de manière étanche en modules logiques au sein du code source afin de faciliter une éventuelle transition vers des microservices si nécessaire.

Chaque module suit les principes de la **Clean Architecture** appliqués à la **découpe par Feature** :

```
📂 src/
├── 📂 Bootstrapper/              # Point d'entrée de l'application (Program.cs, configuration globale)
└── 📂 Modules/
    ├── 📂 CaseManagement/        # Exemple du module de gestion des dossiers
    │   ├── 📂 Features/
    │   │   ├── 📂 CreateCase/    # Chaque feature regroupe ses composants verticalement
    │   │   │   ├── CreateCaseCommand.cs
    │   │   │   ├── CreateCaseCommandHandler.cs
    │   │   │   ├── CreateCaseValidator.cs
    │   │   │   └── CreateCaseEndpoint.cs (Minimal API)
    │   │   └── 📂 GetCaseById/
    │   ├── 📂 Domain/            # Entités, Value Objects, Logic Métier pure du module
    │   ├── 📂 Infrastructure/    # Persistance (DbContext spécifique), Intégrations externes
    │   └── 📄 CaseManagementModule.cs # Point d'activation et d'injection du module
    └── 📂 Billing/               # Autre module indépendant
```

#### Règles de dépendance strictes :
1.  **Communication inter-modules :** Interdite au niveau de la base de données. La communication se fait soit par des interfaces/contrats définis (`IModuleAPI`), soit de manière asynchrone via un bus d'événements interne en mémoire (MediatR INotification).
2.  **Clean Architecture interne :** L'Infrastructure et les Features dépendent du Domaine. Le Domaine ne dépend de rien.

---

## 4. Stratégie de Tests (Objectif 90%+)

Pour garantir l'objectif de **90% de couverture de code**, la pyramide des tests sera structurée comme suit :

### 4.1 Niveaux de Tests
1.  **Tests Unitaires (Domain & Application) [60%] :** Focus sur les règles métiers, les validateurs (FluentValidation) et les entités du domaine. Utilisation de `xUnit`, `Moq`/`NSubstitute`, et `FluentAssertions`.
2.  **Tests d'Intégration (Features & Infrastructure) [25%] :** Vérification de la communication avec la base de données réelle (via `Testcontainers` PostgreSQL) et l'exécution des pipelines MediatR.
3.  **Tests d'Architecture [5%] :** Utilisation de `NetArchTest` pour valider automatiquement par build que les règles de découpe modulaire et de Clean Architecture ne sont pas violées (ex: interdire au Domaine de référencer l'Infrastructure).
4.  **Tests Frontend (Angular) [10%] :** Tests unitaires des composants/services avec `Jasmine`/`Karma` ou `Jest`, et couverture des flux critiques via `Cypress` ou `Playwright`.

### 4.2 Pipeline CI/CD
Le build échouera automatiquement sur la branche principale si la couverture globale des tests descend en dessous du seuil de 90% (vérifié via des outils comme `Coverlet` et `SonarQube`).

---

## 5. Exigences Non-Fonctionnelles (Qualité & Sécurité)

### 5.1 Sécurité & Confidentialité (Secret Professionnel)
* **Chiffrement :** Chiffrement des données au repos (TDE sur la base de données) et en transit (TLS 1.3). Chiffrement spécifique pour les pièces jointes sensibles stockées.
* **Authentification & Autorisation :** Implémentation d'OAuth2/OIDC avec JWT. Contrôle d'accès basé sur les rôles et les permissions (RBAC) fine (ex: seul l'avocat en charge d'un dossier peut voir ses pièces confidentielles).
* **Piste d'Audit (Auditing) :** Traçabilité complète de chaque action (Qui a consulté, modifié ou supprimé quelle information et quand).

### 5.2 Performance et Scalabilité
* **Pagination & Caching :** API REST entièrement paginée. Utilisation d'un cache en mémoire distribuée pour les données de référence (liste des juridictions, codes de taxes).
* **Séparation des lectures/écritures (CQRS) :** Utilisation de requêtes légères (Dapper ou requêtes EF asNoTracking) pour l'affichage et de commandes structurées pour l'écriture.

### 5.3 Gestion de la Rétention Légale & Conformité RGPD (Privacy by Design)
Le système doit intégrer nativement les cycles de conservation légaux des données dictés par les règles déontologiques des avocats et le RGPD.

* **Délais de Rétention par Cycle de Vie :**
    * **Dossiers standards : 5 ans** de rétention obligatoire à compter de la clôture officielle du dossier (délai de prescription de droit commun de la responsabilité professionnelle de l'avocat - Art. 2224 du Code civil).
    * **Dossiers spécifiques (Dommages corporels, Construction/Immobilier) : 10 ans** (liés aux délais de consolidation médicale et aux garanties décennales).
    * **Pièces comptables & Factures : 10 ans** (obligation fiscale et commerciale).
    * **Données de prospection (CRM sans mandat) : 3 ans maximum** après le dernier contact avec le prospect.
* **Implémentation Technique dans le Monolithe Modulaire :**
    * **Filtre Global de Lecture (EF Core) :** Dès qu'un dossier est clôturé, il bascule dans l'archive intermédiaire. Un filtre global (`IsArchived == false`) l'exclut des recherches quotidiennes pour désencombrer l'application, tout en restant consultable via un module dédié avec accès tracé.
    * **Background Workers (Quartz.NET ou HostedServices) :** Tâche de fond mensuelle qui calcule les dates d'expiration selon le type de dossier.
    * **Purge & Droit à l'oubli :** À l'expiration du délai légal, exécution automatique d'une suppression définitive (Hard Delete) des pièces jointes physiques du stockage Cloud et anonymisation/suppression des enregistrements correspondants dans PostgreSQL (sauf les données financières qui doivent atteindre leurs 10 ans de rétention).

---

## 6. Stratégie DevOps, CI/CD & Hébergement Cloud (Phase Alpha/Beta)

Afin d'assurer des déploiements fluides, automatisés et sans frais (0€) durant les phases Alpha et Beta, l'infrastructure s'appuiera sur l'écosystème GitHub et des fournisseurs Cloud offrant des tiers gratuits (Free Tiers) robustes.

### 6.1 Automatisation CI/CD (GitHub Actions)
Le code source étant hébergé sur GitHub, deux pipelines de CI/CD distincts seront configurés via **GitHub Actions** (gratuit pour les dépôts publics, et incluant 2 000 minutes/mois pour les dépôts privés).

* **Pipeline d'Intégration Continue (CI) :** 
    * Se déclenche à chaque Pull Request vers `main` ou `develop`.
    * **Backend :** Restauration des packages NuGet, build de la solution, exécution des tests unitaires/intégration (via `Testcontainers`), vérification du seuil de couverture de code (>90% avec `Coverlet`).
    * **Frontend :** Linters, build de l'application Angular, exécution des tests unitaires (`Jest`).
    * Échec du build si la couverture est inférieure à 90% ou si les règles d'architecture (`NetArchTest`) sont enfreintes.
* **Pipeline de Déploiement Continu (CD) :**
    * Se déclenche automatiquement lors du merge sur la branche `main` (ou tags de release).
    * Construction des images Docker et déploiement automatisé vers les plateformes d'hébergement cibles.

### 6.2 Hébergement Cloud 100% Gratuit (Propositions)

Étant donné la stack choisie (ASP.NET REST API + Angular + PostgreSQL), deux stratégies d'hébergement gratuit s'offrent au projet pour la validation Alpha/Beta :

#### Architecture Hybride (Render + Supabase + Vercel)
Pour maximiser les performances et la stabilité sans débourser un centime, la séparation des responsabilités offre les meilleurs quotas gratuits :
1.  **Backend (ASP.NET Core API) : Hosted on Render (Free Tier)**
    * Render permet de déployer des applications web (supporte l'exécution d'un Dockerfile ou de .NET directement). 
    * *Contrainte du plan gratuit :* L'instance s'endort après 15 minutes d'inactivité (temps de réveil de ~30s). Acceptable pour une démo Alpha/Beta.
2.  **Base de Données (PostgreSQL) : Hosted on Supabase (Free Tier)**
    * Supabase offre une base de données PostgreSQL complète et managée (500 Mo de stockage, ce qui est amplement suffisant pour tester les fiches clients et dossiers en phase de test).
3.  **Frontend (Angular) : Hosted on Vercel ou Netlify (Free Tier)**
    * Hébergement statique ultra-performant avec CDN mondial gratuit. Intégration native avec GitHub (déploiement automatique en 2 minutes à chaque commit).

### 6.3 Dockerisation du Monolithe
Pour garantir la portabilité entre l'environnement de développement local et les tiers gratuits du Cloud, un `Dockerfile` multi-stage sera mis en place pour packager le monolithe :
1.  **Stage 1 :** Build et publication du frontend Angular.
2.  **Stage 2 :** Build de l'API ASP.NET Core et intégration des assets Angular pré-compilés dans le dossier `wwwroot` du backend.
3.  **Stage 3 :** Image finale d'exécution ultra-légère basée sur `mcr.microsoft.com/dotnet/aspnet`.
