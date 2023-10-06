using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRequest : BaseRequest
{
    private string playerName;
    public bool isFire=false;

    public FireRequest(string playerName="", bool isFire=false)
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.Fire;
        this.playerName = playerName;
        this.isFire = isFire;
    }

    public override MainPack GetPack()
    {
        MainPack pack = new MainPack();
        pack.RequestCode = requestCode;
        pack.ActionCode = actionCode;

        PlayerPack playerPack = new PlayerPack();
        playerPack.PlayerName = playerName;

        InputPack inputPack = new InputPack();
        inputPack.IsFire = isFire;

        playerPack.InputPack = inputPack;
        pack.PlayerDic.Add(playerPack.PlayerName, playerPack);

        //设置房间名
        RoomPack roomPack = new RoomPack();
        roomPack.RoomName = GameMgr.Instance.roomName;
        pack.RoomPackList.Add(roomPack);

        return pack;
    }

    public override void OnResponse(MainPack pack)
    {
        base.OnResponse(pack);

        switch (pack.ReturnCode)
        {
            case ReturnCode.Succeed:

                break;
            case ReturnCode.Fail:
                Debug.Log("开火同步失败");
                break;
        }
    }
}
