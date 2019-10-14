using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Leap.Unity;

public static class ProjectEditor
{
    private static readonly List<GameObject> mUpUIList = new List<GameObject>();

    [MenuItem("MR/리소스/리소스 이름 변경")]
    static void RenameResourcesFiles()
    {
        GameObject ob = new GameObject();
        var com = ob.AddComponent<ResourceMgr>();
        com.PathInit();
        com.RenameAll();
        UnityEngine.Object.DestroyImmediate(ob);
    }

    [MenuItem("MR/리소스/TTS 목록 로드")]
    static void MakeTTSData()
    {
        GameObject ob = new GameObject();
        var com = ob.AddComponent<ResourceMgr>();
        com.PathInit();
        com.TTSLoad();
        UnityEngine.Object.DestroyImmediate(ob);
    }


    [MenuItem("MR/리소스/EFFECT 목록 로드")]
    static void MakeEffectData()
    {
        GameObject ob = new GameObject();
        var com = ob.AddComponent<ResourceMgr>();
        com.PathInit();
        com.EffectLoad();
        UnityEngine.Object.DestroyImmediate(ob);
    }


    [MenuItem("MR/리소스/상단메시지 UI 목록 로드")]
    static void MakeUpUI()
    {
        GameObject ob = new GameObject();
        var com = ob.AddComponent<ResourceMgr>();
        com.PathInit();
        com.PreparingUpUI();
        UnityEngine.Object.DestroyImmediate(ob);
    }

    [MenuItem("MR/리소스/하단메시지 UI 목록 로드")]
    static void MakeDownUI()
    {
        GameObject ob = new GameObject();
        var com = ob.AddComponent<ResourceMgr>();
        com.PathInit();
        com.PreparingDownUI();
        UnityEngine.Object.DestroyImmediate(ob);
    }

    [MenuItem("MR/리소스/학습내용 UI 목록 로드")]
    static void MakeLearningContentUI()
    {
        GameObject ob = new GameObject();
        var com = ob.AddComponent<ResourceMgr>();
        com.PathInit();
        com.PreparingLearningContentUI();
        UnityEngine.Object.DestroyImmediate(ob);
    }

    [MenuItem("MR/리소스/상단메시지 UI 생성")]
    static void CreateUpUI()
    {  
        GameObject ob = new GameObject();
        var com = ob.AddComponent<ResourceMgr>();
        com.PathInit();

        UIMgr.Inst.DeleteAllUpUIImg();
        com.CreateUpUI();     
        UnityEngine.Object.DestroyImmediate(ob);
       
    }

    [MenuItem("MR/리소스/하단메시지 UI 생성")]
    static void CreateDownUI()
    {
      
        GameObject ob = new GameObject();
        var com = ob.AddComponent<ResourceMgr>();
        com.PathInit();

        UIMgr.Inst.DeleteAllDownUIImg();
        com.CreateDownUI();
        UnityEngine.Object.DestroyImmediate(ob);
    }

    [MenuItem("MR/립모션/립 Hand Pool 세팅")]
    public static void PlayerHandInit()
    {
        var pool = Get("LeapHandController").Get<HandPool>();
        pool.ModelPool = new List<HandPool.ModelGroup>();

        var hands = Get("Hands");
        var allhands = hands.GetChilds();

        foreach (var ob in allhands)
        {
            var mg = new HandPool.ModelGroup();
            mg.GroupName = ob.name;

            if (ob.name.Contains("RIGHT_"))
            {
                mg.RightModel = ob.Get<RiggedHand>();

                var dic = ob.GetAllChilds();

                SetFingerComponent(dic["Bip01 R Finger0Nub"], HandSide.RIGHT, "[RIGHT]Finger");
                SetFingerComponent(dic["Bip01 R Finger1Nub"], HandSide.RIGHT, "[RIGHT]Finger");
                SetFingerComponent(dic["Bip01 R Finger2Nub"], HandSide.RIGHT, "[RIGHT]Finger");
                SetFingerComponent(dic["Bip01 R Finger3Nub"], HandSide.RIGHT, "[RIGHT]Finger");
                SetFingerComponent(dic["Bip01 R Finger4Nub"], HandSide.RIGHT, "[RIGHT]Finger");
            }
            else
            {
                mg.LeftModel = ob.Get<RiggedHand>();

                var dic = ob.GetAllChilds();

                SetFingerComponent(dic["Bip01 R Finger0Nub001"], HandSide.LEFT, "[LEFT]Finger");
                SetFingerComponent(dic["Bip01 R Finger1Nub001"], HandSide.LEFT, "[LEFT]Finger");
                SetFingerComponent(dic["Bip01 R Finger2Nub001"], HandSide.LEFT, "[LEFT]Finger");
                SetFingerComponent(dic["Bip01 R Finger3Nub001"], HandSide.LEFT, "[LEFT]Finger");
                SetFingerComponent(dic["Bip01 R Finger4Nub001"], HandSide.LEFT, "[LEFT]Finger");
            }

            if (ob.name.Contains("Default"))
            {
                mg.IsEnabled = true;
            }

            pool.ModelPool.Add(mg);

            GameObject go = new GameObject();
            var com = go.AddComponent<ResourceMgr>();
            com.PathInit();
            com.PreparingHand();
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    public static void SetFingerComponent(GameObject finger, HandSide side, string tag)
    {
        var c = finger.Get<Collider>();
        if (c != null)
        {
            GameObject.DestroyImmediate(c);
        }

        var i = finger.Get<FingerDetection>();
        if (i != null)
        {
            GameObject.DestroyImmediate(i);
        }

        var r = finger.Get<Rigidbody>();
        if (r != null)
        {
            GameObject.DestroyImmediate(r);
        }

        var ff = finger.AddComponent<SphereCollider>();
        ff.radius = 0.02f;
        ff.isTrigger = true;

        var fd = finger.AddComponent<FingerDetection>();
        fd._side = side;

        var fr = finger.AddComponent<Rigidbody>();
        fr.isKinematic = true;
        fr.useGravity = false;

        finger.tag = tag;
    }

    static GameObject Get(string name)
    {
        return GameObject.Find(name);
    }

    static void WriteString(string path)
    {
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine("Test");
        writer.Close();

        //Re-import the file to update the reference in the editor
        AssetDatabase.ImportAsset(path);
        TextAsset asset = Resources.Load("test") as TextAsset;

        //Print the text from the file
        Debug.Log(asset.text);
    }

    static void ReadString()
    {
        string path = "Assets/Resources/test.txt";

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }

    //[MenuItem("Assets/MR/GetTypes")]
    private static void GetTypes()
    {
        Debug.Log(Selection.activeObject.GetType());
        Debug.Log(Application.dataPath);
        Debug.Log(Application.persistentDataPath);
        Debug.Log(Application.streamingAssetsPath);
        Debug.Log(Application.temporaryCachePath);
        Debug.Log(Application.dataPath + "/");
        Debug.Log(Application.persistentDataPath);
        Debug.Log(Application.streamingAssetsPath);
        Debug.Log(Application.temporaryCachePath);
    }

    //[MenuItem("Assets/MR/GetFolderName")]
    private static void GetFolderName()
    {
        Debug.Log(Selection.activeObject.name);
    }
}