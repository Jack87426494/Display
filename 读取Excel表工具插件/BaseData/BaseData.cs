using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ϸ��������
/// </summary>
[System.Serializable]
public class BaseData
{
    //Ŀǰ��Ϸ��ʹ�õ�Ӣ�۵�����
    public string nowHeroName;
    //Ŀǰʹ�õ����Ѿ���������Ϸ��ɫ�е��ĸ���ɫ
    public int heroIndex;
    //Ŀǰ���ڵĽ�ɫ����
    public Dictionary<int, RoleInfo> existRoleInfoDic;
}
