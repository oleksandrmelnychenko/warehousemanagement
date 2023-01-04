using Microsoft.Identity.Client;
using System.Text;
using TestApp.MSALClient;

namespace TestApp;

public partial class MainPage : BaseContentPage
{ 
    public MainPage(MainPageViewModel vm) : base(vm) => InitializeComponent();
}

