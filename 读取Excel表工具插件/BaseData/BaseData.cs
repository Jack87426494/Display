using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏基础属性
/// </summary>
[System.Serializable]
public class BaseData
{
    //目前游戏中使用的英雄的名字
    public string nowHeroName;
    //目前使用的是已经创建的游戏角色中的哪个角色
    public int heroIndex;
    //目前存在的角色数据
    public Dictionary<int, RoleInfo> existRoleInfoDic;
}
