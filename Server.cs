using System;
using System.IO;
using System.Collections.Specialized;
using System.Text;
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
            Console.WriteLine("Сервер запущен.");
            listener.Start();
            while (true)
            {
                Console.WriteLine("Ожидаем запрос...");
                context = listener.GetContext();
                request = context.Request;
                HandleRequest();
            }
        }

        private void HandleRequest()
        {
            string requestType = request.HttpMethod;

            Console.WriteLine(string.Format("Получен {0} запрос на {1}.", requestType, request.RawUrl));

            if (requestType == "POST")
            {
                HandleTokens();
            }
            else if (requestType == "OPTIONS")
            {
                SendResponse(203);
            }
        }

        private void HandleTokens()
        {
            switch (request.RawUrl)
            {
                case "/login-password": CheckAuthorizationData(); break;//check

                case "/registration-company":SetCompanyData(); break; //server
                case "/registration-employee":SetEmployeeData(); break; //server

                case "/set-message": SetMessageData(); break; //server
                case "/set-tag": SetData("tag.txt"); break; //server
                case "/set-dept": SetData("dept.txt"); break; //server
                case "/set-rating":SetRatingData();break; //server
                case "/set-average": setAverageData(); break; //server

                case "/get-message": GetMessageData(); break; //client
                case "/get-message-dept": GetDeptMessageData("tag.txt"); break; //client
                case "/get-rating": getRatingData(); break; //client
                case "/get-dept-rating": GetDeptRatingData(); break; //client
                case "/get-average-rating": getAverageRatingData(); break; //client
                case "/get-average-dept-rating": getDeptAverageRatingData(); break; //client

            }
        }

        private void GetDeptRatingData()
        {
            string[] value = ParseData();
            string dept = value[0].Split(':')[1];
            HttpListenerResponse response = context.Response;

            StreamReader reader = new StreamReader("tag.txt");
            string[] data = reader.ReadLine().Split(new string[] { "/&&/" }, StringSplitOptions.RemoveEmptyEntries);
            string result = "";
            foreach (string str in data)
            {
                string st = GetMessage(dept, str + "_rating");
                if (st != null)
                {
                    result += st;
                }
            }
            StreamWriter writer = new StreamWriter(response.OutputStream);
            writer.Write(result);
            response.StatusCode = 200;
            writer.Close();
        }

        private void getRatingData()
        {
            string[] value = ParseData();
            string name = value[0].Split(':')[1] + "_" + value[1].Split(':')[1] + "_rating" + ".txt";
            HttpListenerResponse response = context.Response;
            StreamWriter stream = new StreamWriter(response.OutputStream, enc);
            try
            {
                StreamReader reader = new StreamReader(name);
                string values = reader.ReadLine();
                response.ContentEncoding = enc;
                stream.Write(values);
                response.StatusCode = 200;
                reader.Close();
            }
            catch
            {
                response.StatusCode = 200;
            }

            stream.Close();
        }

        private void SetRatingData()
        {
            string[] value = ParseData();
            string name = value[0].Split(':')[1] + "_" + value[1].Split(':')[1] + "_rating.txt";
            StreamWriter writer = new StreamWriter(name, true);
            writer.Write(value[2].Split(':')[1] + " ");
            writer.Close();
            SendResponse(200);

        }

        private void SetEmployeeData()
        {
            throw new NotImplementedException();
        }

        private void SetCompanyData()
        {
            throw new NotImplementedException();
        }

        private void getDeptAverageRatingData()
        {
            string[] value = ParseData();
            string dept = value[0].Split(':')[1];
            HttpListenerResponse response = context.Response;

            StreamReader reader = new StreamReader("tag.txt" );
            string[] data = reader.ReadLine().Split(new string[] { "/&&/" }, StringSplitOptions.RemoveEmptyEntries);
            int result = 0, count = 0;
            foreach (string str in data)
            {
                string st = GetMessage(dept, str + "_rating");
                if (st != null)
                {
                    int counter = int.Parse(st.Split(' ')[0]),
                        sum = int.Parse(st.Split(' ')[1]);
                    result += sum ;
                    count++;
                }
            }

            StreamWriter stream = new StreamWriter(response.OutputStream);
            stream.Write((result / count).ToString());
            response.StatusCode = 200;
            stream.Close();

        }

        private void getAverageRatingData()
        {
            string[] value = ParseData();
            string name = value[0].Split(':')[1] + "_" + value[1].Split(':')[1] + "_average" + ".txt";
            try
            {
                string[] values = GetMessage(value[0].Split(':')[1], value[1].Split(':')[1] + "_average").Split(' ');
                int counter = int.Parse(values[0]),
                    sum = int.Parse(values[1]);
                HttpListenerResponse response = context.Response;
                StreamWriter stream = new StreamWriter(response.OutputStream, enc);
                stream.Write((sum / counter).ToString());
                response.StatusCode = 200;
                stream.Close();
            }
            catch
            {
                SendResponse(402);
            }
        }

        private void setAverageData()
        {
            string[] value = ParseData();
            string name = value[0].Split(':')[1] +  "_" +  value[1].Split(':')[1] + "_average" + ".txt";
            try
            {
                StreamReader reader = new StreamReader(name);
                string[] values = reader.ReadLine().Split(' ');
                int counter = int.Parse(values[0]);
                int sum = int.Parse(values[1]);
                counter++;
                sum += int.Parse(value[2].Split(':')[1]);
                reader.Close();
                StreamWriter writer = new StreamWriter(name);
                writer.Write(counter.ToString() + " " + sum.ToString());
                writer.Close();
                SendResponse(200);
            }
            catch
            {
                StreamWriter writer = new StreamWriter(name);
                writer.Write("1 " + value[2].Split(':')[1]);
                writer.Close();
                SendResponse(200);
            }

            



        }

        private void SetData(string name)
        {
            string[] data = ParseData();
            StreamWriter writer = new StreamWriter(name);
            foreach (string str in data)
                writer.Write(str.Split(':')[1] + "/&&/");
            writer.Close();
            SendResponse(200);
        }

        private void GetDeptMessageData(string name)
        {
            string dept = ParseData()[0].Split(':')[1];
            HttpListenerResponse response = context.Response;

            StreamReader reader = new StreamReader(name, enc);
            string[] data = reader.ReadLine().Split(new string[] { "/&&/" }, StringSplitOptions.RemoveEmptyEntries);
            string result = "";
            foreach (string str in data)
            {
                string st = GetMessage(dept, str);
                if (st != null)
                    result += st;
            }
            StreamWriter stream = new StreamWriter(response.OutputStream);
            stream.Write(result);
            response.StatusCode = 200;
            stream.Close();
        }

        private void GetMessageData()
        {
            string[] logpasses = ParseData();

            HttpListenerResponse response = context.Response;
            string result = GetMessage(logpasses[0].Split(':')[1], logpasses[1].Split(':')[1]);
            response.ContentEncoding = enc;
            StreamWriter stream = new StreamWriter(response.OutputStream, enc);
            stream.Write(result);
            response.StatusCode = 200;
            stream.Close();
        }
        private string GetMessage(string dept, string tag)
        {
            try
            {
                StreamReader reader = new StreamReader(dept + "_" + tag + ".txt", enc);
                string result = reader.ReadToEnd();
                reader.Close();
                return result;
            } catch 
            {
                return null;
            }
        }

        private void SetMessageData()
        {
            string[] logpasses = ParseData();

            AddMessage(logpasses[0].Split(':')[1], logpasses[1].Split(':')[1], logpasses[2].Split(':')[1]);
            SendResponse(200);
        }
        private void AddMessage(string dept, string tag, string message)
        {
            StreamWriter writer = new StreamWriter(dept + "_" + tag + ".txt", true, enc);
            writer.Write(message + "/&&/");
            writer.Close();
        }

        private void CheckAuthorizationData()
        {
            string[] logpasses = ParseData();
            int status;
            if (LogpassContains(logpasses[0].Split(':')[1], logpasses[1].Split(':')[1]))
                status = 200;
            else
                status = 403;
            SendResponse(status);
        }
        private bool LogpassContains(string login, string password)
        {
            StreamReader reader = new StreamReader("logpass.txt", enc);
            while (!reader.EndOfStream)
            {
                string[] logpass = reader.ReadLine().Split(':');
                if (logpass[0] == login && logpass[1] == password)
                    return true;               
            }
            reader.Close();
            return false;
        }

        private string[] ParseData()
        {
            Stream body = request.InputStream;
            //Encoding encoding = request.ContentEncoding;
            StreamReader reader = new StreamReader(body, enc);
            Console.WriteLine(request.ContentEncoding);

            string str = reader.ReadLine();
            Console.WriteLine(str);
            return str.Substring(1, str.Length - 2).Replace("\"", "").Split(',');
        }

        private void SendResponse(int status)
        {
            Console.WriteLine(string.Format("Отправляем ответ клиенту со статусом {0}.", status));
            Console.WriteLine();
            context.Response.ContentEncoding = enc;
            context.Response.StatusCode = status;
            context.Response.OutputStream.Close();

        }
    }
}
