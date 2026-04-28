# Nano.Eventing.RabbitMq
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Eventing.RabbitMq.svg)](https://www.nuget.org/packages/Nano.Eventing.RabbitMq/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Eventing.RabbitMq.svg)](https://www.nuget.org/packages/Nano.Eventing.RabbitMq/)

> _RabbitMq eventing for Nano applications._

*** 

## Table of Contents
* **[Home](https://github.com/Nano-Core/Nano.Library/blob/master/README.md#nanolibrary)**
* **[Summary](#summary)**
* **[Registration](#registration)**
* **[Configuration](#configuration)**
* **[Docker Compose](#docker-compose)**
* **[Kuberentes](#kuberentes)**

## Summary
Eventing Provider implementation for RabbitMq.  

> 📖 Learn more about **[Nano Eventing](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Eventing/README.md#nanoeventing)**.  
> 📖 Learn more about **[Nano Kubernetes RabbitMq](https://github.com/Nano-Core/Nano.Kubernetes/tree/master/Nano.Kubernetes.RabbitMq)**.  

Try it out yourself using the **[Api.Eventing.RabbitMq](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Eventing.RabbitMq)** or 
**[Console.Eventing.RabbitMq](https://github.com/Nano-Core/Nano.Lessons/tree/master/Console.Eventing.RabbitMq)** example.  

## Registration
Install the **[Nano.Eventing.RabbitMq](https://www.nuget.org/packages/Nano.Eventing.RabbitMq/README.md#nanoeventingrabbitmq)** NuGet package.  

```powershell
dotnet add package Nano.Eventing.RabbitMq;
```

Continue with registering the `RabbitMqProvider` provider during application startup in the `ConfigureServices(...)` method.

```csharp
...
.ConfigureServices(services =>
{
    services
        .AddNanoEventing<RabbitMqProvider>();
})
...
```

## Configuration
Add the eventing configuration.  

```json
"Eventing": {
  "Host": null,
  "VHost": null,
  "Port": 5672,
  "Timeout": 30,
  "UseSsl": false,
  "Heartbeat": 60,
  "PrefetchCount": 50,
  "Credentials": {
    "Id": null,
    "Secret": null
  }
  "HealthCheck": {
    "UnhealthyStatus": "Unhealthy"
  }
}
```

## Docker Compose
In addition to registering and configuring storage, you also need to configure your docker-compose setup. The rabbitmq image is the manangement image, 
so you are able to access the interface here: **[http://localhost:15672](http://localhost:15672)** and monitor and observe event messages being published and consumed.  

```yaml
services:
  {service-name}:
    depends_on:
      - eventing

  eventing: 
    image: rabbitmq:management
    hostname: rabbitmq
    ports:
      - 5671:5671
      - 5672:5672
      - 15671:15671
      - 15672:15672
    networks:
      - network
    environment: 
      RABBITMQ_DEFAULT_USER: rabbitmq_user
      RABBITMQ_DEFAULT_PASS: password
      RABBITMQ_DEFAULT_VHOST: "/"

```

## Kubernetes
Next, add the following to your Kubernetes `deployment.yaml` or `cronjob.yaml` (depending on application type) for the Nano application.

```yaml
spec:
  template:
    spec:
      containers:
        env:
        - name: Eventing__Credentials__Secret
          valueFrom:
            secretKeyRef:
              name: rabbitmq
              key: rabbitmq-password
```

You also need to map the `rabbitmq` secret that is created alongside the **[Nano Azure Kuberenetes Eventing](https://github.com/Nano-Core/Nano.Azure.Kubernetes/tree/master/Nano.Azure.Kubernetes.RabbitMQ)** 
in the `deployment.yaml` or `cronjob.yaml` for eventing authentication.

> ⚠️ The `rabbitmq` secret will be reused for all applications using RabbitMQ as eventing provider.  
That makes it easy to change the secret values later on when needed, and just requires the secret values to be updated and re-deployed.  
