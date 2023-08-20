using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameDataMgr : BaseMgr<GameDataMgr>
{
    //使用json存储读取数据

    /// <summary>
    ///保存数据 
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="obj">数据</param>
    /// <param name="jsonType">保存的方式</param>
    public void SaveData<T>(T obj,EJsonType jsonType=EJsonType.JsonUtility) 
    {
        if(jsonType==EJsonType.JsonUtility)
        {
            File.WriteAllText(Application.persistentDataPath+"/"+ typeof(T).Name + ".json",JsonUtility.ToJson(obj));
        }
        else if(jsonType==EJsonType.JsonMapper)
        {
            File.WriteAllText(Application.persistentDataPath + "/" + typeof(T).Name + ".json", JsonMapper.ToJson(obj));
        }
    }

    /// <summary>
    /// 读取数据
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="jsonType">读取的方式</param>
    /// <returns></returns>
    public T GetData<T>(EJsonType jsonType=EJsonType.JsonUtility) where T:new ()
    {
        string path = Application.persistentDataPath + "/" + typeof(T).Name + ".json";
        if (!File.Exists(path))
        {
            path = Application.streamingAssetsPath + "/" + typeof(T).Name + ".json";
            if(!File.Exists(path))
            {
                return new T();
            }
        }
        string dataStr = File.ReadAllText(path);
        if (jsonType == EJsonType.JsonUtility)
        {
            return JsonUtility.FromJson<T>(dataStr);
        }
        else if (jsonType == EJsonType.JsonMapper)
        {
            return JsonMapper.ToObject<T>(dataStr);
        }
        return new T();
    }
}
