using System;
using System.Collections.Generic;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ShareGate.Extensions.Configuration.Secrets;

public sealed class SecretClientProvider : ISecretClientProvider
{
    private static readonly IReadOnlyDictionary<KeyVaultKind, string> ConfigurationKeys = new Dictionary<KeyVaultKind, string>
    {
        [KeyVaultKind.ApplicationConfiguration] = SecretClientConfigurationKeys.ApplicationConfiguration,
        [KeyVaultKind.UserPersonalInformation] = SecretClientConfigurationKeys.UserPersonalInformation,
    };

    private readonly IConfiguration _configuration;
    private readonly ITokenCredentialProvider _tokenCredentialProvider;

    public SecretClientProvider(IConfiguration configuration, IHostEnvironment environment)
    {
        this._configuration = configuration;
        this._tokenCredentialProvider = new TokenCredentialProvider(environment);
    }

    public SecretClientProvider(IConfigurationBuilder configurationBuilder, IHostEnvironment environment)
        : this(configurationBuilder.Build(), environment)
    {
    }

    public SecretClient GetSecretClient(KeyVaultKind keyVaultKind)
    {
        var keyVaultUri = this.GetKeyVaultUri(keyVaultKind);
        return this.GetSecretClient(keyVaultUri);
    }

    public SecretClient GetSecretClient(Uri keyVaultUri)
    {
        if (keyVaultUri == null)
        {
            throw new ArgumentNullException(nameof(keyVaultUri));
        }

        var azureCredential = this._tokenCredentialProvider.GetTokenCredential();

        // SecretClient already has a default retry policy (max 3 retries)
        return new SecretClient(keyVaultUri, azureCredential);
    }

    public SecretClient GetSecretClient(string configurationKey)
    {
        if (configurationKey == null)
        {
            throw new ArgumentNullException(nameof(configurationKey));
        }

        var keyVaultUri = this.GetKeyVaultUri(configurationKey);
        return this.GetSecretClient(keyVaultUri);
    }

    private Uri GetKeyVaultUri(KeyVaultKind keyVaultKind)
    {
        if (!ConfigurationKeys.TryGetValue(keyVaultKind, out var configurationKey))
        {
            throw new NotSupportedException("There is no configuration key associated to the key vault kind " + keyVaultKind + " in the code");
        }

        return this.GetKeyVaultUri(configurationKey);
    }

    private Uri GetKeyVaultUri(string configurationKey)
    {
        var keyVaultUriStr = this._configuration.GetValue<string>(configurationKey);
        if (!Uri.TryCreate(keyVaultUriStr, UriKind.Absolute, out var keyVaultUri))
        {
            throw new InvalidOperationException("The configuration value " + configurationKey + " must be a valid absolute URI");
        }

        return keyVaultUri;
    }
}