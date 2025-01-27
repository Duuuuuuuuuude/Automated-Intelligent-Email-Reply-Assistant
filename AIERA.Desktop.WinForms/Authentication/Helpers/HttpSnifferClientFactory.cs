//using Microsoft.Identity.Client;

//namespace AIERA.Desktop.WinForms.Authentication.Helpers;

///// <summary>
///// This client should not be used in production and only for logging.
///// https://learn.microsoft.com/da-dk/entra/msal/dotnet/advanced/exceptions/msal-logging?tabs=dotnet#network-traces
///// </summary>
//public class HttpSnifferClientFactory : IMsalHttpClientFactory
//{
//    readonly HttpClient _httpClient;

//    public IList<(HttpRequestMessage, HttpResponseMessage)> RequestsAndResponses { get; }

//    public static string LastHttpContentData { get; set; }

//    public HttpSnifferClientFactory()
//    {
//        RequestsAndResponses = new List<(HttpRequestMessage, HttpResponseMessage)>();

//        var recordingHandler = new RecordingHandler((req, res) =>
//        {
//            if (req.Content != null)
//            {
//                req.Content.LoadIntoBufferAsync().GetAwaiter().GetResult();
//                LastHttpContentData = req.Content.ReadAsStringAsync().GetAwaiter().GetResult();
//            }
//            RequestsAndResponses.Add((req, res));
//            Trace.WriteLine($"[MSAL][HTTP Request]: {req}");
//            Trace.WriteLine($"[MSAL][HTTP Response]: {res}");
//        });
//        recordingHandler.InnerHandler = new HttpClientHandler();
//        _httpClient = new HttpClient(recordingHandler);
//    }

//    public HttpClient GetHttpClient()
//    {
//        return _httpClient;
//    }
//}