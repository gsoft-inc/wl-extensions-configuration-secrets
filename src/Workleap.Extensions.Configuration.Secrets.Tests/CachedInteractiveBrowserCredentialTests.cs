namespace Workleap.Extensions.Configuration.Secrets.Tests;

public class CachedInteractiveBrowserCredentialTests
{
    [Fact]
    public void InteractiveBrowserCredential_Record_Property_Exists()
    {
        // This should throw if the property info can no longer be accessed through reflection
        _ = CachedInteractiveBrowserCredential.InternalRecordProperty;
    }
}