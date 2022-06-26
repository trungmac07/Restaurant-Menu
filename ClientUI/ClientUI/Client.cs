using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
namespace Client
{
    class Client
    {
        private
        TcpClient client;
        NetworkStream stream;
        StreamReader sr;
        StreamWriter sw;
        public

        Client()
        {
            client = new TcpClient("127.0.0.1", 6969);
            stream = client.GetStream();
            sr = new StreamReader(stream);
            sw = new StreamWriter(stream);
        }

        ~Client()
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
                if (str != "")
                    Console.WriteLine(str);
            }
        }
        public List<string> recvMenu()
        {
            List<string> menuList = new List<string>();
            var response = sr.ReadLine();
            Console.WriteLine(response);
            var numberOfFood = Int32.Parse(sr.ReadLine());
            for (int i = 0; i < numberOfFood; i++)
            {
                response = sr.ReadLine();
                //nhan ten....gia
                menuList.Add(response);
                Console.WriteLine(response);
            }
            return menuList;
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

    }
}

