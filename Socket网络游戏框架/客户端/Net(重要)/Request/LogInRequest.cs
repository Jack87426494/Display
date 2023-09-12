using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogInRequest : BaseRequest
{
    private string userName;
    private string password;

    public LogInRequest(string userName, string password)
    {
        this.userName = userName;
        this.password = password;
        actionCode = ActionCode.Login;
        requestCode = RequestCode.User;
    }

    public override void OnResponse(MainPack pack)
    {
        switch (pack.ReturnCode)
        {
            case ReturnCode.Succeed:
                Debug.Log("登录成功");
                UIMgr.Instance.HidePanel<LoadPanel>();
                UIMgr.Instance.ShowPanel<RoomPanel>();
                GameMgr.Instance.localName = pack.LoginPack.UserName;
                break;
            case ReturnCode.Fail:
                Debug.Log("登陆失败");
                break;
        }
    }

    public override MainPack GetPack()
    {
        MainPack pack = new MainPack();
        pack.RequestCode = requestCode;
        pack.ActionCode = actionCode;
        LoginPack loginPack = new LoginPack();
        loginPack.UserName = userName;
        loginPack.Password = password;
        pack.LoginPack = loginPack;
        return pack;
    }
}
