using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BasePlayerController
{
    private InputRequest inputRequest;

    private FireRequest fireRequest;

    private HitRequest hitRequest;

    private PlayerCamera playerCamera;


    private void Awake()
    {
        inputRequest = new InputRequest(gameObject.name);
        fireRequest = new FireRequest(gameObject.name);
        hitRequest = new HitRequest(gameObject.name);
    }

    protected override void Start()
    {
        base.Start();
        playerCamera = Camera.main.GetComponent<PlayerCamera>();
       
    }

    private void Update()
    {
        SendFire();
        SendInput();
    }
    private void LateUpdate()
    {
        Shoot();
        Move();
    }

    /// <summary>
    /// 发送玩家的输入
    /// </summary>
    private void SendInput()
    {
        inputRequest.inputX = Input.GetAxis("Horizontal");
        inputRequest.inputY = Input.GetAxis("Vertical");
        inputRequest.mouseX = Input.GetAxis("Mouse X");

        if (inputRequest.inputX!=inputX||
            inputRequest.inputY!=inputY||
            inputRequest.mouseX!=mouseX)
        {
             ClientMgr.Instance.SendRequest(inputRequest);
        }
    }

    private void SendFire()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //服务端同步
            fireRequest.isFire = true;
            if(fireRequest.isFire!=isFire)
            {
                ClientMgr.Instance.SendRequest(fireRequest);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            //服务端同步
            fireRequest.isFire = false;
            if (fireRequest.isFire != isFire)
            {
                ClientMgr.Instance.SendRequest(fireRequest);
            }
        }
    }


    private void SendHit(float hitDamage)
    {
        if(hitDamage>0)
        {
            hitRequest.hitDamage = hitDamage;
            Debug.Log(gameObject.name + "受到"+hitDamage+ "点伤害");
            ClientMgr.Instance.SendRequest(hitRequest);
        }
    }

    Bullet bullet;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("bullet"))
        {
            bullet = other.gameObject.GetComponent<Bullet>();
            SendHit(bullet.damage);
            bullet.DestroyBullet();
        }
    }

    protected override void Move()
    {
        base.Move();
        playerCamera.MoveCamera();
    }

    public override void HandleInput(InputPack inputPack)
    {
        base.HandleInput(inputPack);
        playerCamera.RotateCamera();
    }

    
}
