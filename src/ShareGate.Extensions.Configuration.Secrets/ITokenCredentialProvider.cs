using Azure.Core;

namespace ShareGate.Extensions.Configuration.Secrets;

public interface ITokenCredentialProvider
{
    TokenCredential GetTokenCredential();
}