using Android.App;
using Android.Content;
using Microsoft.Identity.Client;

namespace TestApp.Platforms.Android.Resources;

[Activity(Exported = true)]
[IntentFilter(new[] { Intent.ActionView },
      Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
      DataHost = "auth",
      DataScheme = "msal0afe08d5-c0c2-43dd-893e-3672442f0a03")]
public class MsalActivity : BrowserTabActivity
{
}
