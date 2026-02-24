# Nano.Eventing.RabbitMq
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Eventing.RabbitMq.svg)](https://www.nuget.org/packages/Nano.Eventing.RabbitMq/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Eventing.RabbitMq.svg)](https://www.nuget.org/packages/Nano.Eventing.RabbitMq/)

> _RabbitMq eventing for Nano applications._

*** 

## Table of Contents
* [Home](https://github.com/Nano-Core/Nano.Library#nano-library)
* [Summary](#summary)
* [Registration](#registration)
* [Dependencies](#dependencies)

## Summary
Eventing Provider implementation for RabbitMq.  

> 📖 Learn more about **[Nano Eventing](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Eventing)**.  
> 📖 Learn more about **[Nano Kubernetes RabbitMq](https://github.com/Nano-Core/Nano.Kubernetes/tree/master/Nano.Kubernetes.RabbitMq)**.  

Try it out yourself using the **[Api.Eventing.RabbitMq](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Eventing.RabbitMq)** or 
**[Console.Eventing.RabbitMq](https://github.com/Nano-Core/Nano.Lessons/tree/master/Console.Eventing.RabbitMq)** example.  

## Registration
Install the **[Nano.Eventing.RabbitMq](https://www.nuget.org/packages/Nano.Eventing.RabbitMq)** NuGet package.  

```powershell
dotnet add package Nano.Eventing.RabbitMq;
```

Register the `RabbitMqProvider` provider during application startup in the `ConfigureServices(...)` method.

```csharp
...
.ConfigureServices(services =>
{
    services
        .AddNanoEventing<RabbitMqProvider>();
})
...
```
