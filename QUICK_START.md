# Getting Started

> _Getting started with Nano._

***

## Table of Contents
* [Choosing Applcation Type](#choosing-applcation-type)
* [Adding Providers](#adding-providers)
  * [Logging](#logging)
  * [Data](#data)
  * [Eventing](#eventing)
  * [Storage](#storage)

## Choosing Applcation Type

## Adding Providers

## Logging

## Data

## Eventing

## Storage




To rename a blank solution:
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
