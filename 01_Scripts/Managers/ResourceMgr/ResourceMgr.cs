using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

//사운드는 JSON파일로 저장시켜 다시 읽어오는 작업 추가. //1206
public class ResourceMgr : MonoBehaviour
{

    #region SOUND
    //////TTS///////////
    string mTTSCSFolerPath;
    string mTTSCSFileName;
    string mTTSPath;
    string mTTSloadPath;

    public string mTTSJsonPath;
    public string mTTSJsonName;

    //////Effect///////////
    string mEffectCSFolerPath;
    string mEffectCSFileName;
    string mEffectPath;
    string mEffectloadPath;

    private string mEffectJsonPath;
    private string mEffectJsonName;
    #endregion

    string 핸드폴더;
    string 핸드파일;
    string 핸드경로;
    string mHandCSFolderPath;


    ////// UI ///////////
    string 상단이미지폴더;
    string 상단이미지파일;
    string 상단이미지경로;
    string mUPImageCSFilePath;

    string 하단이미지폴더;
    string 하단이미지파일;
    string 하단이미지경로;
    string mDownImageCSFilePath;

    string 학습내용이미지폴더;
    string 학습내용이미지파일;
    string 학습내용이미지경로;

    string resourceMgrFolderPath;
    string 상단탭이미지경로;
    string 하단탭이미지경로;
    string 학습내용탭이미지경로;
    string 핸드프리팹경로;

    public void PathInit()
    {
        resourceMgrFolderPath = "/01_Scripts/Managers/ResourceMgr/";
        상단탭이미지경로 = "/Resources/Image/UI/상단탭";
        하단탭이미지경로 = "/Resources/Image/UI/하단탭";
        학습내용탭이미지경로 = "/Resources/Image/UI/학습내용탭";
        핸드프리팹경로 = "/Resources/Hand/Model";

        #region  SOUND    
        mTTSCSFolerPath = Application.dataPath + "/01_Scripts/Define/";
        mTTSPath = Application.dataPath + "/Resources/Sound/TTS";
        mTTSCSFileName = "DefineTTS.cs";
        mTTSloadPath = "Sound/TTS";
        mTTSJsonPath = Application.dataPath + "/Resources/Sound/JSON";
        mTTSJsonName = "TTSList.txt";


        mEffectCSFolerPath = Application.dataPath + resourceMgrFolderPath;
        mEffectPath = Application.dataPath + "/Resources/Sound/Effect";
        mEffectCSFileName = "DefineEffect.cs";
        mEffectloadPath = "Sound/Effect";
        mEffectJsonPath = Application.dataPath + "/Resources/Sound/JSON";
        mEffectJsonName = "EffectList.txt";
        #endregion

 
        mHandCSFolderPath= Application.dataPath + resourceMgrFolderPath;
        핸드폴더 = Application.dataPath + resourceMgrFolderPath;
        핸드경로= Application.dataPath + 핸드프리팹경로;
        핸드파일 = "DefineHand.cs";


        mUPImageCSFilePath= Application.dataPath + resourceMgrFolderPath;
        상단이미지폴더 = Application.dataPath + resourceMgrFolderPath;
        상단이미지경로 = Application.dataPath + 상단탭이미지경로;
        상단이미지파일 = "DefineUpUI.cs";

        mDownImageCSFilePath= Application.dataPath + resourceMgrFolderPath;
        하단이미지폴더 = Application.dataPath + resourceMgrFolderPath;
        하단이미지경로 = Application.dataPath + 하단탭이미지경로;
        하단이미지파일 = "DefineDownUI.cs";

        학습내용이미지폴더 = Application.dataPath + resourceMgrFolderPath;
        학습내용이미지경로 = Application.dataPath + 학습내용탭이미지경로;
        학습내용이미지파일 = "DefineLearningContentUI.cs";
    }

