using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindRoomRequest : BaseRequest
{
    private string roomName;

    public FindRoomRequest(string roomName)
    {
        actionCode = ActionCode.FindRoom;
        requestCode = RequestCode.Room;
        this.roomName = roomName;
    }

    public override void OnResponse(MainPack pack)
    {
        switch (pack.ReturnCode)
        {
            case ReturnCode.Succeed:
                Debug.Log("查找房间成功");
                UIMgr.Instance.GetPanel<RoomPanel>().UpdateRoomNum(1);
                break;
            case ReturnCode.Fail:
                Debug.Log("查找房间失败");
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
        pack.RoomPackList.Add(roomPack);
        return pack;
    }
}
