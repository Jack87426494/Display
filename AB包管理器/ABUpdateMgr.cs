using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.IO;
using UnityEngine.Events;
using System.Threading.Tasks;
using System.Text;
using UnityEngine.Networking;

public class ABUpdateMgr : MonoBehaviour
{
    private static ABUpdateMgr instance;
    public static ABUpdateMgr Instance
    {
        get
        {
            if(instance==null)
            {
                GameObject obj = new GameObject("ABUpdateMgr");
                instance=obj.AddComponent<ABUpdateMgr>();
            }
            return instance;
        }
    }

    //待处理ab包对比文件字典,第一个参数是ab包的名字,第二个参数是ab包对比文件信息
    private Dictionary<string, ABCompareClass> abCompareDic = new Dictionary<string, ABCompareClass>();

    //待下载列表 第一个参数为要下载的ab的名字
    private List<string> downloadList=new List<string>();

    //远端下载的临时对比文件的信息
    private string cacheCompareStr="";

    //远端的服务器地址
    private string serverPath = "ftp://192.168.43.200";

    //总的下载字节数
    private int byteNums;
    //目前下载的字节数
    private int nowNums;

    /// <summary>
    /// 更新服务端的所有ab包的对比文件信息到本地
    /// </summary>
    /// <param name="isDownloadCallback">是否下载成功回调函数</param>
    public async void UpdateABCompareFile(UnityAction<bool> isDownloadCallback=null)
    {
        //避免如果上次失败残留信息
        abCompareDic.Clear();
        int reDownloadCount = 3;

        string path =
#if UNITY_IOS
            Application.persistentDataPath + "/AB/IOS/ABCompareFile.txt";
#elif UNITY_ANDRIOD
            Application.persistentDataPath + "/AB/Andriod/ABCompareFile.txt";
#else
            Application.persistentDataPath + "/AB/PC/ABCompareFile.txt";
#endif
        //异步处理避免卡顿
        await Task.Run(() =>
        {
            bool isDownload = false;
            //下载ab包对比文件
            while (!isDownload && reDownloadCount > 0)
            {
                try
                {
                    FtpWebRequest ftpWebRequest = FtpWebRequest.Create(new Uri(serverPath+"/AB/PC/ABCompareFile.txt")) as FtpWebRequest;
                    NetworkCredential networkCredential = new NetworkCredential("xyj", "qwe123123");
                    ftpWebRequest.Credentials = networkCredential;
                    ftpWebRequest.KeepAlive = false;
                    ftpWebRequest.Proxy = null;
                    ftpWebRequest.UseBinary = true;
                    ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                    FtpWebResponse ftpWebResponse = ftpWebRequest.GetResponse() as FtpWebResponse;
                    using (Stream downloadStream = ftpWebResponse.GetResponseStream())
                    {
                        byte[] bytes = new byte[1024 * 20];
                        int nums = downloadStream.Read(bytes, 0, bytes.Length);
                        while (nums != 0)
                        {
                            nums = downloadStream.Read(bytes, 0, bytes.Length);
                        }
                        downloadStream.Dispose();
                        cacheCompareStr = Encoding.UTF8.GetString(bytes);
                        string[] abStrs = cacheCompareStr.Split('|');
                        foreach (string abStr in abStrs)
                        {
                            string[] abInfo = abStr.Split(' ');
                            abCompareDic.Add(abInfo[0], new ABCompareClass(abInfo[0], abInfo[1], abInfo[2]));
                        }
                    }
                    isDownload = true;
                }
                catch
                {
                    isDownload = false;
                }
                --reDownloadCount;
            }
        });

        isDownloadCallback(reDownloadCount != 0);
    }

    /// <summary>
    /// 获取远端ab包对比文件信息
    /// </summary>
    public void GetABCompareInfo()
    {
        //读取下载好的文件，并且拆分其中的信息存储
        string abCompareContent =
#if UNITY_IOS
            File.ReadAllText(Application.persistentDataPath + "/AB/IOS/ABCompareFile.txt");
#elif UNITY_ANDRIOD
            File.ReadAllText(Application.persistentDataPath + "/AB/Andriod/ABCompareFile.txt");
#else
            File.ReadAllText(Application.persistentDataPath + "/AB/PC/ABCompareFile.txt");
#endif

        string[] abStrs = abCompareContent.Split('|');
        foreach (string abStr in abStrs)
        {
            string[] abInfos = abStr.Split(' ');
            abCompareDic.Add(abInfos[0], new ABCompareClass(abInfos[0], abInfos[1], abInfos[2]));
        }
        print("远端ab包对比文件信息位置：" + Application.persistentDataPath);
    }

