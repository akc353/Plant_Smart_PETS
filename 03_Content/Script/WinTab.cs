using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class WinTab : ScriptableWizard
{
    [MenuItem("Window/WindowA &a", false, 111)]
    public static void Init()
    {
        //EditorWindow window = EditorWindow.GetWindow<WindowA>(typeof(WindowB));
        //window.Show();
    }
}

#endif