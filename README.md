# Nano.Library
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/NanoCore.svg)](https://www.nuget.org/packages/NanoCore/)
[![NuGet](https://img.shields.io/nuget/v/NanoCore.svg)](https://www.nuget.org/packages/NanoCore/)
![GitHub Release](https://img.shields.io/github/v/release/Nano-Core/Nano.Library)
![GitHub Release Date](https://img.shields.io/github/release-date-pre/Nano-Core/Nano.Library)
![GitHub License](https://img.shields.io/github/license/Nano-Core/Nano.Library)
![GitHub Top Language](https://img.shields.io/github/languages/top/Nano-Core/Nano.Library)
![GitHub Branch Count](https://img.shields.io/github/branches/Nano-Core/Nano.Library)
![GitHub Commit Activity](https://img.shields.io/github/commit-activity/y/Nano-Core/Nano.Library)
![GitHub Bugs](https://img.shields.io/github/issues-search/Nano-Core/Nano.Library?query=is%3Aopen%20label%3Abug&label=bugs&labelColor=grey&color=red)
![GitHub Tasks](https://img.shields.io/github/issues-search/Nano-Core/Nano.Library?query=is%3Aopen%20-label%3Abug&label=tasks)
![GitHub Pull Requests](https://img.shields.io/github/issues-pr/Nano-Core/Nano.Library)
![GitHub Contributors](https://img.shields.io/github/contributors/Nano-Core/Nano.Library)
![GitHub Sponsors](https://img.shields.io/github/sponsors/Nano-Core)
![GitHub Discussions](https://img.shields.io/github/discussions/Nano-Core/Nano.Library)
![GitHub Size](https://img.shields.io/github/languages/code-size/Nano-Core/Nano.Library)

> _Nano - Your lightweight foundation for building modern .NET applications._  

***

## Table of Contents
&nbsp;&nbsp;&nbsp;&nbsp;📌 **[Summary](#-summary)**  
&nbsp;&nbsp;&nbsp;&nbsp;✨ **[Highlighted Features](#-highlighted-features)**  
&nbsp;&nbsp;&nbsp;&nbsp;🏛️ **[Nano Architectures](#%EF%B8%8F-nano-architectures)**  
&nbsp;&nbsp;&nbsp;&nbsp;⚙️ **[Required Tools](#-required-tools)**  
&nbsp;&nbsp;&nbsp;&nbsp;🧩 **[Solution Composition](#-solution-composition)**  
&nbsp;&nbsp;&nbsp;&nbsp;📦 **[NuGet Packages](#-nuget-packages)**  
&nbsp;&nbsp;&nbsp;&nbsp;🔏 **[Licenses](#-licenses)**  

### Documentation
&nbsp;&nbsp;&nbsp;&nbsp;🚀 **[Applications](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App#README.md#nanoapp)**  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;📡 **[Api](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Api/README.md#nanoappapi)**  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;⚙️ **[Console](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Console/README.md#nanoappconsole)**  
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;🌐 **[Web](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Web/README.md#nanoappweb)**  
&nbsp;&nbsp;&nbsp;&nbsp;📜 **[Logging](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging/README.md#nanologging)**  
&nbsp;&nbsp;&nbsp;&nbsp;🛢️ **[Data](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data/README.md#nanodata)**  
&nbsp;&nbsp;&nbsp;&nbsp;⚡ **[Eventing](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Eventing/README.md#nanoeventing)**  
&nbsp;&nbsp;&nbsp;&nbsp;🗂 **[Storage](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Storage/README.md#nanostorage)**  

## 📌 Summary
Nano is a lightweight library for rapidly building modern .NET applications.

These pages provide documentation, guides, samples, and other resources to help you understand and use Nano effectively. To set the stage, here’s why Nano is a strong choice 
when building .NET applications.

At its core, Nano gives you everything you need to start building microservices without getting bogged down in non-business concerns. It provides configurable implementations 
based on proven practices, patterns, and conventions from modern development. Designed to integrate naturally with dependency injection, Nano allows you to register custom services 
while retaining full control to override, extend, or disable any part of the framework. This flexibility lets you derive enriched implementations from core abstractions, so you can 
stay focused on modeling your business domain rather than managing boilerplate.

As your solution evolves, Nano scales with you. It becomes easy to compose multiple applications that work seamlessly together, whether you're building APIs or web applications for 
different audiences or reusing functionality across domains. Console applications can support scheduled or background batch processing, while built-in features such as event-driven 
entity synchronization and a dedicated API client simplify communication between services. Combined with support for health monitoring, startup tasks, background workers, audit and 
more, Nano helps cover the full application lifecycle, from development and configuration to deployment and hosting.

> ✨ Check out our **[Highlighted Features](#highlighted-features)**

To support production-grade environments, Nano also provides the foundation for running applications securely and at scale in Kubernetes on Azure. Nano includes ready-to-use templates
for Kubernetes and GitHub Actions, enabling consistent CI/CD pipelines and infrastructure provisioning.

> 📖 Learn more about **[Nano.Azure](https://github.com/Nano-Core/Nano.Azure/README.md#nanoazure)** and **[Nano.Azure.Kubernetes](https://github.com/Nano-Core/Nano.Azure.Kubernetes/README.md#nanoazurekubernetes)**

Together, these capabilities make Nano a strong choice for designing, building, and maintaining microservice architectures, even for small teams.

### 🚀 Launch Your App in Under 60 Minutes 
Get started quickly by building your own Nano **[Api](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Api/README.md&nanoapi)**, **[Web](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Web/README.md#nanoweb)**, 
or **[Console](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Console/README.md#nanoconsole)** application, and have it configured and running in less than an hour. Nano follows a flexible, 
opt-in approach, allowing you to configure and enable only the features you need. For the four core pillars of distributed systems, **[Logging](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging/README.md#nanologging)**, 
**[Data](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data/README.md#nanodata)**, **[Eventing](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Eventing/README.md#nanoeventing)**, 
and **[Storage](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Storage#/README.md#nanostorage)**, Nano provides pluggable providers that integrate seamlessly into your application. 
Simply choose and configure the providers that match your requirements for each pillar, and Nano handles the rest.

It’s recommended to start with the **[Quick Start Guide](https://github.com/Nano-Core/Nano.Library/tree/master/QUICK_START.md)** to get familiar with Nano, and then continue exploring 
the documentation for applications and providers to learn how to configure, use, and customize it. You can also dive into **[Nano.Lessons](https://github.com/Nano-Core/Nano.Lessons)**, 
which contains **100+** focused examples that demonstrate individual features in isolation.

> 💡 Explore API requests for all lessons in our **[Public Nano Workspace on Postman](https://www.postman.com/nanocore/nano-lessons)**.

## ✨ Highlighted features

### ✨ Api Clients
Nano provides a generic API client that enables seamless communication between distributed Nano services. It is designed for API-based applications and allows services to interact 
in a structured and consistent way across systems.

API clients are configured through application settings and are automatically injected where needed, making service-to-service communication simple and centralized. They provide access 
to common functional areas such as data operations, authentication, auditing, and identity workflows through consistent conventions.

In addition to built-in capabilities, Nano supports extending API clients with custom endpoints, enabling strongly typed and flexible integration with application-specific 
functionality. Authentication, headers, and request metadata are handled automatically, including secure propagation between services. This ensures reliable, consistent, and 
secure communication across all connected Nano applications without additional boilerplate.

> 📖 Learn more about **[Api Clients](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App/README#api-clients)**

### ✨ Entity Eventing
Entity Events in Nano provide a lightweight, attribute-driven mechanism for synchronizing entity changes across distributed applications. They automatically propagate create, update, 
and delete operations through structured `EntityEvent`s, reducing the need for manual integration logic in microservice architectures.  

The `PublishAttribute` defines which entities emit events and which properties are included in the event payload, including support for navigation paths. The `SubscribeAttribute` 
enables models to react to incoming events and automatically apply changes through the built-in event handler.  

Entity eventing supports inheritance, deterministic payload shaping, and automatic data hydration to ensure complete event consistency. Reverse dependency tracking allows changes in 
dependent entities to propagate back to root aggregates.  

This model reduces coupling between services while maintaining data consistency across boundaries.  

> 📖 Learn more about **[Entity Eventing](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data/README.md#entity-eventing)**

### ✨ Include Annotation (graph-lite)
Nano’s Include feature automatically loads navigation properties when retrieving entities through repositories, enabling full entity graphs with recursive inclusion up to a configured 
depth. It supports both references and collections, simplifying data access for rich models.  

This is especially useful when you want to work with complete object graphs instead of manually composing related data. However, inclusion can be selectively controlled per query, 
allowing you to retrieve only the main entity when full graphs are not needed.  

> 📖 Learn more about **[Include Annotations](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data/README.md#include-annotation)**

### ✨ Cascading Health Checks
Nano provides built-in health checks exposed through a /healthz endpoint for monitoring application status.

All Nano providers and services can participate in health monitoring when enabled, automatically reporting their status. Health checks are structured as a dependency tree, where 
failures propagate through related components and applications to provide a consistent view of overall system health.

In addition, custom health checks can be registered during application startup, allowing application-specific components to integrate seamlessly into the same monitoring system.

> 📖 Learn more about **[Api Health Checks](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Api/README.md#health-checks)**

## 🏛️ Nano Architectures
Nano supports multiple distributed architecture styles, depending on the complexity and communication patterns of your system.

| Architecture                                     | Description                                                                                                                                                               |
| ------------------------------------------------ | -------------------------------------------------------------------------------- |
| Solo Application                                 | A single, independent application.                                               |
| Microservice Orchestration (Top-down)            | API-driven orchestration where a central API coordinates downstream services.    |
| Microservice Orchestration (Service-to-Service)  | Decentralized communication where services interact directly with each other.    |

## ⚙️ Required Tools
Before getting started, make sure you have the following tools installed and configured.  

| Tool                     | Description                                                                  |
| ------------------------ | ---------------------------------------------------------------------------- |
| **Docker Desktop**       | Runs supporting services such as databases and message brokers locally.      |
| **Visual Studio (.NET)** | Primary IDE with .NET workload for building and running the application.     |
| Git                      | Used for cloning the repository and managing source code.                    |
| Terminal / CLI           | Executes commands (PowerShell, bash, Windows Terminal, etc.).                |

And optional, but recommended, tools.  

| Tool                     | Description                                                                  |
| ------------------------ | ---------------------------------------------------------------------------- |
| Postman (or similar)     | Helps test and explore API endpoints during development.                     |

## 🧩 Solution Composition
All Nano applications follow a consistent and predictable solution structure.  

In local `Development`, the application is orchestrated using Docker Compose, while `Staging` and `Production` environments are deployed and managed using Kubernetes.  

Continuous Integration and Continuous Delivery (CI/CD) is handled by GitHub Actions and is included as part of the Nano Visual Studio solution.  

The solution root also contains several supporting files, such as Docker- and Git-related configuration files, along with various asset files used by 
the Git repository and NuGet packages.

In the following table shows the different files and folder strucutre.

| Directory / File                                    | API | WEB | CON | Description                                                                                                                                                                                                                                     |
| --------------------------------------------------- | --- | --- | --- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `.docker/docker-compose.dcproj`                     | ✓   | ✓   | ✓   | The Docker Compose project used by Visual Studio for local orchestration.                                                                                                                                                                       |
| `.docker/docker-compose.yml`                        | ✓   | ✓   | ✓   | The Docker Compose specification for orchestrating the application locally in the `Development` environment.                                                                                                                                    |
| `.github/config/slack.yml`                          | ✓   | ✓   | ✓   | Configuration for posting build and deploy notifications to Slack _(optional)_.                                                                                                                                                                 |
| `.github/workflows/build-and-deploy.yml`            | ✓   | ✓   | ✓   | GitHub Actions workflow that builds, tests, publishes artifacts, and deploys a Nano application.                                                                                                                                                |
| `.kubernetes/configmap.yaml`                        | ✓   | ✓   | ✓   | [ConfigMap](https://kubernetes.io/docs/concepts/configuration/configmap/) specification for Kubernetes.                                                                                                                                         |
| `.kubernetes/autoscaler.yaml`                       | ✓   | ✓   | ✗  | [Horizontal Pod Autoscaler (HPA)](https://kubernetes.io/docs/concepts/workloads/autoscaling/horizontal-pod-autoscale/) specification.                                                                                                           |
| `.kubernetes/deployment.yaml`                       | ✓   | ✓   | ✗  | [Deployment](https://kubernetes.io/docs/concepts/services-networking/ingress/) specification.                                                                                                                                                   |
| `.kubernetes/service.yaml`                          | ✓   | ✓   | ✗  | [Service](https://kubernetes.io/docs/concepts/services-networking/service/) exposure specification.                                                                                                                                             |
| `.kubernetes/ingress.yaml`                          | (✓) | (✓) | ✗  | [Ingress](https://kubernetes.io/docs/concepts/services-networking/ingress/) specification _(Optional)_.                                                                                                                                         |
| `.kubernetes/certificate.yaml`                      | (✓) | (✓) | ✗  | [SSL Certificate](https://cert-manager.io/docs/usage/certificate/) specification _(Optional)_.                                                                                                                                                  |
| `.kubernetes/cronjob.yaml`                          | ✗  | ✗   | ✓   | [CronJob](https://kubernetes.io/docs/concepts/workloads/controllers/cron-jobs/) specification.                                                                                                                                                  |
| `.tests/Tests.{name}.csproj`                        | ✓   | ✓   | ✓   | Test project, which is empty by default and included to demonstrate the structure and where unit or integration tests should be added.                                                                                                          |
| `.tests/Properties/DoNotParallelize.cs`             | ✓   | ✓   | ✓   | Ensures tests are not Parallelized.                                                                                                                                                                                                             |
| `{name}/{name}.csproj`                              | ✓   | ✓   | ✓   | The application project file.                                                                                                                                                                                                                   |
| `{name}/Properties/InternalsVisibleTo.cs`           | ✓   | ✓   | ✓   | Exposes internal types to the test project.                                                                                                                                                                                                     |
| `{name}/wwwroot`                                    | ✓   | ✓   | ✗   | Root folder for static and dnyamic web content.                                                                                                                                                                                                |
| `{name}/appsettings.json`                           | ✓   | ✓   | ✓   | Default application configuration file.                                                                                                                                                                                                         |
| `{name}/appsettings.{environment}.json`             | ✓   | ✓   | ✓   | Overrides application configuration files for for environments: `Development`, `Staging` and `Production`.                                                                                                                                      |
| `{name}/Dockerfile.Local`                           | ✓   | ✓   | ✓   | Used by Docker Compose in `Development` environment; must remain in the application project folder.                                                                                                                                             |
| `{name}/Program.cs`                                 | ✓   | ✓   | ✓   | The main entry point to the Nano application, and where the application is configured, build and run.                                                                                                                                           |
| `{name}/{name}.Models.csproj`                       | ✓   | ✓   | ✗  | The application models project file. The project is configured to publish a NuGet for sharing models and api-client. Nano Nugets should be included here, as a minimum the [Nano.App](https://www.nuget.org/packages/Nano.App) NuGet package.   |
| `.dockerignore`                                     | ✓   | ✓   | ✓   | Lists files and folders to ignore when building Docker images.                                                                                                                                                                                  |
| `.gitignore`                                        | ✓   | ✓   | ✓   | Lists files and folders to ignore in Git version control.                                                                                                                                                                                       |
| `Dockerfile`                                        | ✓   | ✓   | ✓   | The `Dockerfile` used to build the container image for `Staging` and `Production` deployments.                                                                                                                                                  |
| `README.md`                                         | (✓) | (✓) | (✓) | Documentation asset for the Git repository and the application’s NuGet package _(Optional)_.                                                                                                                                                   |
| `icon.png`                                          | (✓) | (✓) | ✗   | Icon asset for the application’s NuGet package _(Optional)_.                                                                                                                                                                                   |
| `LICENSE`                                           | (✓) | (✓) | ✗   | License file for the Git repository and the application’s NuGet package _(Optional)_.                                                                                                                                                          |
| `{name}.sln`                                        | ✓   | ✓   | ✓   | The visual studio solution file.                                                                                                                                                                                                                |

## 📦 NuGet Packages
These packages are all-inclusive packages, where all other Nano packages are included. Easy to get started, but it's recommended to instead select only the
packages needed by your application, to avoid unnecessary depdendencies.  

| Package    | Downloads                                                                                           |                                                                                                     |
| ---------- | --------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------- |
| Nano.All   | [![NuGet](https://img.shields.io/nuget/dt/Nano.All.svg)](https://www.nuget.org/packages/Nano.All/)  | [![NuGet](https://img.shields.io/nuget/v/Nano.All.svg)](https://www.nuget.org/packages/Nano.All/)   |
| NanoCore   | [![NuGet](https://img.shields.io/nuget/dt/NanoCore.svg)](https://www.nuget.org/packages/NanoCore/)  | [![NuGet](https://img.shields.io/nuget/v/NanoCore.svg)](https://www.nuget.org/packages/NanoCore/)   |

And the Common Package.  

| Package                      | Type              | Downloads                                                                                                  | Latest Version                                                                                            |
| ---------------------------- | ----------------- | ---------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------- |
| Nano.Common                  | _`Transitive`_    | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/)   | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/)   |

The next packages are application packages. Include only one matching the application type you want to create.  

| Package                      | Type              | Downloads                                                                                                           | Latest Version                                                                                                     |
| ---------------------------- | ----------------- | ------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------ |
| Nano.App                     | _`Transitive`_    | [![NuGet](https://img.shields.io/nuget/dt/Nano.App.svg)](https://www.nuget.org/packages/Nano.App/)                  | [![NuGet](https://img.shields.io/nuget/v/Nano.App.svg)](https://www.nuget.org/packages/Nano.App/)                  |
| Nano.App.Api                 | `Application`     | [![NuGet](https://img.shields.io/nuget/dt/Nano.App.Api.svg)](https://www.nuget.org/packages/Nano.App.Api/)          | [![NuGet](https://img.shields.io/nuget/v/Nano.App.Api.svg)](https://www.nuget.org/packages/Nano.App.Api/)          |
| Nano.App.Console             | `Application`     | [![NuGet](https://img.shields.io/nuget/dt/Nano.App.Console.svg)](https://www.nuget.org/packages/Nano.App.Console/)  | [![NuGet](https://img.shields.io/nuget/v/Nano.App.Console.svg)](https://www.nuget.org/packages/Nano.App.Console/)  |
| Nano.App.Web                 | `Application`     | [![NuGet](https://img.shields.io/nuget/dt/Nano.App.Web.svg)](https://www.nuget.org/packages/Nano.App.Web/)          | [![NuGet](https://img.shields.io/nuget/v/Nano.App.Web.svg)](https://www.nuget.org/packages/Nano.App.Web/)          |

Last, provider packages. These enable functionality related to Nano providers.  

| Package                      | Type                         | Downloads                                                                                                                                | Latest Version                                                                                                                          |
| ---------------------------- | ---------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------- |
| Nano.Data                    | `Data`, _`Transitive`_       | [![NuGet](https://img.shields.io/nuget/dt/Nano.Data.svg)](https://www.nuget.org/packages/Nano.Data/)                                     | [![NuGet](https://img.shields.io/nuget/v/Nano.Data.svg)](https://www.nuget.org/packages/Nano.Data/)                                     |
| Nano.Data.Abstractions       | `Data`, _`Transitive`_       | [![NuGet](https://img.shields.io/nuget/dt/Nano.Data.Abstractions.svg)](https://www.nuget.org/packages/Nano.Data.Abstractions/)           | [![NuGet](https://img.shields.io/nuget/v/Nano.Data.Abstractions.svg)](https://www.nuget.org/packages/Nano.Data.Abstractions/)           |
| Nano.Data.InMemory           | `Data`                       | [![NuGet](https://img.shields.io/nuget/dt/Nano.Data.InMemory.svg)](https://www.nuget.org/packages/Nano.Data.InMemory/)                   | [![NuGet](https://img.shields.io/nuget/v/Nano.Data.InMemory.svg)](https://www.nuget.org/packages/Nano.Data.InMemory/)                   |
| Nano.Data.MySql              | `Data`                       | [![NuGet](https://img.shields.io/nuget/dt/Nano.Data.MySql.svg)](https://www.nuget.org/packages/Nano.Data.MySql/)                         | [![NuGet](https://img.shields.io/nuget/v/Nano.Data.MySql.svg)](https://www.nuget.org/packages/Nano.Data.MySql/)                         |
| Nano.Data.PostgreSQL         | `Data`                       | [![NuGet](https://img.shields.io/nuget/dt/Nano.Data.PostgreSQL.svg)](https://www.nuget.org/packages/Nano.Data.PostgreSQL/)               | [![NuGet](https://img.shields.io/nuget/v/Nano.Data.PostgreSQL.svg)](https://www.nuget.org/packages/Nano.Data.PostgreSQL/)               |
| Nano.Data.SqLite             | `Data`                       | [![NuGet](https://img.shields.io/nuget/dt/Nano.Data.SqLite.svg)](https://www.nuget.org/packages/Nano.Data.SqLite/)                       | [![NuGet](https://img.shields.io/nuget/v/Nano.Data.SqLite.svg)](https://www.nuget.org/packages/Nano.Data.SqLite/)                       |
| Nano.Data.SqlServer          | `Data`                       | [![NuGet](https://img.shields.io/nuget/dt/Nano.Data.SqlServer.svg)](https://www.nuget.org/packages/Nano.Data.SqlServer/)                 | [![NuGet](https://img.shields.io/nuget/v/Nano.Data.SqlServer.svg)](https://www.nuget.org/packages/Nano.Data.SqlServer/)                 |
|                              |                              |                                                                                                                                          |                                                                                                                                         | 
| Nano.Eventing                | `Eventing`, _`Transitive`_   | [![NuGet](https://img.shields.io/nuget/dt/Nano.Eventing.svg)](https://www.nuget.org/packages/Nano.Eventing/)                             | [![NuGet](https://img.shields.io/nuget/v/Nano.Eventing.svg)](https://www.nuget.org/packages/Nano.Eventing/)                             |
| Nano.Eventing.Abstractions   | `Eventing`, _`Transitive`_   | [![NuGet](https://img.shields.io/nuget/dt/Nano.Eventing.Abstractions.svg)](https://www.nuget.org/packages/Nano.Eventing.Abstractions/)   | [![NuGet](https://img.shields.io/nuget/v/Nano.Eventing.Abstractions.svg)](https://www.nuget.org/packages/Nano.Eventing.Abstractions/)   |
| Nano.Eventing.RabbitMq       | `Eventing`                   | [![NuGet](https://img.shields.io/nuget/dt/Nano.Eventing.RabbitMq.svg)](https://www.nuget.org/packages/Nano.Eventing.RabbitMq/)           | [![NuGet](https://img.shields.io/nuget/v/Nano.Eventing.RabbitMq.svg)](https://www.nuget.org/packages/Nano.Eventing.RabbitMq/)           |
|                              |                              |                                                                                                                                          |                                                                                                                                         |  
| Nano.Logging                 | `Logging`, _`Transitive`_    | [![NuGet](https://img.shields.io/nuget/dt/Nano.Logging.svg)](https://www.nuget.org/packages/Nano.Logging/)                               | [![NuGet](https://img.shields.io/nuget/v/Nano.Logging.svg)](https://www.nuget.org/packages/Nano.Logging/)                               |
| Nano.Logging.Abstractions    | `Logging`, _`Transitive`_    | [![NuGet](https://img.shields.io/nuget/dt/Nano.Logging.Abstractions.svg)](https://www.nuget.org/packages/Nano.Logging.Abstractions/)     | [![NuGet](https://img.shields.io/nuget/v/Nano.Logging.Abstractions.svg)](https://www.nuget.org/packages/Nano.Logging.Abstractions/)     |
| Nano.Logging.Log4Net         | `Logging`                    | [![NuGet](https://img.shields.io/nuget/dt/Nano.Logging.Log4Net.svg)](https://www.nuget.org/packages/Nano.Logging.Log4Net/)               | [![NuGet](https://img.shields.io/nuget/v/Nano.Logging.Log4Net.svg)](https://www.nuget.org/packages/Nano.Logging.Log4Net/)               |
| Nano.Logging.Microsoft       | `Logging`                    | [![NuGet](https://img.shields.io/nuget/dt/Nano.Logging.Microsoft.svg)](https://www.nuget.org/packages/Nano.Logging.Microsoft/)           | [![NuGet](https://img.shields.io/nuget/v/Nano.Logging.Microsoft.svg)](https://www.nuget.org/packages/Nano.Logging.Microsoft/)           |
| Nano.Logging.NLog            | `Logging`                    | [![NuGet](https://img.shields.io/nuget/dt/Nano.Logging.NLog.svg)](https://www.nuget.org/packages/Nano.Logging.NLog/)                     | [![NuGet](https://img.shields.io/nuget/v/Nano.Logging.NLog.svg)](https://www.nuget.org/packages/Nano.Logging.NLog/)                     |
| Nano.Logging.Serilog         | `Logging`                    | [![NuGet](https://img.shields.io/nuget/dt/Nano.Logging.Serilog.svg)](https://www.nuget.org/packages/Nano.Logging.Serilog/)               | [![NuGet](https://img.shields.io/nuget/v/Nano.Logging.Serilog.svg)](https://www.nuget.org/packages/Nano.Logging.Serilog/)               |
|                              |                              |                                                                                                                                          |                                                                                                                                         | 
| Nano.Storage                 | `Storage`, _`Transitive`_    | [![NuGet](https://img.shields.io/nuget/dt/Nano.Storage.svg)](https://www.nuget.org/packages/Nano.Storage/)                               | [![NuGet](https://img.shields.io/nuget/v/Nano.Storage.svg)](https://www.nuget.org/packages/Nano.Storage/)                               |
| Nano.Storage.Abstractions    | `Storage`, _`Transitive`_    | [![NuGet](https://img.shields.io/nuget/dt/Nano.Storage.Abstractions.svg)](https://www.nuget.org/packages/Nano.Storage.Abstractions/)     | [![NuGet](https://img.shields.io/nuget/v/Nano.Storage.Abstractions.svg)](https://www.nuget.org/packages/Nano.Storage.Abstractions/)     |
| Nano.Storage.Azure           | `Storage`                    | [![NuGet](https://img.shields.io/nuget/dt/Nano.Storage.Azure.svg)](https://www.nuget.org/packages/Nano.Storage.Azure/)                   | [![NuGet](https://img.shields.io/nuget/v/Nano.Storage.Azure.svg)](https://www.nuget.org/packages/Nano.Storage.Azure/)                   |
| Nano.Storage.Local           | `Storage`                    | [![NuGet](https://img.shields.io/nuget/dt/Nano.Storage.Local.svg)](https://www.nuget.org/packages/Nano.Storage.Local/)                   | [![NuGet](https://img.shields.io/nuget/v/Nano.Storage.Local.svg)](https://www.nuget.org/packages/Nano.Storage.Local/)                   |

## 🔏 Licenses
Nano is free to use and released under the MIT License.  
Dependencies are distributed under a combination of the following licenses.

| License                                                 | Description                                                                     |
| ------------------------------------------------------- | ------------------------------------------------------------------------------- |
| [MIT](https://licenses.nuget.org/MIT)                   | Permissive license with minimal restrictions.                                   |
| [Apache-2.0](https://licenses.nuget.org/Apache-2.0)     | Permissive with patent protection.                                              |
| [BSD-2 Clause](https://licenses.nuget.org/BSD-2-Clause) | Simple permissive license with attribution.                                     |
| [BSD-3 Clause](https://licenses.nuget.org/BSD-3-Clause) | Permissive with non-endorsement clause.                                         |
| [PostgreSQL](https://licenses.nuget.org/PostgreSQL)     | Permissive, similar to MIT/BSD. _Only when using PostgreSQL as data provider_.  |









## Variables And Secrets

Nano requires a GitHub organization variable named `VERSION` that defines the major and minor version. The full version is then automatically composed during each workflow run 
by appending the run metadata (`run_number` and `run_attempt`) to this base version.

```yaml
env:
  VERSION: "${{ vars.VERSION }}.${{ github.run_number }}.${{ github.run_attempt }}"
```

This produces versions in the following format: `1.0.123.1`, where: `1.0` is manually maintained major/minor version, `123` is the GitHub run number (auto-incremented), and 
`1` is the run attempt (increments on deployment retries).






  APP_NAME: Api.Data.Repository.AutoSave
  IMAGE_NAME: api.data.repository.autosave
  SERVICE_NAME: api-data-repository-autosave
  VERSION: '${{ vars.VERSION }}.${{ github.run_number }}.${{ github.run_attempt }}'
  DOTNET_SDK_VERSION: 10.0
  DOTNET_ASPNET_VERSION: 10.0
  
  API / Web
  ASPNETCORE_ENVIRONMENT: ${{ github.ref == 'refs/heads/master' && 'Production' || 'Staging' }}

  Console
  DOTNET_ENVIRONMENT: ${{ github.ref == 'refs/heads/master' && 'Production' || 'Staging' }}
  KUBERNETES_CRONJOB_SCHEDULE: "0 * * * *"


  Storage
  STORAGE_SIZE: 1000
  STORAGE_SHARE_NAME: nano-storage-local

  Storage Azure
  STORAGE_CREDENTIALS_ID: ${{ github.ref == 'refs/heads/master' && secrets.PRODUCTION_STORAGE_CREDENTIALS_ID  || secrets.STAGING_STORAGE_CREDENTIALS_ID }}
  STORAGE_CREDENTIALS_SECRET: ${{ github.ref == 'refs/heads/master' && secrets.PRODUCTION_STORAGE_CREDENTIALS_SECRET  || secrets.STAGING_STORAGE_CREDENTIALS_SECRET }}

  Https
  CERTIFICATE_ISSUER: letsencrypt-prod
  CERTIFICATE_ORGANIZATION: ${{ vars.CERTIFICATE_ORGANIZATION }}
  CERTIFICATE_HOST: ${{ github.ref == 'refs/heads/master' && vars.HOST_API_SUBDOMAIN + '.' + vars.PRODUCTION_HOST || vars.HOST_API_SUBDOMAIN + '.' + vars.STAGING_HOST }}

  Authentication	
  AUTH_JWT_PUBLIC_KEY: ${{ github.ref == 'refs/heads/master' && secrets.PRODUCTION_AUTH_JWT_PUBLIC_KEY || secrets.STAGING_AUTH_JWT_PUBLIC_KEY }}
  AUTH_JWT_PRIVATE_KEY: ${{ github.ref == 'refs/heads/master' && secrets.PRODUCTION_AUTH_JWT_PRIVATE_KEY || secrets.STAGING_AUTH_JWT_PRIVATE_KEY }}
  AUTH_API_KEY_SECRET: ${{ github.ref == 'refs/heads/master' && secrets.PRODUCTION_AUTH_API_KEY_SECRET || secrets.STAGING_AUTH_API_KEY_SECRET }}

  Data
  DATA_HOST: ${{ github.ref == 'refs/heads/master' && secrets.PRODUCTION_DATA_HOST || secrets.STAGING_DATA_HOST }}
  DATA_NAME: {database-name}
  DATA_USER: {database-user}
  DATA_PASSWORD: ${{ github.ref == 'refs/heads/master' && secrets.PRODUCTION_DATA_NANO_DB_PASSWORD || secrets.STAGING_DATA_NANO_DB_PASSWORD }}
  DATA_ADMIN_USER: ${{ github.ref == 'refs/heads/master' && secrets.PRODUCTION_DATA_ADMIN_USER || secrets.STAGING_DATA_ADMIN_USER }}
  DATA_ADMIN_PASSWORD: ${{ github.ref == 'refs/heads/master' && secrets.PRODUCTION_DATA_ADMIN_PASSWORD || secrets.STAGING_DATA_ADMIN_PASSWORD }}
  DATA_CONNECTIONSTRING: Server=${{ env.DATA_HOST }},${{ vars.DATA_PORT }};Database=${{ env.DATA_NAME }};User Id=${{ env.DATA_USER }};Password=${{ env.DATA_PASSWORD }};
  DATA_MIGRATION_CONNECTIONSTRING: Server=${{ env.DATA_HOST }},${{ vars.DATA_PORT }};Database=${{ env.DATA_NAME }};User Id=${{ env.DATA_ADMIN_USER }};Password=${{ env.DATA_ADMIN_PASSWORD }};

  Sql Server
  DATA_PORT: ${{ vars.DATA_PORT }}



  KUBERNETES_CLUSTER: ${{ github.ref == 'refs/heads/master' && vars.PRODUCTION_KUBERNETES_CLUSTER || vars.STAGING_KUBERNETES_CLUSTER }}
  KUBERNETES_NODEPOOL_COMPUTE: cpu
  KUBERNETES_NAMESPACE: default
  KUBERNETES_REPLICA_COUNT: ${{ github.ref == 'refs/heads/master' && 3 || 2 }}
  KUBERNETES_REPLICA_COUNT_MAX: ${{ github.ref == 'refs/heads/master' && 8 || 5 }}
  KUBERNETES_REPLICA_HISTORY_COUNT: 0
  KUBERNETES_MEMORY_REQUEST: 512Mi   
  KUBERNETES_MEMORY_LIMIT: 1536Mi
  KUBERNETES_MEMORY_SCALING: 180
  KUBERNETES_CPU_REQUEST: 200m
  KUBERNETES_CPU_LIMIT: 600m
  KUBERNETES_CPU_SCALING: 180




  Nano.GitHub
  
  NUGET_HOST: ${{ secrets.NUGET_HOST }}
  NUGET_USERNAME: ${{ secrets.NUGET_USERNAME }}
  NUGET_PASSWORD: ${{ secrets.NUGET_APIKEY }}
  NUGET_APIKEY: ${{ secrets.NUGET_APIKEY }}
  CONTAINER_REGISTRY_HOST: ${{ vars.CONTAINER_REGISTRY_HOST }}
  CONTAINER_REGISTRY_USERNAME: ${{ secrets.CONTAINER_REGISTRY_USERNAME }}
  CONTAINER_REGISTRY_PASSWORD: ${{ secrets.CONTAINER_REGISTRY_PASSWORD }}
  CONTAINER_REGISTRY_SOURCE_LABEL: https://github.com/${{ github.repository }}




  Nano.Azure

  AZURE_GROUP: ${{ vars.AZURE_KUBERNETES_RESOURCE_GROUP }}
  AZURE_TENANT_ID: ${{ secrets.AZURE_TENANT_ID }}
  AZURE_CLIENT_ID: ${{ secrets.AZURE_CLIENT_ID }}
  AZURE_CLIENT_SECRET: ${{ secrets.AZURE_CLIENT_SECRET }}
  AZURE_SUBSCRIPTION_ID: ${{ github.ref == 'refs/heads/master' && secrets.PRODUCTION_AZURE_SUBSCRIPTION_ID || secrets.STAGING_AZURE_SUBSCRIPTION_ID }}

