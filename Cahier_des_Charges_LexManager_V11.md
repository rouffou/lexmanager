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
* **Création/Modification de fiches clients :** Personnes physiques (Nom, prénom, CNIE/Numéro d'identité, coordonnées) et Personnes morales (Raison sociale, SIRET/Registre, représentant légal)[cite: 5].
* **Vérification des conflits d'intérêts :** Recherche globale obligatoire lors de la création d'un client pour s'assurer que le cabinet ne défend pas déjà la partie adverse[cite: 5].
* **Historique des interactions :** Journalisation des appels, rendez-vous et courriels échangés[cite: 5].
* **Portail Client Sécurisé (Espace Client) :** Accès Angular restreint et authentifié permettant aux clients de consulter l'avancement de leur affaire, d'uploader leurs pièces justificatives et d'effectuer des paiements de provisions ou de factures en ligne.
* **Workflow KYC & LCB-FT :** Processus de vérification et de validation d'identité (CNIE, passeport, extrait Kbis) avec système de scoring de conformité réglementaire avant acceptation du mandat.

### Module 2 : Gestion des Dossiers (`Case Management`)
* **Cycle de vie du dossier :** Ouverture, instruction, archivage, clôture[cite: 5].
* **Rattachement automatique :** Liaison avec un ou plusieurs clients, et désignation des parties adverses et de leurs conseils (avocats adverses)[cite: 5].
* **Suivi des instances :** Juridiction saisie (Tribunal judiciaire, Cour d'appel, etc.), numéro de RG (Répertoire Général), juge en charge[cite: 5].
* **Générateur d'Arbre de Procédure :** Représentation graphique interactive du déroulement d'une affaire (de la mise en demeure à l'exécution forcée) permettant d'un coup d'œil de situer le dossier dans le calendrier judiciaire.
* **Gestion des Consorts et Parties Multiples :** Prise en charge des procédures complexes impliquant une pluralité de co-demandeurs ou de co-défendeurs avec individualisation des calculs de préjudices et d'indemnités.

### Module 3 : Gestion Documentaire (`Document Management System - DMS`)
* **Stockage sécurisé :** Dépôt de pièces de procédure, conclusions, et contrats (PDF, Word, Images)[cite: 5].
* **Versionnage :** Suivi des modifications sur les documents de travail[cite: 5].
* **Génération de documents :** Création automatisée de courriers ou d'actes à partir de templates prédéfinis (publipostage)[cite: 5].
* **Moteur d'OCR & Recherche Full-Text (Stratégie Souveraine et Gratuite) :** Reconnaissance optique de caractères automatisée exécutée localement par le moteur open-source **Tesseract OCR** intégré directement dans le conteneur Docker. L'extraction et l'indexation de texte sont gérées de manière asynchrone en tâche de fond (via un .NET Background Worker) pour préserver les ressources CPU, garantissant une confidentialité absolue (les données et documents confidentiels ne quittent jamais l'infrastructure de l'application) et un coût d'exploitation technique de 0,00 €.
* **Signature Électronique Intégrée :** Envoi, suivi et validation juridique de conventions d'honoraires ou d'actes d'avocats via une intégration API tierce (ex: Yousign, DocuSign) directement depuis l'interface Angular.
* **Interfaçage DPA-Deposit (Écosystème DPA Belgique) :** Module de préparation et de pré-validation des conclusions et des inventaires de pièces, structuré conformément aux exigences de la *Digital Platform for Attorneys* (DPA) pour simplifier le dépôt électronique devant les juridictions belges.

### Module 4 : Agenda Juridique & Suivi du Temps (`Calendar & Time-Tracking`)
* **Agenda partagé :** Gestion des audiences, des délibérés, des rendez-vous clients et des échéances de procédure (calcul des délais légaux)[cite: 5].
* **Synchronisation Agendas Professionnels (ex: Outlook/Office 365, Google Calendar) :**
    * Synchronisation bidirectionnelle en temps réel via API (Microsoft Graph API / Google Calendar API)[cite: 5].
    * Gestion des conflits d'horaires et masquage automatique des détails personnels pour respecter la confidentialité des dossiers hors cabinet[cite: 5].
    * Webhooks pour capturer instantanément les modifications effectuées depuis l'extérieur de la plateforme[cite: 5].
* **Feuille de temps (Timesheet) :** Saisie du temps passé par dossier (au quart d'heure ou via un minuteur intégré) pour les prestations facturées au temps passé[cite: 5].
* **Calculateur Automatisé de Délais de Procédure :** Moteur de règles calculant dynamiquement les échéances légales incontournables (ex: délais de recours, conclusions de l'intimé) sur simple saisie de la date de signification d'un acte, couplé à des rappels push/SMS/email.
* **Saisie Vocale Récurrente :** Module "Speech-to-Text" permettant à l'avocat en déplacement de dicter verbalement son activité, automatiquement traduite et pré-remplie dans la feuille de temps correspondante.

### Module 5 : Facturation & Comptabilité (`Billing & Finance`)
* **Modes de facturation :** Au forfait, au temps passé (basé sur la feuille de temps), ou honoraire de résultat[cite: 5].
* **Émission des documents :** Génération de factures, d'appels de provisions et de notes d'honoraires conformément aux normes légales belges (numérotation continue, mentions obligatoires Moniteur)[cite: 5].
* **Moteur de TVA Belge Légale :** Application automatique du taux de TVA belge de 21% sur les prestations d'avocats, avec gestion des cas d'exemption spécifiques (ex: aide juridique / pro deo, ou co-contractant international).
* **Suivi des règlements :** Statut des paiements (En attente, Payé, En retard), relances automatiques[cite: 5].
* **Gestion des Comptes de Tiers (CARPA / Comptes Rubriqués) :** Sous-module d'écriture dédié au suivi des fonds tiers transitant par les comptes rubriqués ou les comptes tiers réglementés en Belgique, avec traçabilité complète exigée par les Ordres (AVOCATS.BE / OVB).
* **Simulateur d'Intérêts Légaux Belges :** Outil de calcul automatisé des intérêts de retard (taux légal en matière civile et commerciale en Belgique, anatocisme/capitalisation selon l'article 1154 du Code civil) applicables aux condamnations pécuniaires.

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

### 6.2 Infrastructure d'Hébergement Cloud (Stratégie 100% Gratuite pour l'Alpha/Beta)

Afin de garantir un coût d'infrastructure nul (0,00 €) pendant toute la phase de développement, de validation technique et de démo MVP auprès des Jeunes Barreaux, l'application est configurée pour être déployée sur une architecture hybride exploitant exclusivement les offres gratuites (*Free Tiers*) des fournisseurs Cloud modernes :

1. **Base de Données Relationnelle : Supabase (Plan Gratuit)**
   * **Ressources :** Instance managée PostgreSQL complète avec un quota de 500 Mo de stockage de données, ce qui permet d'héberger plusieurs milliers de fiches clients, dossiers, factures et logs d'audit simulés.
   * **Sécurité :** Prise en charge native du chiffrement SSL et des extensions de sécurité PostgreSQL pour isoler les données de test.

2. **API Backend (.NET Core) : Render (Plan Web Service Gratuit)**
   * **Déploiement :** Compilée et exécutée via le `Dockerfile` multi-stage à partir de ton dépôt GitHub.
   * **Fonctionnement :** L'API s'interconnecte directement à la base PostgreSQL de Supabase.
   * **Contrainte technique acceptée :** L'instance gratuite bascule en mode veille après 15 minutes d'inactivité. Le premier appel après une mise en veille nécessite un délai de réveil de l'ordre de 30 secondes (valable pour les phases de démonstration planifiées).

3. **Application Frontend (Angular) : Vercel ou Netlify (Plan Gratuit)**
   * **Performance :** Hébergement statique optimisé pour les architectures Single Page Application (SPA). Les fichiers de l'application Angular sont distribués instantanément via un réseau CDN gratuit.
   * **Automatisation :** Liaison directe au dépôt GitHub assurant un déploiement continu en moins de 2 minutes à chaque mise à jour du code.

*Note de transition de souveraineté : Cette infrastructure gratuite sert à "lancer la machine" et à valider l'application avec de fausses données de test. Dès la contractualisation des premiers abonnements payants, cette configuration sera migrée sans réécriture de code vers les environnements payants et souverains d'OVHcloud ou Clever Cloud (estimés à ~12,00 € HT/mois) afin d'assurer la conformité légale face au secret professionnel en Belgique.*

### 6.3 Dockerisation du Monolithe
Pour garantir la portabilité entre l'environnement de développement local et les tiers gratuits du Cloud, un `Dockerfile` multi-stage sera mis en place pour packager le monolithe :
1.  **Stage 1 :** Build et publication du frontend Angular.
2.  **Stage 2 :** Build de l'API ASP.NET Core et intégration des assets Angular pré-compilés dans le dossier `wwwroot` du backend.
3.  **Stage 3 :** Image finale d'exécution ultra-légère basée sur `mcr.microsoft.com/dotnet/aspnet`.


---

## 7. Guide d'Ingénierie de Prompt pour l'Assistant de Code (Claude)

Cette section est spécifiquement rédigée pour servir d'instructions directes à donner à l'assistant de code (Claude) afin de garantir le respect rigoureux de la structure technique et l'implémentation des briques de gratuité.

### 7.1 Prompt d'Initialisation Globale
> "Prends connaissance du cahier des charges ci-dessus. Nous allons développer LexManager sous forme de Monolithe Modulaire en .NET 9 et Angular 17+ avec une architecture Vertical Slices. Chaque module doit être hermétique. L'objectif de couverture de code par les tests automatisés (xUnit, NetArchTest) est de 90% minimum. Ne génère pas tout le code d'un coup, attends mes instructions module par module."

### 7.2 Prompt pour le Dockerfile Multi-Stage (OCR Tesseract Inclus)
> "En te basant sur les sections 3.2, 5.3 et 6.3 du cahier des charges, génère le `Dockerfile` multi-stage pour notre monolithe. 
> * Le Stage 1 doit compiler le frontend Angular. 
> * Le Stage 2 doit compiler l'API .NET Core en y injectant les fichiers statiques Angular dans `wwwroot` via `UseStaticFiles()`.
> * Le Stage 3 (image finale d'exécution) doit être basé sur l'image officielle ASP.NET Core Linux, mais doit obligatoirement inclure l'installation système native (via apt-get) du moteur `tesseract-ocr` ainsi que les packages de langues `tesseract-ocr-fra` et `tesseract-ocr-nld` (français et néerlandais). Cela permettra à notre code C# d'exécuter l'OCR localement et gratuitement, sans aucune API tierce payante."

### 7.3 Prompt d'Exemple pour le Développement d'une Feature (Vertical Slice)
> "En respectant scrupuleusement la structure de fichiers décrite à la section 3.2, génère l'intégralité du code pour la feature `CreateCase` (Module `CaseManagement`). Tu dois fournir dans une structure verticale :
> 1. L'entité du Domaine (`Case.cs`) avec ses règles métiers.
> 2. La commande et le handler MediatR (`CreateCaseCommand.cs`, `CreateCaseCommandHandler.cs`).
> 3. Le validateur de requêtes (`CreateCaseValidator.cs`) avec FluentValidation.
> 4. L'endpoint d'API REST (`CreateCaseEndpoint.cs`) sous forme de Minimal API .NET.
> 5. Les tests unitaires et d'intégration correspondants avec xUnit et FluentAssertions pour valider le comportement et sécuriser notre objectif de 90% de couverture de code."


---

## 8. Charte Graphique et Identité Visuelle (Design System)

Cette section définit l'identité visuelle de LexManager, pensée pour inspirer la confiance et la rigueur juridique auprès des avocats belges, tout en tirant parti des composants Angular Material spécifiés dans l'architecture.

### 8.1 Palette de Couleurs (Thème Custom Angular Material)

Le système exploite des contrastes marqués inspirés des codes des cabinets d'affaires haut de gamme.

| Rôle | Couleur | Code Hex | Utilisation / Signification |
| :--- | :--- | :--- | :--- |
| **Primaire** (*Primary*) | **Bleu Midnight** | `#1E293B` | Inculque le sérieux et l'autorité institutionnelle. Utilisé pour les barres de navigation, les en-têtes et l'identité de marque. |
| **Secondaire** (*Accent*) | **Or Royal / Bronze** | `#B45309` | Rappelle le prestige et les boutons d'or de la robe d'avocat. Utilisé exclusivement pour les boutons d'action principaux (CTA) et les notifications cruciales. |
| **Arrière-plan** (*Background*) | **Gris Brume** | `#F8FAFC` | Teinte très claire réduisant la fatigue visuelle lors des sessions prolongées de lecture par rapport à un blanc pur. |
| **Surfaces** (*Card/Paper*) | **Blanc Pur** | `#FFFFFF` | Dédié aux blocs de dossiers, aux conteneurs de fiches clients et aux structures de tableaux. |
| **Statut : Succès** | **Vert Émeraude** | `#10B981` | Factures acquittées, flux financiers validés, échéances respectées. |
| **Statut : Alerte** | **Rouge Écarlate** | `#EF4444` | Détection de conflits d'intérêts, factures impayées, délais de procédure critiques. |

### 8.2 Typographie et Lisibilité

Afin de garantir un confort de lecture optimal pour les professionnels du droit, le système sépare les polices de structure et les polices de corps :
* **Titres (`h1`, `h2`, `h3`) : `Merriweather` (Serif)**
  * *Raison d'être :* Une police à empattement évoquant la tradition, les ouvrages juridiques classiques et le prestige des Cours et Tribunaux.
* **Corps de texte, Tableaux et Formulaires : `Inter` ou `Roboto` (Sans-Serif)**
  * *Raison d'être :* Une police géométrique épurée, hautement lisible sur écran haute densité pour l'affichage de données d'un DMS ou d'une feuille de temps.

### 8.3 Composants et Ergonomie

Les directives d'intégration pour les interfaces générées sont les suivantes :
* **Angles de structure (*Border-radius*) :** Fixés à un maximum de `4px` pour les boutons, champs de saisie et cartes (`mat-card`). Les formes trop arrondies ("gélules") sont proscrites pour préserver un aspect géométrique rigoureux et professionnel.
* **Tableaux de données (`mat-table`) :** Affichage dense mais aéré. Intégration obligatoire d'un effet de survol au curseur (*hover*) discret en `#F1F5F9` pour identifier instantanément la ligne sélectionnée.
* **Iconographie :** Utilisation exclusive de la bibliothèque **`Lucide Icons`** ou **`Material Symbols (Outlined)`** avec un trait fin et constant.

### 8.4 Prompt de Configuration Graphique pour Claude
> "Pour l'interface Angular 17+ avec Angular Material, applique la charte graphique suivante dans le fichier de thème global (`styles.scss`) :
> * Définis un thème personnalisé (Custom Theme) avec : Primary = `#1E293B` (Bleu Midnight) et Accent = `#B45309` (Or Royal).
> * Utilise la police `Merriweather` pour les titres principaux afin de donner un aspect institutionnel/juridique, et la police `Inter` pour tout le texte courant et les tableaux.
> * Configure l'arrière-plan général de l'application en `#F8FAFC` et utilise des cartes (`mat-card`) blanches avec une ombre très légère (`box-shadow`) pour séparer proprement les zones de travail.
> * Applique un `border-radius: 4px` sur tous les composants interactifs (boutons, inputs) pour conserver un style rigoureux et professionnel."
