using SocketGameProtocol;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class ClientMgr : BaseMgr<ClientMgr>
{
    /// <summary>
    /// tcp的Socket
    /// </summary>
    public Socket clientSocket;

    /// <summary>
    /// udp的socket
    /// </summary>
    public Socket udpSocket;

    //处理消息
    private MessageHandle messageHandle;
    //处理请求
    private RequestHandle requestHandle;

    //心跳消息的间隔
    private float heartOffset=3f;

    //udp字节
    private byte[] udpBuffer = new byte[1024];

    private IPEndPoint iPEndPoint;
    private EndPoint endPoint;

    int len;

    public ClientMgr()
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        Connect();
    }


    private void Connect()
    {
        try
        {
            //117.139.254.233
            //tcp连接
            clientSocket.Connect("127.0.0.1", 8080);
            //udp连接
            iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
            endPoint = (EndPoint)iPEndPoint;
            udpSocket.Connect(endPoint);

            messageHandle = new MessageHandle();
            //tcp开始接收消息
            StartReceiveMessage();
            //udp开始接收消息
            UdpReceiveMsg();

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
        try
        {
            if (clientSocket == null || clientSocket.Connected == false)
            {

                return;
            }

            clientSocket.BeginReceive(messageHandle.Buffer,
                messageHandle.WaitReadLength, messageHandle.Remsize, SocketFlags.None, EndAsyncCallback, null);
        }
        catch (Exception e)
        {
            Console.WriteLine("接受消息出错:" + e.Message);
        }
    }

    private void EndAsyncCallback(IAsyncResult ar)
    {
        try
        {
            if (clientSocket == null || clientSocket.Connected == false)
            {
                return;
            }
            len = clientSocket.EndReceive(ar);
            messageHandle.ReadBuffer(len, HandleMsg);
            StartReceiveMessage();
        }
        catch(Exception e)
        {
            UnityEngine.Debug.Log("tcp接受消息出错："+e.Message);
        }
    }

    private void UdpReceiveMsg()
    {
        udpSocket.BeginReceive(udpBuffer, 0, udpBuffer.Length, SocketFlags.None, UdpEndAsyncCallback, null);
    }

    

    private void UdpEndAsyncCallback(IAsyncResult ar)
    {
        try
        {
            if (udpSocket == null || udpSocket.Connected == false)
            {
                return;
            }
            len = udpSocket.EndReceive(ar);
            HandleMsg((MainPack)MainPack.Descriptor.Parser.ParseFrom(udpBuffer, 0, len));
            UdpReceiveMsg();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log("udp接受消息出错：" + e.Message);
        }
    }

    //private async void UdpReceiveMsg()
    //{
    //    int len;
    //    while (true)
    //    {
    //        await Task.Run(() =>
    //        {
    //            len = udpSocket.ReceiveFrom(udpBuffer, ref endPoint);
    //            HandleMsg((MainPack)MainPack.Descriptor.Parser.ParseFrom(udpBuffer, 0, len));
    //            Thread.Sleep(1000/144);
    //        });
    //    }
    //}

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
    /// <param name="baseRequst"></param>
    /// <param name="isUdp"></param>
    public void SendRequest(BaseRequest baseRequst,bool isUdp=false)
    {
        if(isUdp)
        {
            UdpSend(requestHandle.SendPack(baseRequst));
        }
        else
        {
            Send(requestHandle.SendPack(baseRequst));
        }
        
    }

    /// <summary>
    /// tcp发送消息
    /// </summary>
    /// <param name="pack"></param>
    public void Send(MainPack pack)
    {
        clientSocket.Send(MessageHandle.PackData(pack));
    }

    /// <summary>
    /// udp发送消息
    /// </summary>
    /// <param name="pack"></param>
    public void UdpSend(MainPack pack)
    {
        udpSocket.SendTo(MessageHandle.PackData(pack),endPoint);
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    public void Send()
    {
        clientSocket.Send(Encoding.UTF8.GetBytes("服务端发来的消息"));
    }

    /// <summary>
    /// 绑定消息处理
    /// </summary>
    /// <param name="actionCode"></param>
    /// <param name="responseAction"></param>
    public void BindResponse(ActionCode actionCode,UnityAction<MainPack> responseAction)
    {
        requestHandle.BindResponse(actionCode, responseAction);
    }
}
