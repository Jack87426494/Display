using Server.Data;
using SocketGameProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.MessageHander
{
    internal class RoomHander:BaseHander
    {
        //房间字典
        public Dictionary<string, RoomData> roomDic = new Dictionary<string, RoomData>();

        public RoomHander()
        {
            requestCode = RequestCode.Room;
        }


        /// <summary>
        /// 创建新房间
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack CreateRoom(Server server, Client client, MainPack pack)
        {
            if (pack.RoomPackList[0].RoomName=="")
            {
                pack.ReturnCode = ReturnCode.Fail;
                return pack;
            }

            foreach (RoomPack roomPack in pack.RoomPackList)
            {
                if (!roomDic.ContainsKey(roomPack.RoomName))
                {
                    roomDic.Add(roomPack.RoomName, new RoomData(roomPack.RoomName, roomPack.NowManNum, roomPack.MaxManNum, client));
                    pack.ReturnCode = ReturnCode.Succeed;
                    //广播给其它剩余的客户端
                    foreach (Client otherClient in server.clientDic)
                    {
                        if (otherClient == client)
                        {
                            continue;
                        }
                        otherClient.Send(pack);
                    }
                }
                return pack;
            };
            
            pack.ReturnCode = ReturnCode.Fail;
            return pack;
        }

        /// <summary>
        /// 查找所有房间
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack FindAllRoom(Server server, Client client, MainPack pack)
        {
            if (roomDic!=null)
            {
                pack.RoomPackList.Clear();
                foreach (RoomData roomData in roomDic.Values)
                {
                    RoomPack roomPack = new RoomPack();
                    roomPack.RoomName = roomData.roomName;
                    roomPack.NowManNum = roomData.nowManNum;
                    roomPack.MaxManNum = roomData.maxManNum;
                    pack.RoomPackList.Add(roomPack);
                }

                pack.ReturnCode = ReturnCode.Succeed;
            }
            else
            {
                pack.ReturnCode = ReturnCode.Fail;
            }
            return pack;
        }

        /// <summary>
        /// 查找某个房间
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack FindRoom(Server server, Client client, MainPack pack)
        {
            foreach (RoomPack roomPack in pack.RoomPackList)
            {
                if (roomDic.ContainsKey(roomPack.RoomName))
                {
                    pack.ReturnCode = ReturnCode.Succeed;
                    return pack;
                }
            }
            
            pack.ReturnCode = ReturnCode.Fail;
            return pack;
        }

        /// <summary>
        /// 删除某个房间
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack DeleteRoom(Server server, Client client, MainPack pack)
        {
            if(roomDic.Count<=0)
            {
                pack.ReturnCode = ReturnCode.Fail;
                return pack;
            }

            foreach(RoomPack roomPack in pack.RoomPackList)
            {
                if (roomDic.ContainsKey(roomPack.RoomName))
                {
                    if (client == roomDic[roomPack.RoomName].GetHost)
                    {
                        roomDic.Remove(roomPack.RoomName);
                        pack.ReturnCode = ReturnCode.Succeed;

                        //广播给其它剩余的客户端
                        foreach (Client otherClient in server.clientDic)
                        {
                            if (otherClient == client)
                            {
                                continue;
                            }
                            otherClient.Send(pack);
                        }
                        return pack;
                    }
                }
            }

            pack.ReturnCode = ReturnCode.Fail;
            return pack;
        }
    }
}
