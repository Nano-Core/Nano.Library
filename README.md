# Nano.Library
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/NanoCore.svg)](https://www.nuget.org/packages/NanoCore/)
[![NuGet](https://img.shields.io/nuget/v/NanoCore.svg)](https://www.nuget.org/packages/NanoCore/)
![GitHub License](https://img.shields.io/github/license/Nano-Core/Nano.Library)
![GitHub top language](https://img.shields.io/github/languages/top/Nano-Core/Nano.Library)
![GitHub branch count](https://img.shields.io/github/branches/Nano-Core/Nano.Library)
![GitHub commit activity](https://img.shields.io/github/commit-activity/y/Nano-Core/Nano.Library)
![GitHub Issues or Pull Requests](https://img.shields.io/github/issues/Nano-Core/Nano.Library)
![GitHub Issues or Pull Requests](https://img.shields.io/github/issues-pr/Nano-Core/Nano.Library)
![GitHub contributors](https://img.shields.io/github/contributors/Nano-Core/Nano.Library)
![GitHub Sponsors](https://img.shields.io/github/sponsors/Nano-Core)
![GitHub Discussions](https://img.shields.io/github/discussions/Nano-Core/Nano.Library)
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/Nano-Core/Nano.Library)

> _Nano - Your light-weight foundation for building modern .NET applications._  

***

## Table of Contents
* 💡 **[Summary](#summary)**
* 🏛️ **[Nano Architectures](#nano-architectures)**
* ✨ **[Highlighted Features](#highlighted-features)**
* <img src="https://api.nuget.org/v3-flatcontainer/nuget.protocol/6.0.0/icon" width="19" /> **[NuGet Packages](#nuget-packages)**
* 🧩 **[Solution Composition](#solution-composition)**
* 🚀 **[Applications](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App)**
  * **[Api](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Api)**
  * **[Console](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Console)**
  * **[Web](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Web)**
* 📦 **[Providers](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App)**
  * **[Logging](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging)**
  * **[Data](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data)**
  * **[Eventing](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Eventing)**
  * **[Storage](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Storage)**

## Summary
Nano is a leight-weight library for rapidly building .NET applications.  

These pages covers documentation, guides, samples and other relevant information, to assist you in understanding and using Nano.  

But first let us briefly convince you why Nano is the right choice when building .NET applications.  

Nano gives you everything you tecnically to start building micro-services 
* Provides configurable implementations for non-business related aspects of an application, 
* Combining the best practices, patterns, extensions and conventions of modern development. 
* Inject providers and register custom dependencies.
* Override, extend or disable any part of Nano, not suitable for the application. 
* Derive concrete implementations, enriched with functionality, from base abstractions and interfaces, for all parts of the application.  
* Avoid wasting valuable development resources, on matters not related to the business.
* Remain focused on modelling data and operations of the business domain.

It makes it easy to create and configure a web, api and console applications running in docker locally and in Kubernetes in staging and production environment. Nano enables registering
providers during application startup. Providers represents the pillars in developing distributed services in layered architectures. Nano provides easy integration with
Data, Eventing, Logging and Storage. Additionally, Nano exposes many features in data in distributed applications, allowing entity framework to synchronize entities across services
using the Eventing provider. Also, the api-client in Nano features east integration and commincation between applications, making it seamless to create and maintain a 
distributed archtiecure, even for small teams.

Configure your own Nano [Api](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Api), [Web](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Web) 
or [Console](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Console) application in less than an hour. 
Have enriched features through seeamless configuration, where you can opt in for the features and stuff you want.
Choose providers for Database, Logging, Storage and Eventing and be ready to extend your application with the dependencies required by modern applications.
Exapnd you vision and create a farm of Nano API services seamlessly working together. Built exposed APIs for your different audiences on top of your API services 
and reuse service functionality across audiences or domains, and experience how easy it can be to construct and orchestrate micros-services. Additionally, a handful 
of Nano Consoles applications, periodically run to ensure important batch tasks gets completed.

Nano contains all the boiler-blate for fast and easy create and manage applications. 
Additionally, Nano also provides the specifications for the infrastructure required to run Nano applications securily and scalable in Kubernetes on Azure.

See **[Nano.Azure](https://github.com/Nano-Core/Nano.Azure)** and **[Nano.Azure.Kubernetes](https://github.com/Nano-Core/Nano.Azure.Kubernetes)**.

Easily monitor application health, create startup-tasks, background workers, and more...
Nano takes care of everything to build, deploy and host applications.
Nano supplies it all; The application framework, easy configuration, configurable templates for Kubernetes and GitHub actions for build and deployment. Nano even supplies 
GitHub Actions for deploying all the Azure and Kubernetes components required to use all the features of Nano.  

Check out our ✨ **[Highlighted Features](#highlighted-features)**

It's recommended to start out by following our **[Quick Guide](Quick-Guide)**, to get familiarized with Nano. 
Also, inspect these **[Sample Templates](https://github.com/Nano-Core/Nano.Templates)**.  

Next, continue reading the documentation of applications and providers, to get a more comprehensive understanding, and to learn how to Configure, inject, extend, override 
and otherwise customize the behavior of Nano.  

Also, don't forget to check out our **[Nano.Lessons](https://github.com/Nano-Core/Nano.Lessons)**. There are **100+** examples of Nano applications, show-casing each 
feature individually in isolation.  

> If you are new to Nano, it's recommended to follow your [Quick Start Guide](https://github.com/Nano-Core/Nano.Library/tree/master/QUICK_START.md)

## 🏛️ Nano Architectures
* Solo application
* micro-service orchestratration (top-down api-to-services)
* micro-service orchestratration (service-to-service)

## ✨ Highlighted features

### ✨ Api Clients


### ✨ Entity Eventing
Entity Events in Nano provide a lightweight, attribute-driven mechanism for synchronizing entity changes across distributed applications. They automatically propagate create, update, and delete operations through structured `EntityEvent`s, reducing the need for manual integration logic in microservice architectures.  

The `PublishAttribute` defines which entities emit events and which properties are included in the event payload, including support for navigation paths. The `SubscribeAttribute` enables models to react to incoming events and automatically apply changes through the built-in event handler.  

Entity eventing supports inheritance, deterministic payload shaping, and automatic data hydration to ensure complete event consistency. Reverse dependency tracking allows changes in dependent entities to propagate back to root aggregates.  

This model reduces coupling between services while maintaining data consistency across boundaries. 

### ✨ Include Annotation (graph-lite)


### ✨ Health Check Spider Web

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

## <img src="https://api.nuget.org/v3-flatcontainer/nuget.protocol/6.0.0/icon" width="27" /> NuGet Packages
These packages are all-inclusive packages, where all other Nano packages are included. Easy to get started, but it's recommended to instead select only the
packages needed by your application, to avoid unnecessary depdendencies.  

| Package                                                                      | Downloads                                                                                           |                                                                                                     |
| ---------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------- |
| [Nano.All](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.All)   | [![NuGet](https://img.shields.io/nuget/dt/Nano.All.svg)](https://www.nuget.org/packages/Nano.All/)  | [![NuGet](https://img.shields.io/nuget/v/Nano.All.svg)](https://www.nuget.org/packages/Nano.All/)   |
| [NanoCore](https://github.com/Nano-Core/Nano.Library/tree/master/NanoCore)   | [![NuGet](https://img.shields.io/nuget/dt/NanoCore.svg)](https://www.nuget.org/packages/NanoCore/)  | [![NuGet](https://img.shields.io/nuget/v/NanoCore.svg)](https://www.nuget.org/packages/NanoCore/)   |

The next packages are application packages. Include only one matching the application type you want to create.  

| Package                      | Type              | Downloads                                                                                                           | Latest Version                                                                                                     |
| ---------------------------- | ----------------- | ------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------ |
| Nano.App                     | _`Transitive`_    | [![NuGet](https://img.shields.io/nuget/dt/Nano.App.svg)](https://www.nuget.org/packages/Nano.App/) 0                | [![NuGet](https://img.shields.io/nuget/v/Nano.App.svg)](https://www.nuget.org/packages/Nano.App/)                  |
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
