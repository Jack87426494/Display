using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterRequest : BaseRequest
{
    private string userName;
    private string password;

    public RegisterRequest(string userName,string password)
    {
        this.userName = userName;
        this.password = password;
        actionCode = ActionCode.Register;
        requestCode = RequestCode.User;
    }

    public override void OnResponse(MainPack pack)
    {
        switch(pack.ReturnCode)
        {
            case ReturnCode.Succeed:
                Debug.Log("注册成功");
                break;
            case ReturnCode.Fail:
                Debug.Log("注册失败");
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
