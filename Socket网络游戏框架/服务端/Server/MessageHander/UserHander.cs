using SocketGameProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.MessageHander
{
    internal class UserHander:BaseHander
    {

        public UserHander()
        {
            requestCode = RequestCode.User;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="server">服务端</param>
        /// <param name="client">客户端</param>
        /// <param name="pack">包</param>
        /// <returns></returns>
        public MainPack Register(Server server, Client client, MainPack pack)
        {
            if(client.GetUserData.Register(pack,client.GetMySqlConnection))
            {
                pack.ReturnCode = ReturnCode.Succeed;
            }
            else
            {
                pack.ReturnCode = ReturnCode.Fail;
            }
            return pack;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="server">服务端</param>
        /// <param name="client">客户端</param>
        /// <param name="pack">包</param>
        /// <returns></returns>
        public MainPack Login(Server server, Client client, MainPack pack)
        {
            if(client.GetUserData.LogIn(pack,client.GetMySqlConnection))
            {
                pack.ReturnCode = ReturnCode.Succeed;
            }
            else
            {
                pack.ReturnCode = ReturnCode.Fail;
            }
            return pack;
        }
    }
}
