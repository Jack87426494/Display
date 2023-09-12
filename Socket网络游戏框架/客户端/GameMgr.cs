using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class GameMgr : BaseMgr<GameMgr>
{
    //本机玩家的名字
    public string localName;

    //联机所有的玩家
    private Dictionary<string, INetPlayer> playerDic = new Dictionary<string, INetPlayer>();

    //玩家生成的位置
    private GameObject playersFather;

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
        {new Vector3(-20,0,10) }
    };


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

        //洗牌（打乱角色初始化的位置顺序）
        Vector3 vector3;
        int j;
        for (int i = playerPos.Count - 1; i > 0; --i)
        {
            vector3 = playerPos[i];
            j = Random.Range(0, i);
            playerPos[i] = playerPos[j];
            playerPos[j] = vector3;
        }
        j = 0;
        foreach (PlayerPack playerPack in pack.PlayerDic.Values)
        {
            if (!playerDic.ContainsKey(playerPack.PlayerName))
            {
                //创建角色
                PoolMgr.Instance.Pop<GameObject>("PlayerMan", (playerObj) =>
                {
                    //设置位置
                    playerObj.transform.position = playerPos[j];
                    playerObj.transform.SetParent(playersFather.transform, false);

                    //随机设置角色的旋转量
                    playerObj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

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
                
            }
            ++j;
        }
        //加载摄像机
        ResMgr.Instance.Load<GameObject>("PlayerCamera");

        ClientMgr.Instance.BindResponse(ActionCode.PlayerInput, HandleInput);
    }

    /// <summary>
    /// 处理输入
    /// </summary>
    /// <param name="pack"></param>
    private void HandleInput(MainPack pack)
    {
        foreach(PlayerPack playerPack in pack.PlayerDic.Values)
        {
            if(playerDic.ContainsKey(playerPack.PlayerName))
            {
                playerDic[playerPack.PlayerName].HandleInput(playerPack.InputPack);
            }
        }
    }
}
