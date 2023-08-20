using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 一个池子
/// </summary>
public class PoolData
{
    //池子的父节点
    private GameObject father;
    //对象池
    private Queue<GameObject> pool=new Queue<GameObject>();

    /// <summary>
    /// 初始化对象池
    /// </summary>
    /// <param name="obj"></param>
    public PoolData(GameObject obj, Transform poolsTra)
    {
        if (father == null)
        {
            father = new GameObject(obj.name + "Pool");
            father.transform.SetParent(poolsTra);
        }
        Push(obj);
    }

    /// <summary>
    /// 从对象池中取东西
    /// </summary>
    /// <param name="isCanselFather">是否取消父对象</param>
    /// <returns>池子是否为空</returns>
    public bool Pop<T>(UnityAction<T> callBack = null, bool isCanselFather = false) where T : UnityEngine.Object
    {
        //弹出池子
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            if (isCanselFather)
            {
                //取消父对象
                obj.transform.SetParent(null);
            }
            callBack.Invoke(obj as T);
        }
        //如果池子为空
        if (pool.Count <= 0)
        {
            //删除父节点
            GameObject.Destroy(father);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 从对象池中放东西
    /// </summary>
    /// <param name="obj">要放入池子中的物体</param>
    public void Push(GameObject obj)
    {
        obj.SetActive(false);
        //设置父对象
        obj.transform.SetParent(father.transform);
        pool.Enqueue(obj);
    }
}


/// <summary>
/// 对象池管理器
/// </summary>
public class PoolMgr : BaseMgr<PoolMgr>
{
    //所有池子的父节点
    private GameObject pools;

    //对象池的字典
    Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();

    public PoolMgr()
    {
        if(pools==null)
        {
            pools = new GameObject("Pools");
            MonoBehaviour.DontDestroyOnLoad(pools);
        }
    }

    /// <summary>
    /// 往池子里面放东西
    /// </summary>
    /// <param name="name">东西的名字</param>
    /// <param name="obj">实际的要放的物体</param>
    public void Push(string name,GameObject obj)
    {
        if (poolDic.ContainsKey(name))
        {
            poolDic[name].Push(obj);
        }
        else
        {
            poolDic.Add(name,new PoolData(obj, pools.transform));
        }
    }

    /// <summary>
    /// 往池子里面取东西
    /// </summary>
    /// <param name="name">东西的名字</param>
    /// <param name="callBack">回调函数</param>
    /// <param name="isCanselFather">是否脱离父对象</param>
    public void Pop<T>(string name,UnityAction<T> callBack=null,bool isCanselFather = false) where T:UnityEngine.Object
    {
        if(poolDic.ContainsKey(name))
        {
            //如果池子为空，将该池子从对象池字典中删除
            if(poolDic[name].Pop(callBack,isCanselFather))
            {
                poolDic.Remove(name);
            }
        }
        else
        {
            ResMgr.Instance.Load<GameObject>(name, (obj) =>
            {
                callBack?.Invoke(obj as T);
            });
        }
    }

    /// <summary>
    /// 清空对象池
    /// </summary>
    public void Clear()
    {
        poolDic.Clear();
        MonoBehaviour.Destroy(pools.gameObject);
        pools = new GameObject("Pools");
        MonoBehaviour.DontDestroyOnLoad(pools);
    }
}

