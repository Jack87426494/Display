using MySql.Data.MySqlClient;
using Server.Data;
using SocketGameProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Client
    {
        public Socket clientSocket;

        private Server server;

        private Message message;

        private UserData userData;

        private MySqlConnection sqlConnection;

        //是否已经关闭
        private bool isClose;

        //目前心跳消息的时间
        private float heartTime = -1;
        //心跳消息的最大间隔
        private float heartOffset = 5f;

        public Client(Socket clientSocket,Server server)
        {
            this.clientSocket = clientSocket;
            this.server = server;
            message = new Message();

            //连接数据库
            sqlConnection = new MySqlConnection("database=sys;data source=localhost;user=root;password=@qEd78552364;pooling=false;charset=utf8;port=3306");
            ConnectMysql();
            userData = new UserData();

            isClose = false;
            heartTime = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
            StartReceiveMessage();
            
        }

        private void ConnectMysql()
        {
            try
            {
                sqlConnection.Open();
            }
            catch
            {
                Console.WriteLine("数据库连接失败");
            }
        }

        public UserData GetUserData
        {
            get
            {
                return userData;
            }
        }

        public MySqlConnection GetMySqlConnection
        {
            get
            {
                return sqlConnection;
            }
        }

        private void StartReceiveMessage()
        {
            if(isClose)
            {
                return;
            }


            try
            {
                if (clientSocket == null || clientSocket.Connected == false)
                {
                   
                    return;
                }

                
                clientSocket.BeginReceive(message.Buffer, message.WaitReadLength, message.Remsize, SocketFlags.None, EndAsyncCallback, null);
            }
            catch
            {
                
            }
            
        }

        private void EndAsyncCallback(IAsyncResult ar)
        { 
            try
            {
                if(clientSocket==null||clientSocket.Connected==false)
                {
                    
                    return;
                }
                int len = clientSocket.EndReceive(ar);
                message.ReadBuffer(len, HandleMsg);

                // 检测心跳消息
                CheckHeartMsg();

                StartReceiveMessage();
            }
            catch
            {
               
            }
        }

        /// <summary>
        /// 检测心跳消息
        /// </summary>
        private void CheckHeartMsg()
        {
            if (heartTime !=-1 && (DateTime.Now.Ticks / TimeSpan.TicksPerSecond)-heartTime>heartOffset)
            {
                Console.WriteLine("超过心跳消息时间，判断为客户端失联");
                Close();
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public void Send()
        {
            if (isClose)
            {
                return;
            }
            clientSocket.Send(Encoding.UTF8.GetBytes("服务端发来的消息"));
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="pack"></param>
        public void Send(MainPack pack)
        {
            if (isClose)
            {
                return;
            }
            try
            {
                if (clientSocket.Connected)
                {
                    clientSocket.Send(message.PackData(pack));
                }
            }
            catch
            {

            }
         
        }
        
        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="pack"></param>
        private void HandleMsg(MainPack pack)
        {
            if(pack.ActionCode!=ActionCode.ActionNone)
            {
                server.HandleMsg(pack, this);
            }

            if (pack.RequestCode==RequestCode.Heart)
            {
                Console.WriteLine("收到心跳消息:" + heartTime);
                heartTime = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
            }
        }

        private void Close()
        {
            if(!isClose)
            {
                Console.WriteLine("客户端" + clientSocket.Ttl + "断开连接");
                clientSocket.Dispose();
                server.RemoveClient(this);
                sqlConnection.Close();
                isClose = true;
            }
            
        }
    }
}
