using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Identity.Client;
using System.Diagnostics;
using TestApp.Models;
using TestApp.MSALClient;

namespace TestApp.ViewModels
{
    public sealed partial class MainPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        private MSALClientHelper _helper;

        [RelayCommand]
        void OpenMenu()
        {
            try
            {
                Shell.Current.FlyoutIsPresented = !Shell.Current.FlyoutIsPresented;
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
                    _helper = new MSALClientHelper();

                    UserAccount.AuthenticationResult = await _helper.InitializePublicClientAppAsync();

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

        [RelayCommand]
        async Task TestSilent()
        {
            if (_helper == null)
            {
                return;
            }

            try
            {
                var authResultWithSilentMode = await _helper.RefreshTokenAsync(_helper);

                UserAccount.AuthenticationResult = authResultWithSilentMode;

                WeakReferenceMessenger.Default.Send(new AuthenticationMessage(UserAccount.AuthenticationResult));

                await _navigationService.GoToAsync<PurchaseHeadersPageViewModel>();
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


    }
}
