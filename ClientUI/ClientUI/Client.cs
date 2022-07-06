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
        List<DISH> list;
       
        public Dictionary<KeyValuePair<string, int>, int> dic;
        public ORDER order;
        public
        Client()
        {
            client = new TcpClient("127.0.0.1", 6969);
            stream = client.GetStream();
            sr = new StreamReader(stream);
            sw = new StreamWriter(stream);
            dic = new Dictionary<KeyValuePair<string, int>, int>();
            order = new ORDER();
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
        public void putinCart(DISH dish)
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
            order.dateTime = sr.ReadLine();
            order.numofDishOrders = Int32.Parse(sr.ReadLine());
            for (int i = 0; i < order.numofDishOrders; i++)
            {
                DISH_ORDER a = new DISH_ORDER();
                a.dish = new DISH();
                a.dish.name = sr.ReadLine();
                a.dish.price = Int32.Parse(sr.ReadLine());
                a.numberOfDishes = Int32.Parse(sr.ReadLine());
                a.totalMoney = Int32.Parse(sr.ReadLine());
                if (order.dishOrder == null)
                    order.dishOrder = new List<DISH_ORDER>();
                order.dishOrder.Add(a);
            }
            order.totalMoney = Int32.Parse(sr.ReadLine());
        }
        public void sendPayMent(string type, string bankID)
        {
            sw.WriteLine("6 " + type);
            sw.Flush();
            if (type == "0")
            {
                sw.WriteLine(bankID);
                sw.Flush();
            }
            afterPayment();
            
        }
        public void afterPayment()
        {
         
            string recv = sr.ReadLine();

            MessageBox.Show(recv);
            if (recv == "1")
            {
                MessageBox.Show("Successfully paid your bill");
            }
            else
            {
                MessageBox.Show("Fail to pay your bill");
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
            if (sr.ReadLine() == "1")
                return true;
            else
                return false;
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