using System;
using System.IO;
using System.Collections.Specialized;
using System.Net;

namespace ServerHttp1
{
    class Server: EndPoint
    {
        HttpListener listener;
        HttpListenerContext context;
        HttpListenerRequest request;

        public Server(string ip, int port)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(string.Format("http://{0}:{1}/", ip, port));
        }

        public void StartRecieving()
        {
            listener.Start();
            while (true)
            {
                context = listener.GetContext();
                request = context.Request;
                HandleRequest();
            }
        }

        private void HandleRequest()
        {
            
            string requestType = request.HttpMethod;
            string mime = request.ContentType;
            var enc = request.ContentEncoding;
            NameValueCollection headers = request.Headers;

            Stream body = request.InputStream;
            System.Text.Encoding encoding = request.ContentEncoding;
            StreamReader reader = new StreamReader(body, encoding);

            if (requestType == "GET")
            {
                HandleTokens();
            }
            else if (requestType == "POST")
            {
                HandleTokens();
            }
            else
            {
                Console.WriteLine();
            }
        }

        private void HandleTokens()
        {
            switch (request.RawUrl)
            {
                case "/login": CheckAuthorizationData(); break;
                case "/registration": CheckRegistrationData(); break;
                case "/message": CheckMessageData(); break;
            }
        }

        private void CheckMessageData()
        {
            throw new NotImplementedException();
        }

        private void CheckRegistrationData()
        {
            AddRegistrationData();
        }

        private void AddRegistrationData()
        {
            throw new NotImplementedException();
        }

        private void CheckAuthorizationData()
        {
            bool result = LogpassContains("mylogin", "qwerty");
            Console.WriteLine(result);
        }

        private bool LogpassContains(string login, string password)
        {
            StreamReader reader = new StreamReader("logpass.txt");
            while (!reader.EndOfStream)
            {
                string[] logpass = reader.ReadLine().Split(':');
                if (logpass[0] == login && logpass[1] == password)
                    return true;               
            }
            reader.Close();
            return false;
        }


    }
}
