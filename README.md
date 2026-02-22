# Nano.Library
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/NanoCore.svg)](https://www.nuget.org/packages/NanoCore/)
[![NuGet](https://img.shields.io/nuget/v/NanoCore.svg)](https://www.nuget.org/packages/NanoCore/)

> _Nano._

## Table of Contents
* [Summary](#summary)
* [NuGet Packages](#nuget-packages)
* [Applications](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App)
  * [Api](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Api)
  * [Console](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Console)
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

## NuGet Packages
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





* [Solution Composition](#application-composition)
  * [.docker](#-docker)
  * [.gitHub](#-gitHub)
  * [.kubernetes](#-kubernetes)
  * [.solution](#-solution)
  * [.tests](#-tests)
  * [.application](#-application)


## Solution Composition
THIS IS DIFFERENT FOR CONSOLE APPLICATIONS. WHERE TO PLACE THIS??? MAYBE MOVE TO Nano.App.Api and Nano.App.Console

All Nano applications follow the same structure.  
Locally in `Development` environment the application is orchestratd using docker-compose, and for `Staging` and `Production` it's Kubernetes.
Continius integration and delivery (CI/CD) is handled by GitHub Actions, and it's included in a Nano Visual Studio solution. 
Various other file are included in the solution root directory, such as docker- and gitignore and various asset files for git-repository and NuGet packages.

Below is a table resemblong both the physical files and directories, as well as the Visual Studio solution, excepot items in the root directory are added to
the `.solution ` folder in Visual Studio.

## `.docker`
This folder contains the `docker-compose` project, used for orchestration with the `Development` environment.  

> ⚠️ Rememmber to set the docker-compose project as startup project for you solution, before running it in Visual Studio.

| File / Directory                      | Type      | Description                                                                                      |
| ------------------------------------- | --------- | ------------------------------------------------------------------------------------------------ |
|  `docker-compose`                     | `dcproj`  | The docker-compose project.                                                                      |
|  `docker-compose/docker-compose.yml`  | `yaml`    | The docker-compose spec for orchestrating the application locally in `Development` environment.  |

## `.gitHub`
This folder is the GitHub folder, used by GitHub Actions among other. It contains the `build-and-deploy.yml` workflow file, that is used for building, testing, 
publishing artifacts and deploying to Kubernetes to either `Staging` and `Production` environment.

The file is too large to display here, but you can find it here: [build-and-deploy.yml](https://github.com/Nano-Core/Nano.Lessons/blob/master/Api._Blank/.github/workflows/build-and-deploy.yml), and check it out yourself. 
It requires the environment variables to exists in github var and secrets, so they must be created before the GitHub Action can succeeed. But all variables and secrets
required for the build is specified in the `env:` section of the GitHub action file. No need to fiddle around in multiple files to configure CI/CD. 

The GitHub action basically does:
1. Login to Azure.
2. Build, Test and Packages the solution.
3. Apply database migrations.
4. Publish container image and NuGet package (optional)
5. Apply changes to Kubernetes. 
6. Post GitHub Release and Slack notification (optional).

| File / Directory                   | Type    | Description                                                                            |
| ---------------------------------- | ------- | -------------------------------------------------------------------------------------- |
|  `config/slack.yml`                | `yaml`  | Spec for posting builds to _Slack_ (Optional).                                         |
|  `workflows/build-and-deploy.yml`  | `yaml`  | GitHub Action that builds, test, publushes artifacts and deploys a Nano application.   |

## `.kubernetes`
This folder contains Kubenetes templates required to deploy the solution in CI/CD. 
Used only in `Staging` and `Production` environment.
Minimal set of Kubernetes spec files for a Nano application. Some applications will have additional spec file, such as `ingress.yaml` and `certificate.yaml` when 
enabling https exposure for an application. 


HERE

For Service (API) applications

| File / Directory     | Type    | Application | Description                                                  |
| -------------------- | ------- | ------- | ------------------------------------------------------------ |
|  `autoscaler.yaml`   | `yaml`  | The horizontal pod autoscaler (hpa) spec for Kubernetes.     |
|  `configmap.yaml`    | `yaml`  | The configuration spec for Kuberentes.                       |
|  `deployment.yaml`   | `yaml`  | The application deployment spec for Kubernetes.              |
|  `service.yaml`      | `yaml`  | The application service exposure spec for Kubernetes.        |

|  `ingress.yaml`      | `yaml`  | The application service exposure spec for Kubernetes.        |
|  `certificate.yaml`  | `yaml`  | The application service exposure spec for Kubernetes.        |

|  `cronjob.yaml`      | `yaml`  | The scheduled job spec for Kubernetes.                       |




For Console applications




## `.solution`
The solution directory (root) contains several individual files, such as content for NuGet packages, ignore files for docker and github, etc.

| File / Directory  | Type    | Description                                                                                                                                                                      |
| ----------------- | ------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
|  `.dockerignore`  |         | The files ignored by `Docker`.                                                                                                                                                   |
|  `.gitignore`     |         | The files ignored by git.                                                                                                                                                        |
|  `Dockerfile`     |         | The Dockerfile used to build the container image when deploying to `Staging` and `Production`                                                                                    |
|  `README.md`      |         | Asset file for the `git-repository` and the application models NuGet package (Optional). _* if deleted, remember to remove the reference from the application `csproj` file._   |


NOT CONSOLE
|  `icon.png`       | `image` | Asset file for application models NuGet package (Optional). _* if deleted, remember to remove the reference from the application `csproj` file._                                  |
|  `LICENSE`        |         | Asset file for the `git-repository` and the application models NuGet package (Optional). _* if deleted, remember to remove the reference from the application `csproj` file._  |

## `.tests`
The is a test project, which is empty and just included to show the structure and where the tests should be added.

| File / Directory         | Type      | Description                                                  |
| ------------------------ | --------- | ------------------------------------------------------------ |
|  `Tests.{name}.csproj`   | `csproj`  | Empty Visual Studio Test project. Ready for adding tests.    |

## `.application`
The solution contains two project, first the `Api.Blank`, containg the application.
This application has only minimal configuration in `appsettings.json`, with http, as shown below. `appsettings.json` overrides for the default environments has also
been added, for `appsettings.Development.json`, `appsettings.Staging.json` and `appsettings.Production.json`. Overrides are empty by default.

```json 
"App": {
  "Version": "1.0.0.0",
  "Hosting": {
    "Root": "api",
    "Http": {
      "Ports": [
        8080
      ]
    }
  }
}
```

The project also has a `Properties/InternalsVisibleTo.cs`, which just sets interval types visible for the test project.
The `DockerfileLocal` is used by the `docker-compose`, but is required to be located in the application project folder. Don't move it!
The solution also contains an empty `launchSettings.json`

Last the `program.cs` file with the Nano application setup; 

Next, the `Api.Blank.Models` that include models that should be published and shared on 
private NuGet. For now and for this lesson there the project is empty, as well as the NuGet that is published during CI/CD. 
Nano references needs to be added to this project, as models depends on Nano.App to be used in the api-client

Models project is configured for packaging a NuGet, which should be published internally and used to connect api to services. Also why the Nano dependences must be added here.


