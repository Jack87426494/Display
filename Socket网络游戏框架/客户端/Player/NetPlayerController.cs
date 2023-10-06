using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class NetPlayerController : BasePlayerController
{

    private void Awake()
    {
        
    }

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        Shoot();
        Move();
    }
}
