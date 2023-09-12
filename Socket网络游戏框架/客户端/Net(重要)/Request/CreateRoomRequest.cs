using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRoomRequest : BaseRequest
{
    //房间的名字
    private string roomName;

    //目前房间的人数
    private uint nowManNum;

    //房间人数的最大值
    private uint maxManNum;

    public CreateRoomRequest()
    {
        requestCode = RequestCode.Room;
        actionCode = ActionCode.CreateRoom;
    }

    public CreateRoomRequest(string roomName,uint nowManNum, uint maxManNum)
    {
        this.roomName = roomName;
        this.nowManNum = nowManNum;
        this.maxManNum = maxManNum;
        actionCode = ActionCode.CreateRoom;
        requestCode = RequestCode.Room;
    }

    public override void OnResponse(MainPack pack)
    { 
        switch (pack.ReturnCode)
        {
            case ReturnCode.Succeed:
                Debug.Log("创建房间成功");
                UIMgr.Instance.GetPanel<RoomPanel>().CreatRoos(pack);
                break;
            case ReturnCode.Fail:
                Debug.Log("创建房间失败");
                break;
        }
    }

    public override MainPack GetPack()
    {
        MainPack pack = new MainPack();
        pack.ActionCode = actionCode;
        pack.RequestCode = requestCode;
        RoomPack roomPack = new RoomPack();
        roomPack.RoomName = roomName;
        roomPack.MaxManNum = maxManNum;
        roomPack.NowManNum = nowManNum;
        pack.RoomPackList.Add(roomPack);
        return pack;
    }
}
