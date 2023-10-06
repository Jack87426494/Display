using MySqlX.XDevAPI;
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
                if (!server.roomDic.ContainsKey(roomPack.RoomName))
                {
                    server.roomDic.Add(roomPack.RoomName, new RoomData(roomPack.RoomName, roomPack.NowManNum, roomPack.MaxManNum, client));
                    pack.ReturnCode = ReturnCode.Succeed;
                    //广播给其它剩余的客户端
                    foreach (Client otherClient in server.clientList)
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
            if (server.roomDic !=null)
            {
                pack.RoomPackList.Clear();
                foreach (RoomData roomData in server.roomDic.Values)
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
                if (server.roomDic.ContainsKey(roomPack.RoomName))
                {
                    pack.ReturnCode = ReturnCode.Succeed;
                    pack.RoomPackList[0].NowManNum = server.roomDic[roomPack.RoomName].nowManNum;
                    pack.RoomPackList[0].MaxManNum = server.roomDic[roomPack.RoomName].maxManNum;
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
            if(server.roomDic.Count<=0)
            {
                pack.ReturnCode = ReturnCode.Fail;
                return pack;
            }

            foreach(RoomPack roomPack in pack.RoomPackList)
            {
                if (server.roomDic.ContainsKey(roomPack.RoomName))
                {
                    if (client == server.roomDic[roomPack.RoomName].GetHost)
                    {
                        server.roomDic.Remove(roomPack.RoomName);
                        pack.ReturnCode = ReturnCode.Succeed;

                        //广播给其它剩余的客户端
                        foreach (Client otherClient in server.clientList)
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

        /// <summary>
        /// 加入某个房间
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack AddRoom(Server server, Client client, MainPack pack)
        {
            if (server.roomDic.ContainsKey(pack.RoomPackList[0].RoomName))
            {
                RoomData roomData = server.roomDic[pack.RoomPackList[0].RoomName];

                if(roomData.AddPlayer(client)==false)
                {
                    pack.ReturnCode = ReturnCode.Fail;
                    return pack;
                }
                //pack.LoginPack.UserName = client.GetUserData.userName;

                foreach (Client userClient in roomData.GetPlayers.Values)
                {
                    pack.RoomPackList[0].UserDic.Add(userClient.GetUserData.userName, new UserPack() { UserName= userClient.GetUserData.userName });
                }
                pack.RoomPackList[0].NowManNum = roomData.nowManNum;

                pack.ReturnCode = ReturnCode.Succeed;
                //广播给房间内其它剩余的客户端
                foreach (Client otherClient in roomData.GetPlayers.Values)
                {
                    if (otherClient == client)
                    {
                        continue;
                    }
                    otherClient.Send(pack);
                }


                return pack;
            }
            else
            {
                pack.ReturnCode = ReturnCode.Fail;
                return pack;
            }
        }

        /// <summary>
        /// 退出房间 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack ExitRoom(Server server, Client client, MainPack pack)
        {
            if (server.roomDic.ContainsKey(pack.RoomPackList[0].RoomName))
            {
                pack.ReturnCode = ReturnCode.Succeed;
               
                foreach (UserPack userPack in pack.RoomPackList[0].UserDic.Values)
                {
                    server.roomDic[pack.RoomPackList[0].RoomName].DeletePlayer(userPack.UserName);
                }
                pack.RoomPackList[0].NowManNum = server.roomDic[pack.RoomPackList[0].RoomName].nowManNum;

                RoomData roomData = server.roomDic[pack.RoomPackList[0].RoomName];
                //广播给房间内其它剩余的客户端
                foreach (Client otherClient in roomData.GetPlayers.Values)
                {
                    if (otherClient == client)
                    {
                        continue;
                    }
                    otherClient.Send(pack);
                }

                return pack;
            }
            else
            {
                pack.ReturnCode = ReturnCode.Fail;
                return pack;
            }
        }
        
        /// <summary>
        /// 开始游戏
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack BeginGame(Server server, Client client, MainPack pack)
        {
            if (server.roomDic.ContainsKey(pack.RoomPackList[0].RoomName)&&
                server.roomDic[pack.RoomPackList[0].RoomName].GetHost == client)
            {
                //将房间内所有的玩家数据写入包中
                foreach (Client playerCliet in server.roomDic[pack.RoomPackList[0].RoomName].GetPlayers.Values)
                {
                    PlayerPack playerPack = new PlayerPack();
                    playerPack.PlayerName = playerCliet.GetUserData.userName;
                    pack.PlayerDic.Add(playerPack.PlayerName, playerPack);
                }

                //foreach (string hostName in pack.RoomPackList[0].UserDic.Keys)
                //{
                //    PlayerPack playerPack = new PlayerPack();
                //    playerPack.PlayerName = hostName;
                //    pack.PlayerDic.Add(playerPack.PlayerName, playerPack);
                //}


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

            pack.ReturnCode = ReturnCode.Fail;
            return pack;
        }

    }
}
