using System;
using Azure.Security.KeyVault.Secrets;

namespace ShareGate.Extensions.Configuration.Secrets;

public interface ISecretClientProvider
{
    SecretClient GetSecretClient(KeyVaultKind keyVaultKind);

    SecretClient GetSecretClient(Uri keyVaultUri);

    SecretClient GetSecretClient(string configurationKey);
}