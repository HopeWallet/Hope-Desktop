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
    public static T GetJsonData<T>(string jsonString) where T : class => jsonString == null ? null : JsonConvert.DeserializeObject<T>(jsonString);

    /// <summary>
    /// Writes a json object to a path.
    /// </summary>
    /// <typeparam name="T"> The type of object to write to the path. </typeparam>
    /// <param name="jsonObject"> The object to save a json object for. </param>
    /// <param name="path"> The path to save the json file. </param>
    public static void WriteJsonFile<T>(T jsonObject, string path) => File.WriteAllText(path, JsonConvert.SerializeObject(jsonObject));

}
