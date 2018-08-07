using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class which contains some utility methods for transforms.
/// </summary>
public static class TransformUtils
{
    /// <summary>
    /// Gets a list of children of a given transform.
    /// </summary>
    /// <param name="transform"> The transform to get the children of. </param>
    /// <returns> The list of child transforms of the original transform. </returns>
    public static List<Transform> GetChildrenTransformList(this Transform transform)
    {
        List<Transform> children = new List<Transform>();

        foreach (Transform child in transform)
            children.Add(child);

        return children;
    }
}