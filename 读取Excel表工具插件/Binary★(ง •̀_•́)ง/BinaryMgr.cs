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
    /// �洢���������ݵ��ļ�����
    /// </summary>
    /// <typeparam name="T">���ݽṹ��</typeparam>
    /// <param name="obj">�洢�����ݽṹ</param>
    /// <param name="binaryType">�洢��ʽ</param>
    /// <param name="encryptKey">�����ܳ�</param>
    /// <param name="name">�������������ļ�������</param>
    public void SaveData<T>(object obj, string name = "",BinaryType binaryType=BinaryType.MemoryStream,
        byte encryptKey=99)
    {
        //��������ڴ洢��ȡ���������ݵ��ļ����򴴽�һ��
        if (!Directory.Exists(binary_path))
            Directory.CreateDirectory(binary_path);

        string className = typeof(T).Name + name + ".Xiao";
       

        if (binaryType==BinaryType.FileStream)
        {
            //�򿪴洢���������ݵ��ļ���
            using (FileStream fs = File.Open(binary_path + className, FileMode.OpenOrCreate, FileAccess.Write))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                //ת���ɶ��������ݲ���д��
                binaryFormatter.Serialize(fs, obj);
            }
        }
        else if(binaryType==BinaryType.MemoryStream)
        {
            Byte[] bytes;
            //�򿪴洢���������ݵ��ļ���
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                //ת���ɶ��������ݲ���д��
                binaryFormatter.Serialize(ms, obj);
                //�����������ݶ�����
                bytes = ms.GetBuffer();
            }
            //����
            for(int i=0;i<bytes.Length;i++)
            {
                bytes[i] ^= encryptKey;
            }
            File.WriteAllBytes(binary_path + className, bytes);
        }
    }

    /// <summary>
    /// ��ȡ���������ݽṹ�������
    /// </summary>
    /// <typeparam name="T">���ݽṹ��</typeparam>
    /// <param name="binaryType">�洢��ʽ</param>
    /// <param name="encryptKey">�����ܳ�</param>
    /// <returns>�������ݽṹ��</returns>
    /// <param name="name">�������������ļ�������</param>
    public T LoadData<T>(string name = "",BinaryType binaryType = BinaryType.MemoryStream,
        byte encryptKey = 99) where T:class
    {
        string className = typeof(T).Name + name + ".Xiao";

        //��������ڴ洢��ȡ���������ݵ��ļ��򷵻�һ��Ĭ�ϵĶ���
        if (!File.Exists(binary_path+className))
            return null;


        T result=default;
        if (binaryType == BinaryType.FileStream)
        {
            //�򿪴洢���������ݵ��ļ���
            using (FileStream fs = File.Open(binary_path + className, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                // ���ļ��еĶ���������ת��Ϊ���ݽṹ�෵��
                result = binaryFormatter.Deserialize(fs) as T;
                fs.Close();
            }
        }
        else if (binaryType == BinaryType.MemoryStream)
        {
            Byte[] bytes=File.ReadAllBytes(binary_path + className);
            //����
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] ^= encryptKey;
            }
            //�򿪴洢���������ݵ��ļ���
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                //ת���ɶ��������ݲ���д��
                result = binaryFormatter.Deserialize(ms) as T;
                ms.Close();
            }
        }
        return result;
    }
}
