using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindAllRoomRequest : BaseRequest
{
    public FindAllRoomRequest()
    {
        actionCode = ActionCode.FindAllRoom;
        requestCode = RequestCode.Room;
    }

    public override void OnResponse(MainPack pack)
    {
        switch (pack.ReturnCode)
        {
            case ReturnCode.Succeed:
                Debug.Log("查找所有房间成功");
                UIMgr.Instance.GetPanel<RoomPanel>().UpdateRoomNum((uint)pack.RoomPackList.Count);
                break;
            case ReturnCode.Fail:
                Debug.Log("查找所有房间失败");
                break;
        }
    }

    public override MainPack GetPack()
    {
        MainPack pack = new MainPack();
        pack.ActionCode = actionCode;
        pack.RequestCode = requestCode;
        RoomPack roomPack = new RoomPack();
        pack.RoomPackList.Add(roomPack);
        return pack;
    }
}
