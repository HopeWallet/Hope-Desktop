using UnityEngine;

/// <summary>
/// Class which has extension and utility methods for gameobjects.
/// </summary>
public static class GameObjectUtils
{

    /// <summary>
    /// Selects the highest parent object of this gameobject.
    /// </summary>
    /// <param name="gameObject"> The gameobject to get the parent for. </param>
    /// <returns> The highest parent gameobject of the input gameobject. If no parent exists, returns the input gameobject. </returns>
    public static GameObject SelectParent(this GameObject gameObject)
    {
        var correctTransform = gameObject.transform;
        var currentTransform = correctTransform;

        while ((currentTransform = correctTransform.parent) != null)
            correctTransform = currentTransform;

        return correctTransform.gameObject;
    }

}