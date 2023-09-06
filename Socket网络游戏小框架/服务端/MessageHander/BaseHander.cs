using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketGameProtocol;

namespace Server.MessageHander
{
    /// <summary>
    /// 负责消息处理的类
    /// </summary>
    internal class BaseHander
    {
        protected RequestCode requestCode = RequestCode.RequestNone;

        public RequestCode GetRequestCode
        {
            get
            {
                return requestCode;
            }
        }

       
    }
}
