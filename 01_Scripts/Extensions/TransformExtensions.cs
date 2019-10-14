using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class TransformExtensions
{
    public static string GetFullPath(this Transform transform, string delimiter = ".", string prefix = "/")
    {
        StringBuilder stringBuilder = new StringBuilder();
        GetFullPath(stringBuilder, transform, delimiter, prefix);
        return stringBuilder.ToString();
    }

    private static void GetFullPath(StringBuilder stringBuilder, Transform transform, string delimiter, string prefix)
    {
        if (transform.parent == null)
        {
            stringBuilder.Append(prefix);
        }
        else
        {
            GetFullPath(stringBuilder, transform.parent, delimiter, prefix);
            stringBuilder.Append(delimiter);
        }
        stringBuilder.Append(transform.name);
    }

    public static IEnumerable<Transform> EnumerateHierarchy(this Transform root)
    {
        if (root == null) { throw new ArgumentNullException("root"); }
        return root.EnumerateHierarchyCore(new List<Transform>(0));
    }

    public static IEnumerable<Transform> EnumerateHierarchy(this Transform root, ICollection<Transform> ignore)
    {
        if (root == null) { throw new ArgumentNullException("root"); }
        if (ignore == null)
        {
            throw new ArgumentNullException("ignore", "Ignore collection can't be null, use EnumerateHierarchy(root) instead.");
        }
        return root.EnumerateHierarchyCore(ignore);
    }

    private static IEnumerable<Transform> EnumerateHierarchyCore(this Transform root, ICollection<Transform> ignore)
    {
        var transformQueue = new Queue<Transform>();
        transformQueue.Enqueue(root);

        while (transformQueue.Count > 0)
        {
            var parentTransform = transformQueue.Dequeue();

            if (!parentTransform || ignore.Contains(parentTransform)) { continue; }

            for (var i = 0; i < parentTransform.childCount; i++)
            {
                transformQueue.Enqueue(parentTransform.GetChild(i));
            }

            yield return parentTransform;
        }
    }

    public static Bounds GetColliderBounds(this Transform transform)
    {
        Collider[] colliders = transform.GetComponentsInChildren<Collider>();
        if (colliders.Length == 0) { return new Bounds(); }

        Bounds bounds = colliders[0].bounds;
        for (int i = 1; i < colliders.Length; i++)
        {
            bounds.Encapsulate(colliders[i].bounds);
        }
        return bounds;
    }

    public static bool IsParentOrChildOf(this Transform transform1, Transform transform2)
    {
        return transform1.IsChildOf(transform2) || transform2.IsChildOf(transform1);
    }

    public static void Identity(this Transform transform)
    {
        transform.transform.localPosition = Vector3.zero;
        transform.transform.localRotation = Quaternion.identity;
        transform.transform.localScale = Vector3.one;
    }

    public static void IdentityNonScale(this Transform transform)
    {
        transform.transform.localPosition = Vector3.zero;
        transform.transform.localRotation = Quaternion.identity;
    }

    public static void SetLocalPosition(this Transform transform, Vector3 position, Space space = Space.Self)
    {
        if (space == Space.World)
            transform.position = position;
        else
            transform.localPosition = position;
    }

    public static TElement GetComponentInChildren<TElement>(this GameObject parentObj, string name)
    {
        Transform parentTrans = parentObj.transform;
        int Cnt = parentTrans.childCount;

        for (int i = 0; i < Cnt; i++)
        {
            if (parentTrans.GetChild(i).name == name)
                return parentTrans.GetChild(i).GetComponent<TElement>();
        }

        return default(TElement);
    }

    public static Transform IsExistContainNameInChild(this Transform root, string name)
    {
        int Cnt = root.childCount;

        for (int i = 0; i < Cnt; i++)
        {
            if (root.GetChild(i).name.Contains(name))
                return root.GetChild(i);
        }
        return null;
    }
}