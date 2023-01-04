namespace TestApp.Services;
public interface INavigationService
{
    INavigation Navigation { get; }

    Task GoToAsync(ShellNavigationState state);

    Task GoToAsync(ShellNavigationState state, bool animate);

    Task GoToAsync<TViewModel>(Dictionary<string, object> parameters = default);
}

