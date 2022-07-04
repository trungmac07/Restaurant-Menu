﻿using System;
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
        List<DISH> list;
        Dictionary<KeyValuePair<string, int>, int> dic;
        public

        Client()
        {
            client = new TcpClient("127.0.0.1", 6969);
            stream = client.GetStream();
            sr = new StreamReader(stream);
            sw = new StreamWriter(stream);
            dic = new Dictionary<KeyValuePair<string, int>, int>();
      
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
        public string recvDescription()
        {
            string des = sr.ReadLine();
            return des;
        }
        public void recvPic(ref BitmapImage bi)
        {

            int n = Int32.Parse(sr.ReadLine());         //Reveice size of image
            //MessageBox.Show(n.ToString());
            byte[] buffer = new byte[n];
            
            stream.Read(buffer, 0, n);
            
            try
            {
                using (var Nstream = new MemoryStream(buffer, 0, n))
                {
                    Nstream.Position = 0;
                    try
                    {
                        bi.BeginInit();
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.StreamSource = Nstream;
                        bi.EndInit();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + " 123");
                    }
                    
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

         
        }


        public List<string> recvList()
        {
            List<string> list = new List<string>();

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

        public void sendRequest(string fullRequest)
        {
            string request = "";
            request += fullRequest[1];
            request += ' ';
            request += fullRequest.Remove(0, 3);
            //MessageBox.Show(request);
            sw.WriteLine(request);
            sw.Flush();
        }
        public void putinCart(DISH dish)
        {
            string name = dish.name;
            int price = dish.price;
            var idx = new KeyValuePair<string, int>(name, price);
            
            if(dic.ContainsKey(idx))
                dic[idx] += 1;
            else
                dic.Add(idx, 1);
            Console.WriteLine(dish.name + " " + dic[idx]);
            foreach (var x in dic)
                Console.WriteLine(x.ToString());
        }
        public void requestOrder()
        {
            sw.WriteLine("5");
            foreach(var x in dic)
            {
                sw.WriteLine(x.Key.Key);//ten mon
                sw.WriteLine(x.Key.Value);//gia
                sw.WriteLine(x.Value);//so luong
            }
        }
    }


    public class DISH
    {
        public string name { get; set; }
        public int price { get; set; }
    }
    class FOOD
    {
        public string name { get; set; }
        public List<DISH> foodList { get; set; }
    }
}