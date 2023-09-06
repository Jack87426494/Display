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
        private List<Client> clientList;

        public RoomData(string roomName, uint nowManNum, uint maxManNum,Client client)
        {
            this.roomName = roomName;
            this.nowManNum = nowManNum;
            this.maxManNum = maxManNum;
            //第一个是房主
            clientList = new List<Client>() {client};
        }

        /// <summary>
        /// 返回房主
        /// </summary>
        public Client GetHost
        {
            get
            {
                return clientList[0];
            } 
        }
    }
}
