﻿using System;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


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

        public int totalAll = 0;
        public string id;
        public Dictionary<KeyValuePair<string, int>, int> dic;
        public Dictionary<KeyValuePair<string, int>, ImageBrush> pic;
        public List <ORDER> order;

        public
        Client()
        {
            id = "";
            client = new TcpClient("127.0.0.1", 6969);
            stream = client.GetStream();
            sr = new StreamReader(stream);
            sw = new StreamWriter(stream);
            dic = new Dictionary<KeyValuePair<string, int>, int>();
            pic = new Dictionary<KeyValuePair<string, int>, ImageBrush>();
            order = new List<ORDER>();
            
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
            string response = sr.ReadLine();

            var numberOfFood = Int32.Parse(response);
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
                        MessageBox.Show(ex.Message);
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
        public void putinCart(DISH dish, ImageBrush bi)
        {
            string name = dish.name;
            int price = dish.price;
            var idx = new KeyValuePair<string, int>(name, price);
        
            if (dic.ContainsKey(idx))
                dic[idx] += 1;
            else
                dic.Add(idx, 1);
            Console.WriteLine(dish.name + " " + dic[idx]);
            foreach (var x in dic)
                Console.WriteLine(x.ToString());

            if (!pic.ContainsKey(idx))
                pic.Add(idx, bi);

        }
        public void requestOrder()
        {
            MessageBox.Show("Order sended");
            sw.WriteLine("5");//yeu cau order
            sw.Flush();
            sw.WriteLine(dic.Count);// so luong cua x trong dic 
            sw.Flush();
            foreach (var x in dic)
            {
                sw.WriteLine(x.Key.Key);//ten mon
                sw.Flush();
                sw.WriteLine(x.Key.Value);//gia
                sw.Flush();
                sw.WriteLine(x.Value);//so luong
                sw.Flush();
            }
        }
        public void recvBill()
        {
            ORDER newOrder = new ORDER();
            newOrder.id = sr.ReadLine();
            if (id == "")
                id = newOrder.id;
            newOrder.dateTime = sr.ReadLine();
            var x = sr.ReadLine();
            
            newOrder.numofDishOrders = Int32.Parse(x);
            for (int i = 0; i < newOrder.numofDishOrders; i++)
            {
                DISH_ORDER a = new DISH_ORDER();
                a.dish = new DISH();
                a.dish.name = sr.ReadLine();
                a.dish.price = Int32.Parse(sr.ReadLine());
                a.numberOfDishes = Int32.Parse(sr.ReadLine());
                a.totalMoney = Int32.Parse(sr.ReadLine());
                if (newOrder.dishOrder == null)
                    newOrder.dishOrder = new List<DISH_ORDER>();
                newOrder.dishOrder.Add(a);
            }

            totalAll = Int32.Parse(sr.ReadLine());
            newOrder.totalMoney = Int32.Parse(sr.ReadLine());
            
            
            order.Add(newOrder);
        }

        public void updateOrder()
        {
            
        }

        public int totalMoneyAllBill()
        {
            int sum = 0;
            for(int i = 0; i < order.Count; i++)
                sum += order[i].totalMoney;
            return sum;
        }

        public bool sendPayMent(string type, string bankID)
        {
            sw.WriteLine("6 " + type);
            sw.Flush();
            if (type == "0")
            {
                sw.WriteLine(bankID);
                sw.Flush();
            }
            return afterPayment();
            
        }
        public bool afterPayment()
        {
         
            string recv = sr.ReadLine();

        
            if (recv == "1")
            {
                MessageBox.Show("Successfully paid your bill");
                return true;
            }
            else
            {
                MessageBox.Show("Fail to pay your bill");
                return false;
            }
        }
        public void sendBillID(string str)
        {
            sw.WriteLine("7");//Cho server biet la dang send bill ID
            sw.Flush();
            sw.WriteLine(str);
            sw.Flush();
        }
        public bool recvBillID()
        {
            if (sr.ReadLine() == "0")
            {
                MessageBox.Show("Can't find your bill");
                return false;
            }
            else
            {
                if (sr.ReadLine() == "0")
                {
                    MessageBox.Show("Your bill is out of date");
                    return false;
                }
                else return true;
            }
        }
    }


    public class DISH
    {
        public string name { get; set; }
        public int price { get; set; }
    }
    public class DISH_ORDER
    {
        public DISH dish { get; set; }
        public int numberOfDishes { get; set; }
        public int totalMoney { get; set; }
        public DISH_ORDER()
        {
            totalMoney = 0;
        }
    }
    public class ORDER
    {
        public string id { get; set; }
        public string clientName { get; set; }
        public string dateTime { get; set; }
        public List<DISH_ORDER> dishOrder { get; set; }
        public int numofDishOrders { get; set; }
        public int totalMoney { get; set; }

        public ORDER()
        {
            totalMoney = 0;
        }

    }
}