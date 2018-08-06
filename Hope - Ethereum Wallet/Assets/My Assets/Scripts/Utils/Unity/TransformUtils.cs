using System.Collections.Generic;
using UnityEngine;

public static class TransformUtils
{

    public static List<Transform> GetChildrenTransformList(this Transform transform)
    {
        List<Transform> children = new List<Transform>();

        foreach (Transform child in transform)
            children.Add(child);

        return children;
    }
}