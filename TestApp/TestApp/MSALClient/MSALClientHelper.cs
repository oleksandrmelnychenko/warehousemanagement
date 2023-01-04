using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;
using static System.Formats.Asn1.AsnWriter;

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

        public async Task<AuthenticationResult> RefreshTokenAsync() {
            AuthenticationResult = await this.PublicClientApplication
                            .AcquireTokenSilent(AppConstants.Scopes, Microsoft.Identity.Client.PublicClientApplication.OperatingSystemAccount)
                            .ExecuteAsync()
                            .ConfigureAwait(false);

            return AuthenticationResult;
        }


        public async Task<AuthenticationResult> InitializePublicClientAppAsync()
        {
            PublicClientApplication = PublicClientApplicationBuilder
                .WithRedirectUri($"msal{AppConstants.ClientId}://auth")
                .Build();

            SystemWebViewOptions systemWebViewOptions = new SystemWebViewOptions();

            AuthenticationResult = await PublicClientApplication
                .AcquireTokenInteractive(AppConstants.Scopes)
                .WithAuthority(AppConstants.Authority)
                .WithSystemWebViewOptions(systemWebViewOptions)
                .WithParentActivityOrWindow(PlatformConfig.Instance.ParentWindow)
                .ExecuteAsync()
                .ConfigureAwait(false);

            //var tt = await AttachTokenCache(); 

            return AuthenticationResult;
        }

        /// <summary>
        /// Attaches the token cache to the Public Client app.
        /// </summary>
        /// <returns>IAccount list of already signed-in users (if available)</returns>
        private async Task<IEnumerable<IAccount>> AttachTokenCache()
        {   
            var storageProperties = new StorageCreationPropertiesBuilder("netcore_maui_cache.txt", "D:/temp")
                    .Build();

            var msalcachehelper = await MsalCacheHelper.CreateAsync(storageProperties);
            msalcachehelper.RegisterCache(PublicClientApplication.UserTokenCache);

            // If the cache file is being reused, we'd find some already-signed-in accounts
            return await PublicClientApplication.GetAccountsAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Sign in user using MSAL and obtain a token for MS Graph
        /// </summary>
        /// <returns>GraphServiceClient</returns>
        //private async Task<GraphServiceClient> SignInAndInitializeGraphServiceClient()
        //{
        //    string token = await this.MSALClient.SignInUserAndAcquireAccessToken(this.GraphScopes);
        //    return await InitializeGraphServiceClientAsync(token);
        //}
    }
}
