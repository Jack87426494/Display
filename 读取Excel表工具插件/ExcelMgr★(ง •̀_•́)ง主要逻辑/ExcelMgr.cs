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
    //生成的excel数据的二进制数据存放的位置
    public static string Excel_Binary_Path = Application.streamingAssetsPath + "/Excel/";


    private static ExcelMgr instance = new ExcelMgr();
    public static ExcelMgr Instance => instance;

    //存放数据容器类的字典
    private Dictionary<string, IContainer> containerDic = new Dictionary<string, IContainer>();

    /// <summary>
    /// 加载Excel表实际数据，再存存储到数据结构类中，再存储到数据容器类中
    /// </summary>
    /// <typeparam name="T">数据结构类</typeparam>
    /// <typeparam name=K">容器类</typeparam>
    public void LoadExcelTable<T, K>() where T : IClass where K:IContainer
    {
        string tableName = typeof(T).Name;
        //如果在字典中已经存在该数据容器类则返回
        if (containerDic.ContainsKey(tableName))
            return;
       
        using (FileStream fs = File.Open(ExcelMgr.Excel_Binary_Path + tableName + ".Xiao", FileMode.OpenOrCreate
            , FileAccess.Read))
        {
            Byte[] bytes = new Byte[fs.Length];
            //得到文件中得所有二进制数据
            fs.Read(bytes, 0, (int)fs.Length);

            //记录索引位置
            int index=0;

            //得到实际数据的行数
            int realRowCount = BitConverter.ToInt32(bytes, index);
            index += 4;

            //得到主键字符串的名字
            int keyStrLength = BitConverter.ToInt32(bytes,index);
            index += 4;
            string keyName = Encoding.UTF8.GetString(bytes,index,keyStrLength);
            index += keyStrLength;

            Type classType= typeof(T);
            T classObj;
            FieldInfo[] fieldInfos;

            //得到容器类的type
            Type containerType = typeof(K);
            //得到容器类的实例
            K containerObj = Activator.CreateInstance<K>();
            //得到容器类中的字典
            object dicObj = containerType.GetField("dic").GetValue(containerObj);
            //得到字典中的Add方法
            MethodInfo addMethod = dicObj.GetType().GetMethod("Add");

           
            //遍历一共有多少行的实际数据
            for (int i=0;i<realRowCount;i++)
            {
                classObj = Activator.CreateInstance<T>();
                fieldInfos = classType.GetFields();
                //遍历所有的字段
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
                        //得到字符串的长度
                        int strLength = BitConverter.ToInt32(bytes, index);
                        index += 4;
                        //设置字符串的内容
                        fieldInfos[j].SetValue(classObj, Encoding.UTF8.GetString(bytes,index,strLength));
                        index += strLength;
                    }
                }
                
                //将数据结构类加入到数据容器类的字典中
                object keyObj = classType.GetField(keyName).GetValue(classObj);

                addMethod.Invoke(dicObj, new object[] {
                    keyObj,classObj});
            }
            if (containerDic.ContainsKey(tableName + "Container"))
                containerDic[tableName + "Container"] = containerObj;
            else
                containerDic.Add(tableName + "Container", containerObj);
        }
        ////刷新 
        //AssetDatabase.Refresh();
    }

    /// <summary>
    /// 得到数据容器字典中的数据结构类
    /// </summary>
    /// <typeparam name="T">数据容器类</typeparam>
    /// <returns>数据容器类</returns>
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
    /// 清空表中所有数据
    /// </summary>
    public void ClearAll()
    {
        containerDic.Clear();
    }
}
