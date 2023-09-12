using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Data
{
    internal class RoomData
    {
        //房间的名字
        public string roomName;

        //目前房间的人数
        public uint nowManNum;

        //房间人数的最大值
        public uint maxManNum;

        //房间中的玩家
        private Dictionary<string,Client> clientDic;

        //房主
        private Client hostClient;

        public RoomData(string roomName, uint nowManNum, uint maxManNum,Client client)
        {
            this.roomName = roomName;
            this.nowManNum = nowManNum;
            this.maxManNum = maxManNum;
            //第一个是房主
            clientDic = new Dictionary<string, Client>();
            //设置房主
            hostClient = client;
        }

        /// <summary>
        /// 加入玩家
        /// </summary>
        /// <param name="client"></param>
        public bool AddPlayer(Client client)
        {
            if(nowManNum>=maxManNum)
            {
                return false;
            }
            ++nowManNum;

            if(!clientDic.ContainsKey(client.GetUserData.userName))
            {
                clientDic.Add(client.GetUserData.userName, client);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除玩家
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool DeletePlayer(Client client)
        {
            if(nowManNum<=0)
            {
                return false;
            }
            --nowManNum;
            if (clientDic.ContainsKey(client.GetUserData.userName))
            {
                clientDic.Remove(client.GetUserData.userName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除玩家
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool DeletePlayer(string userName)
        {
            if (nowManNum <= 0)
            {
                return false;
            }
            --nowManNum;
            if (clientDic.ContainsKey(userName))
            {
                clientDic.Remove(userName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 返回房主
        /// </summary>
        public Client GetHost
        {
            get
            {
                return hostClient;
            } 
        }

        /// <summary>
        /// 返回房间内所有的玩家
        /// </summary>
        public Dictionary<string, Client> GetPlayers
        {
            get
            {
                return clientDic;
            }
        }

        /// <summary>
        /// 返回房间内玩家
        /// </summary>
        /// <param name="playerName"></param>
        /// <returns></returns>
        public Client GetPlayer(string playerName)
        {
            return clientDic[playerName];
        }
    }
}
