using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using TestApp.Models;
using TestApp.MSALClient;

namespace TestApp.ViewModels
{
    public sealed partial class PurchaseHeadersPageViewModel : ViewModelBase
    {
        private readonly JsonSerializerOptions _serializerOptions;

        [ObservableProperty]
        bool? isLoading;

        [ObservableProperty]
        string? email;

        [ObservableProperty]
        ObservableCollection<PurchaseHeaderModel> purchaseHeaderModels = default!;

        public GraphServiceClient _graphServiceClient { get; set; }

        [RelayCommand]
        async Task ItemSelected(PurchaseHeaderModel item)
        {
            await NavigationService.GoToAsync<PurchaseLinesPageViewModel>(new Dictionary<string, object>()
            {
                { "item", item }
            });
        }

        public PurchaseHeadersPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Purchase Headers";
            
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
            };
        }
       
        public override Task Initialize(IDictionary<string, object> query)
        {
            IsLoading = !PurchaseHeaderModels?.Any();

            _ = InitializeGraphServiceClientAsync(UserAccount.AuthenticationResult.AccessToken);

            Email = UserAccount.AuthenticationResult.Account.Username;

            _ = GetDataAsync();

            return base.Initialize(query);
        }

        public override void OnNavigatingFrom(bool back)
        {
            base.OnNavigatingFrom(back);
        }

        private async Task GetDataAsync()
        {
            IsLoading = true;

            try
            {
                var result = await CallPurchaseHeaders();             

                if (result != null)
                {
                    PurchaseHeaderModels = new ObservableCollection<PurchaseHeaderModel>(result);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR------: {ex.Message}");
            }

            IsLoading = false;
        }

        /// <summary>
        /// Bootstraps the MS Graph SDK with the provided token and returns it for use
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>
        /// A GraphServiceClient (MS Graph SDK) instance
        /// </returns>
        private async Task<GraphServiceClient> InitializeGraphServiceClientAsync(string token)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            _graphServiceClient = new GraphServiceClient(client);

            return await Task.FromResult(_graphServiceClient);
        }

        private async Task<List<PurchaseHeaderModel>> CallPurchaseHeaders()
        {
            try
            {
                //get data from API
                HttpClient client = new HttpClient();

                string url = "https://api.businesscentral.dynamics.com/v2.0/0abb1602-14da-4687-8978-e9b335f82517/WHS/api/fits/bcwhs/v1.0/companies(e8494bb9-df6c-ed11-81b5-000d3a220355)/purchaseHeaders";

                // create the request
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, url);              

                // ** Add Authorization Header **
                message.Headers.Add("Authorization", $"Bearer {UserAccount.AuthenticationResult.AccessToken}");

                // send the request and return the response
                HttpResponseMessage response = await client.SendAsync(message).ConfigureAwait(false);

                var result = await HandleResponse<OdataModel<List<PurchaseHeaderModel>>>(response);

                return result.Value;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"----error--------------{ex.Message}");
                return null;
            }
        }

        async Task<T> HandleResponse<T>(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return default!;
            }

            string json = string.Empty;

            try
            {
                if (response is not null)
                {
                    using var jsonStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    {
                        if (jsonStream.Length == 0)
                        {
                            return JsonSerializer.Deserialize<T>("{}", _serializerOptions)!;
                        }
                        return JsonSerializer.Deserialize<T>(jsonStream, _serializerOptions)!;
                    }
                }
                return default!;
            }
            catch (Exception ex)
            {
                ex.Data.Add("Details", $"PARSE ERROR\r\n===========\r\nURL: {response?.RequestMessage?.RequestUri?.AbsoluteUri}\r\nRESPONSE: {json}");
                throw;
            }
        }
    }
}
