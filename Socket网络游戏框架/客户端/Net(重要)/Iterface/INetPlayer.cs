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
}
