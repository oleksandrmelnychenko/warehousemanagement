using System.Text.Json.Serialization;

namespace TestApp.Models
{
    public sealed class OdataModel<T>
    {
        [JsonPropertyName("@odata.context")]
        public string OdataContext { get; set; }
        public T Value { get; set; }
    }
}
