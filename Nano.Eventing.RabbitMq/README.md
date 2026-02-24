# Nano.Eventing.RabbitMq
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Eventing.RabbitMq.svg)](https://www.nuget.org/packages/Nano.Eventing.RabbitMq/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Eventing.RabbitMq.svg)](https://www.nuget.org/packages/Nano.Eventing.RabbitMq/)

> _RabbitMQ message queueing (pubSub) for Nano applications._

*** 

## Table of Contents
* [Home](https://github.com/Nano-Core/Nano.Library#nano-library)
* [Summary](#summary)
* [Registration](#registration)
* [Dependencies](#dependencies)

## Summary
Eventing Provider implementation for RabbitMQ.  
Read more about storage here: [Nano.Eventing](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Eventing)

## Registration
First install the [Nano.Eventing.RabbitMq](https://www.nuget.org/packages/Nano.Eventing.RabbitMq) NuGet package.  

```powershell
dotnet add package Nano.Eventing.RabbitMq;
```

The RabbitMQ eventing provider must be registered as dependencies.  
```csharp
    .ConfigureServices(services =>
    {
        services
            .AddNanoEventing<RabbitMqProvider>();
    })
```

## Dependencies
* https://github.com/Nano-Core/Nano.Kubernetes/tree/master/Nano.Kubernetes.RabbitMq

***