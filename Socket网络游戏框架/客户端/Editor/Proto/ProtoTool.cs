using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;

public static class ProtoTool
{
    private static string exePath = "D:\\unitygames\\NetWorkGame\\Protobuf\\protoc.exe";

    private static string protoPath = "D:\\unitygames\\NetWorkGame\\Protobuf\\protoc";

    private static string csharpPath = "D:\\unitygames\\NetWorkGame\\Assets\\Scripts\\Proto";

    [MenuItem("ProtoBuff工具/生成csharp类")]
    public static void GenerateProtoCsharp()
    {
        Generate("csharp_out", csharpPath);
    }
    //cmd窗口执行：
    //D:\\unitygames\\NetWorkGame\\Protobuf\\protoc.exe -I=D:\\unitygames\\NetWorkGame\\Protobuf\\protoc --csharp_out=D:\\unitygames\\NetWorkGame\\Assets\\Scripts\\Proto SocketGameProtocol.proto

    //生成对应脚本的方法
    private static void Generate(string outCmd, string outPath)
    {
        //遍历对应协议配置文件夹 得到所有的配置文件 
        DirectoryInfo directoryInfo = Directory.CreateDirectory(protoPath);
        //获取对应文件夹下所有文件信息
        FileInfo[] files = directoryInfo.GetFiles();
        //遍历所有的文件 为其生成协议脚本
        for (int i = 0; i < files.Length; i++)
        {
            //后缀的判断 只有是 配置文件才能用于生成
            if (files[i].Extension == ".proto")
            {
                //根据文件内容 来生成对应的C#脚本
                Process cmd = new Process();
                //protoc.exe的路径  
                cmd.StartInfo.FileName = exePath;
                //命令  
                cmd.StartInfo.Arguments = $"-I={protoPath} --{outCmd}={outPath} {files[i]}";
                //执行
                cmd.Start();
                //告诉外部 某一个文件 生成结束
                UnityEngine.Debug.Log(files[i] + "生成结束");
            }
        }

        AssetDatabase.Refresh();
        UnityEngine.Debug.Log("所有内容生成结束");
    }
}