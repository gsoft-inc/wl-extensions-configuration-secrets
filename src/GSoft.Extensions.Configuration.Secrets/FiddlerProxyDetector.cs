using System;

namespace GSoft.Extensions.Configuration.Secrets;

internal static class FiddlerProxyDetector
{
    private static readonly Uri UrlThatCanBeProxiedByFiddler = new Uri("https://gsoft.com", UriKind.Absolute);
    private static readonly Uri FiddlerDefaultHttpProxy = new Uri("http://127.0.0.1:8888", UriKind.Absolute);
    private static readonly Uri FiddlerDefaultHttpsProxy = new Uri("https://127.0.0.1:8888", UriKind.Absolute);

    public static bool IsFiddlerActive()
    {
#if NET6_0_OR_GREATER
        var defaultProxy = System.Net.Http.HttpClient.DefaultProxy;
#else
        var defaultProxy = System.Net.WebRequest.DefaultWebProxy;
#endif

        var proxyUrl = defaultProxy.GetProxy(UrlThatCanBeProxiedByFiddler);
        if (proxyUrl == null)
        {
            return false;
        }

        const UriComponents partsToCompare = UriComponents.Scheme | UriComponents.Host | UriComponents.Port;
        return Uri.Compare(FiddlerDefaultHttpProxy, proxyUrl, partsToCompare, UriFormat.Unescaped, StringComparison.OrdinalIgnoreCase) == 0
            || Uri.Compare(FiddlerDefaultHttpsProxy, proxyUrl, partsToCompare, UriFormat.Unescaped, StringComparison.OrdinalIgnoreCase) == 0;
    }
}