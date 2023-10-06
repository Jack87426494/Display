using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INetPlayer : INetObj
{
    /// <summary>
    /// 处理输入
    /// </summary>
    /// <param name="inputPack"></param>
    public void HandleInput(InputPack inputPack);

    /// <summary>
    /// 处理开火
    /// </summary>
    /// <param name="inputPack"></param>
    public void HandleFire(InputPack inputPack);

    /// <summary>
    /// 处理受伤
    /// </summary>
    /// <param name="hitPack"></param>
    public void HandleHit(HitPack hitPack);
}