    public void RenameAll()
    {
        string tts = Application.dataPath + "/Resources/Sound/TTS";
        string upui = Application.dataPath + "/Resources/Image/UI/상단탭";
        string downui = Application.dataPath + "/Resources/Image/UI/하단탭";
        string learningContentUi = Application.dataPath + "/Resources/Image/UI/학습내용탭";

        RenameFileNameInDirectory(tts);
        RenameFileNameInDirectory(upui);
        RenameFileNameInDirectory(downui);
    }

    #region UI 로드
    public void CreateUpUI()
    {
        var fileList = UIMgr.Inst.mUPUIDataList;

        foreach (sUI_Data info in fileList)
        {
            if (info.Name.Contains(".meta")) continue;
            var name = info.Name.Replace(".png", "");
            Sprite sp = Resources.Load<Sprite>("Image/UI/상단탭/" + name);

            if (sp == null)
            {
                Debug.Log("로드 실패 : " + name);
            }
            else
            {
                GameObject image = GameObject.Find("상단메시지패널").transform.GetChild(0).gameObject;
                GameObject go = Instantiate<GameObject>(image);

                go.transform.SetParent(image.transform.parent);
                go.transform.localPosition = image.transform.localPosition;
                go.transform.localRotation = image.transform.localRotation;
                go.transform.localScale = image.transform.localScale;
                go.GetComponent<Image>().sprite = sp;
                go.GetComponent<RectTransform>().sizeDelta = new Vector2(sp.rect.width, sp.rect.height);
                go.name = sp.name;
                go.SetActive(false);
            }
        }
    }


    public void CreateDownUI()
    {
        var fileList = UIMgr.Inst.mDownUIDataList;
   
        //foreach (FileInfo info in fileList)
        foreach (sUI_Data info in fileList)
        {
            if (info.Name.Contains(".meta")) continue;
            var name = info.Name.Replace(".png", "");
            Sprite sp = Resources.Load<Sprite>("Image/UI/하단탭/" + name);
            GameObject image = GameObject.Find("하단메시지패널").transform.GetChild(0).gameObject;
            GameObject go = Instantiate<GameObject>(image);
            go.transform.SetParent(image.transform.parent);
            go.transform.localPosition = image.transform.localPosition;
            go.transform.localRotation = image.transform.localRotation;
            go.transform.localScale = image.transform.localScale;
            go.GetComponent<Image>().sprite = sp;
        
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(sp.rect.width, sp.rect.height);
            go.name = sp.name;
            go.SetActive(false);
        }
    }

    public void CreateLearningContentUI()
    {
        string path = Application.dataPath + 학습내용탭이미지경로;
        DirectoryInfo di = new DirectoryInfo(path);
        var fileList = di.GetFiles();

        foreach (FileInfo info in fileList)
        {
            if (info.Name.Contains(".meta")) continue;
            var name = info.Name.Replace(".png", "");
            Sprite sp = Resources.Load<Sprite>("Image/UI/학습내용탭/" + name);

            if (sp == null)
            {
                Debug.Log("로드 실패 : " + name);
            }
            else
            {
                GameObject image = GameObject.Find("학습내용").transform.GetChild(0).gameObject;
                GameObject go = Instantiate<GameObject>(image);
                go.transform.SetParent(image.transform.parent);
                go.transform.localPosition = image.transform.localPosition;
                go.transform.localRotation = image.transform.localRotation;
                go.transform.localScale = image.transform.localScale;
                go.GetComponent<Image>().sprite = sp;
                go.GetComponent<RectTransform>().sizeDelta = new Vector2(sp.rect.width, sp.rect.height);
                go.name = sp.name;
            }
        }
    }
    #endregion

