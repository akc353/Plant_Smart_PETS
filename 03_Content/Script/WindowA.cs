using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class WindowA : EditorWindow
{
    [MenuItem("Window/WindowA")]
    static void Open()
    {
        CreateInstance<WindowA>().Show();
    }
}

#endif