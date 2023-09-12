using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginGameRequest : BaseRequest
{
    string roomName;
    //用户的名字
    private string userName;

    public BeginGameRequest()
    {
        requestCode = RequestCode.Room;
        actionCode = ActionCode.BeginGame;
    }

    public BeginGameRequest(string roomName, string userName)
    {
        this.roomName = roomName;
        this.userName = userName;
        requestCode = RequestCode.Room;
        actionCode = ActionCode.BeginGame;
    }

    public override MainPack GetPack()
    {
        MainPack pack = new MainPack();
        pack.RoomPackList.Add(new RoomPack());
        pack.RoomPackList[0].RoomName = roomName;
        pack.RoomPackList[0].UserDic.Add(userName, new UserPack() { UserName = userName });
        pack.RequestCode = requestCode;
        pack.ActionCode = actionCode;
        return pack;
    }

    public override void OnResponse(MainPack pack)
    {
        switch (pack.ReturnCode)
        {
            case ReturnCode.Succeed:
                Debug.Log("开始游戏成功");
                if(UIMgr.Instance.GetPanel<RoomInPanel>())
                {
                    UIMgr.Instance.GetPanel<RoomInPanel>().EnterGame(pack);
                }
                
                break;
            case ReturnCode.Fail:
                Debug.Log("开始游戏失败");
                break;
        }
    }
}
