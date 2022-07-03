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
using System.Diagnostics;
using Newtonsoft.Json;
using System.Drawing;

//widTh 340
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
            var numberOfFood = Int32.Parse(sr.ReadLine());
            for (int i = 0; i < numberOfFood; i++)
            {
                // Get content
                response = sr.ReadLine();
                menuList.Add(response);
                var numberOfDish = Int32.Parse(sr.ReadLine());
                for (int j = 0; j < numberOfDish; j++)
                {
                    response = sr.ReadLine();
                    //nhan ten....gia
                    menuList.Add(response);
                }
            }
            return menuList;
        }

        public void recvPic(ref string imagePath)
        {
           
            int n = Int32.Parse(sr.ReadLine());         //Reveice size of image
            MessageBox.Show(n.ToString());
            byte[] buffer = new byte[n];
            
            stream.Read(buffer, 0, n);

            try
            {
                using (var Nstream = new MemoryStream(buffer, 0, n))
                {
                    System.Drawing.Image img = new Bitmap(Nstream);
                    try
                    {
                        if (File.Exists(@".\image.jpg") == true)
                            File.Delete(@".\image.jpg");
                        img.Save(@".\image.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                        imagePath = @".\image.jpg";
                    }
                    catch 
                    {
                        if (File.Exists(@".\image2.jpg") == true)
                            File.Delete(@".\image2.jpg");
                        var sourceImage = new Bitmap(@".\image.jpg");
                        sourceImage.Dispose();
                        img.Save(@".\image2.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                        imagePath = @".\image2.jpg";
                    }
                    img.Dispose();
                }
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

        }


        public List<string> recvList()
        {
            List <string> list = new List<string>();

            return list;
        }

        public void add(object sender, RoutedEventArgs e)
        {

        }

        public void remove(object sender, RoutedEventArgs e)
        {

        }
        public void sendRequest(int index, int foodchoice)
        {
            sw.WriteLine(index.ToString() + " " + foodchoice.ToString());
            sw.Flush();
        }
    }
}

