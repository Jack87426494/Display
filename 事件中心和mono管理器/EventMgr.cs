using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IEvent
{

}

public class EventAction:IEvent
{
    public UnityAction unityAction;

    public EventAction(UnityAction action)
    {
        unityAction += action;
    }
}

public class EventActionOne<T>:IEvent
{
    public UnityAction<T> unityAction;

    public EventActionOne(UnityAction<T> action)
    {
        unityAction += action;
    }
}

public class EventMgr : BaseMgr<EventMgr>
{
    public Dictionary<string, IEvent> eventDic = new Dictionary<string, IEvent>();

    /// <summary>
    /// 添加一个事件
    /// </summary>
    /// <param name="eventName">事件的键</param>
    /// <param name="action">事件本身</param>
    public void AddEvent(string eventName,UnityAction action)
    {
        if(eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventAction).unityAction += action;
        }
        else
        {
            eventDic.Add(eventName, new EventAction(action));
        }
    }

    /// <summary>
    /// 添加一个事件
    /// </summary>
    /// <param name="eventName">事件的键</param>
    /// <param name="action">事件本身</param>
    public void AddEvent<T>(string eventName,UnityAction<T> action)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventActionOne<T>).unityAction += action;
        }
        else
        {
            eventDic.Add(eventName, new EventActionOne<T>(action));
        }
    }

    /// <summary>
    /// 占据一个事件
    /// </summary>
    /// <param name="eventName">事件的名字</param>
    /// <param name="action">事件本身</param>
    public void OccupyEvent(string eventName, UnityAction action)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventAction).unityAction = action;
        }
        else
        {
            eventDic.Add(eventName, new EventAction(action));
        }
    }

    /// <summary>
    /// 占据一个事件
    /// </summary>
    /// <param name="eventName">事件的名字</param>
    /// <param name="action">事件本身</param>
    public void OccupyEvent<T>(string eventName, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventActionOne<T>).unityAction = action;
        }
        else
        {
            eventDic.Add(eventName, new EventActionOne<T>(action));
        }
    }

    /// <summary>
    /// 移除一个字符对应的所有事件
    /// </summary>
    /// <param name="eventName"></param>
    public void RemoveEvent(string eventName)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventAction).unityAction =null ;
        }
    }

    /// <summary>
    /// 移除一个字符对应的所有事件
    /// </summary>
    /// <param name="eventName"></param>
    public void RemoveEvent<T>(string eventName)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventActionOne<T>).unityAction = null;
        }
    }

    /// <summary>
    /// 执行事件
    /// </summary>
    /// <param name="eventName">事件的名字</param>
    public void EventTrigger(string eventName)
    {
        if (eventDic.ContainsKey(eventName))
        {
            if((eventDic[eventName] as EventAction).unityAction!=null)
            {
                (eventDic[eventName] as EventAction).unityAction?.Invoke();
            }
        }
    }

    /// <summary>
    /// 执行事件
    /// </summary>
    /// <param name="eventName">事件的名字</param>
    public void EventTrigger<T>(string eventName,T t)
    {
        if (eventDic.ContainsKey(eventName))
        {
            if ((eventDic[eventName] as EventActionOne<T>).unityAction != null)
            {
                (eventDic[eventName] as EventActionOne<T>).unityAction?.Invoke(t);
            }
        }
    }
}
