using UnityEngine;

/// <summary>
/// Class which contains certain useful methods with interact with the clipboard.
/// </summary>
public static class ClipboardUtils
{

    /// <summary>
    /// Copies a string to the clipboard.
    /// </summary>
    /// <param name="str"> The string to copy to the clipboard. </param>
    public static void CopyToClipboard(string str) => GUIUtility.systemCopyBuffer = str;

	/// <summary>
	/// Returns the string that is copied to the clipboard.
	/// </summary>
	/// <returns> The string on the clipboard. </returns>
	public static string GetClipboardString() => GUIUtility.systemCopyBuffer;

	/// <summary>
	/// Clears the clipboard and sets the contents to empty.
	/// </summary>
	public static void ClearClipboard() => CopyToClipboard("");

}