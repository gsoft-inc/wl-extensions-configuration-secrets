using System;
using System.Collections.Generic;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Hosting;

namespace ShareGate.Extensions.Configuration.Secrets;

public sealed class TokenCredentialProvider : ITokenCredentialProvider
{
    private static readonly HashSet<string> AzureCliCompatibleEnvironments = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        // Mostly local, development, tests environments or any environment where a developer could use Azure CLI
        "local", "test", "ci", Environments.Development,
    };

    private readonly IHostEnvironment _environment;

    public TokenCredentialProvider(IHostEnvironment environment)
    {
        this._environment = environment;
    }

    public TokenCredential GetTokenCredential()
    {
        if (AzureCliCompatibleEnvironments.Contains(this._environment.EnvironmentName))
        {
            return GetAzureCliCompatibleTokenCredential();
        }

        // We prefer to only use Azure Managed Identity over DefaultAzureCredential which allows multiple ways to authenticate against Azure
        // See https://docs.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential?view=azure-dotnet
        return new ManagedIdentityCredential();
    }

    private static TokenCredential GetAzureCliCompatibleTokenCredential()
    {
        // Azure CLI does not work when Fiddler is active so we need to use an interactive authentication method instead
        return FiddlerProxyDetector.IsFiddlerActive()
            ? new CachedInteractiveBrowserCredential()
            : new ChainedTokenCredential(new AzureCliCredential(), new ManagedIdentityCredential());
    }
}