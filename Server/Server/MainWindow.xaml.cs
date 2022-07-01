using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Net.Sockets;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;


namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool isStart;
        static void getMenuFromDatabase(string filePath, ref List<FOOD> menuList)
        {
            var jsonText = File.ReadAllText(filePath);
            menuList = JsonConvert.DeserializeObject<List<FOOD>>(jsonText);
        }
        static void sendMenuToClient(StreamWriter sw, List<FOOD> menuList)
        {
            if (menuList == null)
            {
                sw.WriteLine(0);
                sw.Flush();
                return;
            }

            foreach (var menuItem in menuList)
            {
                //sw.WriteLine(menuItem.name);
                sw.WriteLine(menuItem.foodList.Count);
                sw.Flush();
                foreach (var item in menuItem.foodList)
                {
                    sw.WriteLine(item.name + "...." + item.price);
                    sw.Flush();
                }
            }
            //sw.Flush();

        }

        static void exportOrderToDatabase(ORDER order)
        {
            File.AppendAllText("../../../ORDERS.json", JsonConvert.SerializeObject(order));
        }
        static void getOrderFromDatabase(ref List<ORDER> orderList)
        {
            var jsonText = File.ReadAllText("../../../ORDERS.json");
            orderList = JsonConvert.DeserializeObject<List<ORDER>>(jsonText);
        }
        static void sendOrderToClient(StreamWriter sw, List<ORDER> orderList, string userName)
        {
            if (orderList == null)
                return;
            foreach (var item in orderList)
            {
                if (item.clientName == userName)
                {
                    sw.WriteLine(item.dateTime);
                    sw.WriteLine(item.dishOrder.Count);
                    foreach (var dishItem in item.dishOrder)
                    {
                        sw.WriteLine(dishItem.dish.name);
                        sw.WriteLine(dishItem.dish.price);
                        sw.WriteLine(dishItem.numberOfDishes);
                        sw.WriteLine(dishItem.totalMoney);
                    }
                    sw.WriteLine(item.totalMoney);
                }
            }
            sw.Flush();
        }

        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        public static void sendPic(StreamWriter sw, NetworkStream stream, string request)
        {
            if (request[2] == '0')
            {
                //Backround
                System.Drawing.Image img = System.Drawing.Image.FromFile("./Image/Background/" + request[0] + ".jpg");
                byte[] a = ImageToByteArray(img);

                int len = a.Length;
                sw.WriteLine(len); //send size of image for client to create a buffer
                sw.Flush();
                Console.WriteLine(a.Length);
                Console.WriteLine(len);

                stream.Write(a, 0, len);  //send bytes
                stream.Flush();
            }
            else
            {
                //Food
                System.Drawing.Image img;
                if (request.Length == 3)
                {
                    img = System.Drawing.Image.FromFile("./Image/Food/" + request[0] + "." + request[2] + ".jpg");
                }
                else
                {
                    img = System.Drawing.Image.FromFile("./Image/Food/" + request[0] + "." + request[2] + request[3] + ".jpg");
                }
                    
                byte[] a = ImageToByteArray(img);

                int len = a.Length;
                sw.WriteLine(len); //send size of image for client to create a buffer
                sw.Flush();
                Console.WriteLine(a.Length);
                Console.WriteLine(len);

                stream.Write(a, 0, len);  //send bytes
                stream.Flush();
            }

            /*List<FOOD> menuList = new List<FOOD>();
            List<ORDER> orderList = new List<ORDER>();
            getMenuFromDatabase("../../../SOUP.json", ref menuList);
            sendMenuToClient(sw, menuList);*/
        }

        public static void ServerInit()
        {
            var order = new ORDER
            {
                clientName = "Nguyen Cao Khoi",
                dateTime = new DateTime(2022, 3, 29),
                dishOrder = new List<DISH_ORDER>
                {
                    new DISH_ORDER
                    {
                        dish = new DISH
                        {
                            name = "Chicken Pho",
                            price = 120000
                        },
                        numberOfDishes = 5,
                        totalMoney = 5 * 120000
                    },
                    new DISH_ORDER
                    {
                        dish = new DISH
                        {
                            name = "Vegan Noodles",
                            price = 200000
                        },
                        numberOfDishes = 2,
                        totalMoney = 2 * 200000
                    }
                },
                totalMoney = 5 * 120000 + 2 * 200000
            };

            const int serverPort = 6969;
            TcpListener listener = new TcpListener(System.Net.IPAddress.Any, serverPort);
            listener.Start();

            while (true)
            {
                Console.WriteLine("Waiting for a connection.");
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client accepted");
                NetworkStream stream = client.GetStream();
                StreamReader sr = new StreamReader(client.GetStream());
                StreamWriter sw = new StreamWriter(client.GetStream());
                try
                {

                    recvRequest(sr);
                    //exportOrderToDatabase(order);
                    //getOrderFromDatabase(ref orderList);
                    //sendOrderToClient(sw, orderList, "Nguyen Cao Khoi");

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Something went wrong.");
                    sw.WriteLine(ex.ToString());
                    sw.Flush();
                }

            }
        }
        public static void recvRequest(StreamReader sr)
        {
            string request = "";
            request = sr.ReadLine();

            //Khoi nhan cai request r gui anh trong ham nay luon nhe ng ae
        }
        public void Run(object sender, RoutedEventArgs e)
        {
            if (isStart == false)
            {
                Thread mainThread = new Thread(ServerInit);
                mainThread.Start();
                isStart = true;
            }
            else
            {
                MessageBox.Show("Server has started, stop pressing the button bro ???? ");
            }
        } 
        public MainWindow()
        {
            isStart = false;
            InitializeComponent();
            
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

    }

    [Serializable]
    class FOOD
    {
        public string name { get; set; }
        public List<DISH> foodList { get; set; }
    }

    [Serializable]
    class ORDER
    {
        public string clientName { get; set; }
        public DateTime dateTime { get; set; }
        public List<DISH_ORDER> dishOrder { get; set; }
        public int totalMoney { get; set; }

    }
}
