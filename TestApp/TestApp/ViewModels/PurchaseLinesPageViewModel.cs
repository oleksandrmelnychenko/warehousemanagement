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

        private string _accesToken = "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ii1LSTNROW5OUjdiUm9meG1lWm9YcWJIWkdldyIsImtpZCI6Ii1LSTNROW5OUjdiUm9meG1lWm9YcWJIWkdldyJ9.eyJhdWQiOiJodHRwczovL2FwaS5idXNpbmVzc2NlbnRyYWwuZHluYW1pY3MuY29tIiwiaXNzIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvMGFiYjE2MDItMTRkYS00Njg3LTg5NzgtZTliMzM1ZjgyNTE3LyIsImlhdCI6MTY3MjY3OTA5MSwibmJmIjoxNjcyNjc5MDkxLCJleHAiOjE2NzI2ODQxMzgsImFjciI6IjEiLCJhaW8iOiJBVFFBeS84VEFBQUFTUTNtVVF1TWZPUFA2ZkJ5aUVQZjFpMVpnUUpoQyt1d1JrSzh5ZGtBK0ZGQ1RROGg0N284czdzWWpIbGU5emU0IiwiYW1yIjpbInB3ZCJdLCJhcHBpZCI6IjBhZmUwOGQ1LWMwYzItNDNkZC04OTNlLTM2NzI0NDJmMGEwMyIsImFwcGlkYWNyIjoiMSIsImZhbWlseV9uYW1lIjoiSGF5IiwiZ2l2ZW5fbmFtZSI6IkplZmYiLCJpcGFkZHIiOiI3OC4xNTIuMTc1LjY3IiwibmFtZSI6IkplZmYgSGF5Iiwib2lkIjoiOTk0NDU0YzktM2E5MC00ZjBlLWE2ODItMWExMGU5ODE3MDFmIiwicHVpZCI6IjEwMDMyMDAyNTlEMDAxMEMiLCJyaCI6IjAuQVh3QUFoYTdDdG9VaDBhSmVPbXpOZmdsRnozdmJabHNzMU5CaGdlbV9Ud0J1Si03QUJjLiIsInNjcCI6IkZpbmFuY2lhbHMuUmVhZFdyaXRlLkFsbCB1c2VyX2ltcGVyc29uYXRpb24iLCJzdWIiOiJWQVhzRm4wcmtfWEV0RGNOZVpTajhzZllnYXJLTGw4a1dXWEFzVEtVaVQ4IiwidGlkIjoiMGFiYjE2MDItMTRkYS00Njg3LTg5NzgtZTliMzM1ZjgyNTE3IiwidW5pcXVlX25hbWUiOiJqZWZmaEBDUk1iYzM0NjQ4OS5Pbk1pY3Jvc29mdC5jb20iLCJ1cG4iOiJqZWZmaEBDUk1iYzM0NjQ4OS5Pbk1pY3Jvc29mdC5jb20iLCJ1dGkiOiJUaVlDRWtMUXVreVgxMndHaHRRREF3IiwidmVyIjoiMS4wIiwid2lkcyI6WyI2MmU5MDM5NC02OWY1LTQyMzctOTE5MC0wMTIxNzcxNDVlMTAiLCJiNzlmYmY0ZC0zZWY5LTQ2ODktODE0My03NmIxOTRlODU1MDkiXX0.qzekT_501sQ7pS2S4mY6ihl2_vGuTTC7n1eFIfc6htDKHvjmq0rtYVAbvlVGfO1vDzsJylv9w2uD__5HuMN6fCUXnFNZFPg_IHK8Df9CyP-xx5Q_obHDjaTZ3Vps9jDcBBwyJxlPpb-V48zenkGG9Kry3c7gw1dvd10-EU24AqoguL8xqGIBhofGUsEYt-_eEMPBhCNv6B1_DGTwmaZFctu1sCYCGhoblaVRH9DSzpQ6frPfHegCVxfe-CgqRPkQYfbbmeIr8wAhxHazZMRllZRuymfF0FDjbDK19KLqm5hW4LPPPhqX488-F91y19nkfC6e7UKl3GkBo4jMOx_hYQ";

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
