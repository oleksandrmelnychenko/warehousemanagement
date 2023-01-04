using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.ViewModels
{
    public partial class BaseViewModelWithResult<T> : ViewModelBase
    {
        public virtual T? DismissCommandResult { get; protected set; }
        protected TaskCompletionSource<T>? TCS { get; private set; }

        public BaseViewModelWithResult(INavigationService navigationService) : base(navigationService)
        {
        }

        protected static async Task<T?> ShowWithResult<TViewModel>(INavigationService navigationService, Dictionary<string, object>? parameters = default, TaskCompletionSource<T>? tcs = default)
            where TViewModel : ViewModelBase
        {
            await Show<TViewModel>(navigationService, parameters, tcs ??= new());

            return await tcs.Task;
        }

        protected static Task Show<TViewModel>(INavigationService navigationService, Dictionary<string, object>? parameters = default, TaskCompletionSource<T>? tcs = default)
            where TViewModel : ViewModelBase => navigationService.GoToAsync<TViewModel>(Initialize(tcs, parameters));

        public static Dictionary<string, object> Initialize(TaskCompletionSource<T>? tcs, Dictionary<string, object>? parameters = default)
        {
            (parameters ??= new()).Add(nameof(TCS), tcs ?? new());

            return parameters;
        }

        public override Task Initialize(IDictionary<string, object> query)
        {
            TCS = query.TryGetValue<TaskCompletionSource<T>>(nameof(TCS));
            return base.Initialize(query);
        }

        public override void OnNavigatingFrom(bool back)
        {
            base.OnNavigatingFrom(back);

            if (back && GetOtherResultPages()?.Any() != true)
            {
                TCS?.TrySetResult(DismissCommandResult!);
            }
        }

        protected Task ReturnWithResult(T? result)
        {
            GetOtherResultPages()?.ToList().ForEach(NavigationService.Navigation.RemovePage);

            DismissCommandResult = result;

            return Back();
        }

        IEnumerable<Page>? GetOtherResultPages()
            => NavigationService.Navigation?.NavigationStack?.Where(p => p?.BindingContext != null && p.BindingContext != this && p.BindingContext is BaseViewModelWithResult<T> vm && vm.TCS == TCS);
    }
}
