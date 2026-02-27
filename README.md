# Nano.Library
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/NanoCore.svg)](https://www.nuget.org/packages/NanoCore/)
[![NuGet](https://img.shields.io/nuget/v/NanoCore.svg)](https://www.nuget.org/packages/NanoCore/)

> _Nano - Your foundation for modern .NET._  

***


Where to put:
### Commands
* ```kubectl patch cronjob {{cronjob-name}} -p '{"spec": {"suspend": true}}'```  
* ```kubectl create job --from=cronjob/{{cronjob-name}} {{job-name}}```




## Table of Contents
* [Summary](#summary)
* [NuGet Packages](#nuget-packages)
* [Solution Composition](#solution-composition)
* [Nano Architectures](#nano-architectures)
* [Applications](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App)
  * [Api](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Api)
  * [Console](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Console)
  * [Web](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Web)
* [Providers](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App)
  * [Logging](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging)
  * [Data](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data)
  * [Eventing](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Eventing)
  * [Storage](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Storage)

## Summary
Nano is a library for rapidly building .NET applications.  

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

Nano gives you everything you tecnically to start building micro-services 
* Provides configurable implementations for non-business related aspects of an application, 
* Combining the best practices, patterns, extensions and conventions of modern development. 
* Inject providers and register custom dependencies.
* Override, extend or disable any part of Nano, not suitable for the application. 
* Derive concrete implementations, enriched with functionality, from base abstractions and interfaces, for all parts of the application.  
* Avoid wasting valuable development resources, on matters not related to the business.
* Remain focused on modelling data and operations of the business domain.

Also check out our **[Nano.Lessons](https://github.com/Nano-Core/Nano.Lessons)**. 

> If you are new to Nano, it's recommended to follow your [Getting Started Guide](https://github.com/Nano-Core/Nano.Library/tree/master/GETTING_STARTED.md)

## NuGet Packages
These packages are all-inclusive packages, where all other Nano packages are included. Easy to get started, but it's recommended to instead select only the
packages needed by your application, to avoid unnecessary depdendencies.  

| Name               | Description                                             | NuGet Pacakge                                                                                                                                                                                                                                                                            |
| ------------------ | ------------------------------------------------------- | ------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| [Nano.All](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.All)   | Includes all other Nano.* NuGet packages.                                       | [![NuGet](https://img.shields.io/nuget/dt/Nano.All.svg)](https://www.nuget.org/packages/Nano.All/) [![NuGet](https://img.shields.io/nuget/v/Nano.All.svg)](https://www.nuget.org/packages/Nano.All/)   |
| [NanoCore](https://github.com/Nano-Core/Nano.Library/tree/master/NanoCore)   | Includes Nano.All. _This is the older Nano package name, but still supported._  | [![NuGet](https://img.shields.io/nuget/dt/NanoCore.svg)](https://www.nuget.org/packages/NanoCore/) [![NuGet](https://img.shields.io/nuget/v/NanoCore.svg)](https://www.nuget.org/packages/NanoCore/)   |


The next packages are application packages. Include only one matching the application type you want to create.  

| Package                      | Type           | Downloads                                                                                                                                                                                                         |
| ---------------------------- | -------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Nano.App                     | _`Transitive`_ | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.App.Api                 | `Application`  | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.App.Console             | `Application`  | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.App.Web                 | `Application`  | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |

Last, provider packages. These enable functionality related to a Nano provider.  

| Package                      | Type          | Downloads                                                                                                |                                                                                                         |
| ---------------------------- | ------------- | -------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------- |
| Nano.Data                    | _`Transitive`_    | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Data.Abstractions       | _`Transitive`_    | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Data.InMemory           | `Data`        | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Data.MySql              | `Data`        | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Data.PostgreSQL         | `Data`        | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Data.SqLite             | `Data`        | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Data.SqlServer          | `Data`        | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Eventing                | _`Transitive`_    | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Eventing.Abstractions   | _`Transitive`_    | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Eventing.RabbitMq       | `Eventing`    | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Logging                 | _`Transitive`_    | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Logging.Abstractions    | _`Transitive`_    | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Logging.Log4Net         | `Logging`     | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Logging.Microsoft       | `Logging`     | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Logging.NLog            | `Logging`     | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Logging.Serilog         | `Logging`     | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Storage                 | _`Transitive`_    | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Storage.Abstractions    | _`Transitive`_    | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Storage.Azure           | `Storage`     | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |
| Nano.Storage.Local           | `Storage`     | [![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) | [![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/) |

## Solution Composition
All Nano applications follow a consistent and predictable solution structure.  

In local `Development`, the application is orchestrated using Docker Compose, while `Staging` and `Production` environments are deployed and managed using Kubernetes.  

Continuous Integration and Continuous Delivery (CI/CD) is handled by GitHub Actions and is included as part of the Nano Visual Studio solution.  

The solution root also contains several supporting files, such as Docker- and Git-related configuration files, along with various asset files used by 
the Git repository and NuGet packages.

In the following sections, each part of the solution is described in detail, with files listed alongside their purpose and responsibilities, 
covering both the physical file structure and the Visual Studio solution.  

### `.docker`
This folder contains the Docker Compose project used to orchestrate the application in the local `Development` environment.

> ⚠️ Rememmber to set the docker-compose project as startup project, before running the solution in Visual Studio.

| File                               | API | WEB | CONSOLE | Description                                                                                                   |
| -----------------------------------| --- | --- | ------- | ------------------------------------------------------------------------------------------------------------- |
| `docker-compose.dcproj`            | ✓   | ✓   | ✓       | The Docker Compose project used by Visual Studio for local orchestration.                                     |
| `docker-compose.yml`               | ✓   | ✓   | ✓       | The Docker Compose specification for orchestrating the application locally in the `Development` environment.  |


















#### `.github`

| Directory | File                               | API | WEB | CONSOLE | Description                                                                       |
|| ---------------------------------------------- | --- | --- | ------- | --------------------------------------------------------------------------------- |
| `.github` | `config/slack.yml`                     | ✓   | ✓   | ✓       | Configuration for posting build notifications to Slack _(optional)_.   |
|| `workflows/build-and-deploy.yml`       | ✓   | ✓   | ✓       | GitHub Actions workflow that builds, tests, publishes artifacts, and deploys a Nano application. The workflow performs the following steps: 1. Authenticate with Azure. 2. Build, test, and package the solution. 3. Apply database migrations. 4. Publish container images and NuGet packages (optional). 5. Apply changes to Kubernetes. 6. Create a GitHub release and send Slack notifications (optional)  |

#### `.kubernetes`

| Directory | File                               | API | WEB | CONSOLE | Description                                                                       |
|| ---------------------------------------------- | --- | --- | ------- | --------------------------------------------------------------------------------- |
| `.kubernetes` | `configmap.yaml`                   | ✓   | ✓   | ✓       | Configuration specification for Kubernetes.   |
|| `autoscaler.yaml`                  | ✓   | ✓   | ✗      | [Horizontal Pod Autoscaler (HPA)](https://kubernetes.io/docs/concepts/workloads/autoscaling/horizontal-pod-autoscale/) specification.  |
|| `deployment.yaml`                  | ✓   | ✓   | ✗      | Application [Deployment](https://kubernetes.io/docs/concepts/services-networking/ingress/) specification.    |
|| `service.yaml`                     | ✓   | ✓   | ✗      | Application [Service](https://kubernetes.io/docs/concepts/services-networking/service/) exposure specification. |     
|| `ingress.yaml`                     | ✓   | ✓   | ✗      | Application [Ingress](https://kubernetes.io/docs/concepts/services-networking/ingress/) specification _(Optional)_.   |
|| `certificate.yaml`                 | ✓   | ✓   | ✗      | Application TLS [Certificate](https://cert-manager.io/docs/usage/certificate/) specification _(Optional)_. |
|| `cronjob.yaml`                     | ✗  | ✗   | ✓      | The scheduled job specification.  |

#### `application`

| Directory | File                               | API | WEB | CONSOLE | Description                                                                       |
|| ---------------------------------------------- | --- | --- | ------- | --------------------------------------------------------------------------------- |
| `.tests` | `Tests.{name}.csproj`                   | ✓   | ✓   | ✓       | Test project, which is empty by default and included to demonstrate the structure and where unit or integration tests should be added. |
|| `Properties/DoNotParallelize.cs`        | ✓   | ✓   | ✓       | Ensures tests are not Parallelized. |

#### `.tests`

| Directory | File                               | API | WEB | CONSOLE | Description                                                                       |
|| ---------------------------------------------- | --- | --- | ------- | --------------------------------------------------------------------------------- |
| `{name}` | `Properties/InternalsVisibleTo.cs`      | ✓   | ✓   | ✓       | Exposes internal types to the test project.    |
|| `Properties/launchSettings.json`        | ✓   | ✓   | ✓       | Included but empty.    |
|| `wwwroot/`                              | ✓   | ✓   | ✗      | Root folder for static and dnyamic web content.   |
|| `appsettings.json`                      | ✓   | ✓   | ✓       | Application configuration file. Overrides also included for environments: `Development`, `Staging` and `Production`.    |
|| `Dockerfile.Local`                      | ✓   | ✓   | ✓       | Used by Docker Compose in `Development` environment; must remain in the application project folder. |
|| `Program.cs`                            | ✓   | ✓   | ✓       | The main entry point to the Nano application.   |

#### `Models`

| Directory | File                               | API | WEB | CONSOLE | Description                                                                       |
|| ---------------------------------------------- | --- | --- | ------- | --------------------------------------------------------------------------------- |
| `{name}.Models` | `{name}.Models.csproj`           | ✓   | ✓   | ✗      | Designed to be packaged and published as a private NuGet during deployment |

#### `/`

| Directory | File                               | API | WEB | CONSOLE | Description                                                                       |
|| ---------------------------------------------- | --- | --- | ------- | --------------------------------------------------------------------------------- |
| `/` | `.dockerignore`                                | ✓   | ✓   | ✓       ||
|| `.gitignore`                                   | ✓   | ✓   | ✓       ||
|| `Dockerfile`                                   | ✓   | ✓   | ✓       ||
|| `README.md`                                    | ✓   | ✓   | ✓       ||
|| `icon.png`                                     | ✓   | ✓   | ✗      ||
|| `LICENSE`                                      | ✓   | ✓   | ✗      ||
|| `{name}.sln`                                   | ✓   | ✓   | ✓       ||
 


 * [Solution Composition](#application-composition)
  * [.docker](#-docker)
  * [.github](#-github)
  * [.kubernetes](#-kubernetes)
  * [.solution](#-solution)
  * [.tests](#-tests)
  * [.application](#-application)

## Solution Composition
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
|  `docker-compose.dcproj`              | `dcproj`  | The Docker Compose project used by Visual Studio for local orchestration.                                     |
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











## Nano Architectures
* Solo application
* micro-service orchestratration
