using System.Text.Json;

namespace TestApp.Extensions;

public static class CommonExtensions
{
    public static readonly string GuidEmptyString = Guid.Empty.ToString();

    public static bool IsNullOrEmpty(this Guid? guid)
        => guid == null || guid == Guid.Empty;

    public static T DeepClone<T>(this T input)
        where T : class => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(input))!;

    public static JsonElement GetElement(this JsonElement.ObjectEnumerator enumerator, string propertyName)
        => enumerator.FirstOrDefault(p => string.Compare(p.Name, propertyName, true) == 0).Value;

    public static T TryGetValue<T>(this IDictionary<string, object> query, string key)
        => query?.TryGetValue(key, out var outValue) == true && outValue is T outTValue ? outTValue : default!;
}
