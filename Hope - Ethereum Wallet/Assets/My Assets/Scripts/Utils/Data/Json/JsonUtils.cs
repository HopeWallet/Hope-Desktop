using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Dynamic;
using System.IO;

/// <summary>
/// Class which contains extra json utility methods.
/// </summary>
public static class JsonUtils
{
    /// <summary>
    /// Gets the object from the json string.
    /// </summary>
    /// <typeparam name="T"> The type of the object. </typeparam>
    /// <param name="jsonString"> The json string data. </param>
    /// <returns> The object created from the json. </returns>
    public static T Deserialize<T>(string jsonString) where T : class => string.IsNullOrEmpty(jsonString) ? null : JsonConvert.DeserializeObject<T>(jsonString);

    /// <summary>
    /// Deserializes the json data into an <see cref="ExpandoObject"/>
    /// </summary>
    /// <param name="jsonString"> The json string data. </param>
    /// <returns> The deserialized object. </returns>
    public static dynamic DeserializeDynamic(string jsonString) => string.IsNullOrEmpty(jsonString) ? null : (dynamic)JsonConvert.DeserializeObject<ExpandoObject>(jsonString, new ExpandoObjectConverter());

    /// <summary>
    /// Serializes an object into the respective string json format.
    /// </summary>
    /// <param name="jsonObject"> The object to serialize into json. </param>
    /// <returns> The newly serialized json object. </returns>
    public static string Serialize(object jsonObject) => jsonObject == null ? null : JsonConvert.SerializeObject(jsonObject);
}