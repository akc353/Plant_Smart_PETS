using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

public class ReadFile : Singleton<ReadFile> {
    string strFilePath = "";
    string readstr = "";

    public string flieName = "";

    public Dictionary<string, string> buttonPath;

    public void ReadButtonPath()
    {
        buttonPath = new Dictionary<string, string>();
        strFilePath = @"C:/save";

        DirectoryInfo di = new DirectoryInfo(strFilePath);

        if (!di.Exists)
            di.Create();

        FileStream fs;
        strFilePath += "/" + flieName;

        try
        {
            fs = new FileStream(strFilePath, FileMode.Open);
        }
        catch (FileNotFoundException)
        {
            using (StreamWriter wr = new StreamWriter(strFilePath))
            {

            }

            Debug.Log("Create Data File");
            fs = new FileStream(strFilePath, FileMode.Open);
        }

        StreamReader sr = new StreamReader(fs);

        while ((readstr = sr.ReadLine()) != null)
        {
            char sp = '-';
            string[] spstring = readstr.Split(sp);
            Debug.Log(spstring[0].Substring(0));                //192.168.0.245
            if (!buttonPath.ContainsKey(spstring[0]))
            {
                buttonPath.Add(spstring[0], spstring[1]);
            }
        }

        fs.Close();
        sr.Close();
    }
}
