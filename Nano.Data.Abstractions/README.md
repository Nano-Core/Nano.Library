# Nano.Data.Abstractions
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Data.Abstractions.svg)](https://www.nuget.org/packages/Nano.Data.Abstractions/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Data.Abstractions.svg)](https://www.nuget.org/packages/Nano.Data.Abstractions/)

> _Pluggable, provider-agnostic data access for Nano applications._

> ⚠️ This NuGet is included in other Nano Packages, and is not meant to be included directly.

***

## Table of Contents
* [Home](https://github.com/Nano-Core/Nano.Library#nano-library)
* [Summary](#summary)

THIS NUGET SHOULD NOT BE INSTALLED DIRECTLY SEE Providers

## Summary
Abstractions contains the interfaces used when implementing data providers, as well as the ```DbContext``` for entity framework.  

This should not be included indepentely and is already included in [Nano.Data](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data), 
which you should use when implementing custom storage providers.

Read more here: [Nano.Data](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data)

***