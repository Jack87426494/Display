using SocketGameProtocol;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayerController : MonoBehaviour,INetPlayer
{
    [Header("人物移动速度")]
    public float moveSpeed = 3f;

    [Header("人物旋转速度")]
    public float rotationSpeed = 2f;
    
    //玩家数据
    protected PlayerData playerData=new PlayerData();

    ////目标旋转值
    //protected Quaternion targetRotation; 

    protected Animator animator;

    //获取键盘ad
    public float inputX;
    //获取键盘ws
    public float inputY;
    // 获取鼠标水平移动
    public float mouseX;
    //是否奔跑
    public bool isRun;
   
    //是否正在开火
    protected bool isFire;

    //武器
    protected GameObject weaponObj;
    //武器脚本
    protected BaseWeapon baseWeapon;


    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        //targetRotation = transform.rotation;
        baseWeapon=DFSFindChild(transform, "hand_r").GetComponentInChildren<BaseWeapon>();
        weaponObj = baseWeapon.gameObject;
       
    }

    /// <summary>
    /// 开火
    /// </summary>
    protected virtual void  Shoot()
    {
        baseWeapon.Fire(isFire, animator);
    }

    protected virtual void Move()
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
    public virtual void HandleInput(InputPack inputPack)
    {
        inputX = inputPack.InputX;
        inputY = inputPack.InputY;
        mouseX = inputPack.MouseX;

        //isFire = inputPack.IsFire;

        //// 根据鼠标输入调整目标旋转值
        //transform.rotation = Quaternion.Euler(Vector3.up * mouseX* moveSpeed);

        if (mouseX != 0)
        {
            transform.Rotate(Vector3.up * mouseX * rotationSpeed);
        }
    }

    /// <summary>
    /// 处理开火
    /// </summary>
    /// <param name="inputPack"></param>
    public virtual void HandleFire(InputPack inputPack)
    {
        isFire = inputPack.IsFire;
    }

    /// <summary>
    /// 处理受伤
    /// </summary>
    /// <param name="hitPack"></param>
    public virtual void HandleHit(HitPack hitPack)
    {
        if(playerData.hp<=0)
        {
            return;
        }

        playerData.hp -= hitPack.Damage;
        Debug.Log(gameObject.name + "的血量为：" + playerData.hp);
        //同步显示血量的面板

        //同步死亡的面板

        //死亡动画
        if(playerData.hp<=0)
        {
            animator.SetTrigger("die");
            return;
        }

        //受伤动画
        animator.SetInteger("hitInt", Random.Range(0, 2));
        animator.SetTrigger("hit");
    }

    /// <summary>
    /// // 深度优先搜索子物体
    /// </summary>
    /// <param name="current">父物体</param>
    /// <param name="name">要找物体的名字</param>
    /// <returns></returns>
    protected Transform DFSFindChild(Transform current, string name)
    {
        if (current.gameObject.name == name)
        {
            // 如果找到目标子物体，将其赋值给目标 Transform
            return current;
        }

        foreach (Transform child in current)
        {
            // 递归调用 DFS
            Transform result = DFSFindChild(child, name);

            // 如果在当前子物体的子物体中找到目标，返回结果
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    /// <summary>
    /// 广度优先搜索找到子物体
    /// </summary>
    /// <param name="root">父物体</param>
    /// <param name="name">要找物体的名字</param>
    /// <returns></returns>
    protected Transform BFSFindChild(Transform root, string name)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            Transform current = queue.Dequeue();

            if (current.gameObject.name == name)
            {
                // 如果找到目标子物体，返回它
                return current;
            }

            // 将当前子物体的子物体加入队列
            foreach (Transform child in current)
            {
                queue.Enqueue(child);
            }
        }

        // 未找到目标子物体
        return null;
    }

    
}
