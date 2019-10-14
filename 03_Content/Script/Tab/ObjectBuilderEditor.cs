using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(WindowMgr))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WindowMgr myScript = (WindowMgr)target;
        
        if (GUILayout.Button("Init"))
        {
            myScript.SetName();
        }
    }
}

[CustomEditor(typeof(MyTab))]
public class ObjectBuilderEditor0 : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MyTab myScript = (MyTab)target;
        if (GUILayout.Button("Add Control"))
        {
            WindowMgr.Inst.AddControl(myScript, myScript.newObjectType);
        }

        if (GUILayout.Button("Delete"))
        {
            WindowMgr.Inst.DeleteControl(myScript);
        }
    }
}

[CustomEditor(typeof(MyButton))]
public class ObjectBuilderEditor1 : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MyButton myScript = (MyButton)target;
        if (GUILayout.Button("Add Control"))
        {
            WindowMgr.Inst.AddControl(myScript, myScript.newObjectType);
        }

        if (GUILayout.Button("Delete"))
        {
            WindowMgr.Inst.DeleteControl(myScript);
        }
    }
}


[CustomEditor(typeof(MyLabel))]
public class ObjectBuilderEditor2 : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MyLabel myScript = (MyLabel)target;
        if (GUILayout.Button("Add Control"))
        {
            WindowMgr.Inst.AddControl(myScript, myScript.newObjectType);
        }

        if (GUILayout.Button("Delete"))
        {
            WindowMgr.Inst.DeleteControl(myScript);
        }
    }
}

[CustomEditor(typeof(MyImage))]
public class ObjectBuilderEditor3 : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MyImage myScript = (MyImage)target;
        if (GUILayout.Button("Add Control"))
        {
            WindowMgr.Inst.AddControl(myScript, myScript.newObjectType);
        }

        if (GUILayout.Button("Delete"))
        {
            WindowMgr.Inst.DeleteControl(myScript);
        }
    }
}


[CustomEditor(typeof(TabBox))]
public class ObjectBuilderEditor4 : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TabBox myScript = (TabBox)target;
        if (GUILayout.Button("Add Control"))
        {
            WindowMgr.Inst.AddControl(myScript, ControlType.Tab);
        }
    }
}
#endif