using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitRoomRequest : BaseRequest
{
    string roomName;
    //用户的名字
    private string userName;

    public ExitRoomRequest()
    {
        requestCode = RequestCode.Room;
        actionCode = ActionCode.ExitRoom;
    }

    public ExitRoomRequest(string roomName, string userName)
    {
        this.roomName = roomName;
        this.userName = userName;
        requestCode = RequestCode.Room;
        actionCode = ActionCode.ExitRoom;
    }

    public override MainPack GetPack()
    {
        MainPack pack = new MainPack();
        pack.RoomPackList.Add(new RoomPack());
        pack.RoomPackList[0].RoomName = roomName;
        pack.RoomPackList[0].UserDic.Add(userName,new UserPack() { UserName=userName});
        pack.RequestCode = requestCode;
        pack.ActionCode = actionCode;
        return pack;
    }

    public override void OnResponse(MainPack pack)
    {
        switch (pack.ReturnCode)
        {
            case ReturnCode.Succeed:
                Debug.Log("退出房间成功");
                UIMgr.Instance.GetPanel<RoomInPanel>().ExitUser(pack);
                break;
            case ReturnCode.Fail:
                Debug.Log("退出房间失败");
                break;
        }
    }
}
