using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BasePanel : MonoBehaviour,IPanel
{
    /// <summary>
    /// 控制显隐
    /// </summary>
    protected CanvasGroup fadeCanvasGroup;

    /// <summary>
    /// 显隐时间
    /// </summary>
    public float fadeTime=0.5f;

    /// <summary>
    /// 目前显影的时间
    /// </summary>
    protected float nowFadeTime = 0f;

    /// <summary>
    /// 是否打开面板
    /// </summary>
    private bool isOpen;

    /// <summary>
    /// ui控件的容器，string为控件在场景上面的名字。
    /// </summary>
    protected Dictionary<string, List<UIBehaviour>> uiDic = new Dictionary<string, List<UIBehaviour>>();

    public GameObject obj => gameObject;

    private void Awake()
    {
        fadeCanvasGroup = GetComponent<CanvasGroup>();
        if(fadeCanvasGroup == null)
        {
            fadeCanvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        //获取面板下的控件
        FindUIControl<Button>();
        FindUIControl<Slider>();
        FindUIControl<Toggle>();
        FindUIControl<Text>();
        FindUIControl<TextMeshProUGUI>();
        FindUIControl<Image>();
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <param name="callBack">回调函数</param>
    public virtual void ShowPanel<T>(UnityAction<T> callBack=null) where T:class,IPanel
    {
        SetFadeInOrOut(true,callBack);
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <param name="callBack">回调函数</param>
    public virtual void HidePane<T>(UnityAction<T> callBack =null) where T : class,IPanel
    {
        SetFadeInOrOut(false,callBack);
    }

    /// <summary>
    /// 统一按钮（button）点击按钮事件
    /// </summary>
    protected virtual void onClick()
    {

    }

    /// <summary>
    /// 单选框、多选框（Toggle)统一事件
    /// </summary>
    protected virtual void onValueChanged(string name,bool value)
    {

    }

    /// <summary>
    /// Slider统一事件
    /// </summary>
    protected virtual void onValueChanged(string name, float value)
    {

    }

    /// <summary>
    /// 找到面板下的ui控件，并将其加入容器
    /// </summary>
    /// <typeparam name="T">控件的类型</typeparam>
    private void FindUIControl<T>() where T:UIBehaviour
    {
        T[] uiControls=GetComponentsInChildren<T>();
        string uiName="";
        foreach (T uiControl in uiControls)
        {
            uiName = uiControl.gameObject.name;
            //Debug.Log(uiName);
            if(uiDic.ContainsKey(uiName))
            {
                uiDic[uiName].Add(uiControl);
            }
            else
            {
                uiDic.Add(uiName, new List<UIBehaviour>());
                uiDic[uiName].Add(uiControl);
            }

            //注册对应统一事件
            if (uiControl is Button)
            {
                (uiControl as Button).onClick.AddListener(onClick);
            }
            if(uiControl is Toggle)
            {
                (uiControl as Toggle).onValueChanged.AddListener((value) =>
                {
                    onValueChanged(uiName, value);
                });
            }
            if(uiControl is Slider)
            {
                (uiControl as Slider).onValueChanged.AddListener((value) =>
                {
                    onValueChanged(uiName, value);
                });
            }
        }
    }

    /// <summary>
    /// 获取面板下的ui控件
    /// </summary>
    /// <typeparam name="T">ui控件的类型</typeparam>
    /// <param name="name">ui控件的名字</param>
    /// <returns></returns>
    protected T GetUiControl<T>(string name) where T:UIBehaviour
    {
        if(uiDic.ContainsKey(name))
        {
            foreach(UIBehaviour uiControl in uiDic[name])
            {
                if(uiControl is T)
                {
                    //Debug.Log(name +"_"+ uiControl.gameObject.name);
                    return uiControl as T; 
                }
            }
        }
        Debug.Log("未找到控件：" + name);
        return null;
    }

    /// <summary>
    /// 设置淡入淡出
    /// </summary>
    /// <param name="isFadeInOrOut"></param>
    /// <param name="callBack"></param>
    protected virtual void SetFadeInOrOut<T>(bool isFadeInOrOut, UnityAction<T> callBack=null) where T:class,IPanel
    {
        fadeCanvasGroup.alpha = isFadeInOrOut ? 0f : 1f;
        if(isFadeInOrOut)
        {
            StartCoroutine(IFadeIn(callBack));
        }
        else
        {
            StartCoroutine(IFadeOut(callBack));
        }
    }


    /// <summary>
    /// 淡入
    /// </summary>
    /// <param name="callBack">回调函数</param>
    /// <returns></returns>
    protected IEnumerator IFadeIn<T>(UnityAction<T> callBack = null) where T:class,IPanel
    {
        nowFadeTime = 0f;
        while(nowFadeTime<fadeTime)
        {
            yield return new WaitForEndOfFrame();
            fadeCanvasGroup.alpha = Mathf.Lerp(0, 1f, nowFadeTime / fadeTime);
            nowFadeTime += Time.deltaTime;
        }
        fadeCanvasGroup.alpha = 1f;
        callBack?.Invoke(this as T);
    }

    /// <summary>
    /// 淡出
    /// </summary>
    /// <param name="callBack">回调函数</param>
    /// <returns></returns>
    protected IEnumerator IFadeOut<T>(UnityAction<T> callBack = null) where T:class,IPanel
    {
        nowFadeTime = 1f;
        while(nowFadeTime< fadeTime)
        {
            yield return new WaitForEndOfFrame();
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0, nowFadeTime / fadeTime);
            nowFadeTime += Time.deltaTime;
        }
        fadeCanvasGroup.alpha = 0f;
        callBack?.Invoke(this as T);
    }
}
