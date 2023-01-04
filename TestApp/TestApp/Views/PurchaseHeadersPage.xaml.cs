namespace TestApp.Views;

public partial class PurchaseHeadersPage : BaseContentPage
{
    private PurchaseHeadersPageViewModel _vm;

    public PurchaseHeadersPage(PurchaseHeadersPageViewModel vm) : base(vm)
    {
        _vm = vm; 
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.Initialize(null);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        tt.SelectedItem = null!;
    }
}