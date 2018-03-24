using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServerHttp1.temp;
using System.Reflection;

namespace ServerHttp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Type type = typeof(Dispatcher);
            Dispatcher obj = new Dispatcher();
            type.GetMethod("print").Invoke(obj, null);
            Console.ReadKey();
            //Server server = new Server("127.0.0.1", 3000);
            //server.StartRecieving();
        }
    }
}
