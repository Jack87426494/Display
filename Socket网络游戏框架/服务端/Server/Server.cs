using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using Server.MessageHander;
using SocketGameProtocol;
using MySql.Data.MySqlClient;
using Server.Data;

namespace Server
{
    internal class Server
    {
        private Socket serverSocket;

        private UdpServer udpServer;

        //客户端列表
        public List<Client> clientList=new List<Client>();

        private MessageHanderMgr messageHanderMgr;

        /// <summary>
        /// 初始化服务端
        /// </summary>
        /// <param name="ip">服务端地址</param>
        /// <param name="port">端口号</param>
        public Server(string ip, int port)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            serverSocket.Listen(100);
            messageHanderMgr = new MessageHander.MessageHanderMgr(this);
            Accept();
            udpServer = new UdpServer(ip, port, this, messageHanderMgr);
        }

        private void Accept()
        {
            serverSocket.BeginAccept(AsyncCallback, null);
        }
        private void AsyncCallback(IAsyncResult ar)
        {
            Socket clientSocket = serverSocket.EndAccept(ar);
            clientList.Add(new Client(clientSocket,this,udpServer));
            if (ar.IsCompleted)
            {
                Accept();
            }
        }


        private void Send(Socket clientSocket)
        {
            clientSocket.Send(Encoding.UTF8.GetBytes("服务端发来的消息"));
        }

        public void HandleMsg(MainPack pack,Client client)
        {
            messageHanderMgr.HandleRequest(pack, client);
        }

        public void RemoveClient(Client client)
        {
            clientList.Remove(client);
        }

        public Client ClientFromUserName(string user)
        {
            foreach (Client c in clientList)
            {
                if (c.GetUserData.userName == user)
                {
                    return c;
                }
            }
            return null;
        }
    }
}
