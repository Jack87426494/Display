﻿using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputRequest : BaseRequest
{
    private string playerName;
    public float inputX;
    public float inputY;
    public float mouseX;
    public float rotateAngleY;
    public bool isFire;

    public InputRequest()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.PlayerInput;
    }

    public InputRequest(string playerName = "", float inputX = 0, float inputY = 0, float mouseX = 0,
        float rotateAngleY=0)
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.PlayerInput;
        this.playerName = playerName;
        this.inputX = inputX;
        this.inputY = inputY;
        this.mouseX = mouseX;
        this.rotateAngleY = rotateAngleY;
    }

    public override MainPack GetPack()
    {
        MainPack pack = new MainPack();
        pack.RequestCode = requestCode;
        pack.ActionCode = actionCode;

        PlayerPack playerPack = new PlayerPack();
        playerPack.PlayerName = playerName;

        InputPack inputPack = new InputPack();
        inputPack.InputX = inputX;
        inputPack.InputY = inputY;
        inputPack.MouseX = mouseX;
        inputPack.RotateAngleY = rotateAngleY;

        playerPack.InputPack = inputPack;
        pack.PlayerDic.Add(playerPack.PlayerName,playerPack);

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
                //Debug.Log("input同步成功");
                
                break;
            case ReturnCode.Fail:
                Debug.Log("input同步失败");
                break;
        }
    }
       
}
