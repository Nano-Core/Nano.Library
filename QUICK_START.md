# Quick Start Guide

> _Quick start guide for Nano._

***

## Table of Contents
* [1]()
* [2. Choosing Applcation Type](#choosing-applcation-type)
  * [2.1 API Application](#api-application) 
  * [2.2 Console Application](#console-application) 
  * [2.3 Web Application](#web-application) _(experimental)_
* [3. Adding Providers](#adding-providers)
  * [3.1 Logging](#logging)
  * [3.2 Data](#data)
  * [3.3 Eventing](#eventing)
  * [3.4 Storage](#storage)
  
## Choosing Applcation Type
Depending on the application you are building, choose the application type. Nano supports three types of applications


## API Application
First copy Api._Blank. 
Rename to the name of your liking all over the solution, or leave the name for now. See here how to [Rename Nano API Solution](#rename-api-solution).  
You know have a base Nano solution that can run locally and also be deployed using GitHub Actions to Kubernetes. SHOULD WE SHOW ## Solution Composition here??? or refrence it? 




## Console Application
First copy Console._Blank. 
Rename to the name of your liking all over the solution, or leave the name for now. See here how to [Rename Nano Console Solution](#rename-api-solution).  


## Web Application
First copy Web._Blank. 
Rename to the name of your liking all over the solution, or leave the name for now. See here how to [Rename Nano Web Solution](#rename-api-solution).  


## Adding Providers

## Logging
Show the same as the logging example, and also link to it

## Data
Show the same as the data example, and also link to it

## Eventing
Show the same as the eventing example, and also link to it

## Storage
Show the same as the storage example, and also link to it




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
