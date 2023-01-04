namespace GSoft.Extensions.Configuration.Secrets;

public enum KeyVaultKind
{
    /// <summary>
    /// Represents the Key Vault where we store keys and secrets related to the application configuration.
    /// </summary>
    ApplicationConfiguration,
    
    /// <summary>
    /// Represents the Key Vault where we store any keys and secrets associated to users.
    /// </summary>
    UserPersonalInformation,
}