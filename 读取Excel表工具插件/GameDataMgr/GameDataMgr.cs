using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataMgr:MonoBehaviour
{
    private static GameDataMgr instance;
    public static GameDataMgr Instance => instance;
    //音乐数据
    public MusicData musicData;
    ////目前存在的角色数据
    //public Dictionary<int,RoleInfo> existRoleInfoDic=new Dictionary<int, RoleInfo>();
    //目前玩家的角色数据
    public RoleInfo playerInfo;
    //游戏基础数据
    public BaseData baseData;
   

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            InitData();
        }
        else
            Destroy(this.gameObject);
    }


    //游戏打开的时候，初始化读取游戏数据
    private void InitData()
    {
        ////清空表中所有数据
        //ExcelMgr.Instance.ClearAll();

        ////重置基础数据
        //ReplaceBaseData();

        //加载防御塔数据(每次进入游戏加载一次)
        ExcelMgr.Instance.LoadExcelTable<TowerInfo, TowerInfoContainer>();

        //加载武器数据(每次进入游戏加载一次)
        ExcelMgr.Instance.LoadExcelTable<WeaponInfo, WeaponInfoContainer>();

        //加载僵尸数据(每次进入游戏加载一次)
        ExcelMgr.Instance.LoadExcelTable<ZombieInfo, ZombieInfoContainer>();

       

        //加载游戏场景数据(每次进入游戏加载一次)
        ExcelMgr.Instance.LoadExcelTable<SceneInfo, SceneInfoContainer>();

        musicData = JsonMgr.Instance.LoadData<MusicData>();
        //同步所有音乐数据在实际音乐中
        UpdataAllMusicDataReally();
        //读取游戏角色数据
        baseData = BinaryMgr.Instance.LoadData<BaseData>("",BinaryType.MemoryStream);
        //如果是第一次进入游戏需要重置数据
        if (baseData == null)
        {
            ReplaceBaseData();
        }
    }

    /// <summary>
    /// 得到角色字典中的一条角色数据
    /// </summary>
    /// <param name="index">序列下标</param>
    /// <returns>该角色数据</returns>
    public RoleInfo GetRoleInfo(int index)
    {
        return (ExcelMgr.Instance.GetExcelData<RoleInfoContainer>() as RoleInfoContainer).dic[index];
    }
    
    /// <summary>
    /// 添加目前使用过的角色数据,并且存储在磁盘中
    /// </summary>
    /// <param name="index">英雄的索引</param>
    public void SetExistRoleInfo(int index)
    {
        playerInfo= (ExcelMgr.Instance.GetExcelData<RoleInfoContainer>() as RoleInfoContainer).dic[index];
        
        if (!baseData.existRoleInfoDic.ContainsKey(index))
        {
            baseData.existRoleInfoDic.Add(index,playerInfo);
            BinaryMgr.Instance.SaveData<BaseData>(baseData);
        }
       
    }
    /// <summary>
    /// 得到目前正在使用角色数据
    /// </summary>
    /// <returns></returns>
    public RoleInfo GetExistRoleInfo()
    {
        return baseData.existRoleInfoDic[baseData.heroIndex];
    }

    /// <summary>
    /// 删除目前正在使用角色数据
    /// </summary>
    public void DeleteExistRoleInfo()
    {
        baseData.existRoleInfoDic.Remove(baseData.heroIndex);
    }

    /// <summary>
    /// 修改正在使用的角色序号,并且存储在磁盘中,并且修改保存角色数据,修改保存在磁盘上
    /// </summary>
    /// <param name="index">修改的正在使用的英雄序号</param>
    public void SetBaseData(int index)
    {
        baseData.heroIndex = index;
        BinaryMgr.Instance.SaveData<BaseData>(baseData);
        // 添加目前使用过的角色数据,并且存储在磁盘中
        SetExistRoleInfo(index);
    }

    /// <summary>
    /// 重置音乐数据
    /// </summary>
    public void ReplaceMusicData()
    {
        musicData = new MusicData
        {
            isOpenBk = true,
            isOpenSound = true,
            soundVoluem = 1f,
            bKVoluem = 1f
        };
    }

    /// <summary>
    /// 重置游戏基础数据
    /// </summary>
    public void ReplaceBaseData()
    {
        baseData = new BaseData();
        baseData.heroIndex = 0;
        baseData.nowHeroName = "engineer";
        baseData.existRoleInfoDic = new Dictionary<int, RoleInfo>();
        BinaryMgr.Instance.SaveData<BaseData>(baseData,"",BinaryType.MemoryStream);
    }

    /// <summary>
    /// 保存所有数据
    /// </summary>
    public void SaveAllData()
    {
        JsonMgr.Instance.SaveData<MusicData>(musicData);
        BinaryMgr.Instance.SaveData<BaseData>(baseData);
    }

    /// <summary>
    /// 同步背景音乐开关数据
    /// </summary>
    /// <param name="isOpen"></param>
    public void UpdateBkOpen(bool isOpen)
    {
        musicData.isOpenBk = isOpen;
        //同步实际背景音乐开关
        MusicMgr.Instance.SetBkOpen(isOpen);
       
    }

    /// <summary>
    /// 同步背景音乐大小数据
    /// </summary>
    /// <param name="voluem"></param>
    public void UpdateBkVolume(float voluem)
    {
        musicData.bKVoluem = voluem;
        //同步实际背景音乐的大小
        MusicMgr.Instance.SetBkVolume(voluem);
        
    }

    public void UpdateSoundOpen(bool isOpen)
    {
        musicData.isOpenSound = isOpen;
        //同步实际背景音效的开关
        MusicMgr.Instance.SetSoundOpen(isOpen);
    }

    public void UpdateSoundVoluem(float voluem)
    {
        musicData.soundVoluem = voluem;
        //同步实际背景音效的大小
        MusicMgr.Instance.SetSoundValuem(voluem);
    }

    //同步所有音乐数据在实际音乐中
    private void UpdataAllMusicDataReally()
    {
        //同步实际背景音乐的大小
        MusicMgr.Instance.SetBkVolume(musicData.bKVoluem);
        //同步实际背景音乐开关
        MusicMgr.Instance.SetBkOpen(musicData.isOpenBk);
        //同步实际背景音效开关
        MusicMgr.Instance.SetSoundOpen(musicData.isOpenSound);
        //同步实际背景音效大小
        MusicMgr.Instance.SetSoundValuem(musicData.soundVoluem);
    }
}
