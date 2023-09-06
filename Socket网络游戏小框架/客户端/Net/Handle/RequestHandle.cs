using SocketGameProtocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

/// <summary>
/// 处理请求
/// </summary>
public class RequestHandle
{
    //响应字典
    public Dictionary<ActionCode, BaseRequest> responseDic = new Dictionary<ActionCode, BaseRequest>();
    
    //响应执行列表
    private List<Action> callBackActionList=new List<Action>(); 
    public RequestHandle()
    {
        MonoMgr.Instance.AddFixedUpdateAction(RequestUpdate);
    }

    /// <summary>
    /// 执行请求过后的响应
    /// </summary>
    private void RequestUpdate()
    {
        if(callBackActionList.Count>0)
        {
            foreach(Action action in callBackActionList)
            {
                action.Invoke();
            }
            callBackActionList.Clear();
        }
    }

    /// <summary>
    /// 处理来自服务器的消息
    /// </summary>
    /// <param name="pack"></param>
    public void HandleRequest(MainPack pack)
    {
        if (responseDic.TryGetValue(pack.ActionCode, out BaseRequest requst))
        {
            callBackActionList.Add(() =>
            {
                requst.OnResponse(pack);
            });
        }
        else
        {
            Debug.LogWarning("不能找到对应的处理");
        }
    }

    /// <summary>
    /// 发送请求
    /// </summary>
    /// <param name="actionCode"></param>
    /// <param name="pack"></param>
    public MainPack SendPack(BaseRequest baseRequst)
    {
        if(baseRequst.GetActionCode!=ActionCode.ActionNone)
        {
            if (!responseDic.ContainsKey(baseRequst.GetActionCode))
            {
                //添加请求得到回复的响应
                responseDic.Add(baseRequst.GetActionCode, baseRequst);
            }
        }
        return baseRequst.GetPack();
    }
}
