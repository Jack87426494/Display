using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginGameRequest : BaseRequest
{
    string roomName;
    //用户的名字
    private string userName;

    //玩家的位置
    private List<Vector3> playerPos = new List<Vector3>() {
        {new Vector3(0,0,0)},
        {new Vector3(-10,0,20)},
        {new Vector3(-20,0,20)},
        {new Vector3(-15,0,30)},
        {new Vector3(0,0,30)},
        {new Vector3(20,0,30)},
        {new Vector3(20,0,10)},
        {new Vector3(-15,0,25)},
        {new Vector3(-15,0,10)},
        {new Vector3(-20,0,10) },
        {new Vector3(-30,0,10) }
    };

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
        //打乱顺序
        shuffleCards();
        //传入初始化的位置，避免不同客户端的角色位置不同
        foreach(Vector3 vector3 in playerPos)
        {
            TransformPack transformPack=new TransformPack();
            transformPack.X = vector3.x;
            transformPack.Y = vector3.y;
            transformPack.Z = vector3.z;
            transformPack.RotateAngleY = Random.Range(0, 360);
            pack.OriTransformPack.Add(transformPack);
        }
        
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

    /// <summary>
    /// 洗牌算法（打乱角色初始化的位置顺序）
    /// </summary>
    private void shuffleCards()
    {
        int j;
        Vector3 vector3;
        for (int i = playerPos.Count - 1; i > 0; --i)
        {
            vector3 = playerPos[i];
            j = Random.Range(0, i);
            playerPos[i] = playerPos[j];
            playerPos[j] = vector3;
        }
    }
}
