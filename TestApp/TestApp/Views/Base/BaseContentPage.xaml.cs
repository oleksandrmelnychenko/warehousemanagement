namespace TestApp.Views;

public partial class BaseContentPage : ContentPage
{
	public BaseContentPage(object? bindingContext = default)
	{
        BindingContext = bindingContext;
        InitializeComponent();       
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);

        (BindingContext as ViewModelBase)?.OnNavigatingFrom(Shell.Current.Navigation.NavigationStack?.Contains(this) == false);
    }
}