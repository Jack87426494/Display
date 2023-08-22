using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataMgr:MonoBehaviour
{
    private static GameDataMgr instance;
    public static GameDataMgr Instance => instance;
    //��������
    public MusicData musicData;
    ////Ŀǰ���ڵĽ�ɫ����
    //public Dictionary<int,RoleInfo> existRoleInfoDic=new Dictionary<int, RoleInfo>();
    //Ŀǰ��ҵĽ�ɫ����
    public RoleInfo playerInfo;
    //��Ϸ��������
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


    //��Ϸ�򿪵�ʱ�򣬳�ʼ����ȡ��Ϸ����
    private void InitData()
    {
        ////��ձ�����������
        //ExcelMgr.Instance.ClearAll();

        ////���û�������
        //ReplaceBaseData();

        //���ط���������(ÿ�ν�����Ϸ����һ��)
        ExcelMgr.Instance.LoadExcelTable<TowerInfo, TowerInfoContainer>();

        //������������(ÿ�ν�����Ϸ����һ��)
        ExcelMgr.Instance.LoadExcelTable<WeaponInfo, WeaponInfoContainer>();

        //���ؽ�ʬ����(ÿ�ν�����Ϸ����һ��)
        ExcelMgr.Instance.LoadExcelTable<ZombieInfo, ZombieInfoContainer>();

       

        //������Ϸ��������(ÿ�ν�����Ϸ����һ��)
        ExcelMgr.Instance.LoadExcelTable<SceneInfo, SceneInfoContainer>();

        musicData = JsonMgr.Instance.LoadData<MusicData>();
        //ͬ����������������ʵ��������
        UpdataAllMusicDataReally();
        //��ȡ��Ϸ��ɫ����
        baseData = BinaryMgr.Instance.LoadData<BaseData>("",BinaryType.MemoryStream);
        //����ǵ�һ�ν�����Ϸ��Ҫ��������
        if (baseData == null)
        {
            ReplaceBaseData();
        }
    }

    /// <summary>
    /// �õ���ɫ�ֵ��е�һ����ɫ����
    /// </summary>
    /// <param name="index">�����±�</param>
    /// <returns>�ý�ɫ����</returns>
    public RoleInfo GetRoleInfo(int index)
    {
        return (ExcelMgr.Instance.GetExcelData<RoleInfoContainer>() as RoleInfoContainer).dic[index];
    }
    
    /// <summary>
    /// ���Ŀǰʹ�ù��Ľ�ɫ����,���Ҵ洢�ڴ�����
    /// </summary>
    /// <param name="index">Ӣ�۵�����</param>
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
    /// �õ�Ŀǰ����ʹ�ý�ɫ����
    /// </summary>
    /// <returns></returns>
    public RoleInfo GetExistRoleInfo()
    {
        return baseData.existRoleInfoDic[baseData.heroIndex];
    }

    /// <summary>
    /// ɾ��Ŀǰ����ʹ�ý�ɫ����
    /// </summary>
    public void DeleteExistRoleInfo()
    {
        baseData.existRoleInfoDic.Remove(baseData.heroIndex);
    }

    /// <summary>
    /// �޸�����ʹ�õĽ�ɫ���,���Ҵ洢�ڴ�����,�����޸ı����ɫ����,�޸ı����ڴ�����
    /// </summary>
    /// <param name="index">�޸ĵ�����ʹ�õ�Ӣ�����</param>
    public void SetBaseData(int index)
    {
        baseData.heroIndex = index;
        BinaryMgr.Instance.SaveData<BaseData>(baseData);
        // ���Ŀǰʹ�ù��Ľ�ɫ����,���Ҵ洢�ڴ�����
        SetExistRoleInfo(index);
    }

    /// <summary>
    /// ������������
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
    /// ������Ϸ��������
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
    /// ������������
    /// </summary>
    public void SaveAllData()
    {
        JsonMgr.Instance.SaveData<MusicData>(musicData);
        BinaryMgr.Instance.SaveData<BaseData>(baseData);
    }

    /// <summary>
    /// ͬ���������ֿ�������
    /// </summary>
    /// <param name="isOpen"></param>
    public void UpdateBkOpen(bool isOpen)
    {
        musicData.isOpenBk = isOpen;
        //ͬ��ʵ�ʱ������ֿ���
        MusicMgr.Instance.SetBkOpen(isOpen);
       
    }

    /// <summary>
    /// ͬ���������ִ�С����
    /// </summary>
    /// <param name="voluem"></param>
    public void UpdateBkVolume(float voluem)
    {
        musicData.bKVoluem = voluem;
        //ͬ��ʵ�ʱ������ֵĴ�С
        MusicMgr.Instance.SetBkVolume(voluem);
        
    }

    public void UpdateSoundOpen(bool isOpen)
    {
        musicData.isOpenSound = isOpen;
        //ͬ��ʵ�ʱ�����Ч�Ŀ���
        MusicMgr.Instance.SetSoundOpen(isOpen);
    }

    public void UpdateSoundVoluem(float voluem)
    {
        musicData.soundVoluem = voluem;
        //ͬ��ʵ�ʱ�����Ч�Ĵ�С
        MusicMgr.Instance.SetSoundValuem(voluem);
    }

    //ͬ����������������ʵ��������
    private void UpdataAllMusicDataReally()
    {
        //ͬ��ʵ�ʱ������ֵĴ�С
        MusicMgr.Instance.SetBkVolume(musicData.bKVoluem);
        //ͬ��ʵ�ʱ������ֿ���
        MusicMgr.Instance.SetBkOpen(musicData.isOpenBk);
        //ͬ��ʵ�ʱ�����Ч����
        MusicMgr.Instance.SetSoundOpen(musicData.isOpenSound);
        //ͬ��ʵ�ʱ�����Ч��С
        MusicMgr.Instance.SetSoundValuem(musicData.soundVoluem);
    }
}
