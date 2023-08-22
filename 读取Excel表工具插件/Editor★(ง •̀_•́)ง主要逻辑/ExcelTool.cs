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
    //Excel�ļ���ŵ�·����
    private static string File_Path = Application.dataPath + "/Resources/Editor/ExcelFile";
    //Excel�ļ�������ݽṹ��ű��ļ���·��
    private static string FileClass_Path = Application.dataPath + "/Scripts/Binary/ExcelTable/";
    //Excel�ļ��������������ű��ļ���·��
    private static string File_Container_Path = Application.dataPath + "/Scripts/Binary/ExcelContainer/";
    
    //private static string File_Binary_Path = Application.dataPath + "/Resources/StreamingAssets/ExcelBinary/";

    //����ʵ�����ݿ�ʼ����
    private static int Data_Start_Index = 4;

    /// <summary>
    /// �������ݱ�
    /// </summary>
    [MenuItem("GameTool/GenerateExcel")]
    public static void GenerateExcel()
    {

        DirectoryInfo di = Directory.CreateDirectory(File_Path);
        //���ļ���excel���ݵı�����
        DataTableCollection result;
        //�õ��ļ��������е��ļ�
        FileInfo[] fileInfos = di.GetFiles();
        for (int i = 0; i < fileInfos.Length; i++)
        {
            //����ļ��ĺ�׺����.xlsx����.xls�Ͳ����������߼�
            if (fileInfos[i].Extension != ".xlsx" && fileInfos[i].Extension != ".xls")
                continue;


            //���ļ�
            using (FileStream fs = fileInfos[i].Open(FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                result = excelDataReader.AsDataSet().Tables;
            }

            //����excel�������
            foreach (DataTable table in result)
            {

                //����Excel���ݽṹ��
                GenerateExcelClass(table);
                //����Excel����������
                GenerateExcelContainer(table);
                //����Excelʵ�����ݵĶ���������
                GenerateExcelBinary(table);
            }
        }
    }
    /// <summary>
    /// ����Excel���ݽṹ��
    /// </summary>
    /// <param name="table">���ݱ�</param>
    private static void GenerateExcelClass(DataTable table)
    {

        //��ȡ���ݱ�ı�����
        DataRow dataRowVariableName = GetVariableName(table);
        //��ȡ���ݱ����������
        DataRow dataRowType = GetDataRowType(table);
        //��Ҫд��cs�ļ����ַ���
        string str = "public class " + table.TableName + "\n{\n";
        for (int i = 0; i < table.Columns.Count; i++)
        {
            str += "  public " + dataRowType[i] + " " + dataRowVariableName[i].ToString() + ";\n";
        }
        str += "\n}";

        //��������ڽ�Ҫд����ļ����򴴽�һ��
        if (!Directory.Exists(FileClass_Path))
            Directory.CreateDirectory(FileClass_Path);
        //���ַ���д��ű��ļ�
        File.WriteAllText(FileClass_Path + table.TableName + ".cs", str);

        //ˢ��
        AssetDatabase.Refresh();

    }

    /// <summary>
    /// ����Excel����������
    /// </summary>
    /// <param name="table">���ݱ�</param>
    private static void GenerateExcelContainer(DataTable table)
    {
        //�õ����������е�����
        int primaryKey = GetKey(table);
        //�õ��ֶ�������
        DataRow dataTypeRow = GetDataRowType(table);
        //��Ҫд��������������ַ���
        string str = "using System.Collections.Generic;\npublic class " + table.TableName + "Container\n{\n";

        str += "  public Dictionary<" + dataTypeRow[primaryKey] + "," + table.TableName + "> " + table.TableName + "dic";
        str += " = new Dictionary<" + dataTypeRow[primaryKey] + "," + table.TableName + ">();\n}";

        //�����������������������ļ��оʹ���һ��
        if (!File.Exists(File_Container_Path))
            Directory.CreateDirectory(File_Container_Path);

        //���ַ���д���ļ���
        File.WriteAllText(File_Container_Path + table.TableName + "Container.cs", str);

        //ˢ��
        AssetDatabase.Refresh();
    }


    public static void GenerateExcelBinary(DataTable table)
    {
        //��������ڴ��ļ���·��������һ��
        if (!File.Exists(BinaryDataMgr.File_Binary_Path))
        {
            Directory.CreateDirectory(BinaryDataMgr.File_Binary_Path);
        }
        
        //�洢�õ�bite����
        Byte[] bytes;

        //���ļ�
        using (FileStream fs = File.Open(BinaryDataMgr.File_Binary_Path + table.TableName + ".Xiao", 
            FileMode.OpenOrCreate, FileAccess.Write))
        {
            //�洢һ���ж�����ʵ������
            fs.Write(BitConverter.GetBytes(table.Rows.Count - Data_Start_Index), 0, 4);
            //�õ���������
            string keyName = GetVariableName(table)[GetKey(table)].ToString();
            bytes = Encoding.UTF8.GetBytes(keyName);
            //д�����������ַ����Ķ����Ƴ���
            fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
            //д���������͵��ַ���������
            fs.Write(bytes, 0, bytes.Length);

            //�õ��������͵���
            DataRow dataRowType = GetDataRowType(table);
            //Ŀǰ��ʹ�õ�������
            DataRow nowDataRow;
            //ѭ������������
            for (int i = Data_Start_Index; i < table.Rows.Count; i++)
            {
                
                nowDataRow = table.Rows[i];
                //����ÿ���е�ÿһ��
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    //�������ݵ����Ͳ�ͬд�벻ͬ���͵Ķ���������
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
                            //д���ַ������ݵĶ��������ݵĳ���
                            fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
                            //д���ַ������ݵĶ�����
                            fs.Write(bytes, 0, bytes.Length);
                            break;
                    }
                }

                
            }

            //ˢ��
            AssetDatabase.Refresh();
            //Debug.Log(Application.streamingAssetsPath);
            //�ͷŻ���
            fs.Dispose();
            //�ر��ļ�
            fs.Close();
        }
    }

        /// <summary>
        /// ��ȡ���ݱ�ı�����
        /// </summary>
        /// <param name="table">���ݱ�</param>
        private static DataRow GetVariableName(DataTable table)
        {
            return table.Rows[0];
        }

        private static DataRow GetDataRowType(DataTable table)
        {
            return table.Rows[1];
        }

        /// <summary>
        /// �������������
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

