using UnityEngine;
using System.Collections.Generic;

public class ThisOtherTester
{
    private readonly List<GameObject> stuffs = new List<GameObject>();

    [ReflectionProtect]
    void DoStuff()
    {
    }

    [ReflectionProtect]
    void DoingLotsOfStuff() { }

    [ReflectionProtect(typeof(List<GameObject>))]
    private List<GameObject> GetGameObjects()
    {
        return stuffs;
    }
}
