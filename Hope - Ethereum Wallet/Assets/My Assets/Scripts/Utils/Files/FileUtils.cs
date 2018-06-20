using System;
using System.IO;

/// <summary>
/// Class which has certain utility methods related to file handling.
/// </summary>
public static class FileUtils
{

    /// <summary>
    /// Reads the text from a file if it exists. Returns null if the file doesn't exist.
    /// </summary>
    /// <param name="path"> The path to read the text from. </param>
    /// <returns> The text contained in the file. </returns>
    public static string ReadFileText(string path)
    {
        string text = null;

        try
        {
            using (StreamReader sr = new StreamReader(path))
            {
                text = sr.ReadToEnd();
            }
        }
        catch (Exception e)
        {
            ExceptionManager.DisplayException(e);
        }

        return text;
    }

}
