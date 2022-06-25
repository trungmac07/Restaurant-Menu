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
            sw.WriteLine(menuList.Count);
            foreach (var menuItem in menuList)
            {
                sw.WriteLine(menuItem.name);
                sw.WriteLine(menuItem.foodList.Count);
                foreach (var item in menuItem.foodList)
                {
                    sw.WriteLine(item.name);
                    sw.WriteLine(item.price);
                }
            }
            sw.Flush();

        }

        static void Main(string[] args)
        {
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
                    sw.WriteLine("----------   MENU  ----------");
                    List<FOOD> menuList = new List<FOOD>();
                    getMenuFromDatabase("../../../SOUP.json", ref menuList);
                    sendMenuToClient(sw, menuList);
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