using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excel;
using System;
using UnityEditor;
using System.Text;
using System.IO;
using System.Data;

public class ExcelTool
{
    //Excel文件存放的路径上
    private static string File_Path = Application.dataPath + "/Resources/Editor/ExcelFile";
    //Excel文件存放数据结构类脚本文件的路径
    private static string FileClass_Path = Application.dataPath + "/Scripts/Binary/ExcelTable/";
    //Excel文件存放数据容器类脚本文件的路径
    private static string File_Container_Path = Application.dataPath + "/Scripts/Binary/ExcelContainer/";
    
    //private static string File_Binary_Path = Application.dataPath + "/Resources/StreamingAssets/ExcelBinary/";

    //表中实际数据开始的行
    private static int Data_Start_Index = 4;

    /// <summary>
    /// 创建数据表
    /// </summary>
    [MenuItem("GameTool/GenerateExcel")]
    public static void GenerateExcel()
    {

        DirectoryInfo di = Directory.CreateDirectory(File_Path);
        //打开文件的excel数据的表数据
        DataTableCollection result;
        //得到文件夹下所有的文件
        FileInfo[] fileInfos = di.GetFiles();
        for (int i = 0; i < fileInfos.Length; i++)
        {
            //如果文件的后缀不是.xlsx或者.xls就不处理后面的逻辑
            if (fileInfos[i].Extension != ".xlsx" && fileInfos[i].Extension != ".xls")
                continue;


            //打开文件
            using (FileStream fs = fileInfos[i].Open(FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                result = excelDataReader.AsDataSet().Tables;
            }

            //遍历excel表的数据
            foreach (DataTable table in result)
            {

                //生成Excel数据结构类
                GenerateExcelClass(table);
                //生成Excel数据容器类
                GenerateExcelContainer(table);
                //生成Excel实际数据的二进制数据
                GenerateExcelBinary(table);
            }
        }
    }
    /// <summary>
    /// 生成Excel数据结构类
    /// </summary>
    /// <param name="table">数据表</param>
    private static void GenerateExcelClass(DataTable table)
    {

        //获取数据表的变量名
        DataRow dataRowVariableName = GetVariableName(table);
        //获取数据表变量的类型
        DataRow dataRowType = GetDataRowType(table);
        //将要写入cs文件的字符串
        string str = "public class " + table.TableName + "\n{\n";
        for (int i = 0; i < table.Columns.Count; i++)
        {
            str += "  public " + dataRowType[i] + " " + dataRowVariableName[i].ToString() + ";\n";
        }
        str += "\n}";

        //如果不存在将要写入的文件夹则创建一个
        if (!Directory.Exists(FileClass_Path))
            Directory.CreateDirectory(FileClass_Path);
        //将字符串写入脚本文件
        File.WriteAllText(FileClass_Path + table.TableName + ".cs", str);

        //刷新
        AssetDatabase.Refresh();

    }

    /// <summary>
    /// 生成Excel数据容器类
    /// </summary>
    /// <param name="table">数据表</param>
    private static void GenerateExcelContainer(DataTable table)
    {
        //得到主键所在列的索引
        int primaryKey = GetKey(table);
        //得到字段类型行
        DataRow dataTypeRow = GetDataRowType(table);
        //将要写入数据容器类的字符串
        string str = "using System.Collections.Generic;\npublic class " + table.TableName + "Container\n{\n";

        str += "  public Dictionary<" + dataTypeRow[primaryKey] + "," + table.TableName + "> " + table.TableName + "dic";
        str += " = new Dictionary<" + dataTypeRow[primaryKey] + "," + table.TableName + ">();\n}";

        //如果不存在数据容纳器类的文件夹就创建一个
        if (!File.Exists(File_Container_Path))
            Directory.CreateDirectory(File_Container_Path);

        //将字符串写入文件中
        File.WriteAllText(File_Container_Path + table.TableName + "Container.cs", str);

        //刷新
        AssetDatabase.Refresh();
    }


    public static void GenerateExcelBinary(DataTable table)
    {
        //如果不存在此文件夹路径则生成一个
        if (!File.Exists(BinaryDataMgr.File_Binary_Path))
        {
            Directory.CreateDirectory(BinaryDataMgr.File_Binary_Path);
        }
        
        //存储用的bite数组
        Byte[] bytes;

        //打开文件
        using (FileStream fs = File.Open(BinaryDataMgr.File_Binary_Path + table.TableName + ".Xiao", 
            FileMode.OpenOrCreate, FileAccess.Write))
        {
            //存储一共有多少行实际数据
            fs.Write(BitConverter.GetBytes(table.Rows.Count - Data_Start_Index), 0, 4);
            //得到主键名字
            string keyName = GetVariableName(table)[GetKey(table)].ToString();
            bytes = Encoding.UTF8.GetBytes(keyName);
            //写入主键类型字符串的二进制长度
            fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
            //写入主键类型的字符串二进制
            fs.Write(bytes, 0, bytes.Length);

            //得到数据类型的行
            DataRow dataRowType = GetDataRowType(table);
            //目前所使用的行数据
            DataRow nowDataRow;
            //循环遍历数据行
            for (int i = Data_Start_Index; i < table.Rows.Count; i++)
            {
                
                nowDataRow = table.Rows[i];
                //遍历每行中的每一列
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    //根据数据的类型不同写入不同类型的二进制数据
                    switch (dataRowType[j].ToString())
                    {
                        case "int":
                            fs.Write(BitConverter.GetBytes(int.Parse(nowDataRow[j].ToString())), 0, 4);
                            break;
                        case "float":
                            fs.Write(BitConverter.GetBytes(float.Parse(nowDataRow[j].ToString())), 0, 4);
                            break;
                        case "bool":
                            fs.Write(BitConverter.GetBytes(bool.Parse(nowDataRow[j].ToString())), 0, 1);
                            break;
                        case "string":
                            bytes = Encoding.UTF8.GetBytes(nowDataRow[j].ToString());
                            //写入字符串数据的二进制数据的长度
                            fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
                            //写入字符串数据的二进制
                            fs.Write(bytes, 0, bytes.Length);
                            break;
                    }
                }

                
            }

            //刷新
            AssetDatabase.Refresh();
            //Debug.Log(Application.streamingAssetsPath);
            //释放缓存
            fs.Dispose();
            //关闭文件
            fs.Close();
        }
    }

        /// <summary>
        /// 获取数据表的变量名
        /// </summary>
        /// <param name="table">数据表</param>
        private static DataRow GetVariableName(DataTable table)
        {
            return table.Rows[0];
        }

        private static DataRow GetDataRowType(DataTable table)
        {
            return table.Rows[1];
        }

        /// <summary>
        /// 返回主键的序号
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static int GetKey(DataTable table)
        {
        DataRow dataRow = table.Rows[2];
            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (dataRow[i].ToString() == "Key")
                {
                    return i;
                }
            }
            return 0;
        }
}

