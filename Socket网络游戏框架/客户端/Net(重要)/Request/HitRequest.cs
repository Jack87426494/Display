using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitRequest : BaseRequest
{
    private string playerName;
    public float hitDamage=5f;

    public HitRequest(string playerName = "", float hitDamage=5)
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.Hit;
        this.playerName = playerName;
        this.hitDamage = hitDamage;
    }

    public override MainPack GetPack()
    {
        MainPack pack = new MainPack();
        pack.RequestCode = requestCode;
        pack.ActionCode = actionCode;

        PlayerPack playerPack = new PlayerPack();
        playerPack.PlayerName = playerName;

        HitPack hitPack = new HitPack();
        hitPack.Damage = hitDamage;
        playerPack.HitPack = hitPack;

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
                Debug.Log("受伤同步成功");
                break;
            case ReturnCode.Fail:
                Debug.Log("受伤同步失败");
                break;
        }
    }
}
