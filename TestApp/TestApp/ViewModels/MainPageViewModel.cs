using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp.Models;
using TestApp.MSALClient;

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
                WeakReferenceMessenger.Default.Send(new AuthenticationMessage());

                var helper = new MSALClientHelper();
                UserAccount.AuthenticationResult = await helper.InitializePublicClientAppAsync();

                //WeakReferenceMessenger.Default.Send(new AuthenticationMessage(UserAccount.AuthenticationResult));

                //await _navigationService.GoToAsync<PurchaseHeadersPageViewModel>();
                await _navigationService.GoToAsync("//purchaseHeaders");
            }
            catch (MsalUiRequiredException)
            {
                // This executes UI interaction to obtain token
                AuthenticationResult result = await PCAWrapper.Instance.AcquireTokenInteractiveAsync(AppConstants.Scopes).ConfigureAwait(false);
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
    }
}
