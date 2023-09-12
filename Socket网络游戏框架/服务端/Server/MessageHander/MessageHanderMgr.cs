using SocketGameProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Server.MessageHander
{
    internal class MessageHanderMgr
    {
        /// <summary>
        /// 负责消息处理的类的字典
        /// </summary>
        private Dictionary<RequestCode, BaseHander> handerDic = new Dictionary<RequestCode, BaseHander>();

        private Server server;

        public MessageHanderMgr(Server server)
        {
            this.server = server;
            UserHander userHander = new UserHander();
            handerDic.Add(userHander.GetRequestCode,userHander);
            RoomHander roomHander = new RoomHander();
            handerDic.Add(roomHander.GetRequestCode, roomHander);
            GameHander gameHander = new GameHander();
            handerDic.Add(gameHander.GetRequestCode, gameHander);
        }

        public void HandleRequest(MainPack pack,Client client,bool isUdp=false)
        {
            if(handerDic.TryGetValue(pack.RequestCode,out BaseHander hander))
            {
                //反射执行方法(也可以根据字符串写swich)
                string methodName = pack.ActionCode.ToString();
                MethodInfo methodInfo=hander.GetType().GetMethod(methodName);
                Console.WriteLine(methodName);
                if(methodInfo==null)
                {
                    Console.WriteLine("未找到对应方法"+pack.ActionCode.ToString()); 
                    return;
                }
                object[] objs;
                if (isUdp)
                {
                    objs = new object[] { client, pack };
                    object ret=methodInfo.Invoke(hander,objs);
                    if(ret!=null)
                    {
                        client.SendTo(ret as MainPack);
                    }
                }
                else
                {
                    objs = new object[] { server, client, pack };
                    object ret = methodInfo.Invoke(hander, objs);
                    if (ret != null)
                    {
                        //发送回应
                        client.Send(ret as MainPack);
                    }
                }
            }
            else
            {
                Console.WriteLine("没有找到对应的Controller处理");
            }
        }
    }
}
