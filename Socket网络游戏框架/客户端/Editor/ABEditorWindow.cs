using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System;

public class ABEditorWindow : EditorWindow
{
    [MenuItem("AB包管理器/ab包面板")]
    public static void OpenABEditorWindow()
    {
        //生成编译器窗口
        ABEditorWindow aBEditorWindow = EditorWindow.GetWindowWithRect(typeof(ABEditorWindow), new Rect(0, 0, 430, 260)) as ABEditorWindow;
        aBEditorWindow.Show();
    }

    private int platformIndex;
    private string[] platformStrs = { "PC", "IOS", "Andriod" };
    //服务器地址
    private string serverPath = "ftp://127.0.0.1";

    //编辑器画出
    private void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 100, 30), "PC");
        platformIndex=GUI.Toolbar(new Rect(100, 20, 300, 30), platformIndex, platformStrs);

        GUI.Label(new Rect(20, 80, 100, 30), "服务器地址");
        serverPath = GUI.TextField(new Rect(120, 85, 300, 20), serverPath);

        if(GUI.Button(new Rect(20, 130, 150, 30), "生成ab包对比文件"))
        {
            CreatABCompareFile();
        }
        if (GUI.Button(new Rect(200, 130, 150, 30), "上传AB包"))
        {
            UpLoadAB();
        }
        if (GUI.Button(new Rect(20, 180, 330, 30), "将选中的ab包移动到stringmingasssets文件夹"))
        {
            MoveABFileToSA();
        }
    }

    /// <summary>
    /// 创建ab包对比文件
    /// </summary>
    private void CreatABCompareFile()
    {
        //没有路径创造一个
        DirectoryInfo directoryInfo = Directory.CreateDirectory(Application.dataPath + "/AB/"+ platformStrs[platformIndex] + "/");

        FileInfo[] fileInfos = directoryInfo.GetFiles();
        string abCompareContent = "";

        //加载规则：ab包名字 ab包长度 MD5码|ab包名字 ab包长度 MD5码|ab包名字 ab包长度 MD5码|......ab包名字 ab包长度 MD5码
        foreach (FileInfo fileInfo in fileInfos)
        {
            if (fileInfo.Extension == "")
            {
                abCompareContent += fileInfo.Name;
                abCompareContent += " ";
                abCompareContent += fileInfo.Length;
                abCompareContent += " ";
                abCompareContent += GetMD5(Application.dataPath + "/AB/" + platformStrs[platformIndex] + "/" + fileInfo.Name);
                abCompareContent += "|";
            }
        }
        //把最后一个|去了
        abCompareContent = abCompareContent.Substring(0, abCompareContent.Length - 1);
        File.WriteAllText(Application.dataPath + "/AB/" + platformStrs[platformIndex] + "/ABCompareFile.txt", abCompareContent);
        AssetDatabase.Refresh();
    }
    private string GetMD5(string path)
    {
        using (FileStream fileStream = new FileStream(path, FileMode.Open))
        {
            MD5 d5 = new MD5CryptoServiceProvider();
            byte[] bytes = d5.ComputeHash(fileStream);
            fileStream.Dispose();

            //频繁更改
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in bytes)
            {
                //十六位
                stringBuilder.Append(b.ToString("x2"));
            }
            return stringBuilder.ToString();
        }
    }

    /// <summary>
    /// 将选中的ab包移动到stringmingasssets文件夹
    /// </summary>
    private void MoveABFileToSA()
    {
        //选中
        UnityEngine.Object[] objects = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Deep);

        string abCompareContent = "";

        MD5 d5 = new MD5CryptoServiceProvider();
        
        foreach (UnityEngine.Object obj in objects)
        {
            string filePath = AssetDatabase.GetAssetPath(obj);   
            //反向截取ab包名字
            string fileName = filePath.Substring(filePath.LastIndexOf('/'));
            //建立文件
            FileInfo fileInfo = new FileInfo(Application.dataPath + "/AB/" + platformStrs[platformIndex]+"/"+ fileName);
           
            if (fileInfo.Extension != "")
            {
                continue;
            }
            //拷贝
            AssetDatabase.CopyAsset(filePath, "Assets/StreamingAssets/" + fileName);

            //顺便重新生成对比文件
            using (FileStream fileStream = File.OpenRead(fileInfo.FullName))
            {
                byte[] bytes = d5.ComputeHash(fileStream);
                fileStream.Close();
                
                StringBuilder stringBuilder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    stringBuilder.Append(b.ToString("x2"));
                }
                abCompareContent += fileInfo.Name + " " + fileInfo.Length + " " + stringBuilder.ToString();
                abCompareContent += "|";
            }
        }
        //把最后一个|去了
        abCompareContent.Substring(0, abCompareContent.Length - 1);
        File.WriteAllText(Application.streamingAssetsPath + "/ABCompareFile.txt", abCompareContent);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 上传ab包
    /// </summary>
    private void UpLoadAB()
    {
       
        DirectoryInfo directoryInfo = Directory.CreateDirectory(Application.dataPath + "/AB/" + platformStrs[platformIndex] +"/");
       
        FileInfo[] fileInfos = directoryInfo.GetFiles();
        foreach (FileInfo fileInfo in fileInfos)
        {
           
            if (fileInfo.Name == "ABCompareFile.txt" || fileInfo.Extension == "")
            {
                FTPUpLoadFile(fileInfo.FullName, fileInfo.Name);
            }
        }

    }

    /// <summary>
    /// ftp上传文件
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <param name="name">文件名字</param>
    private async void FTPUpLoadFile(string path, string name)
    {
        await Task.Run(() =>
        {
            try
            {
                FtpWebRequest ftpWebRequest = FtpWebRequest.Create(new Uri(serverPath+"/AB/" + 
                    platformStrs[platformIndex] +"/" + name)) as FtpWebRequest;
                NetworkCredential networkCredential = new NetworkCredential("xyj", "qwe123123");
                //输入账号密码
                ftpWebRequest.Credentials = networkCredential;
                
                ftpWebRequest.KeepAlive = false;
                
                ftpWebRequest.UseBinary = true;
               
                ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
                
                ftpWebRequest.Proxy = null;
                
                //用流上传
                Stream updateStream = ftpWebRequest.GetRequestStream();
                
                using (FileStream localStream = File.OpenRead(path))
                {
                    byte[] bytes = new byte[2048];
                    int count = localStream.Read(bytes, 0, bytes.Length);
                    //上传直到没有传的了
                    updateStream.Write(bytes, 0, count);
                    while (count != 0)
                    {
                        count = localStream.Read(bytes, 0, bytes.Length);  
                        updateStream.Write(bytes, 0, count);
                    }
                    updateStream.Dispose();
                    localStream.Dispose();
                    
                }
            }
            catch (Exception e)
            {
                Debug.Log(name  + e.Message);
            }

        });
    }
}
