namespace TestApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();		
	}

    protected override Window CreateWindow(IActivationState? activationState)
    {
        MainPage = ServiceHelper.GetService<AppShell>();
        return base.CreateWindow(activationState);
    }

}
