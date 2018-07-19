using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ReflectionHelper
{

    public static void InvokeAttributeMethod<T>(Type type, object[] parameters) where T : Attribute
    {
        foreach (var method in type.GetMethods())
            if (Attribute.IsDefined(method, typeof(T)))
                method.Invoke(null, parameters);
    }

    public static bool HasMethodAttributes<T, V>(Type type)
    {
        bool hasT = false, hasV = false;

        foreach(var method in type.GetMethods())
        {
            hasT = Attribute.IsDefined(method, typeof(T)) || hasT;
            hasV = Attribute.IsDefined(method, typeof(V)) || hasV;
        }

        return hasT && hasV;
    }

}
