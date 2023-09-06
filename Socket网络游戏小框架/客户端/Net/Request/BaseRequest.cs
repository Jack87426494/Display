using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseRequest
{
    protected ActionCode actionCode = ActionCode.ActionNone;
    protected RequestCode requestCode = RequestCode.RequestNone;


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
    public abstract void OnResponse(MainPack pack);

    /// <summary>
    /// 得到要发送的包
    /// </summary>
    /// <param name="pack"></param>
    public abstract MainPack GetPack();
}
