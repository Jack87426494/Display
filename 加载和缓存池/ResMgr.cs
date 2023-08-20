using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ResMgr : BaseMgr<ResMgr>
{
    #region 统一加载

    /// <summary>
    /// 统一同步加载
    /// </summary>
    /// <typeparam name="T">资源的类型</typeparam>
    /// <param name="resName">资源的名字</param>
    /// <returns></returns>
    public T Load<T>(string resName) where T : UnityEngine.Object
    {
        return LoadAB<T>(resName);

        ////ab包打包出了点问题
        //return ReLoad<T>(resName);

        //#if UNITY_EDITOR
        //        return EditorLoad<T>(resName);
        //#else
        //        return LoadAB<T>(resName);
        //#endif
    }

    /// <summary>
    /// 统一同步加载
    /// </summary>
    /// <typeparam name="T">资源的类型</typeparam>
    /// <param name="resName">资源的名字</param>
    /// <returns></returns>
    public void Load<T>(string resName, UnityAction<T> callBack) where T : UnityEngine.Object
    {
        callBack.Invoke(LoadAB<T>(resName));
        //ab包打包出了点问题
        //callBack?.Invoke(ReLoad<T>(resName));
//#if UNITY_EDITOR
//        //callBack?.Invoke(EditorLoad<T>(resName));
//#else
//        callBack?.Invoke(LoadAB<T>(resName));
//#endif
    }

    /// <summary>
    /// 统一异步加载
    /// </summary>
    /// <typeparam name="T">资源的类型</typeparam>
    /// <param name="resName">资源的名字</param>
    /// <param name="callBack">回调函数</param>
    /// <returns></returns>
    public void LoadAsync<T>(string resName, UnityAction<T> callBack) where T : UnityEngine.Object
    {
        ////ab包打包出了点问题
        //callBack(ReLoad<T>(resName));
#if UNITY_EDITOR
        
         callBack(EditorLoad<T>(resName));
#else
         LoadABAsync<T>(resName,callBack);
#endif
    }

    #endregion


    #region Reources加载

    /// <summary>
    /// 同步Reources加载
    /// </summary>
    /// <typeparam name="T">资源的类型</typeparam>
    /// <param name="name">资源的名字</param>
    /// <returns></returns>
    public T ReLoad<T>(string name) where T:UnityEngine.Object
    {
        T t = null;
        if (typeof(T) == typeof(Material))
        {
            t = Resources.Load<T>("Material/" + name);
        }
        else if (typeof(T) == typeof(GameObject))
        {
            t = Resources.Load<T>("Prefab/" + name );
        }
        else if (typeof(T) == typeof(Animator))
        {
            t = Resources.Load<T>( "Animator/" + name );
        }
        else if (typeof(T) == typeof(Sprite))
        {
            t = Resources.Load<T>( "png/" + name);
        }
        else
        {
            t = Resources.Load<T>(name);
        }

        if (t is GameObject)
        {
            return GameObject.Instantiate<T>(t);
        }
        else
        {
            return t;
        }
    }

    /// <summary>
    /// 异步加载
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径</param>
    /// <param name="callBack">回调函数</param>
    public void ReLoadAsyn<T>(string path,UnityAction<T> callBack) where T:UnityEngine.Object
    {
        MonoMgr.Instance.StartCoroutine(IResAsyn(path,callBack));
    }

    IEnumerator IResAsyn<T>(string path, UnityAction<T> callBack) where T : UnityEngine.Object
    {
        ResourceRequest rq = Resources.LoadAsync<T>(path);

        while(!rq.isDone)
        {
            yield return rq;
        }

        if(rq.asset as T is GameObject)
        {
            callBack(GameObject.Instantiate(rq.asset) as T);
        }
        else
        {
            callBack(rq.asset as T);
        }
    }

    #endregion


#if UNITY_EDITOR
    #region 编辑器加载

    /// <summary>
    /// 同步编辑器加载
    /// </summary>
    /// <typeparam name="T">资源的类型</typeparam>
    /// <param name="name">资源的名字</param>
    /// <returns></returns>
    public T EditorLoad<T>(string name) where T : UnityEngine.Object
    {
        T t=null;
        if(typeof(T)==typeof(Material))
        {
            t = AssetDatabase.LoadAssetAtPath<T>(NameConfig.EditorLoadPath+ "Material/" + name + ".mat");
        }
        else if(typeof(T)==typeof(GameObject))
        {
            t = AssetDatabase.LoadAssetAtPath<T>(NameConfig.EditorLoadPath+ "Prefab/" + name + ".prefab");
        }
        else if(typeof(T)==typeof(Animator))
        {
            t = AssetDatabase.LoadAssetAtPath<T>(NameConfig.EditorLoadPath + "Animator/" + name + ".controller");
        }
        else if(typeof(T)==typeof(Sprite))
        {
            t = AssetDatabase.LoadAssetAtPath<T>(NameConfig.EditorLoadPath + "png/" + name + ".png");
        }
        
        if (t is GameObject)
        {
            return GameObject.Instantiate<T>(t);
        }
        else
        {
            return t;
        }
    }
#endregion
#endif

    #region ab包加载
    /// <summary>
    /// 同步ab包加载
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="abName">ab包名字</param>
    /// <param name="resName">资源的名字</param>
    /// <returns>成功加载的资源</returns>
    public T LoadAB<T>(string abName,string resName) where T:Object
    {
        return ABMgr.Instance.LoadRes<T>(abName, resName);
    }

    /// <summary>
    /// 同步ab包加载
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="resName">资源的名字</param>
    /// <returns>成功加载的资源</returns>
    public T LoadAB<T>( string resName) where T : Object
    {
        // 在打包后使用ab包加载
        if (typeof(T) == typeof(Material))
        {
            return ABMgr.Instance.LoadRes<T>("mat", resName);
        }
        else if (typeof(T) == typeof(GameObject))
        {
            return ABMgr.Instance.LoadRes<T>("prefab", resName);
        }
        else if (typeof(T) == typeof(Animator))
        {
            return ABMgr.Instance.LoadRes<T>("controller", resName);
        }
        else if(typeof(T)==typeof(Sprite))
        {
            return ABMgr.Instance.LoadRes<T>("img", resName);
        }
        return null;
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <typeparam name="T">资源的类型</typeparam>
    /// <param name="abName">ab包的名字</param>
    /// <param name="resName">资源的名字</param>
    /// <param name="callBack">回调函数</param>
    public void LoadABAsync<T>(string abName,string resName,UnityAction<T> callBack) where T:Object
    {
         ABMgr.Instance.LoadResAsync<T>(abName, resName, callBack);
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <typeparam name="T">资源的类型</typeparam>
    /// <param name="resName">资源的名字</param>
    /// <param name="callBack">回调函数</param>
    public void LoadABAsync<T>(string resName, UnityAction<T> callBack) where T : Object
    {
        // 在打包后使用ab包加载
        if (typeof(T) == typeof(Material))
        {
            ABMgr.Instance.LoadResAsync<T>("mat", resName, callBack);
        }
        else if (typeof(T) == typeof(GameObject))
        {
            ABMgr.Instance.LoadResAsync<T>("prefab", resName, callBack);
        }
        else if(typeof(T)==typeof(Animator))
        {
            ABMgr.Instance.LoadResAsync<T>("controller", resName, callBack);
        }
        else if (typeof(T) == typeof(Sprite))
        {
            ABMgr.Instance.LoadResAsync<T>("img", resName,callBack);
        }
    }

    #endregion
}
