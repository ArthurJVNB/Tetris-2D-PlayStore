using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static Transform[] GetChildren(Transform parent)
    {
        Transform[] children = new Transform[parent.childCount];

        foreach (Transform child in parent)
        {
            int index = child.GetSiblingIndex();
            children[index] = child;
        }

        return children;
    }

    public static void DestroyChildren(Transform parent)
    {
        Transform[] children = GetChildren(parent);
        foreach (var child in children)
        {
            Object.Destroy(child.gameObject);
        }
    }
}
