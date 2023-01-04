using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace TestApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Feathericons.ttf", "FI");
            });

        builder.UseMauiCommunityToolkit();

        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<AppShellViewModel>();      
        builder.Services.AddSingleton<INavigationService, NavigationService>();

        builder.Services.AddTransientWithShellRoute<PurchaseHeadersPage, PurchaseHeadersPageViewModel>();
        builder.Services.AddTransientWithShellRoute<PurchaseLinesPage, PurchaseLinesPageViewModel>();
        builder.Services.AddTransientWithShellRoute<MainPage, MainPageViewModel>();


#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
