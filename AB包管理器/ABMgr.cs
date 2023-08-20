﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class ABMgr : SingletonAutoMono<ABMgr>
{
    //主包
    private AssetBundle mainAB = null;
    //主包依赖获取配置文件
    private AssetBundleManifest manifest = null;

    //选择存储 AB包的容器
    //AB包不能够重复加载 否则会报错
    //字典知识 用来存储 AB包对象
    private Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();

    /// <summary>
    /// 获取AB包加载路径
    /// </summary>
    private string StreamingAssetsPath
    {
        get
        {
            return Application.streamingAssetsPath + "/";
        }
    }

    private string PersistentDataPath
    {
        get
        {
#if UNITY_IOS
            return Application.persistentDataPath + "/AB/IOS/";
#elif  UNITY_Andriod
            return Application.persistentDataPath + "/AB/Andriod/";
#else
            return Application.persistentDataPath + "/AB/PC/";
#endif

        }
    }

    /// <summary>
    /// 主包名 根据平台不同 报名不同
    /// </summary>
    private string MainName
    {
        get
        {
#if UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else
            return "PC";
#endif
        }
    }

    /// <summary>
    /// 加载主包 和 配置文件
    /// 因为加载所有包是 都得判断 通过它才能得到依赖信息
    /// 所以写一个方法
    /// </summary>
    private void LoadMainAB()
    {
        if( mainAB == null )
        {
            string path = File.Exists(PersistentDataPath+MainName) ? PersistentDataPath+MainName : StreamingAssetsPath+MainName;
            mainAB = AssetBundle.LoadFromFile(path);
            manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
    }

    /// <summary>
    /// 加载指定包的依赖包
    /// </summary>
    /// <param name="abName"></param>
    private void LoadDependencies(string abName)
    {
        //加载主包
        LoadMainAB();
        //获取依赖包
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            if (!abDic.ContainsKey(strs[i]))
            {
                string path = File.Exists(PersistentDataPath + strs[i]) ? PersistentDataPath + strs[i] : StreamingAssetsPath + strs[i];
                AssetBundle ab = AssetBundle.LoadFromFile(path);
                abDic.Add(strs[i], ab);
            }
        }
    }

    /// <summary>
    /// 泛型资源同步加载
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <returns></returns>
    public T LoadRes<T>(string abName, string resName) where T:Object
    {
        //加载依赖包
        LoadDependencies(abName);
        //加载目标包
        if ( !abDic.ContainsKey(abName) )
        {
            string path = File.Exists(PersistentDataPath+abName) ? PersistentDataPath + abName : StreamingAssetsPath + abName;
            AssetBundle ab = AssetBundle.LoadFromFile(path);
            abDic.Add(abName, ab);
        }

        //得到加载出来的资源
        T obj = abDic[abName].LoadAsset<T>(resName);
        //如果是GameObject 因为GameObject 100%都是需要实例化的
        //所以我们直接实例化
        if (obj is GameObject)
            return Instantiate(obj);
        else
            return obj;
    }

    /// <summary>
    /// Type同步加载指定资源
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public Object LoadRes(string abName, string resName, System.Type type) 
    {
        //加载依赖包
        LoadDependencies(abName);
        //加载目标包
        if (!abDic.ContainsKey(abName))
        {
            string path = File.Exists(PersistentDataPath+abName) ? PersistentDataPath + abName : StreamingAssetsPath + abName;
            AssetBundle ab = AssetBundle.LoadFromFile(path);
            abDic.Add(abName, ab);
        }

        //得到加载出来的资源
        Object obj = abDic[abName].LoadAsset(resName, type);
        //如果是GameObject 因为GameObject 100%都是需要实例化的
        //所以我们直接实例化
        if (obj is GameObject)
            return Instantiate(obj);
        else
            return obj;
    }

    /// <summary>
    /// 名字 同步加载指定资源
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <returns></returns>
    public Object LoadRes(string abName, string resName)
    {
        //加载依赖包
        LoadDependencies(abName);
        //加载目标包
        if (!abDic.ContainsKey(abName))
        {
            string path = File.Exists(PersistentDataPath+abName) ? PersistentDataPath + abName : StreamingAssetsPath + abName;
            AssetBundle ab = AssetBundle.LoadFromFile(path);
            abDic.Add(abName, ab);
        }

        //得到加载出来的资源
        Object obj = abDic[abName].LoadAsset(resName);
        //如果是GameObject 因为GameObject 100%都是需要实例化的
        //所以我们直接实例化
        if (obj is GameObject)
            return Instantiate(obj);
        else
            return obj;
    }

    /// <summary>
    /// 泛型异步加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="callBack"></param>
    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack) where T:Object
    {
        StartCoroutine(ReallyLoadResAsync<T>(abName, resName, callBack));
    }
    //正儿八经的 协程函数
    private IEnumerator ReallyLoadResAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
    {
        //加载依赖包
        LoadDependencies(abName);
        //加载目标包
        if (!abDic.ContainsKey(abName))
        {
            string path = File.Exists(PersistentDataPath + abName) ? PersistentDataPath + abName : StreamingAssetsPath + abName;
            AssetBundle ab = AssetBundle.LoadFromFile(path);
            abDic.Add(abName, ab);
        }
        //异步加载包中资源
        AssetBundleRequest abq = abDic[abName].LoadAssetAsync<T>(resName);
        yield return abq;

        if (abq.asset is GameObject)
            callBack(Instantiate(abq.asset) as T);
        else
            callBack(abq.asset as T);
    }

    /// <summary>
    /// Type异步加载资源
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="type"></param>
    /// <param name="callBack"></param>
    public void LoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack)
    {
        StartCoroutine(ReallyLoadResAsync(abName, resName, type, callBack));
    }

    private IEnumerator ReallyLoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack)
    {
        //加载依赖包
        LoadDependencies(abName);
        //加载目标包
        if (!abDic.ContainsKey(abName))
        {
            string path = File.Exists(PersistentDataPath+abName) ? PersistentDataPath + abName : StreamingAssetsPath + abName;
            AssetBundle ab = AssetBundle.LoadFromFile(path);
            abDic.Add(abName, ab);
        }
        //异步加载包中资源
        AssetBundleRequest abq = abDic[abName].LoadAssetAsync(resName, type);
        yield return abq;

        if (abq.asset is GameObject)
            callBack(Instantiate(abq.asset));
        else
            callBack(abq.asset);
    }

    /// <summary>
    /// 名字 异步加载 指定资源
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="callBack"></param>
    public void LoadResAsync(string abName, string resName, UnityAction<Object> callBack)
    {
        StartCoroutine(ReallyLoadResAsync(abName, resName, callBack));
    }

    private IEnumerator ReallyLoadResAsync(string abName, string resName, UnityAction<Object> callBack)
    {
        //加载依赖包
        LoadDependencies(abName);
        //加载目标包
        if (!abDic.ContainsKey(abName))
        {
            string path = File.Exists(PersistentDataPath+abName) ? PersistentDataPath + abName : StreamingAssetsPath + abName;
            AssetBundle ab = AssetBundle.LoadFromFile(path);
            abDic.Add(abName, ab);
        }
        //异步加载包中资源
        AssetBundleRequest abq = abDic[abName].LoadAssetAsync(resName);
        yield return abq;

        if (abq.asset is GameObject)
            callBack(Instantiate(abq.asset));
        else
            callBack(abq.asset);
    }

    //卸载AB包的方法
    public void UnLoadAB(string name)
    {
        if( abDic.ContainsKey(name) )
        {
            abDic[name].Unload(false);
            abDic.Remove(name);
        }
    }

    //清空AB包的方法
    public void ClearAB()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
        //卸载主包
        mainAB = null;
    }
}
