using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace TestApp.MSALClient
{
    public class MSALClientHelper
    {
        public PublicClientApplicationBuilder PublicClientApplicationBuilder { get; set; }

        public AuthenticationResult AuthenticationResult { get; set; }

        public GraphServiceClient GraphServiceClient { get; set; }

        public IPublicClientApplication PublicClientApplication { get; set; }

        public MSALClientHelper()
        {
            PublicClientApplicationBuilder = PublicClientApplicationBuilder
               .Create(AppConstants.ClientId)
               .WithAuthority(string.Format(AppConstants.Authority, AppConstants.TenantId))
               .WithExperimentalFeatures()
               .WithClientCapabilities(new string[] { "cp1" })
               .WithIosKeychainSecurityGroup("com.microsoft.adalcache");
        }

        public async Task<AuthenticationResult> InitializePublicClientAppAsync()
        {
            PublicClientApplication = PublicClientApplicationBuilder
#if WINDOWS
                                        .WithRedirectUri(PlatformConfig.Instance.RedirectUri)
#else
                                        .WithRedirectUri($"msal{AppConstants.ClientId}://auth")
#endif
                                        .Build();

            SystemWebViewOptions systemWebViewOptions = new SystemWebViewOptions();

#if IOS
            systemWebViewOptions.iOSHidePrivacyPrompt = true;
#endif

            AuthenticationResult = await PublicClientApplication
                                            .AcquireTokenInteractive(AppConstants.Scopes)
                                            .WithAuthority(AppConstants.Authority)
#if IOS
                                            .WithTenantId(AppConstants.TenantId)
#endif
                                            .WithSystemWebViewOptions(systemWebViewOptions)
                                            .WithParentActivityOrWindow(PlatformConfig.Instance.ParentWindow)
                                            .ExecuteAsync().ConfigureAwait(false);

            return AuthenticationResult;
        }

    }
}
