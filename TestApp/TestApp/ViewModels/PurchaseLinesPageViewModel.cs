using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Identity.Client;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using TestApp.Models;
using TestApp.MSALClient;

namespace TestApp.ViewModels
{
    public sealed partial class PurchaseLinesPageViewModel : ViewModelBase
    {
        private readonly JsonSerializerOptions _serializerOptions;      

        [ObservableProperty]
        bool? isLoading;

        [ObservableProperty]
        ObservableCollection<PurchaseLineModel> purchaseLineModels = default!;

        [RelayCommand]
        Task ItemSelected(PurchaseLineModel item)
        {
            return Task.CompletedTask;
        }

        public PurchaseLinesPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Purchase Lines";

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
            var item = query.TryGetValue<PurchaseHeaderModel>("item");

            _ = GetDataAsync(item);

            return base.Initialize(query);
        }
    
        private async Task GetDataAsync(PurchaseHeaderModel item = default)
        {
            IsLoading = true;

            try
            {
                var result = await CallPurchaseLines(null, item.No);
                if (result != null)
                {
                    PurchaseLineModels = new ObservableCollection<PurchaseLineModel>(result);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR------: {ex.Message}");
            }

            IsLoading = false;
        }

        private async Task<List<PurchaseLineModel>> CallPurchaseLines(AuthenticationResult authResult, long id)
        {
            try
            {
                //get data from API
                HttpClient client = new HttpClient();
                
                string url = $"https://api.businesscentral.dynamics.com/v2.0/0abb1602-14da-4687-8978-e9b335f82517/WHS/api/fits/bcwhs/v1.0/companies(e8494bb9-df6c-ed11-81b5-000d3a220355)/purchaseLines?$filter=documentNo eq '{id}'";

                // create the request
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, url);

                var accesToken = authResult?.CreateAuthorizationHeader();

                // ** Add Authorization Header **
                message.Headers.Add("Authorization", $"Bearer {UserAccount.AuthenticationResult.AccessToken}");

                // send the request and return the response
                HttpResponseMessage response = await client.SendAsync(message).ConfigureAwait(false);

                var result = await HandleResponse<OdataModel<List<PurchaseLineModel>>>(response);

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
