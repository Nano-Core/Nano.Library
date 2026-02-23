# Nano.Library
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/NanoCore.svg)](https://www.nuget.org/packages/NanoCore/)
[![NuGet](https://img.shields.io/nuget/v/NanoCore.svg)](https://www.nuget.org/packages/NanoCore/)

> _Nano._

## Table of Contents
* [Summary](#summary)
* [Getting Started](#getting-started)
* [NuGet Packages](#nuget-packages)
* [Applications](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App)
  * [Api](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Api)
  * [Console](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Console)
  * [Web](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Web)
* [Providers](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App)
  * [Logging](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging)
  * [Data](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data)
  * [Eventing](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Eventing)
  * [Storage](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Storage)
* [Architectures](#architectures)

## Summary
Nano is a library for rapidly building .NET10 applications.  
The project is inspired by years of tedious repetitions, continuously re-writing similar code-snippets and libraries, to handle common functionality, 
not related to the business domain, such as logging, data persistence, message queuing, documentation, validation and similar.

These pages covers documentation, guides, samples and other relevant information, to assist you in understanding and using Nano.  
It's recommended to start out by following our **[Quick Guide](Quick-Guide)**, to get familiarized with Nano. 
Next, continue reading to get a more comprehensive understanding, and to learn how to Configure, inject, extend, override and otherwise customize the behavior of Nano.  
Also, check out **[Nano.Lessons](https://github.com/Nano-Core/Nano.Lessons)**.  

Aspects and Benefits:
The aspects and benefits of Nano, can be summarized as follows.
* Provides configurable implementations for non-business related aspects of an application, 
* Combining the best practices, patterns, extensions and conventions of modern development. 
* Inject providers and register custom dependencies.
* Override, extend or disable any part of Nano, not suitable for the application. 
* Derive concrete implementations, enriched with functionality, from base abstractions and interfaces, for all parts of the application.  
* Avoid wasting valuable development resources, on matters not related to the business.
* Remain focused on modelling data and operations of the business domain.

## Getting Started

## NuGet Packages


| Package                      | Downloads                                                                                                |                                                                                                         |
| ---------------------------- | -------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------- |
| Nano.All                     | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.App                     | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.App.Api                 | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.App.Console             | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.App.Web                 | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Data                    | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Data.Abstractions       | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Data.InMemory           | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Data.MySql              | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Data.PostgreSQL         | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Data.SqLite             | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Data.SqlServer          | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Eventing                | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Eventing.Abstractions   | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Eventing.RabbitMq       | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Logging                 | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Logging.Abstractions    | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Logging.Log4Net         | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Logging.Microsoft       | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Logging.NLog            | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Logging.Serilog         | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Storage                 | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Storage.Abstractions    | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Storage.Azure           | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Storage.Local           | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| NanoCore                     | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |

EXPLAIN STRUCTURE OF PACKAGES

## Architectures
THERE ARE NOW LINKS TO THIS SECTION FROM Api._Blank and Console._Blank, maybe other places
* Solo application
* micro-service orchestratration
REFER TO THESE IN README CONFIG FOR VARIOUS SETTINGS (we need readme's for them)



## To rename a blank solution (Getting started... ???)
docker-compose
services:
  api.policyheaders.contenttype:
    image: api.policyheaders.contenttype
    hostname: api-policyheaders-contenttype
    build:
      context: ../Api.PolicyHeaders.ContentType

.GitHub build-and-deploy
env:
  APP_NAME: Api.PolicyHeaders.ContentType
  IMAGE_NAME: api.policyheaders.contenttype
  SERVICE_NAME: api-policyheaders-contenttype

rename csproj/sln files and rename all referenced projects
- Api.PolicyHeaders.XssProtection.sln
- Api.PolicyHeaders.XssProtection.csproj
- Api.PolicyHeaders.XssProtection.Models.csproj
- Tests.Api.PolicyHeaders.XssProtection.csproj

- change in /Api.PolicyHeaders.XssProtection/Properties/InternalsVisibleTo

Dockerfile / Dockerfile.local
rename Entry Point





MAKE SIMPLER. JUST TABLE FOR Api, Console and Web readme


* [Solution Composition](#application-composition)
  * [.docker](#-docker)
  * [.github](#-github)
  * [.kubernetes](#-kubernetes)
  * [.solution](#-solution)
  * [.tests](#-tests)
  * [.application](#-application)


## Solution Composition
LINKS TO HERE

All Nano applications follow a consistent and predictable solution structure.  

In local `Development`, the application is orchestrated using Docker Compose, while `Staging` and `Production` environments are deployed and managed using Kubernetes.  

Continuous Integration and Continuous Delivery (CI/CD) is handled by GitHub Actions and is included as part of the Nano Visual Studio solution.  

The solution root also contains several supporting files, such as Docker- and Git-related configuration files, along with various asset files used by 
the Git repository and NuGet packages.

In the following sections, each part of the solution is described in detail, with files listed alongside their purpose and responsibilities, 
covering both the physical file structure and the Visual Studio solution.  

## `.docker`
This folder contains the Docker Compose project used to orchestrate the application in the local `Development` environment.

> ⚠️ Rememmber to set the docker-compose project as startup project, before running the solution in Visual Studio.

| File / Directory                      | Type      | Description                                                                                                   |
| ------------------------------------- | --------- | ------------------------------------------------------------------------------------------------------------- |
|  `docker-compose`                     | `dcproj`  | The Docker Compose project used by Visual Studio for local orchestration.                                     |
|  `docker-compose/docker-compose.yml`  | `yaml`    | The Docker Compose specification for orchestrating the application locally in the `Development` environment.  |

## `.github`
This folder contains GitHub-specific configuration used primarily by GitHub Actions. It includes the `build-and-deploy.yml` workflow, 
which is responsible for building, testing, publishing artifacts, and deploying the application to Kubernetes in the `Staging` and `Production` environments.

The workflow relies on environment variables defined as GitHub variables and secrets, which must be created before the action can run successfully. All required 
variables and secrets are explicitly declared in the `env:` section of the workflow file—there is no need to configure CI/CD across multiple files.

The workflow performs the following steps:
1. Authenticate with Azure
2. Build, test, and package the solution
3. Apply database migrations
4. Publish container images and NuGet packages (optional)
5. Apply changes to Kubernetes
6. Create a GitHub release and send Slack notifications (optional)

| File / Directory                   | Type    | Description                                                                                        |
| ---------------------------------- | ------- | -------------------------------------------------------------------------------------------------- |
|  `config/slack.yml`                | `yaml`  | Configuration for posting build notifications to Slack _(optional)_.                               |
|  `workflows/build-and-deploy.yml`  | `yaml`  | GitHub Actions workflow that builds, tests, publishes artifacts, and deploys a Nano application.   |

## `.kubernetes`
This folder holds Kubernetes templates required for automated deployment of the application through GitHub Actions.  
These templates are used exclusively in the `Staging` and `Production` environments.

Different applications uses different Kubernetes templates depending on the type of application. Below is listed which application types used which templates

**For All applications**

| File / Directory     | Type    | Description                                                    |
| -------------------- | ------- | -------------------------------------------------------------- |
|  `configmap.yaml`    | `yaml`  | Configuration specification for Kubernetes.                    |

**Only API and Web applications**

| File / Directory     | Type    | Description                                                                                                                           |
| -------------------- | ------- | ------------------------------------------------------------------------------------------------------------------------------------- |
|  `autoscaler.yaml`   | `yaml`  | [Horizontal Pod Autoscaler (HPA)](https://kubernetes.io/docs/concepts/workloads/autoscaling/horizontal-pod-autoscale/) specification. |
|  `deployment.yaml`   | `yaml`  | Application [Deployment](https://kubernetes.io/docs/concepts/services-networking/ingress/) specification.                             |
|  `service.yaml`      | `yaml`  | Application [Service](https://kubernetes.io/docs/concepts/services-networking/service/) exposure specification.                       |
|  `ingress.yaml`      | `yaml`  | Application [Ingress](https://kubernetes.io/docs/concepts/services-networking/ingress/) specification _(Optional)_.                   |
|  `certificate.yaml`  | `yaml`  | Application TLS [Certificate](https://cert-manager.io/docs/usage/certificate/) specification _(Optional)_.                            |

**Only Console applications**

| File / Directory     | Type    | Description                                                    |
| -------------------- | ------- | -------------------------------------------------------------- |
|  `cronjob.yaml`      | `yaml`  | The scheduled job spec for Kubernetes.                         |

## `.solution`
The solution root directory contains several individual files used for repository management, Docker and Git configuration, and NuGet package assets.  
These files are part of the solution in Visual Studio, organized under the `.solution` folder.  

**Common for all applications**

| File / Directory  | Description                                                                                   |
| ----------------- | --------------------------------------------------------------------------------------------- |
| `.dockerignore`   | Lists files and folders to ignore when building Docker images.                                |
| `.gitignore`      | Lists files and folders to ignore in Git version control.                                     |
| `Dockerfile`      | Dockerfile used to build the container image for `Staging` and `Production` deployments.      |
| `README.md`       | Documentation asset for the Git repository and the application’s NuGet package _(Optional)_.  |

**For API and Web applications**

| File / Directory  | Description                                                                            |
| ----------------- | -------------------------------------------------------------------------------------- |
| `icon.png`        | Icon asset for the application’s NuGet package _(Optional)_.                           |
| `LICENSE`         | License file for the Git repository and the application’s NuGet package _(Optional)_.  |

## `.tests`
This folder contains a test project, which is empty by default and included to demonstrate the structure and where unit or integration tests should be added.  

| File / Directory         | Type      | Description                                                  |
| ------------------------ | --------- | ------------------------------------------------------------ |
| `Tests.{name}.csproj`    | `csproj`  | Empty Visual Studio test project, ready for adding tests.    |

## `.application`
The solution contains one or two projects depending on the application type:

- **Console applications:** only the application project exists.  
- **API and Web applications:** two projects exist — one for the application and one for models, which is designed to be packaged and published as a private NuGet during deployment.

The application projects contains the main application implementation, including the `program.cs` file.  
The project includes minimal configuration in `appsettings.json`, and empty override configurations for all three environments.

Other important files:
* `Properties/InternalsVisibleTo.cs` — exposes internal types to the test project.
* `Dockerfile.Local` — used by Docker Compose in `Development` environment; must remain in the application project folder.
* `launchSettings.json` — included but empty.

The second project contains models that are packaged and published as a private NuGet, enabling internal sharing between services. This separation keeps 
the application lightweight while allowing shared **models** to be versioned and distributed consistently across services.

> ⚠️ The models project must reference the [Nano.App](https://www.nuget.org/packages/Nano.App) NuGet package.  
> This provides access to the API client implementation and allows the models project to expose models along with the derived API client functionality.
