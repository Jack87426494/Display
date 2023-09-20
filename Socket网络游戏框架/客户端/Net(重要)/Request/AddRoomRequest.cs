using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddRoomRequest : BaseRequest
{
    private string roomName;

    public AddRoomRequest()
    {
        requestCode = RequestCode.Room;
        actionCode = ActionCode.AddRoom;
    }

    public AddRoomRequest(string roomName)
    {
        this.roomName = roomName;
        requestCode = RequestCode.Room;
        actionCode = ActionCode.AddRoom;
    }

    public override MainPack GetPack()
    {
        MainPack pack = new MainPack();
        pack.RequestCode = requestCode;
        pack.ActionCode = actionCode;
        pack.RoomPackList.Add(new RoomPack());
        pack.RoomPackList[0].RoomName = roomName;
        return pack;
    }

    public override void OnResponse(MainPack pack)
    {
        switch (pack.ReturnCode)
        {
            case ReturnCode.Succeed:
                Debug.Log("加入房间成功");
                GameMgr.Instance.roomName = pack.RoomPackList[0].RoomName;
                if (UIMgr.Instance.GetPanel<RoomInPanel>()!=null)
                {
                    UIMgr.Instance.GetPanel<RoomInPanel>().CreatUser(pack);
                    
                }
                else
                {
                    UIMgr.Instance.GetPanel<RoomPanel>().AddRoom(pack);
                }
                
                break;
            case ReturnCode.Fail:
                Debug.Log("加入房间失败");
                break;
        }
    }
}
