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