    public void RenameFileNameInDirectory(string path)
    {
        string[] ttsfiles = Directory.GetFiles(path);   //파일들 담는 배열

        foreach (string fullFilePath in ttsfiles)
        {
            FileInfo info = new FileInfo(fullFilePath);

            if (info.Name.Contains(".meta"))
            {
                continue; //메타파일이 포함되어 있으므로 메타 파일을 제외합니다.
            }

            string newFileName = info.FullName.Replace(" ", "").Replace("\"", "").Replace(",", "").Replace(".", "").Replace("ogg", ".ogg").Replace("wav", ".wav").Replace("png", ".png");

            if (File.Exists(newFileName))
            {
                // Debug.Log("파일이 이미 있습니다 ( " + newFileName + " )");
            }
            else
            {
                Debug.Log("리네임 중 : " + newFileName);
                File.Move(info.FullName, newFileName);
            }
        }
    }


    public void TTSLoad()
    {
        mTTSPath = Application.dataPath + "/Resources/Sound/TTS";
        mTTSloadPath = "Sound/TTS";

        mTTSCSFolerPath = Application.dataPath + "/01_Scripts/Define/";
        mTTSCSFileName = "DefineTTS.cs";

        mTTSJsonPath = Application.dataPath + "/Resources/Sound/JSON";
        mTTSJsonName = "TTSList.txt";

        Dictionary<string, string> mFiletDic = new Dictionary<string, string>();   //파일스트링을 담을 딕셔너리
        AddFile(mFiletDic);
        SaveCSFile(mFiletDic);
        SaveJsonFile(mFiletDic);
    }


    public void EffectLoad()
    {
        mEffectPath = Application.dataPath + "/Resources/Sound/Effect";
        mEffectloadPath = "Sound/Effect";

        mEffectCSFolerPath = Application.dataPath + "/01_Scripts/Define/";
        mEffectCSFileName = "DefineEffect.cs";

        mEffectJsonPath = Application.dataPath + "/Resources/Sound/JSON";
        mEffectJsonName = "EffectList.txt";

        Dictionary<string, string> mFiletDic = new Dictionary<string, string>();   //파일스트링을 담을 딕셔너리
        AddEffectFile(mFiletDic);
        SaveEffectCSFile(mFiletDic);
        SaveJsonEffectFile(mFiletDic);
    }

    public void PreparingHand()
    {
        mHandCSFolderPath= Application.dataPath + "/01_Scripts/Define/";

        DirectoryInfo di = new DirectoryInfo(핸드경로);
        var fileList = di.GetFiles().ToList();
        var fileListNonMeta = fileList.Where(x => !x.Name.Contains(".meta")).ToList();
        var nameList = fileListNonMeta.Select(x => x.Name.Replace(".prefab", "")).ToList();
        SaveTextFile(mHandCSFolderPath + 핸드파일, HandCSFile(nameList));
    }


    public void PreparingUpUI()
    {
        mUPImageCSFilePath= Application.dataPath + "/01_Scripts/Define/";

        DirectoryInfo di = new DirectoryInfo(상단이미지경로);
        var fileList = di.GetFiles().ToList();
        var fileListNonMeta = fileList.Where(x => !x.Name.Contains(".meta")).ToList();
        var nameList = fileListNonMeta.Select(x => x.Name.Replace(".png", "")).ToList();
        SaveTextFile(mUPImageCSFilePath + 상단이미지파일, 상단이미지CS파일(nameList));

        UIMgr.Inst.InitUpUIDataList(nameList);
    }

    public void PreparingDownUI()
    {

        mDownImageCSFilePath = Application.dataPath + "/01_Scripts/Define/";

        DirectoryInfo di = new DirectoryInfo(하단이미지경로);
        var fileList = di.GetFiles().ToList();
        var fileListNonMeta = fileList.Where(x => !x.Name.Contains(".meta")).ToList();
        var nameList = fileListNonMeta.Select(x => x.Name.Replace(".png", "")).ToList();
        SaveTextFile(mDownImageCSFilePath + 하단이미지파일, 하단이미지CS파일(nameList));

        UIMgr.Inst.InitDownUIDataList(nameList);
    }

