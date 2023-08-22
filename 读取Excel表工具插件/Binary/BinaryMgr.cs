using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public enum BinaryType
{
    FileStream,
    MemoryStream
}

public class BinaryMgr
{
    private static BinaryMgr instance=new BinaryMgr();
    public static BinaryMgr Instance => instance;

    private string binary_path = Application.persistentDataPath + "/Binary/";


    /// <summary>
    /// 存储二进制数据到文件夹中
    /// </summary>
    /// <typeparam name="T">数据结构类</typeparam>
    /// <param name="obj">存储的数据结构</param>
    /// <param name="binaryType">存储方式</param>
    /// <param name="encryptKey">加密密匙</param>
    /// <param name="name">用来区别其它文件的名字</param>
    public void SaveData<T>(object obj, string name = "",BinaryType binaryType=BinaryType.MemoryStream,
        byte encryptKey=99)
    {
        //如果不存在存储读取二进制数据的文件夹则创建一个
        if (!Directory.Exists(binary_path))
            Directory.CreateDirectory(binary_path);

        string className = typeof(T).Name + name + ".Xiao";
       

        if (binaryType==BinaryType.FileStream)
        {
            //打开存储二进制数据的文件流
            using (FileStream fs = File.Open(binary_path + className, FileMode.OpenOrCreate, FileAccess.Write))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                //转换成二进制数据并且写入
                binaryFormatter.Serialize(fs, obj);
            }
        }
        else if(binaryType==BinaryType.MemoryStream)
        {
            Byte[] bytes;
            //打开存储二进制数据的文件流
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                //转换成二进制数据并且写入
                binaryFormatter.Serialize(ms, obj);
                //将二进制数据读出来
                bytes = ms.GetBuffer();
            }
            //加密
            for(int i=0;i<bytes.Length;i++)
            {
                bytes[i] ^= encryptKey;
            }
            File.WriteAllBytes(binary_path + className, bytes);
        }
    }

    /// <summary>
    /// 读取二进制数据结构类出来到
    /// </summary>
    /// <typeparam name="T">数据结构类</typeparam>
    /// <param name="binaryType">存储方式</param>
    /// <param name="encryptKey">加密密匙</param>
    /// <returns>返回数据结构类</returns>
    /// <param name="name">用来区别其它文件的名字</param>
    public T LoadData<T>(string name = "",BinaryType binaryType = BinaryType.MemoryStream,
        byte encryptKey = 99) where T:class
    {
        string className = typeof(T).Name + name + ".Xiao";

        //如果不存在存储读取二进制数据的文件则返回一个默认的对象
        if (!File.Exists(binary_path+className))
            return null;


        T result=default;
        if (binaryType == BinaryType.FileStream)
        {
            //打开存储二进制数据的文件流
            using (FileStream fs = File.Open(binary_path + className, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                // 将文件中的二进制数据转换为数据结构类返回
                result = binaryFormatter.Deserialize(fs) as T;
                fs.Close();
            }
        }
        else if (binaryType == BinaryType.MemoryStream)
        {
            Byte[] bytes=File.ReadAllBytes(binary_path + className);
            //解密
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] ^= encryptKey;
            }
            //打开存储二进制数据的文件流
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                //转换成二进制数据并且写入
                result = binaryFormatter.Deserialize(ms) as T;
                ms.Close();
            }
        }
        return result;
    }
}
