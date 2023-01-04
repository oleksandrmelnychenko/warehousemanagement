namespace TestApp.MSALClient;

/// <summary>
/// Defines constants for the app.
/// Please change them for your app
/// </summary>
internal class AppConstants
{
    // ClientID of the application in (sample-testing.com)
    internal const string ClientId = "0afe08d5-c0c2-43dd-893e-3672442f0a03"; // TODO - Replace with your client Id. And also replace in the AndroidManifest.xml

    internal const string Authority = "https://login.microsoftonline.com/0abb1602-14da-4687-8978-e9b335f82517/oauth2/authorize";

    internal const string TenantId = "0abb1602-14da-4687-8978-e9b335f8251";

    /// <summary>
    /// Scopes defining what app can access in the graph
    /// </summary>
    internal static string[] Scopes = { "https://api.businesscentral.dynamics.com/.default" };
}
