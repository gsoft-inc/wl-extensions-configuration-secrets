using Azure.Core;

namespace GSoft.Extensions.Configuration.Secrets;

public interface ITokenCredentialProvider
{
    TokenCredential GetTokenCredential();
}