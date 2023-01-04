using System;
using System.Collections.Generic;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Hosting;

namespace GSoft.Extensions.Configuration.Secrets;

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

        // See https://docs.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential?view=azure-dotnet
        return new DefaultAzureCredential();
    }

    private static TokenCredential GetAzureCliCompatibleTokenCredential()
    {
        // Azure CLI does not work when Fiddler is active so we need to use an interactive authentication method instead
        // When Fiddler is not active, we try to use AzureCliCredential because it's way faster than DefaultAzureCredential on startup
        return FiddlerProxyDetector.IsFiddlerActive()
            ? new CachedInteractiveBrowserCredential()
            : new ChainedTokenCredential(new AzureCliCredential(), new DefaultAzureCredential());
    }
}