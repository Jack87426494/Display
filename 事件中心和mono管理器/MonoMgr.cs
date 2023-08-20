using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

public class MonoMgr : BaseMgr<MonoMgr>
{
    private MonoContraller monoContraller;

    public MonoMgr()
    {
        GameObject obj = new GameObject("MonoContraller");
        monoContraller=obj.AddComponent<MonoContraller>();
        GameObject.DontDestroyOnLoad(monoContraller);
    }

    /// <summary>
    /// update事件
    /// </summary>
    /// <param name="updateAction">添加的事件</param>
    public void AddUpdateAction(UnityAction updateAction)
    {
        monoContraller.updateActionList.Add(updateAction);
    }

    /// <summary>
    /// 开始协程
    /// </summary>
    /// <param name="enumerator">传入的协程</param>
    /// <returns></returns>
    public Coroutine StartCoroutine(IEnumerator enumerator)
    {
        return monoContraller.StartCoroutine(enumerator);
    }

    /// <summary>
    /// 开始协程
    /// </summary>
    /// <param name="enumerator">传入的协程</param>
    /// <returns></returns>
    public Coroutine StartCoroutine(string name)
    {
        return monoContraller.StartCoroutine(name);
    }

    /// <summary>
    /// 开始协程
    /// </summary>
    /// <param name="name">协程的名字</param>
    /// <param name="value">协程的参数</param>
    /// <returns></returns>
    public Coroutine StartCoroutine(string name, [DefaultValue(null)] Object value)
    {
        return monoContraller.StartCoroutine(name,value);
    }
}
