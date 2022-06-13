using System;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace ShareGate.Extensions.Configuration.Secrets;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKeyVaultSecrets(this IServiceCollection services)
    {
        services.TryAddSingleton<ITokenCredentialProvider, TokenCredentialProvider>();
        services.TryAddSingleton<ISecretClientProvider, SecretClientProvider>();

        return services;
    }

    public static IConfigurationBuilder AddKeyVaultSecrets(this IConfigurationBuilder builder, IHostEnvironment environment, KeyVaultKind keyVaultKind = KeyVaultKind.ApplicationConfiguration)
    {
        var secretClient = new SecretClientProvider(builder, environment).GetSecretClient(keyVaultKind);
        return builder.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
    }

    public static IConfigurationBuilder AddKeyVaultSecrets(this IConfigurationBuilder builder, IHostEnvironment environment, Uri keyVaultUri)
    {
        var secretClient = new SecretClientProvider(builder, environment).GetSecretClient(keyVaultUri);
        return builder.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
    }

    public static IConfigurationBuilder AddKeyVaultSecrets(this IConfigurationBuilder builder, IHostEnvironment environment, string configurationKey)
    {
        var secretClient = new SecretClientProvider(builder, environment).GetSecretClient(configurationKey);
        return builder.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
    }
}