    /// <summary>
    /// 根据ab包资源对比文件的信息，更新ab包资源
    /// </summary>
    /// <param name="resultCallBack">下载结果委托，参数为下载是否成功</param>
    /// <param name="prossesCallBack">下载进度委托，两个参数为，第一个参数为目前已经下载的资源数，第二个参数为需要下载的资源总数</param>
    public void UpdateABFile(UnityAction<bool> resultCallBack = null, UnityAction<int, int> prossesCallBack = null)
    {
        //避免如果上次失败残留信息
        downloadList.Clear();
        //根据目前的ab对比信息，更新下载列表
        UpdateDownloadList( async () =>
        {
            //根据下载列表,开始下载ab包资源
            //需要下载的资源的总数
            int maxDownloadCount = downloadList.Count;
            //目前已经下载的资源数
            int nowDownloadCount = 0;
            //一共可以重复下载的次数
            int maxReDownloadCount = 5;

            //更新进度
            prossesCallBack?.Invoke(nowDownloadCount, maxDownloadCount);

            string path =
#if UNITY_IOS
            Application.persistentDataPath + "/AB/IOS/";
#elif UNITY_ANDRIOD
            Application.persistentDataPath + "/AB/Andriod/";
#else
                Application.persistentDataPath + "/AB/PC/";
#endif

            while (downloadList.Count != 0 && maxReDownloadCount > 0)
            {
                //异步下载资源
                await Task.Run(() =>
                {
                    //从后向前遍历，避免删除一个结点的时候，影响到整体大小而发生错误
                    for (int i = downloadList.Count - 1; i >= 0; --i)
                    {
                        //如果下载成功就从下载列表里面删除这个结点
                        if (DownLoadABFile(downloadList[i], path + downloadList[i]))
                        {
                            ++nowDownloadCount;
                            downloadList.RemoveAt(i);
                        }
                        
                    }
                    --maxReDownloadCount;
                });
                //更新进度
                prossesCallBack?.Invoke(nowDownloadCount, maxDownloadCount);
            }
            

            //更新成功，更新新的ab包对比文件
            if (downloadList.Count == 0)
            {
                File.WriteAllText(path + "ABCompareFile.txt", cacheCompareStr);
            }

            resultCallBack?.Invoke(downloadList.Count == 0);

            //AssetDatabase.Refresh();

        });
      
    }

    /// <summary>
    /// 根据目前的ab对比信息，更新下载列表
    /// </summary>
    private void UpdateDownloadList(UnityAction callback)
    {
        //总体是去下载新的ab资源，修改老的ab包资源，如果原来有新ab包里面没有的东西，设置成删除persistentDataPath不保留原来的,保留streamingAssetsPath中的
        try
        {
            StartCoroutine(IUpdateDownloadList(callback));
        }
        catch(Exception e)
        {
            Debug.Log("更新下载列表失败："+e.Message);
        }
        
    }

    private IEnumerator IUpdateDownloadList(UnityAction callback)
    {
        string abLocalCompareContent = "";
        string[] abStrs;
        string path=
#if UNITY_IOS
            Application.persistentDataPath + "/AB/Andriod/;
#elif UNITY_Andriod
            Application.persistentDataPath + "/AB/IOS/;
#else
            Application.persistentDataPath + "/AB/PC/";
#endif
        //开始处理persistentDataPath
        if (File.Exists(path + "ABCompareFile.txt"))
        {
            UnityWebRequest unityWebRequest = UnityWebRequest.Get("file:///"+ path + "ABCompareFile.txt");
            yield return unityWebRequest.SendWebRequest();
            if(unityWebRequest.result==UnityWebRequest.Result.Success)
            {
                abLocalCompareContent = unityWebRequest.downloadHandler.text;
                abStrs = abLocalCompareContent.Split('|');
                foreach (string abStr in abStrs)
                {
                    string[] abInfos = abStr.Split(' ');
                    if (abCompareDic.ContainsKey(abInfos[0]))
                    {
                        //如果资源发生过改变，删除以前的，下载新的
                        if (abInfos[2] != abCompareDic[abInfos[0]].md5)
                        {
                            File.Delete(path + abInfos[0]);
                            downloadList.Add(abInfos[0]);
                        }
                        abCompareDic.Remove(abInfos[0]);
                    }
                    else
                    {
                        //直接删除原来的不保留
                        File.Delete(path + abInfos[0]);
                    }
                }
            }
        }
        //开始处理streamingAssetsPath
        else if (File.Exists(Application.streamingAssetsPath + "/ABCompareFile.txt"))
        {
            //读取本地的streamingAssetsPath文件夹的ab资源对比文件信息
            UnityWebRequest unityWebRequest =
#if UNITY_ANDRIOD
            UnityWebRequest.Get(Application.streamingAssetsPath + "/ABCompareFile.txt");
#else
            UnityWebRequest.Get("file:///" + Application.streamingAssetsPath + "/ABCompareFile.txt");
#endif

            yield return unityWebRequest.SendWebRequest();
            if(unityWebRequest.result==UnityWebRequest.Result.Success)
            {
                abLocalCompareContent = unityWebRequest.downloadHandler.text;
                abStrs = abLocalCompareContent.Split('|');
                foreach (string abStr in abStrs)
                {
                    string[] abInfos = abStr.Split(' ');
                    //如果远端和本地都有这个ab包文件,对比MD5码，看看资源是否一样；如果远端没有这个ab包的文件，直接不处理
                    if (abCompareDic.ContainsKey(abInfos[0]))
                    {
                        //如果资源不一样就下载服务器上面的ab包
                        if (abInfos[2] != abCompareDic[abInfos[0]].md5)
                        {
                            downloadList.Add(abInfos[0]);
                        }
                        //移除远端的待处理ab包资源
                        abCompareDic.Remove(abInfos[0]);
                    }
                    else
                    {
                        //如果原来ab包有这个ab包，但是新的ab包里面没有这个ab包，就把原来就有的ab包信息补充到新ab包信息里面。
                        cacheCompareStr += '|';
                        cacheCompareStr += abStr;
                    }
                }
            }
        }
        //最后把streamingAssetsPath和persistentDataPath都没有但是远端有的ab包都下载下来
        foreach (string abStr in abCompareDic.Keys)
        {
            downloadList.Add(abStr);
        }
        abCompareDic.Clear();
        callback();
        StopCoroutine(IUpdateDownloadList(callback));
    }

