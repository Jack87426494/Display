using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseRequest
{
    protected ActionCode actionCode = ActionCode.ActionNone;
    protected RequestCode requestCode = RequestCode.RequestNone;

    /// <summary>
    /// 响应事件
    /// </summary>
    public UnityAction<MainPack> responseAction;


    public BaseRequest()
    {
        
    }
   
    public ActionCode GetActionCode
    {
        get
        {
            return actionCode;
        }
    }

    /// <summary>
    /// 回应服务器要执行的方法
    /// </summary>
    /// <param name="pack"></param>
    public virtual void OnResponse(MainPack pack)
    {
        responseAction?.Invoke(pack);
    }

    /// <summary>
    /// 得到要发送的包
    /// pack.RequestCode = requestCode;
    /// pack.ActionCode = actionCode;
    /// </summary>
    /// <param name="pack"></param>
    public abstract MainPack GetPack();
}
