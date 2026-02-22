# Nano.Storage.Abstractions
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.Storage.Abstractions.svg)](https://www.nuget.org/packages/Nano.Storage.Abstractions/)
[![NuGet](https://img.shields.io/nuget/v/Nano.Storage.Abstractions.svg)](https://www.nuget.org/packages/Nano.Storage.Abstractions/)

> _Pluggable, provider-agnostic file storage for Nano applications._

> ⚠️ This NuGet is included in other Nano Packages, and is not meant to be included directly.

***

## Table of Contents
* [Home](https://github.com/Nano-Core/Nano.Library#nano-library)
* [Summary](#summary)

## Summary
Abstractions contains the interfaces used when implementing storage providers, as well as the ```IPathProvider``` that maps common shared drives.  

This should not be included indepentely and is already included in [Nano.Storage](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Storage), 
which you should use when implementing custom storage providers.

Read more here: [Nano.Storage](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Storage)

***