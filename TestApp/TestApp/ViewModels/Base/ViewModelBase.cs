using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TestApp.ViewModels
{
    public abstract partial class ViewModelBase : ObservableObject, IQueryAttributable
    {
        bool initialized;

        [ObservableProperty]
        string? title, doneButtonTitle;

        [ObservableProperty]
        Color? mainColor;

        protected INavigationService NavigationService { get; }

        public ViewModelBase(INavigationService navigationService)
            => NavigationService = navigationService;

        [RelayCommand]
        public virtual Task Back() => NavigationService.GoToAsync("..", true);

        [RelayCommand]
        public virtual Task Done() => Task.CompletedTask;

        public virtual void OnNavigatingFrom(bool back) { }

        public virtual Task Initialize(IDictionary<string, object> query)
        {
            Title = query.TryGetValue<string>(nameof(Title)) ?? Title;
            MainColor = query.TryGetValue<Color>(nameof(MainColor)) ?? MainColor;
            return Task.CompletedTask;
        }

        void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (initialized)
            {
                return;
            }

            initialized = true;
            Initialize(query);
        }
    }
}
