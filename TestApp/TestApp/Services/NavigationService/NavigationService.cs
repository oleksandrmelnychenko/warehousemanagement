namespace TestApp.Services;
public class NavigationService : INavigationService
{
    readonly Shell shell;

    public INavigation Navigation
        => shell.Navigation;

    public NavigationService(AppShell shell)
        => this.shell = shell;

    public Task GoToAsync<TViewModel>(Dictionary<string, object>? parameters = null)
        => shell.GoToAsync(typeof(TViewModel).FullName, parameters ?? new());

    public Task GoToAsync(ShellNavigationState state)
        => shell.GoToAsync(state);

    public Task GoToAsync(ShellNavigationState state, bool animate)
        => shell.GoToAsync(state, animate);
}
