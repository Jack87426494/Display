using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoContraller : MonoBehaviour
{
    /// <summary>
    /// awake执行事件
    /// </summary>
    public List<UnityAction> awakeActionList=new List<UnityAction>();

    /// <summary>
    /// start执行事件
    /// </summary>
    public List<UnityAction> startActionList=new List<UnityAction>();

    /// <summary>
    /// update执行事件
    /// </summary>
    public List<UnityAction> updateActionList=new List<UnityAction>();


    private void Awake()
    {
        if(awakeActionList.Count<=0)
        {
            return;
        }
        foreach(UnityAction awakeAction in awakeActionList)
        {
            awakeAction();
        }
    }

    private void Start()
    {
        if(startActionList.Count<=0)
        {
            return;
        }
        foreach(UnityAction startAction in startActionList)
        {
            startAction();
        }
    }

    private void Update()
    {
        if(updateActionList.Count<=0)
        {
            return;
        }
        foreach(UnityAction updateAction in updateActionList)
        {
            updateAction();
        }
    }

}
