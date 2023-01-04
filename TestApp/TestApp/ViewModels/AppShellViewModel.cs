using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using TestApp.Models;
using TestApp.MSALClient;

namespace TestApp.ViewModels
{
    public sealed partial class AppShellViewModel : ObservableObject
    {
        [ObservableProperty]
        AuthenticationResult authenticationResult;

        [RelayCommand]
        async Task Logout()
        {
            //await PCAWrapper.Instance.SignOutAsync().ConfigureAwait(false);
            await ServiceHelper.GetService<INavigationService>().GoToAsync("///initial", false);
        }

        public AppShellViewModel()
        {
            WeakReferenceMessenger.Default.Register<AuthenticationMessage>(this, (a, s) =>
            {
                AuthenticationResult = s.AuthenticationResult;
            });            
        }         
    }
}