    public void PreparingLearningContentUI()
    {
        DirectoryInfo di = new DirectoryInfo(학습내용이미지경로);
        var fileList = di.GetFiles().ToList();
        var fileListNonMeta = fileList.Where(x => !x.Name.Contains(".meta")).ToList();
        var nameList = fileListNonMeta.Select(x => x.Name.Replace(".png", "")).ToList();
        SaveTextFile(학습내용이미지폴더 + 학습내용이미지파일, 학습내용이미지CS파일(nameList));
    }

    private void AddFile(Dictionary<string, string> pFileDic)
    {
        string[] files = Directory.GetFiles(mTTSPath);   //파일들 담는 배열

        foreach (string fullFilePath in files)
        {
            FileInfo info = new FileInfo(fullFilePath);
            //File.Move(info.FullName, info.FullName.Replace(" ", "").Replace("\"", "").Replace(",", ""));

            if (info.Name.Contains(".meta"))
            {
                continue; //메타파일이 포함되어 들어가 메타 파일 제외
            }

            string name = info.Name;
            name = name.Replace(".ogg", "").Replace(".wav", ""); // 확장자 제거하여 변수명 및 Resources.Load 불러올 패스로 변경

            if (pFileDic.ContainsKey(name))
            {
                Debug.Log("키가 이미 있습니다. : " + name);
            }
            else
            {
                pFileDic.Add(name, mTTSloadPath + "/" + name);
            }
        }

        string[] directories = Directory.GetDirectories(mTTSPath);

        foreach (string fullDirectoryPath in directories)
        {
            DirectoryInfo directoriyInfo = new DirectoryInfo(fullDirectoryPath);

            string subrootpath = mTTSPath + "/" + directoriyInfo.Name;
            string loadpath = mTTSloadPath + "/" + directoriyInfo.Name;

            AddFile(pFileDic);
        }
    }


    private void AddEffectFile(Dictionary<string, string> pFileDic)
    {
        string[] files = Directory.GetFiles(mEffectPath);   //파일들 담는 배열

        foreach (string fullFilePath in files)
        {
            FileInfo info = new FileInfo(fullFilePath);
            if (info.Name.Contains(".meta")) continue;

            string name = info.Name;
            name = name.Replace(".ogg", "").Replace(".wav", ""); // 확장자 제거하여 변수명 및 Resources.Load 불러올 패스로 변경

            if (pFileDic.ContainsKey(name))
            {
                Debug.Log("키가 이미 있습니다. : " + name);
            }
            else
            {
                pFileDic.Add(name, mEffectloadPath + "/" + name);
            }
        }

        string[] directories = Directory.GetDirectories(mEffectPath);

        foreach (string fullDirectoryPath in directories)
        {
            DirectoryInfo directoriyInfo = new DirectoryInfo(fullDirectoryPath);

            string subrootpath = mEffectPath + "/" + directoriyInfo.Name;
            string loadpath = mEffectloadPath + "/" + directoriyInfo.Name;

            AddEffectFile(pFileDic);
        }
    }



    private void SaveJsonFile(Dictionary<string, string> pDic)
    {
        Debug.Log("JSON 항목 횟수 : " + pDic.Count);

        string strJson = "[";
        foreach (KeyValuePair<string, string> pair in pDic)
        {
            strJson += AddBrace(string.Format("{0} : {1}, {2} : {3}", AddDDa("key"), AddDDa(pair.Key), AddDDa("path"), AddDDa(pair.Value))) + ",";
        }
        strJson += "]";

        StreamWriter sw = new StreamWriter(mTTSJsonPath + "/" + mTTSJsonName, false);
        sw.Write(strJson);
        sw.Close();
    }


    private void SaveJsonEffectFile(Dictionary<string, string> pDic)
    {
        Debug.Log("JSON 항목 횟수 : " + pDic.Count);

        string strJson = "[";
        foreach (KeyValuePair<string, string> pair in pDic)
        {
            strJson += AddBrace(string.Format("{0} : {1}, {2} : {3}", AddDDa("key"), AddDDa(pair.Key), AddDDa("path"), AddDDa(pair.Value))) + ",";
        }
        strJson += "]";

        StreamWriter sw = new StreamWriter(mEffectJsonPath + "/" + mEffectJsonName, false);
        sw.Write(strJson);
        sw.Close();
    }

