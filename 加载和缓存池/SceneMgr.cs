using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneMgr : BaseMgr<SceneMgr>
{
    /// <summary>
    /// 同步加载场景
    /// </summary>
    /// <param name="sceneName">场景名字</param>
    /// <param name="callBack">回调函数</param>
    public void LoadScene(string sceneName,UnityAction callBack=null)
    {
        SceneManager.LoadScene(sceneName);
        callBack?.Invoke();
    }

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="sceneName">场景名字</param>
    /// <param name="callBack">回调函数</param>
    public void LoadSceneAsync(string sceneName,UnityAction callBack=null)
    {
        MonoMgr.Instance.StartCoroutine(IReallyLoadSceneAsync(sceneName,callBack));
    }

    IEnumerator IReallyLoadSceneAsync(string sceneName, UnityAction callBack = null)
    {
        AsyncOperation ao=SceneManager.LoadSceneAsync(sceneName);

        while(!ao.isDone)
        {
            yield return null;
        }

        callBack?.Invoke();
    }
}
