using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class UIMgr : BaseMgr<UIMgr>
{
    //面板的父对象
    private GameObject panelPos;

    //面板的资源加载路径
    private string panelPath = "";

    //面板字典
    private Dictionary<string, IPanel> panelsDic = new Dictionary<string, IPanel>();

    //层级拓展
    //最底层
    private GameObject bot;
    //下层
    private GameObject down;
    //中间层
    private GameObject middle;
    //上层
    private GameObject up;
    //顶层
    private GameObject top;

    //检测是否有摄像机，和发现层级
    private void CheakPanelPos(EPanelLevel panelLevel)
    {
        if (panelPos == null)
        {
            panelPos = GameObject.FindGameObjectWithTag("UICavas");
        }

        if (bot == null)
        {
            bot = GameObject.Find("bot");
            if (bot == null)
            {
                bot = new GameObject("bot");
            }
            bot.transform.SetParent(panelPos.transform, false);
            bot.transform.localPosition = Vector3.zero;
            SetFullOverlay(bot);
        }

        if (down == null)
        {
            down = GameObject.Find("down");
            if (down == null)
            {
                down = new GameObject("down");
            }
            down.transform.SetParent(panelPos.transform, false);
            down.transform.localPosition = Vector3.zero;
            SetFullOverlay(down);
        }


        if (middle == null)
        {
            middle = GameObject.Find("middle");
            if (middle == null)
            {
                middle = new GameObject("middle");
            }
            middle.transform.SetParent(panelPos.transform, false);
            middle.transform.localPosition = Vector3.zero;
            SetFullOverlay(middle);
        }

        if (up == null)
        {
            up = GameObject.Find("up");
            if (up == null)
            {
                up = new GameObject("up");
            }
            up.transform.SetParent(panelPos.transform, false);
            up.transform.localPosition = Vector3.zero;
            SetFullOverlay(up);
        }

        if (top == null)
        {
            top = GameObject.Find("top");
            if (top == null)
            {
                top = new GameObject("top");
            }
            top.transform.SetParent(panelPos.transform, false);
            top.transform.localPosition = Vector3.zero;
            SetFullOverlay(top);
        }

    }

    /// <summary>
    /// 设置UI对象的锚点和偏移，使其完全覆盖父级对象
    /// </summary>
    /// <param name="uiObject"></param>
    private void SetFullOverlay(GameObject uiObject)
    {
        RectTransform parentRectTransform = uiObject.GetComponent<RectTransform>();
        if (parentRectTransform == null)
        {
            parentRectTransform = uiObject.AddComponent<RectTransform>();
        }

        if (parentRectTransform != null)
        {
            // 设置锚点为左下角
            parentRectTransform.anchorMin = Vector2.zero;
            parentRectTransform.anchorMax = Vector2.one;

            // 设置偏移为零，表示UI对象的位置和尺寸与父级对象相同
            parentRectTransform.offsetMin = Vector2.zero;
            parentRectTransform.offsetMax = Vector2.zero;
        }
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <typeparam name="T">面板的类型</typeparam>
    /// <param name="isFade">是否显隐</param>
    /// <param name="fatherPos">是否有父对象</param>
    /// <param name="callBack">回调函数</param>
    public void ShowPanel<T>(bool isFade = false, EPanelLevel panelLevel = EPanelLevel.Middle, 
        GameObject fatherPos = null, UnityAction<T> callBack = null) where T : class,IPanel
    {
        string panelName = typeof(T).Name;
        if (fatherPos == null)
        {
            CheakPanelPos(panelLevel);
            //设置父对象
            switch (panelLevel)
            {
                case EPanelLevel.Bot:
                    fatherPos = bot;
                    break;
                case EPanelLevel.Down:
                    fatherPos = down;
                    break;
                case EPanelLevel.Middle:
                    fatherPos = middle;
                    break;
                case EPanelLevel.Up:
                    fatherPos = up;
                    break;
                case EPanelLevel.Top:
                    fatherPos = top;
                    break;
                default:
                    fatherPos = panelPos;
                    break;
            }

            if (fatherPos == null)
            {
                Debug.Log(panelName + "的父对象不存在");
                return;
            }
        }

        //加载面板
        PoolMgr.Instance.Pop<GameObject>(panelPath + panelName, (panelObj) =>
        {
            //设置父对象
            panelObj.transform.SetParent(fatherPos.transform, false);

            T panel = panelObj.GetComponent<T>();

            //是否显影
            if (isFade)
            {
                panel.ShowPanel(callBack);
            }
            else
            {
                callBack?.Invoke(panel);
            }

            //面板加入字典
            panelsDic.Add(panelName, panel);
        });
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <typeparam name="T">面板的类型</typeparam>
    /// <param name="isFade">是否显影</param>
    /// <param name="callBack">回调函数</param>
    public void HidePanel<T>(bool isFade = false, UnityAction<T> callBack = null) where T : class,IPanel
    {
        string panelName = typeof(T).Name;

        //是否显示过该面板
        if (panelsDic.ContainsKey(panelName))
        {
            if (isFade)
            {
                panelsDic[panelName].HidePane<T>((panel) =>
                {
                    callBack?.Invoke(panelsDic[panelName] as T);

                    PoolMgr.Instance.Push(panelPath + panelName, panel.obj);
                });
            }
            else
            {
                callBack?.Invoke(panelsDic[panelName] as T);

                //Debug.Log(panelsDic[panelName].obj);

                PoolMgr.Instance.Push(panelPath + panelName, panelsDic[panelName].obj);
            }
            panelsDic.Remove(panelName);


        }
    }

    /// <summary>
    /// 获取面板
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    /// <returns></returns>
    public T GetPanel<T>() where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelsDic.ContainsKey(panelName))
        {
            return panelsDic[panelName] as T;
        }
        Debug.Log(panelName + "没有在panel字典中");
        return null;
    }

    /// <summary>
    /// 清空面板容器
    /// </summary>
    public void ClearDic()
    {
        panelsDic.Clear();
    }
}
