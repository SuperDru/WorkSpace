using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerHttp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server("127.0.0.1", 3000);
            server.StartRecieving();
        }
    }
}
