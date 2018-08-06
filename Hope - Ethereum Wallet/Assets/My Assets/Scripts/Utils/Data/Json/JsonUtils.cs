using Newtonsoft.Json;
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
    /// <param name="jsonString"> The string of json text. </param>
    /// <returns> The object created from the text. </returns>
    public static T Deserialize<T>(string jsonString) where T : class => string.IsNullOrEmpty(jsonString) ? null : JsonConvert.DeserializeObject<T>(jsonString);

    /// <summary>
    /// Serializes an object into the respective string json format.
    /// </summary>
    /// <param name="jsonObject"> The object to serialize into json format. </param>
    /// <returns> The newly serialized json object. </returns>
    public static string Serialize(object jsonObject) => jsonObject == null ? null : JsonConvert.SerializeObject(jsonObject);

    /// <summary>
    /// Writes a json object to a path.
    /// </summary>
    /// <typeparam name="T"> The type of object to write to the path. </typeparam>
    /// <param name="jsonObject"> The object to save a json object for. </param>
    /// <param name="path"> The path to save the json file. </param>
    public static void WriteJsonFile<T>(T jsonObject, string path) => File.WriteAllText(path, JsonConvert.SerializeObject(jsonObject));
}