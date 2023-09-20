using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class NetPlayerController : MonoBehaviour, INetPlayer
{
    [Header("人物移动速度")]
    public float moveSpeed = 3f;

    [Header("人物旋转速度")]
    public float rotationSpeed = 2.0f;

    private Animator animator;

    private void Awake()
    {
        
    }

    private void Start()
    {
        animator = GetComponent<Animator>();

    }

    //获取键盘ad
    private float inputX;
    //获取键盘ws
    private float inputY;
    // 获取鼠标水平移动
    private float mouseX;
    //是否奔跑
    private bool isRun;

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (inputX != 0 || inputY != 0)
        {
            if (isRun == false)
            {
                animator.SetBool("isRun", true);
                isRun = true;
            }
        }
        else if (isRun == true)
        {
            animator.SetBool("isRun", false);
            isRun = false;
        }

        if (isRun == true)
        {
            animator.SetFloat("inputX", inputX);
            animator.SetFloat("inputY", inputY);
            transform.Translate((Vector3.forward * inputY + Vector3.right * inputX) * moveSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 处理远端键盘输入
    /// </summary>
    /// <param name="inputPack"></param>
    public void HandleInput(InputPack inputPack)
    {
        inputX = inputPack.InputX;
        inputY = inputPack.InputY;
        mouseX = inputPack.MouseX;


        // 在Y轴上旋转
        transform.Rotate(Vector3.up * mouseX * rotationSpeed);
       
    }
}
