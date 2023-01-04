using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;
using System.Diagnostics;
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

        public async Task<AuthenticationResult> RefreshTokenAsync(MSALClientHelper helper)
        {
            try
            {
                var accounts = await helper.PublicClientApplication.GetAccountsAsync();
                if (accounts == null || accounts.Count() > 1)
                {
                    return AuthenticationResult;
                }
                var account = accounts.SingleOrDefault();

                AuthenticationResult =
                    await helper.PublicClientApplication.AcquireTokenSilent(AppConstants.Scopes, account)
                    .WithForceRefresh(true)
                    .ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                Debug.WriteLine($"-------------ERROR({nameof(MainPageViewModel)}):{ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"-------------ERROR({nameof(MainPageViewModel)}):{ex.Message}");
            }

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

            return AuthenticationResult;
        }
    }
}
