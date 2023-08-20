using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 所有面板的总接口
/// </summary>
public interface IPanel
{
    /// <summary>
    /// 游戏对象
    /// </summary>
    public GameObject obj
    {
        get;
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <param name="callBack">回调函数</param>
    public void ShowPanel<T>(UnityAction<T> callBack = null) where T : class,IPanel;

    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <param name="callBack">回调函数</param>
    public void HidePane<T>(UnityAction<T> callBack = null) where T : class,IPanel;
}
