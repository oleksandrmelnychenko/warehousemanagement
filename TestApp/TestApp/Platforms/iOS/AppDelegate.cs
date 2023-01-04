using Foundation;
using TestApp.MSALClient;
using UIKit;

namespace TestApp;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    private const string iOSRedirectURI = "msauth.com.companyname.mauiappbasic://auth"; // TODO - Replace with your redirectURI

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        // configure platform specific params
        PlatformConfig.Instance.RedirectUri = iOSRedirectURI;

        return base.FinishedLaunching(application, launchOptions);
    }

    public override bool OpenUrl(UIApplication application, NSUrl url, NSDictionary options)
    {
        return base.OpenUrl(application, url, options);
    }
}
