using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using UnityEditor;

public class ExcelMgr
{
    //���ɵ�excel���ݵĶ��������ݴ�ŵ�λ��
    public static string Excel_Binary_Path = Application.streamingAssetsPath + "/Excel/";


    private static ExcelMgr instance = new ExcelMgr();
    public static ExcelMgr Instance => instance;

    //���������������ֵ�
    private Dictionary<string, IContainer> containerDic = new Dictionary<string, IContainer>();

    /// <summary>
    /// ����Excel��ʵ�����ݣ��ٴ�洢�����ݽṹ���У��ٴ洢��������������
    /// </summary>
    /// <typeparam name="T">���ݽṹ��</typeparam>
    /// <typeparam name=K">������</typeparam>
    public void LoadExcelTable<T, K>() where T : IClass where K:IContainer
    {
        string tableName = typeof(T).Name;
        //������ֵ����Ѿ����ڸ������������򷵻�
        if (containerDic.ContainsKey(tableName))
            return;
       
        using (FileStream fs = File.Open(ExcelMgr.Excel_Binary_Path + tableName + ".Xiao", FileMode.OpenOrCreate
            , FileAccess.Read))
        {
            Byte[] bytes = new Byte[fs.Length];
            //�õ��ļ��е����ж���������
            fs.Read(bytes, 0, (int)fs.Length);

            //��¼����λ��
            int index=0;

            //�õ�ʵ�����ݵ�����
            int realRowCount = BitConverter.ToInt32(bytes, index);
            index += 4;

            //�õ������ַ���������
            int keyStrLength = BitConverter.ToInt32(bytes,index);
            index += 4;
            string keyName = Encoding.UTF8.GetString(bytes,index,keyStrLength);
            index += keyStrLength;

            Type classType= typeof(T);
            T classObj;
            FieldInfo[] fieldInfos;

            //�õ��������type
            Type containerType = typeof(K);
            //�õ��������ʵ��
            K containerObj = Activator.CreateInstance<K>();
            //�õ��������е��ֵ�
            object dicObj = containerType.GetField("dic").GetValue(containerObj);
            //�õ��ֵ��е�Add����
            MethodInfo addMethod = dicObj.GetType().GetMethod("Add");

           
            //����һ���ж����е�ʵ������
            for (int i=0;i<realRowCount;i++)
            {
                classObj = Activator.CreateInstance<T>();
                fieldInfos = classType.GetFields();
                //�������е��ֶ�
                for(int j=0;j<fieldInfos.Length;j++)
                {
                    if(fieldInfos[j].FieldType==typeof(int))
                    {
                        fieldInfos[j].SetValue(classObj,BitConverter.ToInt32(bytes,index));
                        index += 4;
                    }
                    else if(fieldInfos[j].FieldType==typeof(float))
                    {
                        fieldInfos[j].SetValue(classObj, BitConverter.ToSingle(bytes, index));
                        index += 4;
                    }
                    else if (fieldInfos[j].FieldType == typeof(bool))
                    {
                        fieldInfos[j].SetValue(classObj, BitConverter.ToBoolean(bytes, index));
                        index += 1;
                    }
                    else if (fieldInfos[j].FieldType == typeof(string))
                    {
                        //�õ��ַ����ĳ���
                        int strLength = BitConverter.ToInt32(bytes, index);
                        index += 4;
                        //�����ַ���������
                        fieldInfos[j].SetValue(classObj, Encoding.UTF8.GetString(bytes,index,strLength));
                        index += strLength;
                    }
                }
                
                //�����ݽṹ����뵽������������ֵ���
                object keyObj = classType.GetField(keyName).GetValue(classObj);

                addMethod.Invoke(dicObj, new object[] {
                    keyObj,classObj});
            }
            if (containerDic.ContainsKey(tableName + "Container"))
                containerDic[tableName + "Container"] = containerObj;
            else
                containerDic.Add(tableName + "Container", containerObj);
        }
        ////ˢ�� 
        //AssetDatabase.Refresh();
    }

    /// <summary>
    /// �õ����������ֵ��е����ݽṹ��
    /// </summary>
    /// <typeparam name="T">����������</typeparam>
    /// <returns>����������</returns>
    public IContainer GetExcelData<T>() where T : IContainer
    {
        string tableName = typeof(T).Name;
        if (containerDic.ContainsKey(tableName))
        {
           return containerDic[tableName];
        }
        return null;
    }
    /// <summary>
    /// ��ձ�����������
    /// </summary>
    public void ClearAll()
    {
        containerDic.Clear();
    }
}
