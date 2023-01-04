namespace TestApp.Views;

public partial class PurchaseLinesPage : BaseContentPage
{
    private PurchaseLinesPageViewModel _vm;

    public PurchaseLinesPage(PurchaseLinesPageViewModel vm) : base(vm)
    {
        _vm = vm;
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();        
    }
}