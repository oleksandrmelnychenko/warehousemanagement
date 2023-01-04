using CommunityToolkit.Maui;
using System.ComponentModel;

namespace TestApp.Extensions;

public static class NavigationExtensions
{
    public static IServiceCollection AddTransientWithShellRoute<TPage, TViewModel>(this IServiceCollection serviceCollection)
    where TPage : NavigableElement
    where TViewModel : class, INotifyPropertyChanged
    => serviceCollection.AddTransientWithShellRoute<TPage, TViewModel>(typeof(TViewModel).FullName!);

    public static void RemoveLastFromBackStack(this INavigation navigation)
        => navigation.RemovePage(navigation.NavigationStack[navigation.NavigationStack.Count - 2]);

    public static void RemoveFromBackStackUntil<TViewModel>(this INavigation navigation)
    {
        var stack = navigation.NavigationStack.ToList();

        if (stack.LastOrDefault(p => p.BindingContext.GetType().FullName == typeof(TViewModel).FullName) is not Page targetPage)
        {
            return;
        }

        var stepsToForBackStack = stack.Count - 1 - stack.IndexOf(targetPage);

        for (int i = 0; i < stepsToForBackStack; i++)
        {
            navigation.RemoveLastFromBackStack();
        }
    }
}
