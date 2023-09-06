using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseUIControl : MonoBehaviour
{

    /// <summary>
    /// ui控件的容器，string为控件在场景上面的名字。
    /// </summary>
    protected Dictionary<string, List<UIBehaviour>> uiDic = new Dictionary<string, List<UIBehaviour>>();

    private void Awake()
    {
        //获取面板下的控件
        FindUIControl<Button>();
        FindUIControl<Slider>();
        FindUIControl<Toggle>();
        FindUIControl<Text>();
        FindUIControl<TextMeshProUGUI>();
        FindUIControl<Image>();
        FindUIControl<InputField>();
        FindUIControl<ScrollRect>();
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
    protected virtual void onValueChanged(string name, bool value)
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
    private void FindUIControl<T>() where T : UIBehaviour
    {
        T[] uiControls = GetComponentsInChildren<T>();
        string uiName = "";
        foreach (T uiControl in uiControls)
        {
            uiName = uiControl.gameObject.name;
            //Debug.Log(uiName);
            if (uiDic.ContainsKey(uiName))
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
            if (uiControl is Toggle)
            {
                (uiControl as Toggle).onValueChanged.AddListener((value) =>
                {
                    onValueChanged(uiName, value);
                });
            }
            if (uiControl is Slider)
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
    protected T GetUiControl<T>(string name) where T : UIBehaviour
    {
        if (uiDic.ContainsKey(name))
        {
            foreach (UIBehaviour uiControl in uiDic[name])
            {
                if (uiControl is T)
                {
                    //Debug.Log(name +"_"+ uiControl.gameObject.name);
                    return uiControl as T;
                }
            }
        }
        Debug.Log("未找到控件：" + name);
        return null;
    }
}
