using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TestApp.Models;
using TestApp.MSALClient;
using static System.Formats.Asn1.AsnWriter;

namespace TestApp.ViewModels
{
    public sealed partial class MainPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        [RelayCommand]
        void OpenMenu()
        {
            try
            {
#pragma warning disable CA1416 // Validate platform compatibility
                Shell.Current.FlyoutIsPresented = true;
#pragma warning restore CA1416 // Validate platform compatibility
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"-------------ERROR:{ex.Message}");
            }
        }

        [RelayCommand]
        async Task SignIn()
        {
            try
            {
                if (UserAccount.AuthenticationResult == null)
                {
                    var helper = new MSALClientHelper();

                    UserAccount.AuthenticationResult = await helper.InitializePublicClientAppAsync();

                   // UserAccount.AuthenticationResult = await helper.RefreshTokenAsync();

                   var tt = await helper.PublicClientApplication.GetAccountsAsync();

                    var x = await helper.PublicClientApplication.AcquireTokenSilent(new string[] { UserAccount.AuthenticationResult.IdToken, "https://api.businesscentral.dynamics.com/.default" },
                        UserAccount.AuthenticationResult.Account).ExecuteAsync();

                    WeakReferenceMessenger.Default.Send(new AuthenticationMessage(UserAccount.AuthenticationResult));

                    await _navigationService.GoToAsync<PurchaseHeadersPageViewModel>();
                }
                else
                {
                    await _navigationService.GoToAsync<PurchaseHeadersPageViewModel>();
                }
            }
            catch (MsalUiRequiredException ex)
            {
                // This executes UI interaction to obtain token
                Debug.WriteLine($"-------------ERROR({nameof(MainPageViewModel)}):{ex.Message}");
                AuthenticationResult result = null;
                UserAccount.AuthenticationResult = result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"-------------ERROR({nameof(MainPageViewModel)}):{ex.Message}");
            }
        }

        public MainPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;

            Title = "Authorization";
        }

        private readonly string GraphUrl = "https://graph.microsoft.com/v1.0/me";

        private async Task<User> RefreshUserDataAsync(string token)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, this.GraphUrl);
            message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
            HttpResponseMessage response = await client.SendAsync(message);
            User currentUser = null;

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                currentUser = JsonSerializer.Deserialize<User>(json);
            }

            return currentUser;
        }
    }
}
