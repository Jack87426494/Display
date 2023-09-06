using SocketGameProtocol;
using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class ClientMgr : BaseMgr<ClientMgr>
{
    public Socket clientSocket;

    //处理消息
    private MessageHandle messageHandle;
    //处理请求
    private RequestHandle requestHandle;

    //心跳消息的间隔
    private float heartOffset=3f;

    public ClientMgr()
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        
        Connect();
    }

    private void Connect()
    {
        try
        {
            //117.139.254.233
            clientSocket.Connect("127.0.0.1", 8080);
            messageHandle = new MessageHandle();
            StartReceiveMessage();
            UnityEngine.Debug.Log("连接成功");
            requestHandle = new RequestHandle();
            //开始发送心跳消息
            //StartSendHeart();
        }
        catch
        {
            UnityEngine.Debug.Log("连接失败");
        }
    }

    private void StartReceiveMessage()
    {
        clientSocket.BeginReceive(messageHandle.Buffer, messageHandle.WaitReadLength, messageHandle.Remsize, SocketFlags.None, EndAsyncCallback, null);
    }

    private void EndAsyncCallback(IAsyncResult ar)
    {
        try
        {
            if (clientSocket == null || clientSocket.Connected == false)
            {
                return;
            }
            int len = clientSocket.EndReceive(ar);
            messageHandle.ReadBuffer(len, HandleMsg);
            StartReceiveMessage();
        }
        catch(Exception e)
        {
            UnityEngine.Debug.Log("接受消息出错："+e.Message);
        }
    }

    private MainPack heartPack=new MainPack() { RequestCode=RequestCode.Heart};

    /// <summary>
    /// 开始发送心跳消息
    /// </summary>
    private void StartSendHeart()
    {
        MonoMgr.Instance.StartCoroutine(Iheart());
    }

    IEnumerator Iheart()
    {
        while(true)
        {
            yield return new WaitForSeconds(heartOffset);
            Send(heartPack);
        }
    }

    /// <summary>
    /// 处理接受得到的消息
    /// </summary>
    /// <param name="pack"></param>
    private void HandleMsg(MainPack pack)
    {
        requestHandle.HandleRequest(pack);
    }

    public void Close()
    {
        clientSocket.Dispose();
        clientSocket = null;
    }

    /// <summary>
    /// 发送请求
    /// </summary>
    /// <param name="pack"></param>
    public void SendRequest(BaseRequest baseRequst)
    {
        Send(requestHandle.SendPack(baseRequst));
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="pack"></param>
    public void Send(MainPack pack)
    {
        clientSocket.Send(messageHandle.PackData(pack));
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    public void Send()
    {
        clientSocket.Send(Encoding.UTF8.GetBytes("服务端发来的消息"));
    }
}
