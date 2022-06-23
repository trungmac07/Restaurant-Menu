using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace SERVER
{
    class Program
    {
        static void getMenuFromDatabase(string filePath, StreamWriter sw, ref List<Menu> menuList)
        {
            var jsonText = File.ReadAllText(filePath);
            menuList = JsonConvert.DeserializeObject<List<Menu>>(jsonText);
            if (menuList == null)
            {
                sw.WriteLine(0);
                return;
            }
            sw.WriteLine(menuList.Count);
            foreach (var menuItem in menuList)
            {
                sw.WriteLine(menuItem.name + "             " + menuItem.price);
            }
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
                    List<Menu> menuList = new List<Menu>();
                    getMenuFromDatabase("../../../Foods.json", sw,ref menuList);
                    sw.WriteLine("----- Your order: ");
                    sw.WriteLine("----- Food name: ");
                    sw.Flush();
                    var foodName = sr.ReadLine();
                    Console.WriteLine("Food name: " + foodName);
                    sw.WriteLine("----- How many?: ");
                    sw.Flush();
                    var number = Int32.Parse(sr.ReadLine());
                    Console.WriteLine("Food number: " + number);
                    sw.WriteLine("-----------------------------");
                    sw.WriteLine("----- Your order is: " + number + " of " + foodName);
                    sw.Flush();
                    int totalMoney = 0;
                    foreach(Menu menuItem in menuList)
                    {
                        if (menuItem.name == foodName)
                            totalMoney = number * menuItem.price;
                    }
                    sw.WriteLine("----- Total money: " + totalMoney);
                    sw.Flush();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Something went wrong.");
                    sw.WriteLine(e.ToString());
                }
            }
        }
    }

    [Serializable]
    class Menu
    {
        public string name { get; set; }
        public int price { get; set; }
    }
}