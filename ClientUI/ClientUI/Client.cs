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
            string response;
            var x = sr.ReadLine();
            
            var numberOfFood = Int32.Parse(x);
            for (int i = 0; i < numberOfFood; i++)
            {
                response = sr.ReadLine();
                //nhan ten....gia
                menuList.Add(response);
            }
           
            return menuList;
        }

        public void recvByte()
        {


            int n = Int32.Parse(sr.ReadLine());               //Reveice size of image

            MessageBox.Show(n.ToString());

            byte[] buffer = new byte[n];                    

            MessageBox.Show(stream.Read(buffer).ToString());   //receive & show number of bytes received (Lost ~1000 bytes)
            
            File.Delete("C:/Users/Trung/Desktop/c.jpg");       //Delete old file
            File.WriteAllBytes("C:/Users/Trung/Desktop/c.jpg", buffer); //Create file
            //int recv = buffer.Length;
            /*string msg_recv = Encoding.ASCII.GetString(buffer, 0, recv);
            Console.WriteLine(msg_recv);*/

        }

        public void createPic(byte[] picByte)
        {

            
            
        }
        
    }
}

