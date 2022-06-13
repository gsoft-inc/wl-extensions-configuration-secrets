# ShareGate.Extensions.Configuration.Secrets

[![nuget](https://img.shields.io/nuget/v/ShareGate.Extensions.Configuration.Secrets.svg?logo=nuget)](https://www.nuget.org/packages/ShareGate.Extensions.Configuration.Secrets/)
[![build](https://img.shields.io/github/workflow/status/gsoft-inc/sg-extensions-configuration-secrets/CI%20build?logo=github)](https://github.com/gsoft-inc/sg-extensions-configuration-secrets/actions/workflows/ci.yml)

This package allows storing configuration values in [Azure Key Vault Secrets](https://docs.microsoft.com/en-us/dotnet/api/overview/azure/extensions.aspnetcore.configuration.secrets-readme),
using the right [Azure credentials](https://docs.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme#credential-classes) based on the current [environment](https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host#ihostenvironment).

It can also be used to register `ITokenCredentialProvider` and `ISecretClientProvider` in `IServiceCollection` in order to access Azure credentials or a [`SecretClient`](https://docs.microsoft.com/en-us/dotnet/api/azure.security.keyvault.secrets.secretclient?view=azure-dotnet) instance.


## Getting started

```
dotnet add package ShareGate.Extensions.Configuration.Secrets
```

Example with an **ASP.NET Core minimal API**:

```csharp
var builder = WebApplication.CreateBuilder();

// There are three ways to load configuration values from Azure Key Vault:
builder.Configuration.AddKeyVaultSecrets(builder.Environment);
builder.Configuration.AddKeyVaultSecrets(builder.Environment, new Uri("<my-key-vault-url>"));
builder.Configuration.AddKeyVaultSecrets(builder.Environment, "<my-configuration-key>");

// Register ITokenCredentialProvider and ISecretClientProvider services (optional)
builder.Services.AddKeyVaultSecrets();
```


## Using the registered services

`ITokenCredentialProvider` and its public implementation `TokenCredentialProvider` provides an instance of `TokenCredential`  based on the current environment:
* `ManagedIdentityCredential` on a non-development environment,
* Chained credentials of `AzureCliCredential` and `ManagedIdentityCredential` in development environment, or
* `CachedInteractiveBrowserCredential` in development environment only when **Fiddler** is opened (Fiddler interferes with `az login` authentication).

> Note that `DefaultAzureCredential` is never used.

```csharp
var azureCredential = new TokenCredentialProvider(environment).GetTokenCredential(); // or
var azureCredential = services.GetRequiredService<ITokenCredentialProvider>().GetTokenCredential();
```

`ISecretClientProvider` and its public implementation `SecretClientProvider` provides an instance of `SecretClient` based on the current environment:

```csharp
var secretClientProvider = new SecretClientProvider(configurationBuilder, environment); // or
var secretClientProvider = new SecretClientProvider(configuration, environment); // or
var secretClientProvider = services.GetRequiredService<ISecretClientProvider>();
```

```csharp
var secretClient = secretClientProvider.GetSecretClient(keyVaultKind); // or
var secretClient = secretClientProvider.GetSecretClient(keyVaultUri); // or
var secretClient = secretClientProvider.GetSecretClient(configurationKey);
```


## License

Copyright © 2022, GSoft inc. This code is licensed under the Apache License, Version 2.0. You may obtain a copy of this license at https://github.com/gsoft-inc/gsoft-license/blob/master/LICENSE.