    private void SaveCSFile(Dictionary<string, string> dic)
    {
        List<string> list = dic.Keys.ToList();

        string cs = TTSResourcesCSFile(list);

        SaveTextFile(mTTSCSFolerPath + mTTSCSFileName, cs);
    }

    private void SaveEffectCSFile(Dictionary<string, string> dic)
    {
        List<string> list = dic.Keys.ToList();

        string cs = EffectResourcesCSFile(list);

        SaveTextFile(mEffectCSFolerPath + mEffectCSFileName, cs);
    }

   

    private static void SaveTextFile(string path, string str)
    {
        File.WriteAllText(path, str);
        Debug.Log(path + " : 저장완료");
    }

    private string HandCSFile(List<string> inputList)
    {
        string cs = "";

        AddCSLine(ref cs, "// 사용할 립모션 핸드 들이 변수로 잡혀 있습니다.");
        AddCSLine(ref cs, "// 이 소스코드는 자동으로 생성됩니다. 임의로 수정하지 마세요.");
        AddCSLine(ref cs, "// 수정할 경우 수정된 사항을 잃어버릴 수 있습니다.");
        AddCSLine(ref cs, "");
        AddCSLine(ref cs, "using System.Collections;");
        AddCSLine(ref cs, "using System.Collections.Generic;");
        AddCSLine(ref cs, "using UnityEngine;");

        AddCSLine(ref cs, "public static class DefineHand");
        AddCSLine(ref cs, "{");

        foreach (string str in inputList)
        {
            AddCSLine(ref cs, "public readonly static string " + str.Replace(".ogg", "").Replace(".wav", "") + " = \"" + str.Replace(".ogg", "").Replace(".wav", "") + "\";");
        }

        AddCSLine(ref cs, "}");

        return cs;
    }





    private string 상단이미지CS파일(List<string> inputList)
    {
        string cs = "";

        AddCSLine(ref cs, "// 사용할 상단 이미지 들이 변수로 잡혀 있습니다.");
        AddCSLine(ref cs, "// 이 소스코드는 자동으로 생성됩니다. 임의로 수정하지 마세요.");
        AddCSLine(ref cs, "// 수정할 경우 수정된 사항을 잃어버릴 수 있습니다.");
        AddCSLine(ref cs, "");
        AddCSLine(ref cs, "using System.Collections;");
        AddCSLine(ref cs, "using System.Collections.Generic;");
        AddCSLine(ref cs, "using UnityEngine;");

        AddCSLine(ref cs, "public static class DefineUpUI");
        AddCSLine(ref cs, "{");

        foreach (string str in inputList)
        {
            AddCSLine(ref cs, "public readonly static string " + str.Replace(".ogg", "").Replace(".wav", "") + " = \"" + str.Replace(".ogg", "").Replace(".wav", "") + "\";");
        }

        AddCSLine(ref cs, "}");

        return cs;
    }

