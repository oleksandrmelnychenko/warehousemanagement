﻿using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Content;
using TestApp.MSALClient;
using Microsoft.Identity.Client;
using Android.Runtime;

namespace TestApp;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    private const string AndroidRedirectURI = $"msal{AppConstants.ClientId}://auth";

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        // configure platform specific params
        PlatformConfig.Instance.RedirectUri = AndroidRedirectURI;
        PlatformConfig.Instance.ParentWindow = this;       
    }

    /// <summary>
    /// This is a callback to continue with the authentication
    /// Info about redirect URI: https://docs.microsoft.com/en-us/azure/active-directory/develop/msal-client-application-configuration#redirect-uri
    /// </summary>
    /// <param name="requestCode">request code </param>
    /// <param name="resultCode">result code</param>
    /// <param name="data">intent of the actvity</param>
    protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
    }
}
