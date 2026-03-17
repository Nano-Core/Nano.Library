# Nano.App.Api
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.App.Api.svg)](https://www.nuget.org/packages/Nano.App.Api/)
[![NuGet](https://img.shields.io/nuget/v/Nano.App.Api.svg)](https://www.nuget.org/packages/Nano.App.Api/)

> _Nano API application._

***

## Table of Contents
* [Home](https://github.com/Nano-Core/Nano.Library#nano-library)
* [Summary](#summary)
* [Registration](#registration)
* [Configuration](#configuration)
  * [Hosting](#hosting)
    * [Http](#http)
    * [Https](#https)
    * [Routing](#routing)
    * [Multipart Limits](#multipart-limits)
  * [Http Policy Headers](#http-policy-headers)
    * [Content Type Options](#content-type-options) 
    * [Referrer Policy](#referrer-policy) 
    * [Frame Options](#frame-options)
    * [Xss Protection](#xss-protection)
    * [Content Security Policy (CSP))](#content-security-policy-csp)
    * [Cors](#cors)
    * [Strict Transport Security (HSTS)](#strict-transport-security-hsts)
    * [Robots](#robots)
    * [Forwarded Headers](#forwarded-headers)
  * [Response Cache](#response-cache)
  * [Response Compression](#response-cache)
  * [Session](#session)
  * [Cookies](#cookies)
  * [TimeZone](#timezone)
  * [Localization](#localization)
  * [Versioning](#versioning)
  * [Documentation](#documentation)
  * [Health Checks](#health-checks)
  * [Virus Scan](#virus-scan)
  * [Content Negotiation](#content-negotiation)
  * [Request Tracing](#request-tracing)
  * [Error Handling](#error-handling)
  * [Static Files](#static-files)
  * [Authentication](#authentication)
  * [Authorization](#authorization)
  * [Api Clients](#api-clients)
* [Controllers](#controllers)
  * [Request Validation](#request-validation)
  * [Request Multipart JSON](#request-multipart-json)
  * [Response Serialization](#response-serialization)
* [Startup Tasks](#startup-tasks)

## Summary
The `NanoApiApplication` is a ready-to-use application template for building APIs with Nano.  

It derives from `BaseNanoApplication` and implements the `IApplication` interface, following the common Nano application patterns and providing a concrete implementation 
for building web API applications with Nano. This class includes a structured setup for configuring services, middleware, routing, etc. 
It also provides convenient static methods to create and configure the application with sensible defaults, while allowing full customization of services 
through the `ConfigureServices` method. This design ensures that all core API behaviors are initialized consistently using you configuration, reducing boilerplate code 
and simplifying the setup of new API applications.  

> ⚠️ Before proceeding, it is highly recommended to familiarize yourself generally with **[Nano Applications](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App)**.  

The `NanoApiApplication` can operate as either an internal service or an externally accessible API.
As an internal service, it can run behind your network boundary, handling requests from other applications within the system, 
using the built-in **[Nano Api Client](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App#api-client)**.
When exposed as an external API, it sits behind an entry point that manages incoming traffic, providing controlled access to clients while keeping 
the internal implementation consistent. This design allows the same application to function in both roles without changing its core configuration or service logic, 
supporting flexible deployment scenarios.  

> 📖 Learn more about the overall Nano architecture here: **[Nano Architectures](https://github.com/Nano-Core/Nano.Library#nano-architectures)**.  

Also checkout the **[Api.Blank](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api._Blank)** example, that shows a minimal configured API application.  

## Registration
First install the [Nano.App.Api](https://www.nuget.org/packages/Nano.App.Api) NuGet package.  

```powershell
dotnet add package Nano.App.Api;
```

Then, to create a `NanoApiApplication` simply add the following code to `program.cs`.  

```csharp
NanoApiApplication
    .ConfigureApp()
    .ConfigureServices(x =>
    {
        // Your services...
    })
    .Build()
    .Run();
```

Register your custom services in the `ConfigureServices(x => { })` method to extend Nano with additional functionality or integrations.  

## Configuration
The `App` section in the configuration defines behavior related to the application.  

| Setting                    | Type       | Default    | Description                                                                                                                                                          |
| -------------------------- | ---------- | ---------- | ----------------------------------------------------------------------------------------------------------- |
|  `Version`                 | string     | 1.0.0.0    | Application version identifier.                                                                             |
|  `ShutdownTimeout`         | int        | 10         | Number of seconds to wait after a SIGTERM signal before shutting down.                                      |
|  `Hosting`                 | object     | default    | Hosting options. See **[Hosting](#hosting)**.                                                               |
|  `HttpPolicyHeaders`       | object     | default    | HTTP policy header options. See **[Http Policy Headers](#http-policy-headers)**.                            |
|  `ResponseCache`           | object     | null       | Response caching options. See **[Response Cache](#response-cache)**.                                        |
|  `ResponseCompression`     | object     | null       | Response compression options. See **[Response Compression](#response-compression)**.                        |
|  `Session`                 | object     | null       | Session management options. See **[Session](#session)**.                                                    |
|  `TimeZone`                | object     | null       | Timezone configuration options. See **[TimeZone](#timezone)**.                                              |
|  `Localization`            | object     | null       | Localization configuration options. See **[Localization](#localization)**.                                  |
|  `Documentation`           | object     | null       | API documentation options (Swagger). See **[Documentation](#documentation)**.                               |
|  `HealthCheck`             | object     | null       | Health-check configuration options. See **[health Check](#health-check)**.                                  |
|  `VirusScan`               | object     | null       | Virus scanning options. See **[Virus Scan](#virus-scan)**.                                                  |
|  `ErrorHandling`           | object     | default    | Error handling configuration options. See **[Error Handling](#error-handling)**.                            |
|  `Identity`                | object     | null       | Identity configuration options. See **[Identity](#identity)**.                                              |
|  `Apis`                    | dictionary | []         | Named Nano API client configurations available to the application. See **[Api Client](api-clients)**.       |

```json
"App": {
  "Version": "1.0.0.0",
  "ShutdownTimeout": 10,
  "Hosting": { },
  "HttpPolicyHeaders": { },
  "ResponseCache": null,
  "ResponseCompression": null,
  "Session": null,
  "TimeZone": null
  "Localization": null
  "Documentation": null,
  "HealthCheck": null,
  "VirusScan": null,
  "ErrorHandling": { }
  "Identity": null,
  "Apis": []
}
```

## Hosting
Hosting configuration specifies how the API is hosted on the Kestrel web server, defining endpoint exposure as well as request handling limits.  

| Setting                  | Type    | Default  | Description                                                                                       |
| ------------------------ | ------- | -------- | ------------------------------------------------------------------------------------------------- |
|  `Root`                  | string  | api      | Root route for the application endpoints.                                                         |
|  `Http`                  | object  | default  | Options for HTTP. See **[Http](#http)**.                                                          |
|  `Https`                 | object  | null     | Options for HTTPS. See **[Https](#https)**.                                                       |
|  `MultipartLimits`       | object  | null     | Multipart upload limits. See **[MultiPart Limits](#multipart-limits)**.                           |

```json
"App": {
  "Hosting": {
    "Root": "api",
    "Http": { }
    "Https": null
    "MultipartLimits": null
  }
}
```

## Http
Configure the HTTP ports for the API, as well as additional settings related to HTTP behavior.  
If **[Https](#https)** is also enabled, consider turning on `UseHttpsRedirection` to automatically redirect HTTP requests to HTTPS.  

> ⚠️ You should at least specify one HTTP or HTTPS port. 

> ⚠️ Avoid using the default HTTP port (80), as it may trigger security warnings in Kubernetes.

| Setting                 | Type    | Default  | Description                               |
| ----------------------- | ------- | -------- | ----------------------------------------- |
|  `Ports`                | array   | []       | List of ports for HTTP.                   |
|  `UseHttpsRedirection`  | string  | false    | Enforce HTTPS redirect for all requests.  |

```json
"App": {
  "Hosting": {
    "Http": {
      "Ports": [
      ],
      "UseHttpsRedirection": false
    }
  }
}

```

Try it out yourself using the **[Api.Hosting.Http](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Hosting.Http)** example.  

## Https
Configuring HTTPS allows the API to communicate over a secure SSL/TLS connection.  
To use HTTPS, you must specify at least one port along with a certificate path and password to establish a valid security protocol. If you want the 
application to accept only secure connections, enable `UseHttpsRequired`.  

HTTPS is primarily intended for local development. In production environments, secure connections and certificates are typically handled at the network ingress level.  

> ⚠️ You should at least specify one HTTP or HTTPS port. 

> ⚠️ Avoid using the default HTTP port (443), as it may trigger security warnings in Kubernetes.

| Setting                  | Type    | Default  | Description                                   |
| ------------------------ | ------- | -------- | --------------------------------------------- |
|  `Ports`                 | array   | []       | List of ports for HTTPS.                      |
|  `UseHttpsRequired`      | bool    | false    | Enforce HTTPS required for all requests.      |
|  `Certificate`           | object  | default  | SSL certificate configuration.                |
|  `Certificate.Path`      | string  | null     | Required. File path to the certificate.       |
|  `Certificate.Password`  | string  | null     | Required. Password for the certificate.       |

```json
"App": {
  "Hosting": {
    "Https": {
      "Ports": [
      ],
      "UseHttpsRequired": false
      "Certificate": { 
        "Path": null,
        "Password": null
      }
    }
  }
}
```

When configuring HTTPS, the `docker-compose.yml` file must also be updated:

```yaml
services:
  {service-name}:
    ports:
      - 4443:4443
    volumes:
      - ../:/root/.dotnet/https
```

The solution should also include `certificate.yaml` and `ingress.yml` Kubernetes resources for both the `Staging` and `Production`, and the `build-and-deploy.yml` should 
include additional environmental variables and apply commands.  

Try it out yourself using the **[Api.Hosting.Https](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Hosting.Https)** example.  

## Routing
Routing in Nano is handled automatically and requires no explicit configuration. Routes are derived from controllers that inherit from the Nano base controllers, which 
provide the default routing behavior. When implementing a controller by deriving from a base controller, Nano automatically configures the route template and 
integrates API versioning into the route. This means that controllers only need to focus on their actions and logic, while Nano handles the underlying routing structure.  

By convention, all routes exposed by Nano are normalized to _lowercase_ to ensure consistency and predictable API endpoints.  

## MultiPart Limits
Configure upload limits for the API.  

> ⚠️ Leaving this configuration as null allows unlimited uploads, which may be acceptable if limits are enforced at the orchestration level. 
Otherwise, it is recommended to specify maximum upload sizes and timeouts to protect the application from large or slow requests.  

| Setting                  | Type    | Default  | Description                                               |
| ------------------------ | ------- | -------- | --------------------------------------------------------- |
|  `MaxUploadBytes`        | array   | 33554432 | Maximum allowed upload size in bytes. Defaults to 32 MB.  |
|  `KeepAliveTimeout`      | bool    | 00:02:10 | Timeout for slow uploads.                                 |

```json
"App": {
  "Hosting": {
    "MultipartLimits": {
      "MaxUploadBytes": 33554432,
      "KeepAliveTimeout": 130
    }
  }
}
```

Try it out yourself using the **[Api.Hosting.MultipartLimits](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Hosting.MultipartLimits)** example.  

## Http Policy Headers
Configure headers such as HSTS, XSS protection, CSP, CORS, and other policies to secure and control HTTP behavior.

| Setting              | Type    | Default  | Description                                                                                           |
| -------------------- | ------- | -------- | ----------------------------------------------------------------------------------------------------- |
|  `ContentType`       | object  | null     | Content-Type header options. See **[Content Type](#content-type)**                                    |
|  `ReferrerPolicy`    | object  | null     | Referrer-Policy header options. See **[Referrer Policy](#referrer-policy)**                           |
|  `FrameOptions`      | object  | null     | X-Frame-Options header options. See **[Frame Options](#frame-options)**                               |
|  `XssProtection`     | object  | null     | XSS-Protection header options. See **[Xss Protection](#xss-protection)**                              |
|  `Csp`               | object  | null     | Content-Security-Policy (CSP) options. See **[Content Security Policy](#content-security-policy)**    |
|  `Cors`              | object  | null     | CORS configuration options. See **[Cors](#cors)**                                                     |
|  `Hsts`              | object  | null     | HSTS configuration options. See **[Hsts](#hsts)**                                                     |
|  `Robots`            | object  | null     | Robots meta tag options. See **[Robots](#robots)**                                                    |
|  `ForwardedHeaders`  | object  | null     | Forwarded headers configuration. See **[Forwarded Headers](#forwarded-headers)**                      |

```json
"App": {
  "HttpPolicyHeaders": {
    "ContentType": null,
    "ReferrerPolicy": null,
    "FrameOptions": null,
    "XssProtection": null,
    "Csp": null,
    "Cors": null,
    "Hsts": null,
    "Robots": null,
    "ForwardedHeaders": null
  }
}
```

## Content Type Options
The HTTP X-Content-Type-Options response header indicates that the MIME types advertised in the Content-Type headers should be respected and not changed. 
The header allows you to avoid MIME type sniffing by specifying that the MIME types are deliberately configured.

| Setting          | Type  | Default  | Description                                       |
| ---------------- | ----- | -------- | ------------------------------------------------- |
|  `NoSniff`       | bool  | false    | If true, prevents MIME type sniffing. ⭐ `true`  |

```json
"App": {
  "HttpPolicyHeaders": {
    "ContentType": {
      "NoSniff": false
    }
  }
}
```

> 📖 Learn more about **[Content Type Options](https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/X-Content-Type-Options)**

Try it out yourself using the **[Api.PolicyHeaders.ContentType](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.PolicyHeaders.ContentTypeOptions)** example.  

## Referrer Policy
The HTTP Referrer-Policy response header controls how much referrer information (sent with the Referer header) should be included with requests.  

| Setting                  | Type  | Default   | Description                                      |
| ------------------------ | ----- | --------- | ------------------------------------------------ |
|  `ReferrerPolicyHeader`  | enum  | Disabled  | The referrer-policy. See possible values below   |

```json
"App": {
  "HttpPolicyHeaders": {
    "ReferrerPolicy": {
      "ReferrerPolicyHeader": "Disabled"
    }
  }
}
```

#### Referrer Policies
| Policy                          | Description                                                                                                                                                                                                                                                                             |
| ------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
|  `Disabled`                     | Specifies that the Referrer-Policy header should not be set in the HTTP response.                                                                                                                                                                                                       |
|  `NoReferrer`                   | Enables the no-referrer policy, instructing the browser to not send referrer information.                                                                                                                                                                                               |
|  `NoReferrerWhenDowngrade`      | Enables the no-referrer-when-downgrade policy, instructing the browser to send full referrer information unless navigation is from HTTPS to HTTP.                                                                                                                                       |
|  **`SameOrigin`**⭐             | **Enables the same-origin policy, instructing the browser to send full referrer information for same-origin requests and no referrer for cross-origin requests.**                                                                                                      |
|  `Origin`                       | Enables the origin policy, instructing the browser to send origin (no path and query) as referrer information for both same-origin and cross-origin requests.                                                                                                                           |
|  `StrictOrigin`                 | Enables the strict-origin policy, instructing the browser to send origin (no path and query) as referrer information for both same-origin and cross-origin HTTPS to HTTPS and HTTP to HTTP requests. HTTPS / HTTP requests will not include referrer information.                       |
|  `OriginWhenCrossOrigin`        | Enables the origin-when-cross-origin policy, instructing the browser to send full referrer information for same-origin requests and origin (no path and query) as referrer information for cross-origin requests (includes HTTPS to HTTP and HTTP to HTTPS).                            |
|  `StrictOriginWhenCrossOrigin`  | Enables the strict-origin-when-cross-origin policy, instructing the browser to send full referrer information for same-origin requests and origin (no path and query) as referrer information for cross-origin requests. Referrer information is not sent for HTTPS to HTTP requests.   |
|  `UnsafeUrl`                    | Enables the unsafe-url policy, instructing the browser to send full referrer information for all requests. Note that this will leak full referrer information for HTTPS to HTTP requests, which is even more unsafe than default browser behaviour.                                     |

Use `[ReferrerPolicy]` to override the global configuration, if needed.  

> 📖 Learn more about **[Referrer Policy](https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/Referrer-Policy)**

Try it out yourself using the **[Api.PolicyHeaders.ReferrerPolicy](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.PolicyHeaders.ReferrerPolicy)** example.  

## Frame Options
The HTTP X-Frame-Options response header can be used to indicate whether a browser should be allowed to render the document 
in a `<frame>`, `<iframe>`, `<embed>` or `<object>`. Sites can use this to avoid clickjacking attacks and some cross-site leaks, 
by ensuring that their content is not embedded into other sites.

If this header is not sent, and the website has not implemented any other mechanisms to restrict embedding (such as the frame-ancestors CSP directive), 
then the browser will allow other sites to embed this document.

| Setting                           | Type  | Default   | Description                                           |
| --------------------------------- | ----- | --------- | ----------------------------------------------------- |
|  `FrameOptionsPolicyHeader`       | enum  | Disabled  | Specifies the X-Frame-Options policy header value.    |

```json
"App": {
  "HttpPolicyHeaders": {
    "FrameOptions": {
      "FrameOptionsPolicyHeader": "Disabled"
    }
  }
}
```

#### Frame Options Policies
| Policy              | Description                                                                                                                                                                                                             |
| ------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
|  `Disabled`         | Specifies that the X-Frame-Options header should not be set in the HTTP response.                                                                                                                                       |
|  **`Deny`** ⭐       | **Specifies that the X-Frame-Options header should be set in the HTTP response, instructing the browser to not display the page when it is loaded in an iframe.**                                                       |
|  `SameOrigin`       | Specifies that the X-Frame-Options header should be set in the HTTP response, instructing the browser to display the page when it is loaded in an iframe - but only if the iframe is from the same origin as the page.  |
    
> 📖 Learn more about **[Frame Options Policy](https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/X-Frame-Options)**

Try it out yourself using the **[Api.PolicyHeaders.FrameOptions](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.PolicyHeaders.FrameOptions)** example.  

## Xss Protection
The HTTP X-XSS-Protection response header was a feature of Internet Explorer, Chrome and Safari that stopped pages from loading 
when they detected reflected cross-site scripting (XSS) attacks. These protections are largely unnecessary in modern browsers when sites 
implement a strong Content-Security-Policy that disables the use of inline JavaScript (`unsafe-inline`).  

> ⚠️ Deprecated: This feature is no longer recommended, and only supported by older browsers.  

| Setting                       | Type    | Default  | Description                                           |
| ----------------------------- | ------- | -------- | ----------------------------------------------------- |
|  `XssProtectionPolicyHeader`  | enum    | null     | Specifies the X-XSS-Protection policy header value.   |
|  `ReportingUrl`               | string  | null     | URL to report XSS attempts.                           |

```json
"App": {
  "HttpPolicyHeaders": {
    "XssProtection": {
      "XssProtectionPolicyHeader": "Disabled",
      "ReportingUrl": null
    }
  }
}
```

#### Xss Protection Policies
| Policy                           | Description                                                                                                                                           |
| -------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------- |
|  `FilterDisabled`                | Specifies that the X-Xss-Protection header should be set in the HTTP response, explicitly disabling the IE XSS filter.                                |
|  `FilterEnabled`                 | Specifies that the X-Xss-Protection header should be set in the HTTP response, explicitly enabling the IE XSS filter.                                 |
|  **`FilterEnabledBlockMode`**⭐   | **Specifies that the X-Xss-Protection header should be set in the HTTP response, explicitly enabling the IE XSS filter. BlockMode is set to true.**   |
|  `ProtectionReport`              | Report is sent.                                                                                                                                       |

> 📖 Learn more about **[Xss Protection](https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/X-XSS-Protection)**

Try it out yourself using the **[Api.PolicyHeaders.XssProtection](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.PolicyHeaders.XssProtection)** example.  

## Content Security Policy
The HTTP Content-Security-Policy response header allows website administrators to control resources the user agent is allowed to load for a given page. 
With a few exceptions, policies mostly involve specifying server origins and script endpoints. This helps guard against cross-site scripting attacks.  

See the **[Content Security Policy (CSP) Guide](https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/CSP)** for details about how a CSP is delivered to the browser, 
what it looks like, along with use cases and deployment strategies.  

| Setting                      | Type    | Default  | Description                                                                                                                                           |
| ---------------------------- | ------- | -------- | ----------------------------------------------------------------------------------------------------------------------------------------------------- |
|  `ReportOnly`                | object  | false    | The CSP will be enforced in report-only mode. Violations will be reported but not blocked.                                                            |
|  `UpgradeInsecureRequests`   | object  | false    | Instructs the browser to upgrade all HTTP requests to HTTPS automatically.                                                                            |
|  `ReportTo`                  | object  | null     | CSP Report-To header configuration. Specifies where CSP violation reports are sent (`report-to`). See [Report-To Directive](#report-to-directive)     |
|  `Defaults`                  | object  | null     | Default directive (`default-src`) for all unspecified content types. See [Common Directive](#common-directive)                                        |
|  `Scripts`                   | object  | null     | Controls allowed sources for scripts (`script-src`). See [Script Directive](#script-directive)                                                        |
|  `ScriptsElem`               | object  | null     | Controls allowed sources for script elements (`script-src-elem`). See [Script Directive](#script-directive)                                           |
|  `ScriptsAttr`               | object  | null     | Controls allowed sources for inline script attributes (`script-src-attr`). See [Script Directive](#script-directive)                                  |
|  `Styles`                    | object  | null     | Controls allowed sources for stylesheets (`style-src`). See [Style Directive](#style-directive)                                                       |
|  `StylesElem`                | object  | null     | Controls allowed sources for style elements (`style-src-elem`). See [Style Directive](#style-directive)                                               |
|  `StylesAttr`                | object  | null     | Controls allowed sources for inline style attributes (`style-src-attr`). See [Style Directive](#style-directive)                                      |
|  `Objects`                   | object  | null     | Controls allowed sources for object elements (`object-src`). See [Common Directive](#common-directive)                                                |
|  `Images`                    | object  | null     | Controls allowed sources for images (`img-src`). See [Common Directive](#common-directive)                                                            |
|  `Media`                     | object  | null     | Controls allowed sources for audio and video elements (`media-src`). See [Common Directive](#common-directive)                                        |
|  `Frames`                    | object  | null     | Controls allowed sources for frames and iframes (`frame-src`). See [Common Directive](#common-directive)                                              |
|  `FencedFrames`              | object  | null     | Controls allowed sources for fenced frames (`fenced-frame-src`). See [Common Directive](#common-directive)                                            |
|  `FrameAncestors`            | object  | null     | Controls which sources can embed this document (`frame-ancestors`). See [Common Directive](#common-directive)                                         |
|  `Fonts`                     | object  | null     | Controls allowed sources for fonts (`font-src`). See [Common Directive](#common-directive)                                                            |
|  `Connections`               | object  | null     | Controls allowed URLs for fetch, XHR, WebSocket, EventSource (`connect-src`). See [Common Directive](#common-directive)                               |
|  `BaseUris`                  | object  | null     | Controls allowed base URLs for the document (`base-uri`). See [Common Directive](#common-directive)                                                   |
|  `Children`                  | object  | null     | Controls allowed sources for nested browsing contexts (`child-src`). See [Common Directive](#common-directive)                                        |
|  `Forms`                     | object  | null     | Controls allowed URLs for form submissions (`form-action`). See [Common Directive](#common-directive)                                                 |
|  `Manifests`                 | object  | null     | Controls allowed sources for web app manifests (`manifest-src`). See [Common Directive](#common-directive)                                            |
|  `Workers`                   | object  | null     | Controls allowed sources for web workers, service workers, and shared workers (`worker-src`). See [Common Directive](#common-directive)               |
|  `TrustedTypes`              | object  | null     | Restricts which Trusted Types policies are allowed to create DOM objects (`trusted-types`). See [Trusted Types Directive](#trusted-types-directive)   |
|  `Sandbox`                   | object  | null     | Restricts features of a page when embedded in an iframe (`sandbox`). See [Sandbox Directive](#sandbox-directive)                                      |
|  `PermissionsPolicy`         | object  | null     | Controls access to powerful browser features (`permissions-policy`). See [Permissions Policy Directive](#permissions-policy-directive)                |

```json 
"App": {
  "HttpPolicyHeaders": {
    "Csp": {
      "ReportOnly": false,
      "UpgradeInsecureRequests": false,
      "ReportTo": null,
      "Defaults": null,
      "Scripts": null,
      "ScriptsElem": null,
      "ScriptsAttr": null,
      "Styles": null,
      "StylesElem": null,
      "StylesAttr": null,
      "Objects": null,
      "Images": null,
      "Media": null,
      "Frames": null,
      "FencedFrames": null,
      "FrameAncestors": null,
      "Fonts": null,
      "Connections": null,
      "BaseUris": null,
      "Children": null,
      "Forms": null,
      "Manifests": null,
      "TrustedTypes": null,
      "Sandbox": null,
      "PermissionsPolicy": null
    }
  }
}
```

#### Common Directive
Common configuration applicable to the following CSP directives:   
`Defaults`, `Objects`, `Media`, `Images`, `Frames`, `FencedFrames`, `FrameAncestors`, `Fonts`, `Connections`, `BaseUris`, `Children`, `Forms`, `Manifests`, `Workers`

| Setting        | Type    | Default | Description                                                        |
| -------------- | ------- | ------- | ------------------------------------------------------------------ |
|  `IsNone`      | bool    | false   | If `true`, only 'none' is allowed. All other sources are ignored.  |
|  `IsSelf`      | bool    | false   | If `true`, 'self' is allowed as a source. ⭐                        |
|  `Sources`     | array   | []      | Custom sources for the directive.                                  |

```json 
"App": {
  "HttpPolicyHeaders": {
    "Csp": {
      "{Directive}": {
        "IsNone": false,
        "IsSelf": false,
        "Sources": [
        ]
      }
    }
  }
}
```

#### Scripts Directive
The HTTP Content-Security-Policy (CSP) script directive specifies valid sources for JavaScript.  
Scripts directive are split into three different directives. `script-src`, `script-src-elem` and `script-src-attr`.  

This table shows options for `script-src` and `script-src-elem`:  

| Setting                     | Type    | Default | Description                                                                                           |
| --------------------------- | ------- | ------- | ----------------------------------------------------------------------------------------------------- |
|  `IsNone`                   | bool    | false   | Only `none` is allowed. All other sources are ignored.                                                |
|  `IsSelf`                   | bool    | false   | `self` is allowed as a source. ⭐                                                                      |
|  `IsUnsafeInline`           | bool    | false   | Allows inline scripts (`unsafe-inline`).                                                              |
|  `IsUnsafeEval`             | bool    | false   | Allows eval() and similar constructs (`unsafe-eval`).                                                 |
|  `IsUnsafeWasmEval`         | bool    | false   | Allows WebAssembly unsafe evaluation (`wasm-unsafe-eval`).                                            |
|  `IsTrustedTypesEval`       | bool    | false   | Allows undo of trusted-type evaluation (`trusted-types-eval`).                                        |
|  `StrictDynamic`            | bool    | false   | Enables 'strict-dynamic' behavior for script execution (`strict-dynamic`).                            |
|  `IsUnsafeHashes`           | bool    | false   | Allows unsafe hashes for inline scripts (`unsafe-hashes`). ⚠️ Only allowed for `script-src`           |
|  `UnsafeHashedAttributes`   | bool    | false   | Allows unsafe hashed attributes (`unsafe-hashed-attributes`). ⚠️ Only allowed for `script-src`        |
|  `UnsafeAllowRedirects`     | bool    | false   | Allows redirects from unsafe sources (`unsafe-allow-redirects`).                                      |
|  `InlineSpeculationRules`   | bool    | false   | Allows inline speculation rules (`inline-speculation-rules`).                                         |
|  `Sources`                  | array   | []      | Custom sources for scripts.                                                                           |
|  `Nonces`                   | array   | []      | Specific nonces to allow inline scripts.                                                              |
|  `Hashes`                   | array   | []      | SHA hashes to allow inline script content. Must be prefixed with `sha256-`, `sha384-`, or `sha512-`.  |
|  `RequireTrustedTypes`      | bool    | false   | Requires Trusted Types for script execution. ⚠️ Only allowed for `script-src`                         |
|  `RequireSri`               | bool    | false   | Requires Subresource Integrity (SRI) for scripts. ⚠️ Only allowed for `script-src`                    |
|  `ReportSample`             | bool    | false   | Enables 'report-sample' in CSP violation reports.                                                     |

This table shows the options for `script-src-attr`.

| Setting                     | Type    | Default | Description                                                                                           |
| --------------------------- | ------- | ------- | ----------------------------------------------------------------------------------------------------- |
|  `IsNone`                   | bool    | false   | Only `none` is allowed. All other sources are ignored.                                                |
|  `IsSelf`                   | bool    | false   | `self` is allowed as a source.⭐                                                                       |
|  `IsUnsafeInline`           | bool    | false   | Allows inline scripts (`unsafe-inline`).                                                              |
|  `IsUnsafeHashes`           | bool    | false   | Allows unsafe hashes for inline scripts (`unsafe-hashes`).                                            |
|  `UnsafeHashedAttributes`   | bool    | false   | Allows unsafe hashed attributes (`unsafe-hashed-attributes`).                                         |
|  `Sources`                  | array   | []      | Custom sources for scripts.                                                                           |
|  `ReportSample`             | bool    | false   | Enables 'report-sample' in CSP violation reports.                                                     |

```json 
"App": {
  "HttpPolicyHeaders": {
    "Csp": {
      "Scripts": {
        "IsNone": false,
        "IsSelf": false,
        "IsUnsafeInline": false,
        "IsUnsafeEval": false,
        "IsUnsafeWasmEval": false,
        "IsTrustedTypesEval": false,
        "IsUnsafeHashes": false,
        "StrictDynamic": false,
        "UnsafeHashedAttributes": false,
        "UnsafeAllowRedirects": false,
        "InlineSpeculationRules": false
        "Sources": [
        ],
        "Nonces": [
        ],
        "Hashes": [
        ],
        "RequireTrustedTypes": false,
        "RequireSri": false,
        "ReportSample": false
      }
      "ScriptsElem": {
        "IsNone": false,
        "IsSelf": false,
        "IsUnsafeInline": false,
        "IsUnsafeEval": false,
        "IsUnsafeWasmEval": false,
        "IsTrustedTypesEval": false,
        "StrictDynamic": false,
        "UnsafeHashedAttributes": false,
        "UnsafeAllowRedirects": false,
        "InlineSpeculationRules": false
        "Sources": [
        ],
        "Nonces": [
        ],
        "Hashes": [
        ],
        "RequireTrustedTypes": false,
        "RequireSri": false,
        "ReportSample": false
      },
      "ScriptsAttr": {
        "IsNone": false,
        "IsSelf": false,
        "IsUnsafeInline": false,
        "IsUnsafeHashes": false,
        "UnsafeHashedAttributes": false,
        "Sources": [
        ],
        "ReportSample": false
      }
    }
  }
}
```

#### Styles Directive
The HTTP Content-Security-Policy (CSP) style directive specifies valid sources for stylesheets.  
Styles directive are split into three different directives. `style-src`, `style-src-elem` and `style-src-attr`.  

This table shows options for `style-src` and `style-src-elem`:  

| Setting                     | Type    | Default | Description                                                                                           |
| --------------------------- | ------- | ------- | ----------------------------------------------------------------------------------------------------- |
|  `IsNone`                   | bool    | false   | Only `none` is allowed. All other sources are ignored.                                                |
|  `IsSelf`                   | bool    | false   | `self` is allowed as a source. ⭐                                                                      |
|  `IsUnsafeInline`           | bool    | false   | Allows inline styles (`unsafe-inline`).                                                               |
|  `IsUnsafeHashes`           | bool    | false   | Allows unsafe hashes for inline styles (`unsafe-hashes`). ⚠️ Only allowed for `style-src`             |
|  `Sources`                  | array   | []      | Custom sources for styles.                                                                            |
|  `Nonces`                   | array   | []      | Specific nonces to allow inline styles.                                                               |
|  `Hashes`                   | array   | []      | SHA hashes to allow inline styles content. Must be prefixed with `sha256-`, `sha384-`, or `sha512-`.  |
|  `RequireSri`               | bool    | false   | Requires Subresource Integrity (SRI) for styles. ⚠️ Only allowed for `style-src`                      |
|  `ReportSample`             | bool    | false   | Enables 'report-sample' in CSP violation reports.                                                     |

This table shows trhe options for `script-src-attr`.

| Setting                     | Type    | Default | Description                                                                                           |
| --------------------------- | ------- | ------- | ----------------------------------------------------------------------------------------------------- |
|  `IsNone`                   | bool    | false   | Only `none` is allowed. All other sources are ignored.                                                |
|  `IsSelf`                   | bool    | false   | `self` is allowed as a source. ⭐                                                                      |
|  `IsUnsafeInline`           | bool    | false   | Allows inline styles (`unsafe-inline`).                                                               |
|  `IsUnsafeHashes`           | bool    | false   | Allows unsafe hashes for inline styles (`unsafe-hashes`).                                             |
|  `Sources`                  | array   | []      | Custom sources for styles.                                                                            |
|  `ReportSample`             | bool    | false   | Enables 'report-sample' in CSP violation reports.                                                     |

```json 
"App": {
  "HttpPolicyHeaders": {
    "Csp": {
      "Styles": {
        "IsNone": false,
        "IsSelf": false,
        "IsUnsafeInline": false,
        "IsUnsafeHashes": false,
        "Sources": [
        ],
        "Nonces": [
        ],
        "Hashes": [
        ],
        "RequireSri": false,
        "ReportSample": false
      },
      "StylesElem": {
        "IsNone": false,
        "IsSelf": false,
        "IsUnsafeInline": false,
        "Sources": [
        ],
        "Nonces": [
        ],
        "Hashes": [
        ],
        "ReportSample": false
      },
      "StylesAttr": {
        "IsNone": false,
        "IsSelf": false,
        "IsUnsafeInline": false,
        "IsUnsafeHashes": false,
        "Sources": [
        ],
        "ReportSample": false
      }
    }
  }
}
```

#### TrustedTypes Directive
The HTTP Content-Security-Policy (CSP) trusted-types directive is used to specify an allowlist of Trusted Type policy names 
that a website can create using `trustedTypes.createPolicy()`.

| Setting             | Type    | Default | Description                                               |
| ------------------- | ------- | ------- | --------------------------------------------------------- |
|  `IsNone`           | bool    | false   | Only `none` is allowed. All other sources are ignored.    |
|  `AllowDuplicates`  | bool    | false   | Allow duplicate policy names.                             |
|  `Policies`         | array   | []      | List of allowed Trusted Types policy names.               |

```json 
"App": {
  "HttpPolicyHeaders": {
    "Csp": {
      "TrustedTypes": {
        "IsNone": false,
        "AllowDuplicates": false,
        "Policies": [
        ]
      }
    }
  }
}
```

#### Sandbox Directive
The HTTP Content-Security-Policy (CSP) sandbox directive enables a sandbox for the requested resource similar to the `<iframe>` sandbox attribute. 
It applies restrictions to a page's actions including preventing popups, preventing the execution of plugins and scripts, and enforcing a same-origin policy.  

| Setting                                 | Type    | Default     | Description                                                  |
| --------------------------------------- | ------- | ----------- | ------------------------------------------------------------ |
|  `AllowDownloads`                       | bool    | false       | Allows downloads in the sandbox.                             |
|  `AllowForms`                           | bool    | false       | Allows form submissions from the sandboxed page.             |
|  `AllowModals`                          | bool    | false       | Allows opening modal windows.                                |
|  `AllowOrientationLock`                 | bool    | false       | Allows disabling screen orientation lock.                    |
|  `AllowPointerLock`                     | bool    | false       | Allows usage of Pointer Lock API.                            |
|  `AllowPopups`                          | bool    | false       | Allows popups (window.open, target=_blank).                  |
|  `AllowPopupsToEscapeSandbox`           | bool    | false       | Allows popups to escape sandbox restrictions.                |
|  `AllowPresentation`                    | bool    | false       | Allows initiating presentations from the sandboxed page.     |
|  `AllowSameOrigin`                      | bool    | false       | Allows same-origin access from sandboxed content.            |
|  `AllowScripts`                         | bool    | false       | Allows execution of scripts.                                 |
|  `AllowStorageAccessByUserActivation`   | bool    | false       | Allows storage access via user activation.                   |
|  `AllowTopNavigation`                   | bool    | false       | Allows top-level navigation.                                 |
|  `AllowTopNavigationByUserActivation`   | bool    | false       | Allows top-level navigation via user activation.             |
|  `AllowTopNavigationToCustomProtocols`  | bool    | false       | Allows navigation to custom protocols.                       |

```json 
"App": {
  "HttpPolicyHeaders": {
    "Csp": {
      "Sandbox": {
        "AllowDownloads": false,
        "AllowForms": false,
        "AllowModals": false,
        "AllowOrientationLock": false,
        "AllowPointerLock": false,
        "AllowPopups": false,
        "AllowPopupsToEscapeSandbox": false,
        "AllowPresentation": false,
        "AllowSameOrigin": false,
        "AllowScripts": false,
        "AllowStorageAccessByUserActivation": false,
        "AllowTopNavigation": false,
        "AllowTopNavigationByUserActivation": false,
        "AllowTopNavigationToCustomProtocols": false
      }
    }
  }
}
```

#### Permissions Policy Directive
The HTTP Permissions-Policy response header provides a mechanism to allow and deny the use of browser features in a document or 
within any `<iframe>` elements in the document.  

The following directives may be added to `PermissionPolicy` in the configuration:

| Directive                      | Description                                                                                                                                                                                                                            |
| ------------------------------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Accelerometer`                | Controls whether the current document is allowed to gather information about the acceleration of the device through the Accelerometer interface.                                                                                       |
| `AmbientLightSensor`           | Controls whether the current document is allowed to gather information about the amount of light in the environment around the device through the AmbientLightSensor interface.                                                        |
| `AriaNotify`                   | Controls whether the current document is allowed to use the `ariaNotify()` method to fire screen reader announcements.                                                                                                                 |
| `AutoPlay`                     | Controls whether the current document is allowed to autoplay media requested through the HTMLMediaElement interface.                                                                                                                   |
| `Bluetooth`                    | Controls whether the use of the Web Bluetooth API is allowed.                                                                                                                                                                          |
| `Battery`                      | Controls whether the use of the Battery Status API is allowed.                                                                                                                                                                         |
| `Camera`                       | Controls whether the current document is allowed to use video input devices.                                                                                                                                                           |
| `CapturedSurfaceControl`       | Controls whether or not the document is permitted to use the Captured Surface Control API.                                                                                                                                             |
| `HighEntropyValues`            | Controls whether or not the document is permitted to use the `NavigatorUAData.getHighEntropyValues()` method to retrieve high-entropy user-agent data.                                                                                 |
| `ComputePressure`              | Controls access to the Compute Pressure API.                                                                                                                                                                                           |
| `CrossOriginIsolated`          | Controls whether the current document can be treated as cross-origin isolated.                                                                                                                                                         |
| `DeferredFetch`                | Controls the allocation of the top-level origin's `fetchLater()` quota.                                                                                                                                                                |
| `DeferredFetchMinimal`         | Controls the allocation of the shared cross-origin subframe `fetchLater()` quota.                                                                                                                                                      |
| `DisplayCapture`               | Controls whether the current document is permitted to use the `getDisplayMedia()` method to capture screen contents.                                                                                                                   |
| `DocumentDomain`               | Controls whether the current document is allowed to set `document.domain`.                                                                                                                                                             |
| `EncryptedMedia`               | Controls whether the current document is allowed to use the Encrypted Media Extensions API (EME).                                                                                                                                      |
| `ExecutionWhileNotRendered`    | Execution While Not Rendered. Controls whether tasks should execute in frames while they're not being rendered (e.g. if an iframe is hidden or display: none).                                                                         |
| `ExecutionWhileOutOfViewport`  | Execution While Out Of Viewport. Controls whether tasks should execute in frames while they're outside the visible viewport.                                                                                                           |
| `FullScreen`                   | Controls whether the current document is allowed to use `Element.requestFullScreen()`.                                                                                                                                                 |
| `Gamepad`                      | Controls whether the current document is allowed to use the Gamepad API.                                                                                                                                                               |
| `Geolocation`                  | Controls whether the current document is allowed to use the Geolocation Interface.                                                                                                                                                     |
| `Gyroscope`                    | Controls whether the current document is allowed to gather information about the orientation of the device through the Gyroscope interface.                                                                                            |
| `Hid`                          | Controls whether the current document is allowed to use the WebHID API to connect to uncommon or exotic human interface devices such as alternative keyboards or gamepads.                                                             |
| `IdentityCredentialsGet`       | Controls whether the current document is allowed to use the Federated Credential Management API (FedCM).                                                                                                                               |
| `IdleDetection`                | Controls whether the current document is allowed to use the Idle Detection API to detect when users are interacting with their devices, for example to report "available"/"away" status in chat applications.                          |
| `LanguageDetector`             | Controls access to the language detection functionality of the Translator and Language Detector APIs.                                                                                                                                  |
| `LocalFonts`                   | Controls whether the current document is allowed to gather data on the user's locally-installed fonts via the Window.queryLocalFonts() method (see also the Local Font Access API).                                                    |
| `LayoutAnimations`             | Controls whether the current document is allowed to show layout animations.                                                                                                                                                            |
| `LegacyImageFormats`           | Controls whether the current document is allowed to display images in legacy formats.                                                                                                                                                  |
| `Magnetometer`                 | Controls whether the current document is allowed to gather information about the orientation of the device through the Magnetometer interface.                                                                                         |
| `Microphone`                   | Controls whether the current document is allowed to use audio input devices.                                                                                                                                                           |
| `Midi`                         | Controls whether the current document is allowed to use the Web MIDI API.                                                                                                                                                              |
| `OnDeviceSpeechRecognition`    | Controls access to the on-device speech recognition functionality of the Web Speech API.                                                                                                                                               |
| `OtpCredentials`               | Controls whether the current document is allowed to use the WebOTP API to request a one-time password (OTP) from a specially-formatted SMS message sent by the app's server, i.e., via `navigator.credentials.get({otp: ..., ...})`.   |
| `NavigationOverride`           | Controls the availability of mechanisms that enables the page author to take control over the behavior of spatial navigation, or to cancel it outright.                                                                                |
| `OversizedImages`              | Controls whether the current document is allowed to download and display large images.                                                                                                                                                 |
| `Payment`                      | Controls whether the current document is allowed to use the Payment Request API.                                                                                                                                                       |
| `PictureInPicture`             | Controls whether the current document is allowed to play a video in a Picture-in-Picture mode via the corresponding API.                                                                                                               |
| `PrivateStateTokenIssuance`    | Controls usage of private state token token-request operations.                                                                                                                                                                        |
| `PrivateStateTokenRedemption`  | Controls usage of private state token token-redemption and send-redemption-record operations.                                                                                                                                          |
| `PublickeyCredentialsCreate`   | Controls whether the current document is allowed to use the Web Authentication API to create new asymmetric key credentials, i.e., via `navigator.credentials.create({ publicKey: ..., ...})`.                                         |
| `PublicKeyCredentialsGet`      | Controls whether the current document is allowed to use the Web Authentication API to retrieve already stored public-key credentials, i.e. via `navigator.credentials.get({publicKey: ..., ...})`.                                     |
| `ScreenWakeLock`               | Controls whether the current document is allowed to use Screen Wake Lock API to indicate that device should not turn off or dim the screen.                                                                                            |
| `Serial`                       | Controls whether the current document is allowed to use the Web Serial API to communicate with serial devices, either directly connected via a serial port, or via USB or Bluetooth devices emulating a serial port.                   |
| `SpeakerSelection`             | Controls whether the current document is allowed to use the Audio Output Devices API to list and select speakers.                                                                                                                      |
| `StorageAccess`                | Controls whether a document loaded in a third-party context (i.e., embedded in an iframe) is allowed to use the Storage Access API to request access to unpartitioned cookies.                                                         |
| `Translator`                   | Controls access to the translation functionality of the Translator and Language Detector APIs.                                                                                                                                         |
| `Summarizer`                   | Controls access to the Summarizer API.                                                                                                                                                                                                 |
| `SyncXhr`                      | Controls whether the current document is allowed to make synchronous XMLHttpRequest requests.                                                                                                                                          |
| `UnoptimizedImages`            | Controls whether the current document is allowed to download and display unoptimized images.                                                                                                                                           |
| `UnsizedMedia`                 | Controls whether the current document is allowed to change the size of media elements after the initial layout is complete.                                                                                                            |
| `Usb`                          | Controls whether the current document is allowed to use the WebUSB API.                                                                                                                                                                |
| `WebShare`                     | Controls whether the current document is allowed to use the `Navigator.share()` of Web Share API to share text, links, images, and other content to arbitrary destinations of user's choice, e.g. mobile apps.                         |
| `WindowManagement`             | Controls whether or not the current document is allowed to use the Window Management API to manage windows on multiple displays.                                                                                                       |
| `XrSpatialTracking`            | Controls whether the current document is allowed to use the WebXR Device API to interact with a WebXR session.                                                                                                                         |

Each directive may be configured as shown below.  

| Setting                     | Type    | Default | Description                                                 |
| --------------------------- | ------- | ------- | ----------------------------------------------------------- |
|  `IsNone`                   | bool    | false   | Only `none` is allowed. All other sources are ignored.      |
|  `IsSelf`                   | bool    | false   | `self` is allowed as a source. ⭐                            |
|  `Sources`                  | array   | []      | Allowed custom asources.                                    |

```json 
"App": {
  "HttpPolicyHeaders": {
    "Csp": {
      "PermissionsPolicy": {
        "{directive}": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        }
      }
    }
  }
}
```

#### Report-To Directive
Defines the CSP `report-to` directive for reporting policy violations.  
If configured, both the `Report-To` and `Reporting-Endpoints` headers are emitted on every response using the provided configuration.  

| Setting        | Type    | Default     | Description                                                                                                                                                  |
| -------------- | ------- | ----------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------ |
|  `Group`       | string  | csp-reports | Reporting group name referenced by CSP.                                                                                                                      |
|  `MaxAge`      | int     | 10886400    | Max age (seconds) for the report group.                                                                                                                      |
|  `Endpoints`   | array   | []          | URLs to receive CSP reports. If no endpoints is specified, report-to will default to the built-in Nano endpoint: `/csp/report-to`, that logs the violation.  |

```json 
"App": {
  "HttpPolicyHeaders": {
    "Csp": {
      "ReportTo": {
        "Group": "csp-reports",
        "MaxAge": "10886400",
        "Endpoints": [
        ]
      }
    }
  }
}
```

> 📖 Learn more about **[Content Security Policy](https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/Content-Security-Policy)**

Try it out yourself using the **[Api.PolicyHeaders.ContentSecurityPolicy](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.PolicyHeaders.ContentSecurityPolicy)** example.  

## Cors
Cross-Origin Resource Sharing (CORS) is an HTTP-header-based security mechanism that allows a server to authorize web browsers to load resources 
from a domain different than the one that served the original page.  

Nano intercepts browser preflight (OPTIONS) requests and responds with the correct `Access-Control-*` headers, fully respecting the configured CORS policy.  

| Setting                   | Type    | Default  | Description                                                                                                                                |
| ------------------------- | ------- | -------- | ------------------------------------------------------------------------------------------------------------------------------------------ |
|  `AllowedOrigins`         | array   | []       | Allowed origins.                                                                                                                           |
|  `AllowedHeaders`         | array   | []       | Allowed HTTP headers.                                                                                                                      |
|  `AllowedMethods`         | array   | []       | Allowed HTTP methods.                                                                                                                      |
|  `AllowCredentials`       | bool    | false    | Indicates whether credentials are allowed.                                                                                                 |
|  `Origin`                 | object  | default  | Origin-specific CORS policies.                                                                                                             |
|  `Origin.EmbedderPolicy`  | object  | default  | The HTTP Cross-Origin-Embedder-Policy (COEP) response header configures the current document's policy for loading and embedding cross-origin resources. Allowed values: `UnsafeNone`⭐, `RequireCorp` or `Credentialless`.  |
|  `Origin.OpenerPolicy`    | object  | default  | The HTTP Cross-Origin-Opener-Policy (COOP) response header allows a website to control whether a new top-level document, opened using Window.open() or by navigating to a new page, is opened in the same browsing context group (BCG) or in a new browsing context group. Allowed values: `SameOrigin`⭐, `UnsafeNone` or `SameOriginAllowPopups`.  |
|  `Origin.ResourcePolicy`  | object  | default  | The HTTP Cross-Origin-Resource-Policy response header (CORP) indicates that the browser should block no-cors cross-origin or cross-site requests to the given resource. Allowed values: `SameOrigin`⭐, `SameSite` or `CrossOrigin`.  |
|  `ExposedHeaders`         | object  | default  | Additional exposed headers. Nano exposes these headers by default: `TZ`, `RequestId`, `Content-Disposition` and `api-supported-versions`.  |

```json
"App": {
  "HttpPolicyHeaders": {
    "Cors": {
      "AllowedOrigins": [
      ],
      "AllowedHeaders": [
      ],
      "AllowedMethods": [
      ],
      "AllowCredentials": false,
      "Origin": {
        "EmbedderPolicy": null,
        "OpenerPolicy": null,
        "ResourcePolicy": null
      }
      "ExposedHeaders": [
      ]
    }
  }
}
```

Use `[EnableCors]` and `[DisableCors]` to override the global configuration, if needed.  

> 📖 Learn more about **[Hsts](https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/CORS)**

Try it out yourself using the **[Api.PolicyHeaders.Cors](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.PolicyHeaders.Cors)** example.  

## Strict Transport Security (Hsts)
HTTP Strict Transport Security (HSTS) is a web security policy mechanism that forces browsers to interact with websites solely through secure HTTPS connections.  

| Setting               | Type     | Default       | Description                                                                                           |
| --------------------- | -------- | ------------- | ----------------------------------------------------------------------------------------------------- |
|  `MaxAge`             | TimeSpan | 182:00:00:00  | Maximum age for HSTS. Default 182 days.                                                               |
|  `UsePreload`         | bool     | false         | Enable or disable the preload directive. Preload will only used if `MaxAge` is greater than 7 weeks.  |
|  `IncludeSubdomains`  | bool     | false         | Include subdomains in HSTS policy.                                                                    |

```json
"App": {
  "HttpPolicyHeaders": {
    "Hsts": {
      "MaxAge": "182:00:00:00",
      "UsePreload": false,
      "IncludeSubdomains": false
    }
  }
}
```

> 📖 Learn more about **[Hsts](https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/Strict-Transport-Security)**

Try it out yourself using the **[Api.PolicyHeaders.Hsts](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.PolicyHeaders.Hsts)** example.  

## Robots
The `X-Robots-Tag` response header defines how crawlers should index URLs. While not part of any specification, it is a de-facto standard method 
for communicating with search bots, web crawlers, and similar user agents.

| Setting             | Type  | Default  | Description                                                                         |
| ------------------- | ----- | -------- | ----------------------------------------------------------------------------------- |
|  `UseNoIndex`       | bool  | false    | Instructs search engines to not index the page.                                     |
|  `UseNoFollow`      | bool  | false    | Instructs search engines to not follow links on the page.                           |
|  `UseNoSnippet`     | bool  | false    | Instructs search engines to not display a snippet for the page in search results.   |
|  `UseNoArchive`     | bool  | false    | Instructs search engines to not offer a cached version of the page.                 |
|  `UseNoOdp`         | bool  | false    | Instructs search engines to not use Open Directory Project info for title/snippet.  |
|  `UseNoTranslate`   | bool  | false    | Instructs search engines to not offer translation of the page (Google only).        |
|  `UseNoImageIndex`  | bool  | false    | Instructs search engines to not index images on the page (Google only).             |

```json
"App": {
  "HttpPolicyHeaders": {
    "Robots": {
      "UseNoIndex": false,
      "UseNoFollow": false,
      "UseNoSnippet": false,
      "UseNoArchive": false,
      "UseNoOdp": false,
      "UseNoTranslate": false,
      "UseNoImageIndex": false
    }
  }
}
```

> 📖 Learn more about **[X-Robots-Tag](https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/X-Robots-Tag)**

Try it out yourself using the **[Api.PolicyHeaders.Robots](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.PolicyHeaders.Robots)** example.  

## Forwarded Headers
When connecting through a HTTP proxy (or load balancer), server logs will only contain the IP address, host address, and protocol of the proxy; 
this header can be used to identify the IP address, host, and protocol, of the original request.

| Setting                   | Type    | Default  | Description                                                                                              |
| ------------------------- | ------- | -------- | -------------------------------------------------------------------------------------------------------- |
|  `Headers`                | object  | All      | Defines the headers that should forwarded.                                                               |
|  `RequireHeaderSymmetry`  | bool    | true     | Specifies that forwarded headers will only be processed if the set of headers is complete for that hop.  |

```json
"App": {
  "HttpPolicyHeaders": {
    "ForwardedHeaders": {
      "Headers": "All",
      "RequireHeaderSymmetry": true
    }
  }
}
```

#### Headers Values
| Setting              | Description                                                                                            |
| -------------------- | ------------------------------------------------------------------------------------------------------ |
|  `None`              | Do not process any forwarders.                                                                         |
|  `XForwardedFor`     | Process `X-Forwarded-For`, which identifies the originating IP address of the client.                  |
|  `XForwardedHost`    | Process `X-Forwarded-Host`, which identifies the original host requested by the client.                |
|  `XForwardedPort`    | Process `X-Forwarded-Port`, which identifies the original port requested by the client.                |
|  `XForwardedProto`   | Process `X-Forwarded-Proto`, which identifies the protocol (HTTP or HTTPS) the client used to connect. |
|  `XForwardedPrefix`  | Process `X-Forwarded-Prefix`, which identifies the original path base used by the client.              |
|  `All`               | Process X-Forwarded-For, X-Forwarded-Host, X-Forwarded-Proto and X-Forwarded-Prefix.                   |

#### X-Forwarded-Headers
| Header               | HttpContext                                     |
| -------------------- | ----------------------------------------------- |
| `X-Forwarded-Proto`  | Sets `HttpContext.Request.Scheme`.              |
| `X-Forwarded-Host`   | Sets `HttpContext.Request.Host`.                |
| `X-Forwarded-Port`   | Sets `HttpContext.Request.Host.Port`.           |
| `X-Forwarded-For`	   | Sets `HttpContext.Connection.RemoteIpAddress`.  |
| `X-Forwarded-Prefix` | Ignored, and not transferred to `httpContext`.  |

If your app is directly exposed to the internet without a reverse proxy in front, clients can spoof X-Forwarded headers: fake IP, scheme, or host.
Therefore, this approach is safe only if your traffic always passes through a trusted proxy / load balancer, which is exactly the case in cloud deployments.

> 📖 Learn more about **[Forwarded Headers](https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/Forwarded)**

Try it out yourself using the **[Api.PolicyHeaders.ForwardedHeaders](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.PolicyHeaders.ForwardedHeaders)** example.  

## Response Cache
The HTTP cache stores a response associated with a request and reuses the stored response for subsequent requests.  

There are several advantages to reusability. First, since there is no need to deliver the request to the origin server, 
then the closer the client and cache are, the faster the response will be. The most typical example is when the browser itself stores a cache for browser requests.  

Also, when a response is reusable, the origin server does not need to process the request — so it does not need to parse and route the request, 
restore the session based on the cookie, query the DB for results, or render the template engine. That reduces the load on the server.

> ⚠️ It's recommended to enable this in configuration, then disable for specific actions using `[ResponseCache(...)]`.

| Setting        | Type     | Default  | Description                                         |
| -------------- | -------- | -------- | --------------------------------------------------- |
|  `MaxSize`     | int      | 1024     | Maximum cache size in KB. Default is 1 MB.          |
|  `MaxBodySize` | int      | 102400   | Maximum cached body size in KB. Default is 100 MB.  |
|  `MaxAge`      | TimeSpan | 00:20:00 | Maximum cache duration. Default is 20 minutes.      |

```json
"App": {
  "ResponseCache": {
    "MaxSize": 1024,
    "MaxBodySize": 102400,
    "MaxAge": "00:20:00"
  }
}
```

> 📖 Learn more about **[Response Caching](https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/Caching)**

Try it out yourself using the **[Api.ResponseCache](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.ResponseCache)** example.  

## Response Compression
HTTP response compression is a technique used to reduce the size of data sent from a web server to a client (usually a browser). 
By shrinking the payload, it improves website loading speeds, reduces bandwidth consumption, and enhances overall network efficiency.  

| Setting        | Type   | Default  | Description                            |
| -------------- | ------ | -------- | -------------------------------------- |
|  `UseGzip`     | bool   | true     | Enable or disable Gzip compression.    |
|  `UseBrotli`   | bool   | true     | Enable or disable Brotli compression.  |

```json
"App": {
  "ResponseCompression": {
    "UseGzip": true,
    "UseBrotli": true
  }
}
```

> 📖 Learn more about **[Response Compression](https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/Compression)**

Try it out yourself using the **[Api.ResponseCompression](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.ResponseCompression)** example.  

## Session
Adds session state support to the application, allowing user-specific data to persist across requests. 
It stores this data on the server and uses a session cookie to track individual users. This enables features like authentication, shopping carts, or temporary user settings.  

Cookie name: `.AspNetCore.Session`

> ⚠️ Sessions are discouraged, as tit breaks statelessness and complicates scaling.

For sessions to work properly in a scaled environment, enable sticky sessions by adding these annotations to your `ingress.yaml` resource.

```yaml
kind: Ingress
metadata:
  annotations:
    nginx.ingress.kubernetes.io/affinity: "cookie"
    nginx.ingress.kubernetes.io/affinity-mode: "persistent"
    nginx.ingress.kubernetes.io/session-cookie-name: "eventssticky"
    nginx.ingress.kubernetes.io/session-cookie-max-age: "172800"
```

| Setting        | Type     | Default  | Description                                         |
| -------------- | -------- | -------- | --------------------------------------------------- |
|  `Timeout`     | TimeSpan | 00:20:00 | Session timeout duration. Default is 20 minutes.    |

```json
"App": {
  "Session": {
    "Timeout": "00:20:00"
  }
}
```

Try it out yourself using the **[Api.Session](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Session)** example.  

## Cookies
Cookies in Nano are pre-configured for security and cannot be customized. This ensures that all cookies follow best practices by default.  

Auto configured cookie settings:

| Setting                | Value          | Description                                                             |
| ---------------------- | -------------- | ----------------------------------------------------------------------- |
|  `HttpOnly`            | Always         | This cookie inaccessible to the JavaScript `document.cookie` API.       |
|  `CookieSecurePolicy`  | SameAsRequest  | If the URI that provides the cookie is HTTPS, then the cookie will only be returned to the server on subsequent HTTPS requests. Otherwise if the URI that provides the cookie is HTTP, then the cookie will be returned to the server on all HTTP and HTTPS requests. This value ensures HTTPS for all authenticated requests on deployed servers, and also supports HTTP for localhost development and for servers that do not have HTTPS support.  |
|  `SameSiteMode`        | Strict         | Cookie is restricted to `same-site` requests, mitigating CSRF attacks.  |

When a cookie is created, Nano automatically enforces these settings.  
If a cookie’s options already match the policy or are stricter, they remain unchanged. 
However, if a cookie’s options violate the policy, the middleware adjusts them to ensure they conform before the response is sent to the client.

Try it out yourself using the **[Api.Cookies](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Cookies)** example.  

## TimeZone
Nano supports built-in methods for specifying the timezone when making requests.  

When `DateTimeOffset` values are passed in requests or querystrings, Nano automatically converts them to UTC, simplifying date and time handling within the application, 
as all internal operations work in UTC. When returning responses, `DateTimeOffset` properties are converted back to the timezone specified in the request. 
If no timezone is provided, the configured `TimeZone.DefaultTimeZone` is used for converting response values.  

> ⚠️ Note that `DateTime` values are not converted, either in requests or responses.

To specify the timezone in a request, you can use one of the following methods:
* Http header (`tz=Europe/Copenhagen`)
* Querystring parameter (`tz=Europe/Copenhagen`)
* Cookie (`.AspNetCore.TimeZone=Europe/Copenhagen`)

When using layered Nano APIs, the `tz` header is automatically propagated across all layers when leveraging
the built-in [Nano Api Client](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App#api-client).  

To easily obtain the current date and time, use the following properties on `DateTimeInfo`:

```csharp
var local = DateTimeInfo.Now;   // Local date-time based on the timezone specified in the request or DefaultTimeZone
var utc   = DateTimeInfo.UtcNow; // UTC date-time
```

Cookie name: `.AspNetCore.TimeZone`

| Setting             | Type    | Default  | Description                             |
| ------------------- | ------- | -------- | --------------------------------------- |
|  `DefaultTimeZone`  | string  | UTC      | Default time zone for the application.  |

```json
"App": {
  "TimeZone": {
    "DefaultTimeZone": "UTC"
  }
}
```

> 📖 Learn more about **[Request TimeZone](https://github.com/vivet/Vivet.AspNetCore/tree/master/Vivet.AspNetCore.RequestTimeZone#vivetaspnetcorerequesttimezone)**.  

Try it out yourself using the **[Api.TimeZone](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.TimeZone)** example.  

## Localization
Nano provides built-in support for specifying the language when making requests.

To specify the language for a request, you can use one of the following methods:
* Http header (```Accept-Language=da-DK```)
* Query parameter (```culture=da-DK```)
* Cookie (`.AspNetCore.Culture=c=da-DK|uic=da-DK`)

When using layered Nano APIs, the `Accept-Language` header is automatically propagated across all layers when leveraging 
the built-in [Nano Api Client](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App#api-client).  

Cookie name: `.AspNetCore.Culture`

| Setting               | Type    | Default  | Description                                                                                                |
| --------------------- | --------| -------- | ---------------------------------------------------------------------------------------------------------- |
|  `DefaultCulture`     | string  | en-US    | The default culture used by the application.                                                               |
|  `SupportedCultures`  | enum    | []       | The set of cultures supported by the application. Unsupported cultures will fallback to `DefaultCulture`.  |

```json
"App": {
  "Localization": {
    "DefaultCulture": "en-US",
    "SupportedCultures": [
    ]
  }
}
```

> 📖 Learn more about **[Request Localization](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization)**.  

Try it out yourself using the **[Api.Localization](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Localization)** example.  

## Versioning
API versioning in **Nano** requires no additional configuration. It leverages the built-in versioning support provided by ASP.NET Core. 
To enable versioning, apply the `[ApiVersion]` attribute to the controller and the `[MapToApiVersion]` attribute to each action with the desired version number. 
Both attributes are required for versioning to work correctly.  

The API version can be specified using the following mechanisms, evaluated in this order:
* Route segment (`v{version}`)
* HTTP header (`api-version`)
* Query parameter (`api-version`)

By default, routes use the configured application version. The value set in `App:Version` is treated as the default API version, 
allowing routes targeting the default version to work without explicitly specifying a version in the URL. Controllers and actions targeting the default version 
do not need to be annotated, as this version is assumed automatically.

Nano automatically adds API versioning headers to every response. The `Api-Version` header reflects the version requested by the client 
(or the default if none is specified), and the `Api-Supported-Versions` header lists all versions supported by the API.  

Only **major** and **minor** version numbers are considered when specifying versions 
For example, when specifying version in route, `/api/v1/...` and `/api/v1.0/...` are valid, while `/api/v1.0.0/...` is not supported. The same applies 
to the other veresion providers.  

> ⚠️ **Versioning should be used with caution**  
> Managing multiple API versions quickly adds complexity and maintenance overhead. Whenever possible, prefer evolving the API in a backward-compatible way 
so existing clients continue to work without requiring new versions. Use versioning only in rare cases where breaking changes are unavoidable.

Try it out yourself using the **[Api.Versioning](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Versioning)** example.  

## Documentation
When documentation is enabled in the configuration, the API's web-based documentation interface (Swagger) is available at `/docs`.

| Setting               | Type    | Default   | Description                                                                                                                                  |
| --------------------- | ------- | --------- | -------------------------------------------------------------------------------------------------------------------------------------------- |
|  `Name`               | string  | Nano App  | Name of the application or API.                                                                                                              |
|  `Description`        | string  | null      | Description of the application or API.                                                                                                       |
|  `TermsOfServiceUrl`  | string  | null      | URL for terms of service. Must be a valid url.                                                                                               |
|  `Contact`            | string  | null      | Contact information for the API.                                                                                                             |
|  `Contact.Name`       | string  | null      | The identifying name of the contact person/organization.                                                                                     |
|  `Contact.Email`      | string  | null      | The email address of the contact person/organization. Must be a valid email address.                                                         |
|  `Contact.Url`        | string  | null      | The URL pointing to the contact information. MUST be in the format of a URL. Must be a valid url.                                            |
|  `License`            | string  | null      | License information for the API.                                                                                                             |
|  `License.Name`       | string  | null      | The license name used for the API.                                                                                                           |
|  `License.Identifier` | string  | null      | An SPDX license expression for the API. The identifier field is mutually exclusive of the url field.                                         |
|  `License.Url`        | string  | null      | The URL pointing to the contact information. MUST be in the format of a URL. Must be a valid url.                                            |
|  `CspNonce`           | string  | null      | Optional Content Security Policy nonce. See [CSP Nonce](#csp-nonce).                                                                         |
|  `HideDefaultVersion` | bool    | true      | Hide default API version (`App:Version`). Default version routes will be hidden in swagger, only the default non-versioned routes will show. |

```json
"App": {
  "Documentation": {
    "Name": "Application",
    "Description": null,
    "TermsOfServiceUrl": null,
    "Contact": {
      "Name": null,
      "Email": null,
      "Url": null
    },
    "License": {
      "Name": null,
      "Url": null
    },
    "CspNonce": null,
    "HideDefaultVersion": true
  }
}
```

#### CSP Nonce:
This value allows Swagger to function correctly when using Content Security Policy (CSP) nonce values for scripts and styles. 
A static nonce is configured for Swagger and any other frontends you may have. The `ingress-controller` in Kubenetes will then replace these static nonces 
with dynamically generated tokens before serving the pages to clients.  

Example `ingress.yaml` configuration:

```
kind: Ingress
metadata:
  annotations:
    nginx.ingress.kubernetes.io/configuration-snippet: | 
      more_set_headers "Content-Security-Policy: script-src 'self' 'nonce-${request_id}'; style-src 'self' 'nonce-${request_id}'";
      sub_filter_once off;
      sub_filter '%NONCE_TOKEN%' $request_id;
      sub_filter '(<body[^>]*>)(.*?)%NONCE_TOKEN%(.*?<\/body>)' '$1$2"$request_id"$3';
```

> 📖 Learn more about **[Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)**.  

Try it out yourself using the **[Api.Documentation](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Documentation)** example.  

## Health Checks
When health checks are enabled in the configuration, a `/healthz` endpoint is exposed, along with a web-based health monitor interface at `/healthz-ui`.  

A startup health check is performed to await the completion of all pending startup tasks before the application is reported as ready. 
As additional Nano providers and services are added to the application, they will automatically appear in the health checks and report their status, 
if configured with health-check enabled.  

Dependencies between services are represented as a tree of health checks. If any service in the chain fails, its failure status propagates according 
to the configured rules, affecting the overall health status of the application. This makes it easy to monitor the health of all components and dependencies 
in a consistent and centralized way.  

| Setting                              | Type   | Default | Description                                                                                                                                                               |
| ------------------------------------ | ------ | ------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
|  `EvaluationInterval`                | int    | 10      | Interval between health-check evaluations, in seconds.                                                                                                                    |
|  `FailureNotificationInterval`       | int    | 60      | Minimum interval between failure notifications, in seconds.                                                                                                               |
|  `MaximumHistoryEntriesPerEndpoint`  | int    | 50      | Maximum number of historical entries per endpoint stored in the UI database.                                                                                              |
|  `WebHooks`                          | array  | []      | Configured web-hooks triggered on health-check events. ⚠️ Normally, webhooks aren’t needed; in the cloud, `/healthz` is polled and monitoring uses more robust alerting.  |
|  `WebHooks.Name`                     | string | null    | Required. Name of the web-hook.                                                                                                                                           |
|  `WebHooks.Url`                      | string | null    | Required. URL to which the web-hook will send requests.                                                                                                                   |
|  `WebHooks.Payload`                  | string | null    | Optional. Payload to include in the web-hook request.                                                                                                                     |

```json
"App": {
  "HealthCheck": {
    "EvaluationInterval": 10,
    "FailureNotificationInterval": 60,
    "MaximumHistoryEntriesPerEndpoint": 50,
    "WebHooks": [
      {
        "Name": null,
        "Url": null,
        "Payload": null
      }
    ]
  }
}
```

It is also possible to add health checks for custom services. Simply register the health check during application startup in `ConfigureServices(...)`, and it will 
run alongside the built-in health checks provided by Nano.  

> 📖 Learn more about **[AspNetCore.Diagnostics.HealthChecks](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks)**.  

Try it out yourself using the **[Api.HealthChecks](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.HealthChecks)** example.  

## Virus Scan
Nano provides built-in virus scanning through a connected `ClamAV` service.  

When configured, all uploaded files are automatically processed through the ClamAV virus scan middleware before they are saved or processed further. 
If any uploaded file is found to contain a virus, the request is rejected and a `500 Internal Server Error` is returned. The response includes a message indicating 
the name of the virus detected and which file(s) triggered the scan.  

This feature ensures that all file uploads are automatically checked for malware, providing an extra layer of security for your application. By integrating ClamAV scanning 
into the middleware pipeline, Nano helps enforce security best practices and prevents potentially harmful files from entering your system.

> ⚠️ `ClamAV` has no authentication, so only run it internally in Kubernetes.

| Setting                         | Type   | Default   | Description                                                                                        |
| ------------------------------- | ------ | --------- | -------------------------------------------------------------------------------------------------- |
|  `Host`                         | string | clamav    | Hostname of the virus scanning service.                                                            |
|  `Port`                         | int    | 3310      | Port of the virus scanning service.                                                                |
|  `HealthCheck`                  | object | null      | Health check configuration for the virus scanning service.                                         |
|  `HealthCheck.UnhealthyStatus`  | enum   | Unhealthy | Gets or sets the health status level to report when a monitored service is detected as unhealthy.  |

```json
"App": {
  "VirusScan": {
    "Host": "clamav",
    "Port": 3310,
    "HealthCheck": {
      "UnhealthyStatus": "Unhealthy"
    }
  }
}
```

> 📖 Learn more about **[Request Virus Scan](https://github.com/vivet/Vivet.AspNetCore/tree/master/Vivet.AspNetCore.RequestVirusScan#vivetaspnetcorerequestvirusscan)**.  

Try it out yourself using the **[Api.VirusScan](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.VirusScan)** example.  

## Content Negotiation
Content negotiation allows clients to request a specific response format via the `Accept` header.  

Default is `application/json`, and currently the only supported format in Nano. Exceptions occur in cases where content negotiation is bypassed, 
such as when returning files. If `Accept` header is omitted from the request, Nano will assume `application/json` as well.  

No configuration or additional setup is required.  

Try it out yourself using the **[Api.ContentNegotiation](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.ContentNegotiation)** example.  

## Request Tracing
A `X-Request-Id` is generated by the first Nano instance encountered in the architecture and is propagated through all layers of the system. 
When using layered Nano APIs with [Nano Api Client](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App#api-client), the `X-Request-Id` is 
automatically passed along. It can also be set by the frontend, which is recommended to ensure that every layer uses the same identifier.  

In controllers deriving from `BaseController`, the `X-Request-Id` header value is accessible via the `RequestId` property.
The `X-Request-Id` is also added to the http response, so the consumer can see it.  

No configuration or additional setup is required.  

When [Logging](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging) is enabled, Nano adds the `X-Request-Id` to all logs for endpoint requests and responses, 
enabling request-level tracing and correlation.  

You can try this out using the **[Api.RequestTracing](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.RequestTracing)** example.

## Error Handling
This configuration section is required and will automatically be populated if omitted.

| Setting        | Type | Default | Description                                                                                                                            |
| -------------- | ---- | ------- | -------------------------------------------------------------------------------------------------------------------------------------- |
| `ExposeErrors` | bool | `false` | Expose detailed error information (`500 Internal Server Errors`). ⚠️ It's not recommended to enable this for `Production` environment  |

```json
"App": {
  "ErrorHandlong": {
    "ExposeErrors": false
  }
}
```

Nano includes a centralized error handling middleware that catches all unhandled exceptions, thrown anywhere in the application, and converts them into 
consistent HTTP error responses with appropriate status codes. All error responses are written using `ProblemDetails`, in accordance 
with [https://datatracker.ietf.org/doc/html/rfc7807](https://datatracker.ietf.org/doc/html/rfc7807).

Additionally, when [Logging](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Logging) is registered with the application, 
the error is logged using the configured provider.  

Nano provides built-in mappings between common exception types and HTTP error responses.

| Exception                    | Status Code                   | Description                                                                                                                             |
| ---------------------------- | ----------------------------- | --------------------------------------------------------------------------------------------------------------------------------------- |
| `IdentityException`          | `400` Bad Request             | Thrown when identity-related operations fail. Sets `IsTranslated=true`                                                                  |
| `UnauthorizedException`      | `401` Unauthorized            | Thrown when authentication fails.                                                                                                       |
| `PermissionDeniedException`  | `403` Forbidden               | Thrown when access to a resource is denied.                                                                                             |
| `OperationCanceledException` | `408` Request Timeout         | Thrown when an operation is cancelled by the client or server.                                                                          |
| `VirusScanException`         | `422` Unprocessable Entity    | Thrown when a virus is detected in one or more uploaded files. Sets `IsTranslated=true`                                                 |
| `AggregateException`         | `500` Internal Server Error   | Thrown anywhere.                                                                                                                        |
| `Exception`                  | `500` Internal Server Error   | Thrown anywhere.                                                                                                                        |
| `ProblemDetailsException`    | `Any`                         | Used to throw a fully defined `ProblemDetails` directly.                                                                                |
| `BadRequestException`        | `400` Bad Request             | Thrown for bad request errors. Can be `IsCoded`, exposing a machine-readable error code, or `IsTranslated` exposing a server-translated message. Always appears in `ProblemDetails.Detail`.  |

The exceptions above may be thrown anywhere in the application, and the Nano error handling middleware will automatically construct the appropriate `ProblemDetails` response.

Nano supports any HTTP status code as long as `ProblemDetails` is used. This also enables proper error propagation when using Nano in a layered architecture 
with the [Nano Api Client](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App#api-client). When returning custom error responses directly from controllers, 
always return `ProblemDetails` or no response body. Returning custom objects for error responses will not work in a layered Nano architecture, 
as the API client can only propagate `ProblemDetails` and will otherwise fall back to a generic `500 Internal Server Error`.

Try it out yourself using the **[Api.ErrorHandling](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.ErrorHandling)** example.  

## Static Files
Static Files are served directly by the web host without passing through the endpoint pipeline. Common static assets such as 
CSS, JavaScript, images, and fonts are supported out of the box.  

Static files must always be placed in the `wwwroot` folder.  

This is enabled by default and requires no additional configuration.  

Try it out yourself using the **[Api.StaticFiles](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.StaticFiles)** example.  

## Authentication
In Nano, authentication can be either persistent, using an identity store to manage user data, or transient, relying on external providers without storing identity information.  

When your application needs to manage usernames, passwords, roles, permissions, or other persistent user data, you should configure 
**[Data Identity](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data#identity)**. This enables Nano to store and maintain user credentials and claims in a 
centralized identity store. Roles and claims can then be automatically loaded for each user, simplifying access control and authorization across your application.

Nano also supports transient authentication and must be assigned each time a user logs in. Users authenticate through external providers, and JWT tokens are generated for 
use in subsequent requests. Roles and claims are transient, meaning they must be assigned each time a user logs in. To access all base controller actions in Nano, 
the `administrator` role must be included. Otherwise, roles can be assigned more selectively from Nano’s default roles to provide finer-grained access control for 
transient users.  

| Name          | Description                          |
| ------------- | ------------------------------------ |
| reader        | Authorized to read.                  | 
| writer        | Authorized to read and write.        | 
| creator       | Authorized to create.                | 
| editor        | Authorized to update.                | 
| deleter       | Authorized to create.                | 
| identity      | Authorized to use identity actions.  | 
| Administrator | Full access to everything.           | 

Transient roles and claims may also be added when using persistent authentication, and its often preferred for data than might change. Consider you might fetch a user’s 
legal name at login and add it as a transient claim, rather than storing it permanently in the identity system. This approach ensures that certain information is always 
current without updating persistent claims.

Nano provides a concrete `AuthController`, which exposes endpoints based on your authentication configuration. Learn more about [Authentication Controllers](#controllers). 
If you prefer implementing you own authentication controller, you can use the interfaces `IAuthRootRepository`, `IAuthTransientRepository` or `IIdentityAuthRepository`, 
to derive authentication functionality.  

The following configuration is available for authentication.  

| Setting                   | Type     | Default   | Description                                                                                         |
| ------------------------- | -------- | --------- | --------------------------------------------------------------------------------------------------- |
|  `HideAuthController`     | bool     | false     | Controls whether the `AuthController` should be visible when authentication is enabled.             |
|  `Jwt`                    | object   | null      | Optional JWT authentication configuration.                                                          |
|  `Jwt.Issuer`             | string   | null      | Required. JWT issuer.                                                                               |
|  `Jwt.Audience`           | string   | null      | Required. JWT audience.                                                                             |
|  `Jwt.PublicKey`          | string   | null      | Required. Base64-encoded public key.                                                                |
|  `Jwt.PrivateKey`         | string   | null      | Optional Base64-encoded private key. _This is required if the application must create JWT tokens_.  |
|  `Jwt.Expiration`         | TimeSpan | 00:60:00  | Expiration for the access token.                                                                    |
|  `Jwt.RefreshExpiration`  | TimeSpan | 72:00:00  | Expiration for the refresh token.                                                                   |
|  `Jwt.RootLogin`          | object   | null      | Optional root login options.                                                                        |
|  `Jwt.ExternalLogins`     | object   | null      | Optional external login options.                                                                    |

```json
"App": {
  "Authentication": {
    "HideAuthController": false,
    "Jwt": {
      "Issuer": null,
      "Audience": null,
      "PublicKey": null,
      "PrivateKey": null,
      "Expiration": 00:60:00,
      "RefreshExpiration": 72:00:00
    }
  }
}
```

Nano supports a statically configured login called `RootLogin`. It is primarily intended for use in `Development` environments when testing services in isolation, but where 
the application still requires an authenticated user. Another common scenario is when console applications need to authenticate through the Nano API client but do not have 
a specific user account available for login. This login type is transient and does not rely on an identity store. The `RootLogin` configuration is defined as follows.  

| Setting       | Type     | Default   | Description                                 |
| ------------- | -------- | --------- | ------------------------------------------- |
|  `Username`   | string   | null      | The username used for root authentication.  |
|  `Password`   | string   | null      | The password used for root authentication.  |

The supported `ExternalLogins` in Nano are Facebook, Google, and Microsoft. Nano allows you to use external logins either alongside 
**[Data Identity](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data#identity)** or as transient logins without an identity store.  

The configuration is defined as follows.  

**Facebook**

| Setting                    | Type   | Default  | Description                         |
| -------------------------- | ------ | -------- | ----------------------------------- |
|  `Facebook.AppId`          | string | null     | Required. Facebook App Id.          |
|  `Facebook.AppSecret`      | string | null     | Required. Facebook App Secret.      |
|  `Facebook.Scopes`         | array  | []       | Required. OAuth Scopes.             |

**Google**

| Setting                    | Type   | Default  | Description                         |
| -------------------------- | ------ | -------- | ----------------------------------- |
|  `Google.ClientId`         | string | null     | Required. Google Client Id.         |
|  `Google.ClientSecret`     | string | null     | Required. Google Client Secret.     |
|  `Google.Scopes`           | array  | []       | OAuth Scopes.                       |

**Microsoft**

| Setting                    | Type   | Default  | Description                         |
| -------------------------- | ------ | -------- | ----------------------------------- |
|  `Microsoft.TenantId`      | string | null     | Required. Microsoft Tenant Id.      |
|  `Microsoft.ClientId`      | string | null     | Required. Microsoft Client Id.      |
|  `Microsoft.ClientSecret`  | string | null     | Required. Microsoft Client Secret.  |
|  `Microsoft.Scopes`        | array  | []       | OAuth Scopes.                       |

```json
"App": {
  "Authentication": {
    "Jwt": {
      "ExternalLogins": {
        "Google": {
          "ClientId": null,
          "ClientSecret": null,
          "Scopes": [ ]
        },
        "Facebook": {
            "AppId": null,
            "AppSecret": null,
            "Scopes": [ ]
        },
        "Microsoft": {
            "TenantId": null,
            "ClientId": null,
            "ClientSecret": null,
            "Scopes": [ ]
        }
    }
  }
}
```

> ⚠️ The external provider application must be configured with at least the following scopes: `id`, `email`, and `username`.

The `AuthController` exposes endpoints for all configured authentication methods. Each method allows specifying an `AppId`, a unique string that identifies the application 
or platform the user is authenticating from. This enables logins to be managed independently per application. If no `AppId` is provided, it defaults to `Default`. All 
authentication methods also return a consistent `AccessToken` in the response, with default values as shown below.  

```json
{
  "AppId": null,
  "UserId": null,
  "Token": null,
  "ExpireAt": "2026-03-06T12:00:00Z",
  "IsExpired": false,
  "RefreshToken": {
    "Token": null,
    "ExpireAt": "2026-03-07T12:00:00Z",
    "IsExpired": false
  }
}
```

The token contains the following default claims. And Nano contains `HttpContext` extension methods for easy retrieval of the claims values.

| Name      | HttpContext Extension | Description                      |
| --------- | --------------------- | -------------------------------- |
| AppId     | `GetJwtAppId()`       | The app id. Default: "Default".  |
| Id        | `GetJwtUserId()`      | The user id.                     |
| Name      | `GetJwtUserName()`    | The username.                    |
| Email     | `GetJwtUserEmail()`   | The user's email address.        |

All the `HttpContext` extension methods returns null, if not authenticated.  

The refresh token may be used to extend the access-token when expired.  

Nano also supports authentication using an API key, provided in the `X-Api-Key` header. This requires 
**[Data Identity](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data#identity)** to be configured for API keys. When an API key is included in the HTTP header, 
Nano authenticates the request using `ApiKeyAuthenticationHandler<TIdentity>`.

> ⚠️ In a layered architecture, when using API key authentication, the Kubernetes ingress must handle the authentication step before requests reach your services. This means the 
ingress needs to validate the API key by calling an authentication endpoint and exchanging it for a JWT token that can be forwarded to your backend service. Without this, services 
behind the ingress won't automatically authenticate API-key requests.  

For example, with NGINX ingress, you can use the `nginx.ingress.kubernetes.io/auth-url` annotation to forward requests to your auth endpoint, like this.  

```yaml
metadata:
  annotations:
    nginx.ingress.kubernetes.io/auth-url: "http://{service-name}/auth/api-key"
    nginx.ingress.kubernetes.io/auth-method: "GET"
    nginx.ingress.kubernetes.io/auth-response-headers: "Authorization"
```

Try out transient authentication yourself using one of these examples.  

* **[Api.Authentication.RootLogin](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Authentication.RootLogin)** 
* **[Api.Authentication.External.Facebook](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Authentication.External.Facebook)** 
* **[Api.Authentication.External.Google](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Authentication.External.Google)** 
* **[Api.Authentication.External.Microsoft](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Authentication.External.Microsoft)** 

or examples with identity store configured.  

* **[Api.Data.MySql.Identity.Authentication.Jwt](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.MySql.Identity.Authentication.Jwt)** 
* **[Api.Data.MySql.Identity.Authentication.External.Facebook](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.MySql.Identity.Authentication.External.Facebook)** 
* **[Api.Data.MySql.Identity.Authentication.External.Google](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.MySql.Identity.Authentication.External.Google)** 
* **[Api.Data.MySql.Identity.Authentication.External.Microsoft](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.MySql.Identity.Authentication.External.Microsoft)** 

## Authorization
Nano supports authorization using either a JWT token or an API key. JWT tokens are provided in the `Authorization` header, while API keys are provided in 
the `X-Api-Key` header. If both headers are present in a request, the JWT token takes precedence. If no authentication has been configured for an application, 
Nano allows _anonymous_ access by default. You can change this behavior by registering a custom authentication handler during application startup 
in `ConfigureServices(...)`.

When building layered Nano applications, the `Authorization` header is automatically propagated between applications when using the built-in  
[Nano Api Client](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App#api-client). This ensures that the authenticated context is preserved 
across application boundaries.

In Nano, _claims_ are primarily used for carrying user information, while _roles_ are used for authorization. Nano can be extended to support any kind of custom 
authorization strategies. For example, you can override the `[Authorize]` attribute used by base controllers in Nano and register custom authorization policies 
during application startup. By default, however, Nano base controllers rely on role-based authorization. See [Controllers](#controllers) for details on which 
roles are required for specific base controllers and actions.

## Api Clients
Nano API clients provide a structured way to communicate with other Nano API applications. They are designed to simplify service-to-service communication while maintaining 
consistent error handling, logging, and resilience across applications.

API clients make it easy to compose Nano APIs into a layered architecture, where higher-level services can call lower-level services without leaking transport or protocol concerns 
into the business logic. Responses and errors are propagated in a predictable way, enabling reliable chaining of services.

When used together with Nano’s **[Error Handling](#error-handling)** and `ProblemDetails` support, API clients ensure that errors can flow through multiple layers without being lost 
or transformed into generic failures.

> 📖 Learn more **[Nano Api Clients](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App#api-clients)**

## Controllers
Nano provides several base controller classes that concrete API controllers are expected to inherit from.

All controllers must inherit from `BaseController`, either directly or indirectly. This base class establishes the fundamental API behavior in Nano, including routing, versioning, 
model validation, authorization, and shared response handling. 

> ⚠️ Skipping this inheritance can lead to inconsistent behavior or parts of Nano not functioning as intended.

Nano controls the base route for all controllers and applies the following structure: `http(s)://{host}:{port}/{root}/{controller}`.  

This base route is defined by `BaseController` and must not be overridden. Concrete controllers should therefore not declare a `[Route]` attribute at the 
controller level, as doing so may interfere with Nano’s routing configuration. Action methods, however, must define their own route templates. The `[Route]` (or HTTP verb) 
attribute on actions can specify any route segment as needed by the consumer. This ensures consistent API structure across the application while still allowing 
flexibility at the action level.

The `BaseController` also exposes a `RequestId` property, allowing easy access to the unique identifier of the current request. This is particularly useful for 
logging and correlation. See **[Request Tracing](#request-tracing)**.

In addition to `BaseController`, Nano includes specialized base entity controllers designed for working with Nano data entities. These controllers expose standard 
CRUD operations depending on their intended responsibility, as shown below.

| Controller                                                      | Get | Query | Create | Update | Delete |
| --------------------------------------------------------------- | --- | ----- | ------ | ------ | ------ |
| `BaseEntityController`                                          | ❌  | ❌   | ❌     | ❌     | ❌     |
| `BaseEntityController<TEntity, TCriteria>`                      | ✔   | ✔   | ✔      | ✔     | ✔      |
| `BaseEntityReadOnlyController<TEntity, TCriteria>`              | ✔   | ✔   | ❌     | ❌     | ❌     |
| `BaseEntityCreatableController<TEntity, TCriteria>`             | ✔   | ✔   | ✔      | ❌     | ❌     |
| `BaseEntityCreatableAndUpdatableController<TEntity, TCriteria>` | ✔   | ✔   | ✔      | ✔     | ❌     |
| `BaseEntityUpdatableController<TEntity, TCriteria>`             | ✔   | ✔   | ❌     | ✔      | ❌     |
| `BaseEntityDeletableController<TEntity, TCriteria>`             | ✔   | ✔   | ❌     | ❌     | ✔      |
| `BaseEntityViewController<TEntity, TCriteria>`                  | ❌  | ✔   | ❌      | ❌    | ❌     |

Derive you concrete entity controllers classes from one of these base classes, and choose the most restrictive controller that satisfies the domain requirements 
to keep the API surface minimal and explicit.  

When exposing entity models mapped from SQL views, use `BaseEntityViewController` base class.  

> ⚠️ Entity controllers require **[Nano.Data](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data)** to be configured for the application.

Each concrete implementation of an entity controller must specify two generic parameters. First, the entity model, which defines the database table and its properties 
that the controller will work with. This model comes from **[Nano Data Models](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data#data-models)** and uses 
Entity Framework. 

```csharp
public class MyEntity : BaseEntity
{
    // Properties
}
```

The entity model used by a controller does not need to match the models used for creation, updates, or other operations. For example, it is perfectly valid to have 
a `BaseEntity` mapped to a `BaseEntityReadOnlyController`. This allows the entity to be fully modified internally within the application, while exposing only read-only 
actions through the controller.  

> ⚠️ By convention, concrete entity controllers **must** be named pluralized relative to the entity model, e.g. `MyEntity` to `MyEntitysController`.   

Second, the query criteria model defines how consumers can query the entity. Its properties determine the search criteria that can be applied to the underlying entity model, 
and entity controllers also support ordering and pagination. A query criteria model is created by deriving from `BaseQueryCriteria`. For each property of the corresponding 
entity that should be queryable, add a property to the derived criteria class. You must also override the base method `GetExpressions()` and implement the logic to generate the 
necessary expressions for each property. The `BaseQueryCriteria` class already includes properties and implementation for querying the start and end dates 
of `BaseEntity.CreatedAt`, so don't forget to call `base.GetExpressions()`.

```csharp 
public class MyEntityQueryCriteria : BaseQueryCriteria
{
    public virtual string? Name { get; set; }

    public override IList<CriteriaExpression> GetExpressions()
    {
        var expressions = base.GetExpressions();

        var expression = expressions.FirstOrDefault() ?? new CriteriaExpression();

        if (!string.IsNullOrEmpty(this.Name))
        {
            expression
                .StartsWith("Name", this.Name);
        }

        expressions
            .Add(expression);

        return expressions;
    }
}
```

Converting query models into linq expressions for Entity Framework is based on the **[DynamicExpression](https://github.com/vivet/DynamicExpression)** library.  

Nano entity controllers have two required constructor dependencies and one optional dependency.  

| Dependency    | Purpose                                                                                                                                                                                            |
| ------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `ILogger`     | Required. Provides logging capabilities for the controller.                                                                                                                                        |
| `IRepository` | Required. Provides methods to get, add, update, delete, and query entity data.                                                                                                                     |
| `IEventing`   | Optional. If eventing is configured, this allows the controller to publish events from its actions. See **[Nano.Eventing](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Eventing)**.  |

A controller using the default `Guid` as `TIdentity` would look like this.  

```csharp
public class MyEntitysController(ILogger<MyEntitysController> logger,IRepository repository,IEventing? eventing) 
    : BaseEntityController<MyEntity, MyEntityQueryCriteria>(logger, repository, eventing);
```

If you have specified a `TIdentity` type when registering **[Nano.Data](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data)**, you must also specify the same type 
when deriving your concrete controllers from the Nano base controllers. If `TIdentity` is a `string`, your controller would look like this.  

```csharp
public class MyEntitysController(ILogger<MyEntitysController> logger, IRepository repository, IEventing? eventing) 
    : BaseEntityController<MyEntity, string, MyEntityQueryCriteria>(logger, repository, eventing);
```

When everything is configured and registered, the following endpoints becomes available for each entity controller.

| Endpoint                           | Method        | Paramters                        | Role    | Description                                                                  |
| ---------------------------------- | ------------- | -------------------------------- | ------- | ---------------------------------------------------------------------------- |
| `/api/{entity}s/create`            | POST          | entity                           | creator | Creates a single model instance.                                             |
| `/api/{entity}s/create/get`        | POST          | entity                           | creator | Creates or retrieves a single model instance.                                |
| `/api/{entity}s/create/reload`     | POST          | entity                           | creator | Creates a single model instance and retrieves it with included navigations.  |
| `/api/{entity}s/create/edit`       | POST          | entity                           | creator | Creates or edits (upsert) a single model instance.                           |
| `/api/{entity}s/create/many`       | POST          | entities                         | creator | Creates multiple model instances.                                            |
| `/api/{entity}s/create/many/bulk`  | POST          | entities                         | creator | Creates multiple model instances in bulk.                                    |
| `/api/{entity}s/{id}/details`      | GET           | id, includeDepth                 | reader  | Gets a single entity by its identifier.                                      |
| `/api/{entity}s/details/many`      | GET, POST     | ids, includeDepth                | reader  | Gets multiple entities by their identifiers.                                 |
| `/api/{entity}s/index`             | GET, POST     | query, includeDepth              | reader  | Gets all entities matching the specified query.                              |
| `/api/{entity}s/query`             | GET, POST     | query, criteria, includeDepth    | reader  | Queries entities matching the specified criteria.                            |
| `/api/{entity}s/query/first`       | GET, POST     | query, criteria, includeDepth    | reader  | Retrieves the first entity matching the specified criteria.                  |
| `/api/{entity}s/query/count`       | GET, POST     | criteria, includeDepth           | reader  | Gets the total count of entities matching the specified criteria.            |
| `/api/{entity}s/edit`              | PUT, POST     | entity                           | editor  | Edits a single model instance.                                               |
| `/api/{entity}s/edit/reload`       | PUT, POST     | entity                           | editor  | Edits a single model instance and retrieves it with included navigations.    |
| `/api/{entity}s/edit/many`         | PUT, POST     | entities                         | editor  | Edits multiple model instances.                                              |
| `/api/{entity}s/edit/many/bulk`    | PUT, POST     | entities                         | editor  | Edits multiple model instances in bulk.                                      |
| `/api/{entity}s/edit/query`        | PUT, POST     | update-query, criteria           | editor  | Edits entities that match the specified criteria.                            |
| `/api/{entity}s/edit/query/bulk`   | PUT, POST     | update-query, criteria           | editor  | Edits entities that match the specified criteria in bulk.                    |
| `/api/{entity}s/{id}/delete`       | POST, DELETE  | id                               | deleter | Deletes a single entity by its identifier.                                   |
| `/api/{entity}s/delete/many`       | POST, DELETE  | ids                              | deleter | Deletes multiple entities by their identifiers.                              |
| `/api/{entity}s/delete/many/bulk`  | POST, DELETE  | ids                              | deleter | Deletes multiple entities by their identifiers in bulk.                      |
| `/api/{entity}s/delete/query`      | POST, DELETE  | criteria                         | deleter | Deletes entities matching the specified criteria.                            |
| `/api/{entity}s/delete/query/bulk` | POST, DELETE  | criteria                         | deleter | Deletes entities matching the specified criteria in bulk.                    |

> ⚠️ Do not set `includeDepth` higher than the configured include depth. **[Response Serialization](#response-serialization)** will only consider the configured value.

Try it yourself using one of the **[Api.Data Lessons](https://github.com/Nano-Core/Nano.Lessons)**, such as **[Api.Data.MySql](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.MySql)**, 
or any of the other data provider examples.

When **[Data Identity](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data#identity)** is enabled, Nano provides a specialized base controller for managing 
entity identities. The `BaseIdentityController<TEntity, TCriteria>` offers a rich set of methods for creating, updating, and managing user identities within your application. 
To use it, derive a concrete implementation of this controller to expose identity-related actions for your application, using a user entity model derived from `BaseEntityUser` or 
`BaseEntityUser<TIdentity>`. It behaves similarly to other entity controllers but includes additional actions tailored for identity management, such as handling usernames, passwords, 
emails, phone numbers, external logins, claims, roles, and API keys.  

The following endpoints are available in the `BaseIdentityController<TEntity, TCriteria>` for managing user identities. Not all identity features might be configured. Nano only 
exposes endpoints that match the current configuration; any features not configured will not be registered or available in the controller.

| Endpoint                                     | Method        | Role          | Description                                                                 |
|--------------------------------------------- |-------------- | ------------- | --------------------------------------------------------------------------- |
| `/{entity}s/password/options`                | GET           | Anonymous     | Retrieves the configured password options.                                  |
| `/{entity}s/email/is-taken`                  | GET           | Anonymous     | Determines whether an email address is already in use.                      |
| `/{entity}s/phone/is-taken`                  | GET           | Anonymous     | Determines whether a phone number is already in use.                        |
| `/{entity}s/signup`                          | POST          | Anonymous     | Registers a new user.                                                       |
| `/{entity}s/signup/external/direct`          | POST          | Anonymous     | Registers a new user using externally provided login data.                  |
| `/{entity}s/signup/external/facebook`        | POST          | Anonymous     | Registers a new user using an external Facebook login provider.             |
| `/{entity}s/signup/external/google`          | POST          | Anonymous     | Registers a new user using an external Google login provider.               |
| `/{entity}s/signup/external/microsoft`       | POST          | Anonymous     | Registers a new user using an external Microsoft login provider.            |
| `/{entity}s/username/set`                    | POST          | Anonymous     | Sets the username of a user.                                                |
| `/{entity}s/password/set`                    | POST          | identity      | Assigns a password to a user that does not already have one.                |
| `/{entity}s/password/change`                 | POST          | identity      | Changes the password of an existing user.                                   |
| `/{entity}s/password/reset`                  | POST          | Anonymous     | Resets the password of a user using a reset token.                          |
| `/{entity}s/password/reset/token`            | POST          | Anonymous     | Generates a password reset token.                                           |
| `/{entity}s/email/change`                    | POST          | identity      | Changes the email address of a user.                                        |
| `/{entity}s/email/change/token`              | POST          | identity      | Generates an email change token.                                            |
| `/{entity}s/email/confirm`                   | POST          | identity      | Confirms the email address of a user.                                       |
| `/{entity}s/email/confirm/token`             | POST          | identity      | Generates an email confirmation token.                                      |
| `/{entity}s/phone/change`                    | POST          | identity      | Changes the phone number of a user.                                         |
| `/{entity}s/phone/change/token`              | POST          | identity      | Generates a phone number change token.                                      |
| `/{entity}s/phone/confirm`                   | POST          | identity      | Confirms the phone number of a user.                                        |
| `/{entity}s/phone/confirm/token`             | POST          | identity      | Generates a phone number confirmation token.                                |
| `/{entity}s/custom-purpose/confirm`          | POST          | identity      | Generates a custom-purpose token for a user.                                |
| `/{entity}s/custom-purpose/confirm/token`    | POST          | identity      | Confirms a previously generated custom-purpose token.                       |
| `/{entity}s/{id}/activate`                   | POST          | identity      | Activates the user with the specified identifier.                           |
| `/{entity}s/{id}/deactivate`                 | POST / DELETE | identity      | Deactivates the user with the specified identifier.                         |
| `/{entity}s/{id}/delete`                     | POST / DELETE | deleter       | Deletes the user with the specified identifier.                             |
| `/{entity}s/delete/many`                     | POST / DELETE | deleter       | Deletes multiple users with the specified identifiers.                      |
| `/{entity}s/{userId}/external-logins`        | GET           | identity      | Retrieves the external login providers associated with a user.              |
| `/{entity}s/external-logins/add/facebook`    | POST          | identity      | Adds a Facebook external login to a user account.                           |
| `/{entity}s/external-logins/add/google`      | POST          | identity      | Adds a Google external login to a user account.                             |
| `/{entity}s/external-logins/add/microsoft`   | POST          | identity      | Adds a Microsoft external login to a user account.                          |
| `/{entity}s/external-logins/remove`          | POST / DELETE | identity      | Removes an external login from a user account.                              |
| `/{entity}s/{userId}/refresh-tokens`         | GET           | identity      | Retrieves all refresh tokens associated with a specific user.               |
| `/{entity}s/{userId}/refresh-tokens/active`  | GET           | identity      | Retrieves all active refresh tokens for a specific user.                    |
| `/{entity}s/refresh-tokens/{refreshTokenId}` | DELETE        | identity      | Deletes a specific refresh token by its identifier.                         |
| `/{entity}s/{userId}/api-keys`               | GET           | identity      | Retrieves all API keys associated with a specific user.                     |
| `/{entity}s/api-keys/create`                 | POST          | identity      | Creates a new API key for a user.                                           |
| `/{entity}s/api-keys/edit`                   | PUT / POST    | identity      | Edits an existing API key.                                                  |
| `/{entity}s/api-keys/{apiKeyId}/revoke`      | DELETE        | identity      | Revokes a specific API key.                                                 |
| `/{entity}s/{userId}/claims`                 | GET           | identity      | Retrieves all claims assigned to a specific user.                           |
| `/{entity}s/claims/assign`                   | POST          | identity      | Assigns a new claim to a user.                                              |
| `/{entity}s/claims/replace`                  | PUT           | identity      | Replaces an existing claim of a user.                                       |
| `/{entity}s/claims/assign-or-replace`        | PUT           | identity      | Assigns or replaces a claim of a user.                                      |
| `/{entity}s/claims/remove`                   | POST / DELETE | identity      | Removes a claim from a user.                                                |
| `/{entity}s/roles`                           | GET           | administrator | Retrieves all roles in the system.                                          |
| `/{entity}s/roles/create`                    | POST          | administrator | Creates a new role.                                                         |
| `/{entity}s/roles/delete`                    | POST / DELETE | administrator | Deletes a role from the system.                                             |
| `/{entity}s/{userId}/roles`                  | GET           | identity      | Retrieves all roles assigned to a specific user.                            |
| `/{entity}s/roles/user/assign`               | POST          | identity      | Assigns a role to a user.                                                   |
| `/{entity}s/roles/user/remove`               | POST / DELETE | identity      | Removes a role from a user.                                                 |
| `/{entity}s/roles/{roleId}/claims`           | GET           | administrator | Retrieves all claims associated with a role.                                |
| `/{entity}s/roles/claims/assign`             | POST          | administrator | Assigns a claim to a role.                                                  |
| `/{entity}s/roles/claims/replace`            | PUT           | administrator | Replaces a claim of a role.                                                 |
| `/{entity}s/roles/claims/assign-or-replace`  | PUT           | administrator | Assigns or replaces a claim of a role.                                      |
| `/{entity}s/roles/claims/remove`             | POST / DELETE | administrator | Removes a claim from a role.                                                |

> 📖 Learn more about **[Data Identity](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data#identity)**.

Try it yourself using the **[Api.Data.Identity](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.Identity)** example.  

Another specialized base entity controller is provided for audit data. You can expose audit data by deriving from `BaseAuditController` or `BaseAuditController<TIdentity>`. These 
controllers derive from `BaseEntityReadOnlyController` and automatically provide read-only actions for querying audit records.  

> ⚠️ The `BaseAuditController` requires the _adminstrator_ role assigned.

> 📖 Learn more about **[Data Audit](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data#audit)**.

Try it yourself using the **[Api.Data.Audit](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.Data.Audit)**, or any of the other data provider examples.

Nano provides the `BaseAuthController` and `BaseAuthController<TIdentity>` for authentication-related operations. A concrete implementation, `AuthController`, is already included 
with Nano. Unless you have specific requirements, there is usually no need to override it. The `AuthController` is only exposed if authentication has been configured and if the 
configuration option `HideAuthController` is not set to `true`.  

The following endpoints are available in the `BaseAuthController` for managing authentication. Nano only exposes endpoints that match the current configuration; any features that 
are not configured will not be registered or available in the controller.  

| Endpoint                                  | Method | Role      | Description                                                                          |
| ----------------------------------------- | ------ | --------- | ------------------------------------------------------------------------------------ |
| /auth/login                               | POST   | Anonymous | Authenticates a user and returns an access token (JWT).                              |
| /auth/login-root                          | POST   | Anonymous | Authenticates the root user from configuration and returns an access token.          |
| /auth/login-external-direct               | POST   | Anonymous | Signs in a user via direct external authentication data.                             |
| /auth/login-external-direct-transient     | POST   | Anonymous | Signs in a transient user via direct external authentication data.                   |
| /auth/login-external-facebook             | POST   | Anonymous | Signs in a user via external Facebook authentication.                                |
| /auth/login-external-facebook-transient   | POST   | Anonymous | Signs in a transient user via external Facebook authentication.                      |
| /auth/login-external-google               | POST   | Anonymous | Signs in a user via external Google authentication.                                  |
| /auth/login-external-google-transient     | POST   | Anonymous | Signs in a transient user via external Google authentication.                        |
| /auth/login-external-microsoft            | POST   | Anonymous | Signs in a user via external Microsoft authentication (auth-code flow).              |
| /auth/login-external-microsoft-transient  | POST   | Anonymous | Signs in a transient user via external Microsoft authentication (auth-code flow).    |
| /auth/login-refresh                       | POST   | Anonymous | Refreshes an existing access token.                                                  |
| /auth/logout                              | POST   | Anonymous | Logs out the current user and clears external authentication cookies.                |
| /auth/external-schemes                    | GET    | Anonymous | Retrieves all configured external authentication schemes (e.g., Google, Facebook).   |
| /auth/external-facebook-data              | POST   | Anonymous | Retrieves external login data from Facebook authentication provider.                 |
| /auth/external-google-data                | POST   | Anonymous | Retrieves external login data from Google authentication provider.                   |
| /auth/external-microsoft-data             | POST   | Anonymous | Retrieves external login data from Microsoft authentication provider.                |

> 📖 Learn more about **[Authentication](#authentication)**.

## Request Validation
When deriving a controller from `BaseController`, model validation is automatically enabled. Validation is based on the attributes applied to model properties. If 
a model fails validation, the controller automatically returns a 400 Bad Request with the validation errors.  

Beyond this automatic behavior, validation works the same way as in standard ASP.NET Core controllers. For more details, see the 
official Microsoft documentation: **[Model Validation Documentation](https://docs.microsoft.com/en-us/aspnet/core/mvc/models/validation)**.  

Nano also provides a set of useful validation attributes that can be applied to entity models. These annotations simplify common validation tasks and help ensure 
consistency across your models.

| Annotation                         | Description                                                                                                                                                |
| ---------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `InternationalPhoneAttribute`      | Ensures a string contains a valid international phone number. Works with properties and action parameters.                                                |
| `RequiredOneOfAttribute`           | Ensures that at least one of the specified members, including the decorated member, has a non-null value. Works with properties, fields, and parameters. |
| `UrlAttribute`                     | Ensures a string contains a valid URL. Works with properties, fields, and parameters.                                                                     |
| `FileExtensionValidationAttribute` | Ensures uploaded files have allowed extensions.                                                                                                           |

## Request Multipart JSON
Nano supports scenarios where a controller needs to receive both files and JSON data in the same request. This is achieved using the `FromFormBody` attribute together 
with a custom model binder.  

To use this, create a controller action with parameters like the following.  

```csharp
public IActionResult UploadFile(IFormFile file, [FromFormBody] MyClass instance)
{
    // Your logic here
}
```

The ModelBinder deserializes the form field string (JSON) into the specified model type, and also validates properties and fields decorated with ValidationAttribute, 
populating the ModelState with any validation errors. This allows clients to send a `multipart/form-data` request containing both files and structured JSON data, 
while the controller receives fully bound and validated models.

> ⚠️ Make sure the JSON field name matches the name of the model parameter in your action.

Try it out yourself using the **[Api.MultipartJson](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.MultipartJson)** example.  

## Response Serialization
Nano uses `Newtonsoft.Json` for serialization and deserialization. It supports all built-in Nano types, types derived from Nano base types, 
and all `Geometry` types from `NetTopologySuite`.  

The serializer only serializes navigations that is of type `IEntity`, when they are annotated with `IncludeAttribute`. This is to avoid returning unwanted navigation 
references, that is automatically added if dependent navigations are loaded separately into the data context. Read more about 
**[Include Annotation](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.App#include-annotation)**. Responses that doesn't inherit from `IEntity` will be 
serialized normally.

> ⚠️ Serialization respects only the configured include depth. Loaded navigations within that depth may in rare cases be returned even if the request `includeDepth` is lower.

The serializer is case-insensitive.  

Besides that, the serializer is configured to handle various edge cases for robustness.  

## Start-Up Tasks
Nano supports start-up tasks that are executed before the application begins handling requests. These tasks are intended for work that must complete successfully 
during application initialization, such as warming caches, validating external dependencies, running migrations, or performing initial connectivity checks.  

When Health Checks are configured, Nano automatically exposes a built-in self start-up health check. This health check will report the application as ready 
only after all configured start-up tasks have completed successfully. Until then, the application is considered not ready to receive traffic. This makes start-up tasks 
especially useful in orchestrated environments, where readiness signals are required to control traffic flow and ensure the application 
is fully initialized before becoming available.

> ⚠️ When using health checks and startup tasks, configure the Kubernetes readiness probe correctly to prevent unwanted pod restarts.  
Keep startup tasks simple and fast to ensure smooth application startup

> 📖 Learn more about **[Nano Startup Tasks](https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App#startup-tasks)**.

Try it out yourself using the **[Api.StartupTasks](https://github.com/Nano-Core/Nano.Lessons/tree/master/Api.StartupTasks)** example.  
