using SocketGameProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.MessageHander
{
    internal class GameHander : BaseHander
    {
        public GameHander()
        {
            requestCode = RequestCode.Game;
        }

        /// <summary>
        /// 处理玩家输入
        /// </summary>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack PlayerInput(Server server, Client client, MainPack pack)
        {
            pack.ReturnCode = ReturnCode.Succeed;
            //开始广播
            foreach (Client userClient in server.roomDic[pack.RoomPackList[0].RoomName].GetPlayers.Values)
            {
                if (userClient == client)
                {
                    continue;
                }
                userClient.Send(pack);
            }
               
            return pack;
        }

        /// <summary>
        /// 开火
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack Fire(Server server, Client client, MainPack pack)
        {
            pack.ReturnCode = ReturnCode.Succeed;
            //开始广播
            foreach (Client userClient in server.roomDic[pack.RoomPackList[0].RoomName].GetPlayers.Values)
            {
                if (userClient == client)
                {
                    continue;
                }
                userClient.Send(pack);
            }

            return pack;
        }

        /// <summary>
        /// 受伤
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack Hit(Server server, Client client, MainPack pack)
        {
            pack.ReturnCode = ReturnCode.Succeed;
            //开始广播
            foreach (Client userClient in server.roomDic[pack.RoomPackList[0].RoomName].GetPlayers.Values)
            {
                if (userClient == client)
                {
                    continue;
                }
                userClient.Send(pack);
            }

            return pack;
        }
    }
}
