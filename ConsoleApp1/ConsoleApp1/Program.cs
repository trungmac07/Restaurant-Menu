using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace SERVER
{
    class Program
    {
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
                return;
            }
            
            foreach (var menuItem in menuList)
            {
                //sw.WriteLine(menuItem.name);
                sw.WriteLine(menuItem.foodList.Count);
                foreach (var item in menuItem.foodList)
                {
                    sw.WriteLine(item.name + "...." + item.price);
                }
            }
            sw.Flush();

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

        static void Main(string[] args)
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
                    byte[] a = File.ReadAllBytes("C:/Users/Trung/Desktop/1.jpg"); //Images Bytes
                    //sw.WriteLine("----------   MENU  ----------");
                    List<FOOD> menuList = new List<FOOD>();
                    List<ORDER> orderList = new List<ORDER>();
                    getMenuFromDatabase("../../../SOUP.json", ref menuList);
                    //sendMenuToClient(sw, menuList);         comment lai cho de^~ sua? anh?
                    
                    int len = (int)a.Length;           
                  
                   
                    //byte[] sender = BitConverter.GetBytes(len);
                    sw.WriteLine(len); //send size of image for client to create a buffer
                    sw.Flush();
                    Console.WriteLine(a.Length);  
                    //stream.Position = 0;
                    stream.Write(a,0,a.Length);  //send bytes
                   

                    byte[] g = File.ReadAllBytes("E:/Code/Project/Socket-Project/SOCKET-PROJECT/ClientUI/ClientUI/bin/Debug/net6.0-windows/a.jpg");

                    bool check = false;

                    for(int i = 0; i < g.Length; i++)
                    {
                        if (g[i] != a[i])
                        {
                            Console.WriteLine("{0}...{1}", g[i], a[i]);
                            check = true;
                        }
                    }
                    if (check == false)
                        Console.WriteLine("ALL CORRECT");
                    Console.WriteLine("{0}    {1}", g.Length, a.Length);                    
                    //exportOrderToDatabase(order);
                    getOrderFromDatabase(ref orderList);
                    sendOrderToClient(sw, orderList, "Nguyen Cao Khoi");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Something went wrong.");
                    sw.WriteLine(e.ToString());
                }
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