using Org.BouncyCastle.Bcpg;
using Server.MessageHander;
using SocketGameProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class UdpServer
    {
        private Socket udpSocket;
        IPEndPoint iPEndPoint;
        EndPoint endPoint;
        //服务端
        Server server;

        //消息处理器
        private MessageHanderMgr messageHanderMgr;

        Byte[] buffer = new Byte[1024];


        public UdpServer(string ip,int port,Server server,MessageHanderMgr messageHanderMgr)
        {
            this.server = server;
            this.messageHanderMgr = messageHanderMgr;
            udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            endPoint = (EndPoint)iPEndPoint;
            udpSocket.Bind(iPEndPoint);
            ReceiveMsg();
        }
        public void ReceiveMsg()
        {

            try
            {
                if (udpSocket == null || udpSocket.Connected == false)
                {

                    return;
                }


                udpSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, EndAsyncCallback, null);
            }
            catch
            {

            }
        }
        private int len;

        private void EndAsyncCallback(IAsyncResult ar)
        {
            
            try
            {
                len=udpSocket.EndReceive(ar);
                MainPack pack = (MainPack)MainPack.Descriptor.Parser.ParseFrom(buffer, 0, len);
                HandleRequest(pack, endPoint);
                ReceiveMsg();
            }
            catch (SocketException s)
            {
                Console.WriteLine("接受消息出错" + s.SocketErrorCode + s.Message);
            }
        }

        //public async void ReceiveMsg()
        //{
        //    int len;
        //    while (true)
        //    {
        //        await Task.Run(() =>
        //        {
        //            len = udpSocket.ReceiveFrom(buffer, ref endPoint);
        //            MainPack pack = (MainPack)MainPack.Descriptor.Parser.ParseFrom(buffer, 0, len);
        //            HandleRequest(pack, endPoint);
        //            Thread.Sleep(1000 / 144);
        //        });
        //    }
        //}

        public void HandleRequest(MainPack pack,EndPoint endPoint)
        {
            Client client = server.ClientFromUserName(pack.LoginPack.UserName);
            if (client.IEP == null)
            {
                client.IEP = iPEndPoint;
                
            }

            messageHanderMgr.HandleRequest(pack, client, true);
            
        }

        public void SendTo(MainPack pack,EndPoint endPoint)
        {
            byte[] buff = Message.PackData(pack);
            udpSocket.SendTo(buff, buff.Length, SocketFlags.None, endPoint);
        }
    }
}
