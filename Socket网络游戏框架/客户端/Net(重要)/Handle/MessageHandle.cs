using Google.Protobuf;
using SocketGameProtocol;
using System;
using System.Linq;

/// <summary>
/// 专门用来处理消息
/// </summary>
internal class MessageHandle
{
    private byte[] buffer = new byte[1024];

    /// <summary>
    /// 目前剩余待解析消息的总长度
    /// </summary>
    private int waitReadLength;

    /// <summary>
    /// 待解析的字节数组
    /// </summary>
    public byte[] Buffer
    {
        get
        {
            return buffer;
        }
    }

    /// <summary>
    /// 待解析的消息的长度
    /// </summary>
    public int WaitReadLength
    {
        get
        {
            return waitReadLength;
        }
    }

    /// <summary>
    /// 剩余容器的大小
    /// </summary>
    public int Remsize
    {
        get
        {
            return buffer.Length - waitReadLength;
        }
    }

    /// <summary>
    /// 读取消息(分包黏包)
    /// </summary>
    /// <param name="len"></param>
    public void ReadBuffer(int len, Action<MainPack> callBack)
    {
        waitReadLength += len;
        if (waitReadLength <= 4)
        {
            return;
        }
        int count = BitConverter.ToInt32(buffer, 0);

        //黏包
        while (true)
        {
            if (waitReadLength >= (count + 4))
            {
                //解析消息
                MainPack pack = (MainPack)MainPack.Descriptor.Parser.ParseFrom(buffer, 4, count);
                callBack?.Invoke(pack);

                //将前面count+4开始的字节数组搬运到0位置长度为剩余长度的位置(分包)
                Array.Copy(buffer, count + 4, buffer, 0, waitReadLength - count - 4);
                waitReadLength -= (count + 4);
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// 将消息转化为bytes数组
    /// </summary>
    /// <param name="pack"></param>
    /// <returns></returns>
    public byte[] PackData(MainPack pack)
    {
        byte[] data = pack.ToByteArray();
        byte[] head = BitConverter.GetBytes(data.Length);
        return head.Concat(data).ToArray();
    }
}
