using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public struct sPlayTime
{
    public DateTime startTime;
    public DateTime endTime;
    
    //현재 안씀.
    //public  override string ToString()
    //{
    //    return string.Format("{0} {1}\r\n", startTime.ToString(), endTime.ToString());
    //}

    public string ToReadableString()
    {
        return string.Format("시작시간: {0:yy/MM/dd H:mm:ss}\t끝시간: {1:yy/MM/dd H:mm:ss}", startTime.ToString(), endTime.ToString());
    }

    public sPlayTime(string pStartTime)
    {
        string[] dts = pStartTime.Split(' ');
        startTime = new DateTime(long.Parse(dts[0]));
        endTime = new DateTime(long.Parse(dts[1]));
    }
}


public class PlayLog : MonoBehaviour {


  //  public List<DateTime> mDataTimeList = new List<DateTime>();
    string mPath = @"C:\save\{0:yyyyMM}\";
    string mPlayLogFileName = "PlayLog.txt";
 
    public sPlayTime mPlayTime; //현재 실행되고 있는 콘텐츠  타임 저장

  

    private void Start()
    {
        DateTime dt = DateTime.Now;
        mPlayTime.startTime = dt;

        this.CreateTextLog();
      
    }


    [ContextMenu("WriteLogTest")]
    private void WriteLog()
    {
        DateTime dt = DateTime.Now;
        mPlayTime.endTime = dt;


        string path = string.Format(mPath, dt);

        this.ExistAndCreateDirectory(path);
        this.AppendText(path + mPlayLogFileName, mPlayTime.ToReadableString());
    }

    [ContextMenu("CreateText")]
    private void CreateTextLog()
    {
        
        DateTime dt = DateTime.Now;
        mPlayTime.endTime = dt;
        string savePath = string.Format(mPath, dt);
        
        var filename = string.Format(DateTime.Now.ToString("yyyyMMdd") +".txt");

        if (ExistFile(savePath))
        {
            Debug.Log("이미 있음");
        }
        else
        {
          
            FileInfo file = new FileInfo(savePath + filename);
            if (file == null)
            {
                FileStream fs = file.Create();
                fs.Close();
            }
           
            this.ExistAndCreateDirectory(savePath);
            this.AppendText(savePath + filename, mPlayTime.ToReadableString());
          
            Debug.Log(filename+"생성");
        }
    }


  
    private void ExistAndCreateDirectory(string pPath)
    {
        DirectoryInfo di = new DirectoryInfo(pPath);
        if (di.Exists == false)
        {
            di.Create();
        }
    }

    public bool ExistFile(string pPath)
    {
        FileInfo fi = new FileInfo(pPath);
        return fi.Exists;
    }



    //로그를 씁니다.
    private void WriteText(string pPath, string pText)
    {
        FileStream fs = new FileStream(pPath, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);
        sw.Write(pText);
        sw.Close();
        fs.Close();
    }

    //로그를 추가합니다.
    private void AppendText(string pPath, string pText)
    {
        FileStream fs = new FileStream(pPath, FileMode.Append);
        StreamWriter sw = new StreamWriter(fs);
        sw.WriteLine(pText);
        sw.Close();
        fs.Close();
    }


    //로그 텍스트를 읽어올 함수입니다.
    private string ReadPText(string pFilePath)
    {
        string readStr;
        FileStream fs = new FileStream(pFilePath, FileMode.Open);
        StreamReader sr = new StreamReader(fs);

        readStr = sr.ReadToEnd();
        sr.Close();
        fs.Close();
        return readStr;
    }




    public void DailyLog()
    {
        //ystem.DateTime.Now
        DateTime dt = DateTime.Now;


    }

    private void OnDestroy()
    {
        this.WriteLog();
        Debug.Log("안녕");
    }

}
