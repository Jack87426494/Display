using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Net.Mail;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Server server = new Server("123.60.157.114", 8080);
            Server server = new Server("127.0.0.1", 8080);
            Console.ReadLine();
        }
    }
}