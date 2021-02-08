using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionLibrary : MonoBehaviour
{
    public static T GetIfNull<T>(GameObject owner, T component) where T : Component
    {
        return component ? component : owner.GetComponent<T>();
    }
}
