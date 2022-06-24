using System;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace Client
{
    class Program
    {
        private
        TcpClient client;
        NetworkStream stream;
        StreamReader sr;
        StreamWriter sw;
        public

        Program()
        {
            client = new TcpClient("127.0.0.1", 6969);
            stream = client.GetStream();
            sr = new StreamReader(stream);
            sw = new StreamWriter(stream);
        }

        ~Program() 
        {
            stream.Close();
            client.Close();
            Console.ReadKey();
        }

        void recvString()
        {
            string str;
            while (true)
            {
                str = sr.ReadLine();
                if(str != "")
                    Console.WriteLine(str);
            } 
        }

        void recvByte()
        {
            byte[] buffer = new byte[69000];
            stream.Read(buffer, 0, buffer.Length);
            int recv = 0;
            foreach (byte b in buffer)
            {
                if (b != 0)
                {
                    recv++;
                }
            }
            string msg_recv = Encoding.UTF8.GetString(buffer, 0, recv);
            Console.WriteLine(msg_recv);
        }
        void sendString()
        {
            string str = Console.ReadLine();
            sw.WriteLine(str);
            sw.Flush();
        }
        static void Main(string[] args)
        {
        connection:
            try
            {
                Program a = new Program();
                //Gửi tin nhắn cho bên server
                a.sendString();

                //Đọc tin nhắn từ bên server gửi qua
                a.recvString();

            }
            catch (Exception e)
            {
                Console.WriteLine("failed to connect...");
                goto connection;
            }
        }
    }
}

