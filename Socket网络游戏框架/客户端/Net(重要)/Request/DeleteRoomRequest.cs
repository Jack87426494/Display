using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteRoomRequest : BaseRequest
{
    private string roomName;

    public DeleteRoomRequest()
    {
        requestCode = RequestCode.Room;
        actionCode = ActionCode.DeleteRoom;
    }

    public DeleteRoomRequest(string roomName)
    {
        actionCode = ActionCode.DeleteRoom;
        requestCode = RequestCode.Room;
        this.roomName = roomName;
    }

    public override void OnResponse(MainPack pack)
    {
        switch (pack.ReturnCode)
        {
            case ReturnCode.Succeed:
                Debug.Log("删除房间成功");
                UIMgr.Instance.GetPanel<RoomPanel>().DeleteRoom(pack);
                break;
            case ReturnCode.Fail:
                Debug.Log("删除房间失败");
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