    private string 하단이미지CS파일(List<string> inputList)
    {
        string cs = "";

        AddCSLine(ref cs, "// 사용할 하단 이미지 들이 변수로 잡혀 있습니다.");
        AddCSLine(ref cs, "// 이 소스코드는 자동으로 생성됩니다. 임의로 수정하지 마세요.");
        AddCSLine(ref cs, "// 수정할 경우 수정된 사항을 잃어버릴 수 있습니다.");
        AddCSLine(ref cs, "");
        AddCSLine(ref cs, "using System.Collections;");
        AddCSLine(ref cs, "using System.Collections.Generic;");
        AddCSLine(ref cs, "using UnityEngine;");

        AddCSLine(ref cs, "public static class DefineDownUI");
        AddCSLine(ref cs, "{");

        foreach (string str in inputList)
        {
            AddCSLine(ref cs, "public readonly static string " + str.Replace(".ogg", "").Replace(".wav", "") + " = \"" + str.Replace(".ogg", "").Replace(".wav", "") + "\";");
        }

        AddCSLine(ref cs, "}");

        return cs;
    }
    private string 학습내용이미지CS파일(List<string> inputList)
    {
        string cs = "";

        AddCSLine(ref cs, "// 사용할 하단 이미지 들이 변수로 잡혀 있습니다.");
        AddCSLine(ref cs, "// 이 소스코드는 자동으로 생성됩니다. 임의로 수정하지 마세요.");
        AddCSLine(ref cs, "// 수정할 경우 수정된 사항을 잃어버릴 수 있습니다.");
        AddCSLine(ref cs, "");
        AddCSLine(ref cs, "using System.Collections;");
        AddCSLine(ref cs, "using System.Collections.Generic;");
        AddCSLine(ref cs, "using UnityEngine;");

        AddCSLine(ref cs, "public static class DefineLearningContentUI");
        AddCSLine(ref cs, "{");

        foreach (string str in inputList)
        {
            AddCSLine(ref cs, "public readonly static string " + str.Replace(".ogg", "").Replace(".wav", "") + " = \"" + str.Replace(".ogg", "").Replace(".wav", "") + "\";");
        }

        AddCSLine(ref cs, "}");

        return cs;
    }

    private string TTSResourcesCSFile(List<string> inputList)
    {
        string cs = "";

        AddCSLine(ref cs, "// 사용할 TTS들이 변수로 잡혀 있습니다.");
        AddCSLine(ref cs, "// 이 소스코드는 자동으로 생성됩니다. 임의로 수정하지 마세요.");
        AddCSLine(ref cs, "// 수정할 경우 수정된 사항을 잃어버릴 수 있습니다.");
        AddCSLine(ref cs, "");
        AddCSLine(ref cs, "using System.Collections;");
        AddCSLine(ref cs, "using System.Collections.Generic;");
        AddCSLine(ref cs, "using UnityEngine;");

        AddCSLine(ref cs, "public static class DefineTTS");
        AddCSLine(ref cs, "{");

        foreach (string str in inputList)
        {
            AddCSLine(ref cs, "public readonly static string " + str.Replace(".ogg", "").Replace(".wav", "").Replace(".", "") + " = \"" + str.Replace(".ogg", "").Replace(".wav", "") + "\";");
        }

        AddCSLine(ref cs, "}");

        return cs;
    }



    private string EffectResourcesCSFile(List<string> inputList)
    {
        string cs = "";

        AddCSLine(ref cs, "// 사용할 TTS들이 변수로 잡혀 있습니다.");
        AddCSLine(ref cs, "// 이 소스코드는 자동으로 생성됩니다. 임의로 수정하지 마세요.");
        AddCSLine(ref cs, "// 수정할 경우 수정된 사항을 잃어버릴 수 있습니다.");
        AddCSLine(ref cs, "");
        AddCSLine(ref cs, "using System.Collections;");
        AddCSLine(ref cs, "using System.Collections.Generic;");
        AddCSLine(ref cs, "using UnityEngine;");

        AddCSLine(ref cs, "public static class DefineEffect");
        AddCSLine(ref cs, "{");

        foreach (string str in inputList)
        {
            AddCSLine(ref cs, "public readonly static string " + str.Replace(".ogg", "").Replace(".wav", "").Replace(".", "") + " = \"" + str.Replace(".ogg", "").Replace(".wav", "") + "\";");
        }

        AddCSLine(ref cs, "}");

        return cs;
    }

    private void AddCSLine(ref string cs, string line)
    {
        cs += line + "\r\n";
    }

    //문자열에 쌍따옴표 붙여서 반환
    private string AddDDa(string str)
    {
        return "\"" + str + "\"";
    }

    private string AddBrace(string str)
    {
        return "{" + str + "}";
    }
}