    /// <summary>
    /// 下载服务端上面的AB包文件或ab信息对比文件
    /// </summary>
    /// <param name="name">要下载文件的名字</param>
    /// <param name="path">文件要下载到本地的位置</param>
    /// <returns>是否下载成功</returns>
    public bool DownLoadABFile(string name,string path)
    {
        try
        {

            //建立一个ftp连接
            FtpWebRequest ftpWebRequest =
#if UNITY_IOS
            FtpWebRequest.Create(new Uri(serverPath + "/AB/IOS/" + name)) as FtpWebRequest;
#elif UNITY_ANDRIOD
            FtpWebRequest.Create(new Uri(serverPath + "/AB/Andriod/" + name)) as FtpWebRequest;
#else
            FtpWebRequest.Create(new Uri(serverPath + "/AB/PC/" + name)) as FtpWebRequest;
#endif
            //建立秘钥
            NetworkCredential networkCredential = new NetworkCredential("xyj", "qwe123123");
            ftpWebRequest.Credentials = networkCredential;
            //代理置空，防止和http冲突
            ftpWebRequest.Proxy = null;
            //使用二进制传输
            ftpWebRequest.UseBinary = true;
            //下载后就断开连接
            ftpWebRequest.KeepAlive = false;
            //设置要干的事情
            ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            //得到应答的流
            FtpWebResponse ftpWebResponse = ftpWebRequest.GetResponse() as FtpWebResponse;
            Stream downloadStream = ftpWebResponse.GetResponseStream();
            //打开本地文件的路径开始从应答流中写入
            using (FileStream localStream = File.OpenWrite(path))
            {
                byte[] bytes = new byte[2048];
                int length = downloadStream.Read(bytes, 0, bytes.Length);
                //流会自动向后面写
                localStream.Write(bytes, 0, length);
                //不停的从应答流中读取字节，直到读不出来
                while(length!=0)
                {
                    length = downloadStream.Read(bytes, 0, bytes.Length);
                    localStream.Write(bytes, 0, length);
                }
                localStream.Dispose();
                downloadStream.Dispose();
            }
            print("成功下载文件:" + name);
            return true;
        }
        catch(Exception e)
        {
            print("失败下载文件：" + name + " " + e.Message);
            return false;
        }
    }

    private void OnDisable()
    {
        instance = null;
    }

    /// <summary>
    /// ab包对比文件数据结构
    /// </summary>
    private class ABCompareClass
    {
        //ab包名字
        public string abName;
        //ab包长度
        public long length;
        //MD5码
        public string md5;
        public ABCompareClass(string abName,string length,string md5)
        {
            this.abName = abName;
            this.length = Convert.ToInt64(length);
            this.md5 = md5;
        }

        //重载等于和不等于用于比较ab包对比文件
        public static bool operator ==(ABCompareClass a,ABCompareClass b)
        {
            return a.md5 == b.md5;
        }
        public static bool operator !=(ABCompareClass a,ABCompareClass b)
        {
            return a.md5 != b.md5;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }

}
