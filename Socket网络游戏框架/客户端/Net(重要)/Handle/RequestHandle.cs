﻿using SocketGameProtocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 处理请求
/// </summary>
public class RequestHandle
{
    //响应字典
    public Dictionary<ActionCode, BaseRequest> responseDic = new Dictionary<ActionCode, BaseRequest>();
    
    ////响应执行列表
    //private List<Action> callBackActionList=new List<Action>();

    UnityAction unityAction;

    public RequestHandle()
    {
        MonoMgr.Instance.AddFixedUpdateAction(RequestUpdate);
        responseDic.Add(ActionCode.CreateRoom, new CreateRoomRequest());
        responseDic.Add(ActionCode.DeleteRoom, new DeleteRoomRequest());
        responseDic.Add(ActionCode.AddRoom, new AddRoomRequest());
        responseDic.Add(ActionCode.ExitRoom, new ExitRoomRequest());
        responseDic.Add(ActionCode.BeginGame, new BeginGameRequest());
        responseDic.Add(ActionCode.PlayerInput, new InputRequest());
        responseDic.Add(ActionCode.Fire, new FireRequest());
        responseDic.Add(ActionCode.Hit, new HitRequest());
    }

    /// <summary>
    /// 执行请求过后的响应
    /// </summary>
    private void RequestUpdate()
    {
        try
        {
            //if (callBackActionList.Count > 0)
            //{
            //    foreach (Action action in callBackActionList)
            //    {
            //        action?.Invoke();
            //    }
            //    callBackActionList.Clear();
            //}
            unityAction?.Invoke();
            unityAction = null;
        }
        catch(Exception e)
        {
            Debug.Log("执行请求出错" + e.Message);
        }
       
        
    }

    /// <summary>
    /// 处理来自服务器的消息
    /// </summary>
    /// <param name="pack"></param>
    public void HandleRequest(MainPack pack)
    {
        if (responseDic.TryGetValue(pack.ActionCode, out BaseRequest request))
        {
            unityAction +=()=> { request.OnResponse(pack); };
            //callBackActionList.Add(() =>
            //{
            //    request.OnResponse(pack);
            //});
        }
        else
        {
            Debug.LogWarning("不能找到对应的处理");
        }
    }

    /// <summary>
    /// 绑定响应
    /// </summary>
    /// <param name="actionCode"></param>
    /// <param name="responseAction"></param>
    public void BindResponse(ActionCode actionCode, UnityAction<MainPack> responseAction)
    {
        if (responseDic.TryGetValue(actionCode, out BaseRequest request))
        {
            request.responseAction += responseAction;
        }
        else
        {
            Debug.Log("绑定响应失败,不能找到对应的处理");
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
