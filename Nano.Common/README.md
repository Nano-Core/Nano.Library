# Nano.Common
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Common.svg)](https://www.nuget.org/packages/Nano.Common/)

> _Pluggable, provider-agnostic logging for Nano applications._

***

## Table of Contents
* [Summary](#summary)
* [Custom Configuration](#configuration)
* [Special Annotations](#special-annotations)

## Summary
General implementations used many places in the Nano solution.

## Custom Configuration
Nano allows registering custom configuration sectuions.

```csharp
public class MyOptions
{
	public bool IsEnabled { get; set; }
}

services
	.AddNanoConfigSection<MyOptions>("section-name", out var options);
```

## Special Annotations
* InternationalPhoneAttribute
* RequiredOneOfAttribute
* UrlAttribute

***
