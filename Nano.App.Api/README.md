# Nano.App.Api
[![Build and Deploy](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml/badge.svg)](https://github.com/Nano-Core/Nano.Library/actions/workflows/build-and-deploy.yml)
[![NuGet](https://img.shields.io/nuget/dt/Nano.App.Api.svg)](https://www.nuget.org/packages/Nano.App.Api/)
[![NuGet](https://img.shields.io/nuget/v/Nano.App.Api.svg)](https://www.nuget.org/packages/Nano.App.Api/)

> _Nano Api application._

ALL SECTIONS: Link to Microsoft, Lessions, Other official docs, e.g. Mozilla Observatory
## Nano Architectures
* Solo application
* micro-service orchestratration
REFER TO THESE IN README CONFIG FOR VARIOUS SETTINGS (we need readme's for them)


## Table of Contents
* [Summary](#summary)
* [Registration](#registration)
* [Configuration](#configuration)
  * [Hosting](#hosting)
    * [Http](#http)
    * [Https](#https)
    * [Multipart Limits](#multipart-limits)
  * [Http Policy Headers](#http-policy-headers)
    * [Content Type](#content-type) 
    * [Referrer Policy](#referrer-policy) 
    * [Frame Options](#frame-options)
    * [Xss Protection](#xss-protection)
    * [Content Security Policy)](#content-security-policy)
    * [Cors](#cors)
    * [Hsts](#hsts) 
    * [Robots](#robots)
    * [Forwarded Headers](#forwarded-headers)
  * [Response Cache](#response-cache)
  * [Response Compression](#response-cache)
  * [Session](#session)
  * [TimeZone](#timezone)
  * [Localization](#localization)
  * [Documentation](#documentation)
  * [Health Checks](#health-checks)
  * [Virus Scan](#virus-scan)
  * [Versioning](#versioning)
  * [Cookies](#cookies)

  * [Preflight](#preflight)
  * [Content Type Negotiation](#content-type-negotiation)
  * [Authentication](#authentication)
  * [Authorization](#authorization)
* [Controllers](#controllers)
* [Request Validation](#model-validation)
* [Serialization](#serialization)
* [Error Handling](#error-handling)
* [Start-Up Tasks](#start-up-tasks)

## Summary
The ```ApiApplication``` derives from ```BaseApplication```. As the names states, the implementation is for web based applications.  

Nano includes a few base controller classes, providing derived functionality for model validation, content-type negotiation, exception handling and more. 
Furthermore, deriving controllers inherit a rich set of action methods for adding, updating, deleting and querying the model implemented by the controller.  

Additionally, Nano includes the following concrete controllers.
* **```AuditController```** Read-only actions for exposing audit logging entries, if audit is enabled.    
* **```AuthController```** Login and logoff actions.  
* **```IdentityController```** Identity management actions.  

## Registration
```csharp
NanoApiApplication
    .ConfigureApp()
    .ConfigureServices(x =>
    {
        // Additional dependencies...
    })
    .Build()
    .Run();
```

## Configuration
The `App` section in the configuration defines behavior related to the api application

To read more about how Nano handles configuration in gerenal, and how to extend nano woth custom configuration sections. [Read More](Nano.App#configuration)

| Setting                    | Type       | Default    | Description                                                                           |
| -------------------------- | ---------- | ---------- | ------------------------------------------------------------------------------------- |
|  `Version`                 | string     | 1.0.0.0    | Application version identifier.                                                       |
|  `ShutdownTimeout`         | int        | 10         | Number of seconds to wait after a SIGTERM signal before shutting down.                |
|  `Hosting`                 | object     | default    | Hosting options. See [Hosting](#hosting)                                              |
|  `HttpPolicyHeaders`       | object     | default    | HTTP policy header options. See [Http Policy Headers](#http-policy-headers)           |
|  `ResponseCache`           | object     | null       | Response caching options. See [Response Cache](#response-cache)                       |
|  `ResponseCompression`     | object     | null       | Response compression options. See [Response Compression](#response-compression)       |
|  `Session`                 | object     | null       | Session management options. See [Session](#session)                                   |
|  `TimeZone`                | object     | null       | Timezone configuration options. See [TimeZone](#timezone)                             |
|  `Localization`            | object     | null       | Localization configuration options. See [Localization](#localization)                 |
|  `VirusScan`               | object     | null       | Virus scanning options. See [Virus Scan](#virus-scan)                                 |
|  `HealthCheck`             | object     | null       | Health-check configuration options. See [health Check](#health-check)                 |
|  `Documentation`           | object     | null       | API documentation options (Swagger). See [Documentation](#documentation)              |
|  `Identity`                | object     | null       | Identity configuration options. See [Identity](#identity)                             |
|  `Apis`                    | dictionary | []         | Named Nano API client configurations available to the application. See [Api's]()      |

```json
"App": {
  "DefaultTimeZone": "UTC",
  "ShutdownTimeout": 10,
  "Hosting": { },
  "HttpPolicyHeaders": null,
  "ResponseCache": null,
  "ResponseCompression": null,
  "Session": null,
  "TimeZone": null
  "Localization": null
  "VirusScan": null,
  "HealthCheck": null,
  "Documentation": null,
  "Identity": null,
  "Apis": []
}
```

## Hosting
Hosting refers to the setup and configuration of the exposure and limits of the api.

| Setting                      | Type    | Default  | Description                                                          |
| ---------------------------- | ------- | -------- | -------------------------------------------------------------------- |
|  `Root`                      | string  | api      | Root route for the application endpoints.                            |
|  `ExposeErrors`              | bool    | false    | Expose detailed errors.                                              |
|  `ExposeAuthController`      | bool    | true     | Expose auth controller.                                              |
|  `ExposeAuditController`     | bool    | true     | Expose audit controller.                                             |
|  `Http`                      | object  | default  | Options for HTTP. See [HTTP](#http).                                 |
|  `Https`                     | object  | null     | Options for HTTPS. See [HTTPS](#https).                              |
|  `MultipartLimits`           | object  | null     | Multipart upload limits. See [MultiPart Limits](#multipart-limits).  |

```json
"App": {
  "Hosting": {
    "Root": "api",
    "ExposeErrors": false,
    "ExposeAuditController": true, 
    "ExposeAuthController": true,
    "Http": { }
    "Https": null
    "MultipartLimits": null
  }
}
```

## Http
Setup the http ports.
Ïf you also have setup [Https](#https) then consider enabling the `UseHttpsRedirection` to ensure redirects from http to https.
You should at least specify one http port.

| Setting                 | Type    | Default  | Description                                                        |
| ----------------------- | ------- | -------- | ------------------------------------------------------------------ |
|  `Ports`                | array   | []       | List of ports for HTTP.                                            |
|  `UseHttpsRedirection`  | string  | false    | Enforce HTTPS redirect for all requests.                           |

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

See [Example Api.Hosting.Http](Api.Hosting.Http)

## Https
First, by specifying a https port, will enable secure socket layer communication. Also a certificate path and password must be specified in the configuration,
in order to have a valid security protocol enabled. If the application should only support secure connections, configuring the https redirect option, 
will redirect traditional insecure http requests to the https protocol.  

Https is intended just for local development environment. In kubernetes https and certificates are handled on Ingress level.

if enabled a certificate path and password must also be specified

| Setting                  | Type    | Default  | Description                                                        |
| ------------------------ | ------- | -------- | ------------------------------------------------------------------ |
|  `Ports`                 | array   | []       | List of ports for HTTPS.                                           |
|  `UseHttpsRequired`      | bool    | false    | Enforce HTTPS required for all requests.                           |
|  `Certificate`           | object  | default  | SSL certificate configuration.                                     |
|  `Certificate.Path`      | string  | null     | File path to the certificate.                                      |
|  `Certificate.Password`  | string  | null     | Password for the certificate.                                      |

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

See [Example Api.Hosting.Https](Api.Hosting.Https)

## MultiPart Limits
Upload limits
Leaving the configuration null, sets unlimited for upload limits. 
This may be fine if limits are controlled on the orchestration level in kubernetes, but otherwise it's recommended to set a limit.

See [Example Api.Hosting.MultipartLimits](Api.Hosting.MultipartLimits)

## Http Policy Headers
Additionally, the configuration supports various options, for controlling transport-security, XXS-protection, cache control, 
as well as content-type and download restrictions. 

| Setting              | Type    | Default  | Description                                                                                       |
| -------------------- | ------- | -------- | ------------------------------------------------------------------------------------------------- |
|  `ContentType`       | object  | null     | Content-Type header options. See [Content Type](#content-type)                                    |
|  `ReferrerPolicy`    | object  | null     | Referrer-Policy header options. See [Referrer Policy](#referrer-policy)                           |
|  `FrameOptions`      | object  | null     | X-Frame-Options header options. See [Frame Options](#frame-options)                               |
|  `XssProtection`     | object  | null     | XSS-Protection header options. See [Xss Protection](#xss-protection)                              |
|  `Csp`               | object  | null     | Content-Security-Policy (CSP) options. See [Content Security Policy](#content-security-policy)    |
|  `Cors`              | object  | null     | CORS configuration options. See [Cors](#cors)                                                     |
|  `Hsts`              | object  | null     | HSTS configuration options. See [Hsts](#hsts)                                                     |
|  `Robots`            | object  | null     | Robots meta tag options. See [Robots](#robots)                                                    |
|  `ForwardedHeaders`  | object  | null     | Forwarded headers configuration. See [Forwarded Headers](#forwarded-headers)                      |

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

See [Example Api.PolicyHeaders](Api.PolicyHeaders)

## Content Type
```json
"App": {
  "HttpPolicyHeaders": {
    "ContentType": {
      "NoSniff": false
    }
  }
}
```

## Referrer Policy
```json
"App": {
  "HttpPolicyHeaders": {
    "ReferrerPolicy": {
      "ReferrerPolicyHeader": "Disabled"
    }
  }
}
```

## Frame Options
```json
"App": {
  "HttpPolicyHeaders": {
    "FrameOptions": {
      "FrameOptionsPolicyHeader": "Disabled"
    }
  }
}
```

## Xss Protection
```json
"App": {
  "HttpPolicyHeaders": {
    "XssProtection": {
      "XssProtectionPolicyHeader": "Disabled"
    }
  }
}
```

## Content Security Policy
The csp-reports is configured and uses the default Nano report endpoint `/csp/report-to`, which logs the violation to console. If you need different handling than just logging
the report to console, implement your own endpoint for handling csp violations

```json
"App": {
  "HttpPolicyHeaders": {
    "Csp": {
      "ReportOnly": false,
      "UpgradeInsecureRequests": false,
      "ReportTo": {
        "Group": "csp-reports",
        "MaxAge": "10886400",
        "Endpoints": [
        ]
      },
      "Defaults": {
        "IsNone": false,
        "IsSelf": false,
        "Sources": [
        ]
      },
      "Scripts": {
        "IsNone": false,
        "IsSelf": false,
        "IsUnsafeInline": false,
        "IsUnsafeEval": false,
        "IsUnsafeWasmEval": false,
        "StrictDynamic": false,
        "IsUnsafeHashes": false,
        "UnsafeHashedAttributes": false,
        "UnsafeAllowRedirects": false,
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
        "StrictDynamic": false,
        "IsUnsafeHashes": false,
        "UnsafeHashedAttributes": false,
        "UnsafeAllowRedirects": false,
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
        "IsUnsafeInline": false,
        "ReportSample": false
      },
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
      "StylesAttr": {
        "IsNone": false,
        "IsUnsafeInline": false,
        "ReportSample": false
      },
      "Objects": {
        "IsNone": false,
        "IsSelf": false,
        "Sources": [
        ]
      },
      "Media": {
        "IsNone": false,
        "IsSelf": false,
        "Sources": [
        ]
      },
      "Images": {
        "IsNone": false,
        "IsSelf": false,
        "Sources": [
        ]
      },
      "Frames": {
        "IsNone": false,
        "IsSelf": false,
        "Sources": [
        ]
      },
      "FencedFrames": {
        "IsNone": false,
        "IsSelf": false,
        "Sources": [
        ]
      },
      "FrameAncestors": {
        "IsNone": false,
        "IsSelf": false,
        "Sources": [
        ]
      },
      "Fonts": {
        "IsNone": false,
        "IsSelf": false,
        "Sources": [
        ]
      },
      "Connections": {
        "IsNone": false,
        "IsSelf": false,
        "Sources": [
        ]
      },
      "BaseUris": {
        "IsNone": false,
        "IsSelf": false,
        "Sources": [
        ]
      },
      "Children": {
        "IsNone": false,
        "IsSelf": false,
        "Sources": [
        ]
      },
      "Forms": {
        "IsNone": false,
        "IsSelf": false,
        "Sources": [
        ]
      },
      "Manifests": {
        "IsNone": false,
        "IsSelf": false,
        "Sources": [
        ]
      },
      "Workers": {
        "IsNone": false,
        "IsSelf": false,
        "Sources": [
        ]
      },
      "TrustedTypes": {
        "IsNone": false,
        "AllowDuplicates": false,
        "Policies": [
        ]
      },
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
      },
      "PermissionsPolicy": {
        "Accelerometer": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "AmbientLightSensor": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "AutoPlay": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "Battery": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "Camera": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "DisplayCapture": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "DocumentDomain": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "EncryptedMedia": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "ExecutionWhileNotRendered": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "ExecutionWhileOutOfViewport": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "FullScreen": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "Gamepad": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "Geolocation": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "Gyroscope": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "LayoutAnimations": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "LegacyImageFormats": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "Magnetometer": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "Microphone": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "Midi": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "NavigationOverride": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "OversizedImages": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "Payment": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "PictureInPicture": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "PublicKeyCredentialsGet": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "SpeakerSelection": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "SyncXhr": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "UnoptimizedImages": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "UnsizedMedia": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "Usb": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "ScreenWakeLock": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "WebShare": {
          "IsNone": false,
          "IsSelf": false,
          "Sources": [
          ]
        },
        "XrSpatialTracking": {
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

## Cors
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
      "AllowCredentials": true,
      "Origin": {
        "EmbedderPolicy": null,
        "OpenerPolicy": null,
        "ResourcePolicy": null
      }
    }
  }
}
```

## Hsts
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

## Robots
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

## Forwarded Headers
There is   configuration for forwarded headers. Just enable it be setting an empty json object, and all headers will be forwarded.

| Header               | HttpContext                             |
| -------------------- | --------------------------------------- |
| X-Forwarded Header   | HttpContext Property Updated            |
| X-Forwarded-For	   | HttpContext.Connection.RemoteIpAddress  |
| X-Forwarded-Proto	   | HttpContext.Request.Scheme              |
| X-Forwarded-Host	   | HttpContext.Request.Host                |

If your app is directly exposed to the internet without a reverse proxy in front, clients can spoof X-Forwarded headers: fake IP, scheme, or host.
Therefore, this approach is safe only if your traffic always passes through a trusted proxy / load balancer, which is exactly the case in cloud deployments.

```json
"App": {
  "HttpPolicyHeaders": {
    "ForwardedHeaders": {
    }
  }
}
```

## Response Cache
It's recommended to enable in configuration, and then disable of certain actions where neeeded.

## Response Compression
Try to disable one endpoint in example, and test enabled vs disabled and see the response size difference

## Session
Discouraged to use in Api, but included because it can be used. Should we include it?
When hosting in a scaled environment, sticky sessions needs to be enabled. SHOW ingress and links to official docs about sticky sesssions og Nginx docs or ??

Cookie name: `.AspNetCore.Session`

## TimeZone
Nano supports the built in methods for specifying the timezone when invoking requests.  
See the official documentation about timezone here: [TimeZone Documentation](https://github.com/vivet/Vivet.AspNetCore/tree/master/Vivet.AspNetCore.RequestTimeZone#vivetaspnetcorerequesttimezone)  

| Setting                    | Type    | Default    | Description                                                                           |
|  `DefaultTimeZone`         | string  | UTC        | Default time zone for the application.                                                |

Explain about when to use DateTime (when the date is fixed, and shouldn't change no matter where you are on earth) 
and DateTimeOffset (that should vary depending on where on earth you are).

DateTimeInfo.Now / DateTimeInfo.UtcNow
Use to get the local timezone based on the timezone of the `tz` header parameter.

Cookie name: `.AspNetCore.TimeZone`

## Localization
Nano supports the built in methods for specifying the language when invoking requests.  
See the official Microsoft documentation about localization here: [Localization Documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-2.1)  

Only supported cultures will be used. Any unsupported cultures will default to Default Culture.


| Setting         | Type   | Default   | Description                                                               |
| --------------- | ------ | --------- | ------------------------------------------------------------------------- |
|  `Cultures`     | enum   |           | Culture and localization settings for the application.                    |

```json
"App": {
  "Cultures": {
    "Default": "en-US",
    "Supported": [
    ]
  }
}
```

Cookie name: `.AspNetCore.Culture`


## Documentation
When documentation is enabled in the configuration file, a web-interface documenting the service, it's endpoints and it's models - is created and deployed.  
The documentation is based on [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore).  
When using Default version only non versioned routes are shown in swagger for the default version.

### CspNonce
This value is meant for allowing swagger to work when using Csp nonce values for scripts. You will set a static nonce for swagger and also for other frontends you have,
and the ingresses will be replacing them with dynamically generated nonce tokens before exposure.

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
HOW MUCH MORE DO WE NEED HERE. 

### Use Default Version
When this is `true` then the routes in swagger for the default version (`App:Version`) will be omitted from swagger, and only the default non-versioned routes will show.
The versioned routes still work, but is just hidden from swagger.

## Health Checks
Open http://localhost:8080/healthz-ui#/healthchecks and see that the startup health-check has completed, and reports healthy.
When more Nano providers and services are added to the application, if health-check enabled, these services will appear hear, and report their status.

When enabling health-checks in the web section of the confiugration, the application will be configured with a health-check. The health status of the application, can be found here:  
* ```http://{host}:{port}/healthz```  

If the health check UI is also enabled in the confiugration, an interface for monitoring the health of the application, as well as any enabled health checks for dependent providers, can be found here:  
* ```http://{host}:{port}/healthz-ui```  

## Virus Scan
Nano supports virus scan by specifying a network connection to a clamav instance.  
All uploaded files will be scanned, and a ```VirusScanException``` will be thrown if one or more files contains any virus or malware.  
See the official documentation about virus scan here: [Virus Scan Documentation](https://github.com/vivet/Vivet.AspNetCore/tree/master/Vivet.AspNetCore.RequestVirusScan#vivetaspnetcorerequestvirusscan)  

## Versioning
There is no configuration for versioning. It's a built in feature in .NET, and you just need to use `[ApiVersion]` and `[MapToApiVersion]` annotations.

During application startup, Nano registers dependencies for enabling api versioning. 
Obviously, since most controller actions will be inherited from one or more of the base controller implementations, 
versioning has to be annotated in derived controller classes. Additionally, versioned action methods must be overridden in the derived controller, 
as the versioning would otherwise apply to all derived controller implementations.  

Clients can specify the version in the following ways and order:
* Route segment (```vV```)
* Http header (```api-version```)
* Query parameter (```api-version```)

Besides that, versioning follows the standard .Net Core approach.  

Routes will by default use default version. The `App:Version` you have configured will be default. routes with default version will work without specifying the version
number in the route. ANd there is no need to annotate default version to controllers and actions, it's assumed.

Only major and minor version will be considered in relation to routing. `/api/v1/...` (works), `/api/v1.0/...` (works), `/api/v1.0.0/...` (won't works)

LINK to documentation about version annotation, etc. ISn't there a Microsoft Learn link, or check which packages i use now

## Cookies
Reasonable and not configurable options.

x.HttpOnly = HttpOnlyPolicy.None;
Describes the HttpOnly behavior for cookies.
The cookie does not have a configured HttpOnly behavior. This cookie can be accessed by JavaScript <c>document.cookie</c> API.

x.Secure = CookieSecurePolicy.Always;
Determines how cookie security properties are set
Secure is always marked true. Use this value when your login page and all subsequent pages requiring the authenticated identity are HTTPS. 
Local development will also need to be done with HTTPS urls.

x.MinimumSameSitePolicy = SameSiteMode.Strict;
Indicates the client should only send the cookie with "same-site" requests.
Used to set the SameSite field on response cookies to indicate if those cookies should be included by the client on future "same-site" or "cross-site" requests.
RFC Draft: <see href="https://tools.ietf.org/html/draft-ietf-httpbis-rfc6265bis-03#section-4.1.1"/>

When creating cookies:
CookiePolicyMiddleware in ASP.NET Core enforces the policy you set in AddCookiePolicy after the cookie is added to the response.
Key points:
If a cookie’s options match or exceed the policy (e.g., stricter), nothing changes.
If a cookie violates the policy, the middleware adjusts it to conform before sending to the client.

## Preflight
Describe support for http OPTIONS. Is it called preflight or preflight request or? Read more about this and check code


## Content-Type Negotiation 
WE ARE MISSING EXAMPLE
WE ALWAYS USE JSON. AND ITS NOT ```Content-Type``` or   ```Accept```, CHECK WHICH IS REQUEST AND WHICH IS RESPONSE
Nano supports several different formats (listed below) for the requests and responses of the controller actions.  
The format, also known as content-type may be specified, either through the ```Content-Type``` or   ```Accept``` header, or by appending the following querystring parameter: ```?format={format}```. The later is default by ([Microsoft Formatting](https://docs.microsoft.com/en-us/aspnet/core/mvc/models/formatting)), by any of the three methods will work with Nano.

Supported formats: Json









## Authentication
When authentication has been enabled and configured, and the application is running, users authenticate in order to gain access to the controllers and their actions. Nano has a ```AuthController``` implementation, responsible for this.  

Nano supports various methods of authentication as described in [Security - Authentication](security#authentication) section.  

The ```Security``` section of the configuration defines behavior related to authentication and authorization in the application. The section is deserialized into an instance of ```SecurityOptions```, and injected as dependency during startup, thus available for injection throughout the application.  
```json
"Security": {
    "Jwt": {
        "IsEnabled": true,
        "Issuer": "issuer",
        "Audience": "audience",
        "PublicKey": null,
        "PrivateKey": null,
        "ExpirationInMinutes": 60,
        "RefreshExpirationInHours": 72
    },
    "ExternalLogins": {
        "Google": {
            "ClientId": null,
            "ClientSecret": "N/A",
            "Scopes": [
            ]
        },
        "Facebook": {
            "AppId": null,
            "AppSecret": "N/A",
            "Scopes": [
            ]
        },
        "Microsoft": {
            "TenantId": null,
            "ClientId": null,
            "ClientSecret": "N/A",
            "Scopes": [
            ]
        }
    }
}
```

### Roles & Claims
#### Roles
Name | Type | Description |
---- | ---- | ---- |
Guest | built-in | Currently, not authorized to do anything.
Reader | built-in | Authorized to read.
Writer | built-in | Authorized to read and write
Service | built-in | Authorized to all services.
Administrator | built-in | Full access to everything.
MyRole | custom | Custom role specified during signup or login.

#### Claims
Name | Type | Description |
---- | ---- | ---- |
AppId | built-in | The id of the application. Set during logon, and used for supporting multiple refresh tokens. Default value: "Default"
Id | built-in | The user id.
Email | built-in | The user's email address.
Name | built-in | The username
MyClaim | custom | Custom claim specified during signup or login.


Nano supports authenticating with user credentials (username and password), and also using one of the supported external providers.  

A successful authentication returns a Nano ```AccessToken```. 
```json
{
    "AppId": null,
    "UserId": null,
    "Token": null,
    "ExpireAt": null,
    "IsExpired": false,
    "RefreshToken": {
        "Token": null,
        "ExpireAt": null,
        "IsExpired": false,
    },
}
```

The ```jwt-token``` contains the following claims and values.  
```json
{
  "appId": "Default",
  "jti": "74ec40fe-bb18-4bd6-8ec5-51b37f9c8a5c",
  "sub": "08d9da95-9a3b-4ec6-83b0-fa11da6bae7e",
  "name": "admin@domain.com",
  "email": "admin@domain.com",
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": [
    "service",
    "administrator"
  ],
  "nbf": 1111111111,
  "exp": 1111111111,
  "iss": "development.nano",
  "aud": "development.nano"
}
```

#### Api Key
It's also possible to authenticate using an api-key, by setting the header ```x-api-key``` with a valid api-key.  

Api-keys can be managed through the ``IdentityManager```.  

#### External Providers
Nano supports the following external providers.

Name | Type | Description
-------------|-------------|-------------
Google | Implicit | Google authentication using implicit flow. No token refresh possible.
Facebook | Implicit | Facebook authentication using implicit flow. No token refresh possible.
Microsoft | Auth-Code | Microsoft authentication using auth code flow.

When logging in with an external provider through a single-page-application, Nano finalizes the flow by validating the external response. On successful validation a Nano  jwt-token is created, wherein the retrieved external access-token is embedded, should it later be needed to authorize against the external provider.  

Scopes for ```id```, ```email``` and ```username``` should be enabled for external providers.  

***

### Root Log-In
Nano comes with a built-in administrator, defined in the [Security Section](#configuration), as shown below. The administrator user will be created during start-up, if it doesn't already exist.

The administrator has unrestricted permissions, and may access any part of the application. 
```json
"Security": {
  "User": {
    "AdminUsername": "admin",
    "AdminPassword": "<<secret>>",
  }
}
```

## Authorization
When a user successfully authenticates, a jwt-token is returned to the client. Subsequent requests must include the ```Authorization``` header, containing the value ```Baerer {token}```.  See [JWT Explained](https://jwt.io/introduction/) for further details.  

In Nano the ```HttpContext``` is extended with methods, that decrypts the authorization token, and extracts specific claim values.  
* ```GetIsAnonymous()``` Returns whether the user is anonymous.  
* ```GetJwtAppId()``` Returns app id.  
* ```GetJwtToken()``` Returns the complete token.  
* ```GetJwtUserId()``` Extracts the user-id of the logged in user.  
* ```GetJwtUserName()``` Extracts the user username of the logged in user.  
* ```GetJwtUserEmail()``` Extracts the user email address of the logged in user.  
* ```GetJwtExternalToken()``` Extracts the external provider access token, if logged in with external provider.  

All methods returns null, when not authenticated and no authorization token is passed along with the request.  

Roles:
By default, authorization to controller actions is handling by the built-in roles and policies defined by Nano. Controllers may be decorated with the ```AuthorizeAttribute```, and allow to override the default authorization and use custom defined roles and policies.  
























## Controllers
OPTIONS HTTP METHOD functionality


The ```BaseController<TRepository, TEntity, TIdentity, TCriteria>``` abstract class implementation, is as shown defined by four generic type parameters. 
* ```TRepository```, defines the underlying implementation of the ```IRepository``` interface. 
* ```TEntity```, defines the entity of the controller.
* ```TIdentity```, defines the identity type used by the entity related to finding entity by a unique identifier.
* ```TCriteria```, defines the query criteria implementation related to querying the entity.  

Normally, there is no reason to derive implementations directly from the ```BaseController<TRepository, TEntity, TIdentity, TCriteria>```. Deriving from the ```DefaultController<MyEntity, MyQueryCriteria>```is equivalent to ```BaseController<DefaultRepository, TEntity, Guid, TCriteria>```, where as shown, the ```TRepository``` parameter is defined as ```DefaultRepository```, and ```TIdentity```is of type ```System.Guid```. 

In situations where only read-only actions is permitted on the model associated with a controller, Nano has the ```DefaultControllerReadOnly``` to support that. When deriving from that, only query actions are inherited. Additionally, Nano also provides default controller implementations for ```Creatable-```, ```Updatable-``` and ```DeletableController```, if needed.  

Additionally, some other controllers exists in Nano as well. These controllers differ in the generic parameter types, to allow for a more flexible use. For example, if no data repository is required by the application, a controller exists where the generic parameter ```TRepository``` is omitted.  

```csharp 
public class MyController : DefaultController<MyEntity, MyQueryCriteria>
{
    public MyController(ILogger logger, IRepository repository, IEventing eventing)
        : base(logger, repository, eventing)
    { }
}
```

The ```IdentityController<TEntity, TCriteria>``` contains methods for creating and managing identities used to authenticate with the application. Derive a controller implementation from ```IdentityController<TEntity, TCriteria>```, and inherit all the identity actions.  
The ```IdentityController<TEntity, TCriteria>``` derives from ```DefaultControllerUpdatable<TEntity, TCriteria>```,  allowing identities to only be updated (and fetched, naturally), but may only be created through the inherited identity action ```signup```.  

```csharp 
public class MyController : IdentityController<MyEntity, MyQueryCriteria>
{
    public MyController(ILogger logger, IRepository repository, IEventing eventing, IdentityManager identityManager)
        : base(logger, repository, eventing, identityManager)
    { }
}
```

When deriving a controller implementation from ```DefaultController<MyEntity, MyQueryCriteria>```, a rich set of action methods are inherited through the ```BaseControllerReadOnly<MyEntity, MyQueryCriteria>``` and the ```BaseControllerWriteable<MyEntity, MyQueryCriteria>``` abstract controller implementations. 

Routing requests to controller actions, is done using the default convention, as shown below 
* ```http(s)://{host}:{port}/{root}/{controller}/{action}/{Id?}```

Nano controllers has three dependencies injected into the constructor, all of which is registered when building the application. Derived controller implementations can add further injections as needed.  
* ```ILogger```, is the interface for logging in the controller. 
* ```IRepository```, is the interface for get, add, update, delete and query data in the controller.
* ```IEventing```, is the interface for publishing events in the controller.


### Models in Controller
Now the controller needs a model and a QueryCriteria.
Naming Conventions:
IMPORTANT: Controllers must be named the same as their entity pluralized, e.g. MyEntity and MyEntitysController.

### Query Criteria
THESE CAN ALSO BE DEFINED AND USED IN CONSOLE APP, MAKES LITTLE SENSE THOUGH, BUT `IRepository` has methods with query criteria

Nano uses the [DynamicExpression](https://github.com/vivet/DynamicExpression) library to map properties of the query and criteria to a Linq Expression of the entity.  

Query criteria defines a contract for a model. It's used when invoking the ```Query(...)``` method of the controller of the model. Adding properties to the contract and overridding the method ```GetExpression<TEntity>()```, mapping the contract properties to the actual entity, will at runtime dynamically build the ```Expression<T>``` used in Linq queries by Entity Framework (or for that matter any Linq provider).  

Obviously, since query criteria is only used by controller actions, there is no need for them when building console applications.  

#### Query
The base query class contains pagination (number, count) and ordering (by, direction) properties. Naturally, these are used by controller actions and data queries to control the number of returned results as well as the order of which they are sorted.  

#### Criteria
Nano contains a ```DefaultQueryCritiera``` class, implementing the ```IQueryCriteria``` interface, of the  [DynamicExpression](https://github.com/vivet/DynamicExpression) library. It combines a ```Expression<TEntity>``` for the auditable properties ```IsActive```, ```CreateAt``` and ```UpdatedAt```, and expexts a ```DefaultEntity```. If models doesn't derive from ```DefaultEntity```, either derive the query criteria from ```BaseQueryCriteria``` or implement the interface ```IQueryCriteria``` directly.  

So simply, create a class deriving from ```DefaultQueryCritiera```, add criteria properties, and last override the ```GetExpression<TEntity>()``` method mapping the criteria properties to the entity properties, as shown below.  

```csharp 
public class MyQueryCriteria : DefaultQueryCriteria
{
    public virtual string PropertyOne { get; set; }

    public override CriteriaExpression GetExpression<TEntity>()
    {
        var filter = base.GetExpression<TEntity>();

        if (this.PropertyOne != null)
            filter.StartsWith("PropertyOne", this.PropertyOne);

        return filter;
    }
}
```

## Request Validation
When deriving a controller implementation from ```BaseController```, model validation is automatically enabled. Based on the annotations (attributes) which model properties is decorated with. When a model fails validation, a bad request with the validation errors is returned.  

Other than that, then validation isn't any different from normal.  
See the official Microsoft documentation here: [Model Validation Documentation](https://docs.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-2.1)  

## Serialization
Nano has a custom contract serializer implementation.  
The serializer derives from the regular implementation, but removes empty lists and system properties, related to soft-deletion and lazy-loading for instance. The serializer applies to deserializing incoming requests, though this process does nothing out of the ordinary. It also applies when serializing models into response content.  
When serializing responses, if lazy-loading entities is enabled in data configuration, it's disabled. This ensures that data will not be lazy fetched when the serializer navigates relational properties, but only data existing in the change-tracker will be serialized.  
The serializer will not serialize navigations that is of type ```IEntity```, except when they are annotated with ```IncludeAttribute```. This is to avoid returning unwanted navigation references, that is automatically added if dependent navigations are loaded separately.  

Nano's serializer supports Geometry types, from Nettoplogysuite

## Error Handling
When an exception or other errors occurs for a request, and ```500 Internal Server Error``` is returned to the client, contain ```Error``` response, as shown below.  

```csharp
public class Error
{
    public virtual string Summary { get; set; } // Summary of the error.
    public virtual string[] Exceptions { get; set; } // Array of exceptions.
    public virtual int StatusCode { get; set; } // The http status code.
    public virtual bool IsTranslated { get; set; } // Indicates if the exception messages is translated.
    public virtual bool IsCoded { get; set; } // Indicates if the exception messages is coded.
}
``` 

If ```Error.IsTranslated``` is true, then the consumer can expect the Exceptions to be translated to the language matching the Current CultureInfo, unless translations in that language is not available, and the default translation is used.  
If ```Error.IsCoded``` is true, then the consumer can expect the Exceptions to be a code, that can be used to map an error message for the user.  

## Start-Up Tasks
Nano supports startup-tasks, that executes before the application starts. 
A 'self' startup-healtcheck will report ready when all startup tasks have completed. Only relevant for api applications.

Read more [Nano.App](nano-app#start-up-tasks)
