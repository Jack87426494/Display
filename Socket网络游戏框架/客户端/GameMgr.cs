using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class GameMgr : BaseMgr<GameMgr>
{
    //本机玩家的名字
    public string localName;

    //玩家所处房间
    public string roomName;

    //联机所有的玩家
    private Dictionary<string, INetPlayer> playerDic = new Dictionary<string, INetPlayer>();

    //玩家生成的位置
    private GameObject playersFather;


    /// <summary>
    /// 生成角色
    /// </summary>
    /// <param name="pack"></param>
    public void EnterGame(MainPack pack)
    {
        //根据包内容生成角色
        if(playersFather==null)
        {
            playersFather = new GameObject("playersFather");
            
        }

        int j = 0;
        Vector3 playerPos=new Vector3();
        foreach (PlayerPack playerPack in pack.PlayerDic.Values)
        {
            if (!playerDic.ContainsKey(playerPack.PlayerName))
            {
                //创建角色
                PoolMgr.Instance.Pop<GameObject>("PlayerMan", (playerObj) =>
                {
                    //设置位置
                    playerPos.x = pack.OriTransformPack[j].X;
                    playerPos.y = pack.OriTransformPack[j].Y;
                    playerPos.z = pack.OriTransformPack[j].Z;

                    playerObj.transform.position = playerPos;
                    playerObj.transform.SetParent(playersFather.transform, false);

                    //设置角色的旋转量
                    playerObj.transform.rotation = Quaternion.Euler(0, pack.OriTransformPack[j].RotateAngleY, 0);

                    //设置名字
                    playerObj.name = playerPack.PlayerName;

                    //添加脚本
                    if (localName == playerPack.PlayerName)
                    {
                        playerDic.Add(playerPack.PlayerName, playerObj.AddComponent<PlayerController>());
                    }
                    else
                    {
                        playerDic.Add(playerPack.PlayerName, playerObj.AddComponent<NetPlayerController>());
                    }
                    
                });
                ++j;
            }
        }
        //加载摄像机
        ResMgr.Instance.Load<GameObject>("PlayerCamera");

        //绑定网路事件
        ClientMgr.Instance.BindResponse(ActionCode.PlayerInput, HandleInput);
        ClientMgr.Instance.BindResponse(ActionCode.Fire, HandFire);
        ClientMgr.Instance.BindResponse(ActionCode.Hit, HandleHit);
    }

    /// <summary>
    /// 处理输入
    /// </summary>
    /// <param name="pack"></param>
    public void HandleInput(MainPack pack)
    {
        foreach(PlayerPack playerPack in pack.PlayerDic.Values)
        {
            if(playerDic.ContainsKey(playerPack.PlayerName))
            {
                playerDic[playerPack.PlayerName].HandleInput(playerPack.InputPack);
            }
        }
    }

    /// <summary>
    /// 处理开火
    /// </summary>
    /// <param name="pack"></param>
    public void HandFire(MainPack pack)
    {
        foreach (PlayerPack playerPack in pack.PlayerDic.Values)
        {
            if (playerDic.ContainsKey(playerPack.PlayerName))
            {
                playerDic[playerPack.PlayerName].HandleFire(playerPack.InputPack);
            }
        }
    }

    /// <summary>
    /// 处理受伤
    /// </summary>
    /// <param name="pack"></param>
    public void HandleHit(MainPack pack)
    {
        foreach (PlayerPack playerPack in pack.PlayerDic.Values)
        {
            if (playerDic.ContainsKey(playerPack.PlayerName))
            {
                playerDic[playerPack.PlayerName].HandleHit(playerPack.HitPack);
            }
        }
    }

}
