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
        public static string[] DatabasePath = { "../../../MAIN_DISHES.json", "../../../SOUP.json", "../../../DESSERT.json", "../../../DRINKS.json" };
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
            sw.WriteLine(menuList.Count);
            sw.Flush();
            foreach (var menuItem in menuList)
            {
                sw.WriteLine(menuItem.name);
                sw.Flush();
                sw.WriteLine(menuItem.foodList.Count);
                sw.Flush();
                foreach (var item in menuItem.foodList)
                {
                    string s = item.name;
                    Font font1 = new Font("SVN-Bali Script", 18);
                    SizeF stringSize = new SizeF();
                    using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(new Bitmap(1, 1)))
                    {
                        stringSize = graphics.MeasureString(s, font1);
                    }
                    while (stringSize.Width < 360)
                    {
                        s += '.';
                        using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(new Bitmap(1, 1)))
                        {
                            stringSize = graphics.MeasureString(s + item.price, font1);
                        }
                        if (stringSize.Width >= 360) break;
                    }
                    s += item.price;
                    sw.WriteLine(s);
                    sw.Flush();
                }
            }
            //sw.Flush();

        }
        public bool isCardValid(BANK_CARD clientCard)
        {
            List<BANK_CARD> cardList;
            var jsonText = File.ReadAllText("../../../BANK_CARD.json");
            cardList = JsonConvert.DeserializeObject<List<BANK_CARD>>(jsonText);

            foreach (var card in cardList)
            {
                if (card.cardNumber == clientCard.cardNumber)
                {
                    if (card.money <= 0)
                    {
                        return false;
                    }
                    else return true;
                }
            }
            return false;
        }
        static void exportOrderToDatabase(List<ORDER> orderList)
        {
            File.WriteAllText("../../../ORDERS.json", string.Empty);
            File.WriteAllText("../../../ORDERS.json", "[");
            int count = 0;
            foreach (var order in orderList)
            {
                if (count != 0) File.AppendAllText("../../../ORDERS.json", ",");
                File.AppendAllText("../../../ORDERS.json", JsonConvert.SerializeObject(order));
                count++;
            }
            File.AppendAllText("../../../ORDERS.json", "]");
        }
        static void getOrderFromDatabase(ref List<ORDER> orderList)
        {
            var jsonText = File.ReadAllText("../../../ORDERS.json");
            orderList = JsonConvert.DeserializeObject<List<ORDER>>(jsonText);
        }
        static void sendOrderToClient(StreamWriter sw, ORDER order)
        {
            sw.WriteLine(order.dateTime);
            sw.Flush();
            sw.WriteLine(order.dishOrder.Count);
            sw.Flush();
            foreach (var dishItem in order.dishOrder)
            {
                sw.WriteLine(dishItem.dish.name);
                sw.Flush();
                sw.WriteLine(dishItem.dish.price);
                sw.Flush();
                sw.WriteLine(dishItem.numberOfDishes);
                sw.Flush();
                sw.WriteLine(dishItem.totalMoney);
                sw.Flush();
            }
            sw.WriteLine(order.totalMoney);
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

        public static void sendImageToClient(StreamWriter sw, NetworkStream stream, string link)
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(link);
            byte[] a = ImageToByteArray(img);
            Console.WriteLine(a.Length);
            int len = a.Length;
            sw.WriteLine(len); //send size of image for client to create a buffer
            sw.Flush();
            Console.WriteLine(a.Length);
            Console.WriteLine(len);
            stream.Write(a, 0, len);  //send bytes
            stream.Flush();
        }

        public static void sendBackgroundAndMenu(StreamWriter sw, NetworkStream stream, string request)
        {
            //Backround
            sendImageToClient(sw, stream, "./Image/Background/" + request[0] + ".jpg");
            // Send menu list 
            List<FOOD> menuList = new List<FOOD>();

            getMenuFromDatabase(DatabasePath[Int32.Parse(new string(request[0], 1)) - 1], ref menuList);
            sendMenuToClient(sw, menuList);
        }

        public static void sendFoodImageAndDesciption(StreamWriter sw, NetworkStream stream, string request)
        {
            //Food
            string path;
            int index;
            if (request.Length == 3)
            {
                path = "./Image/Food/" + request[0] + "/" + request[0] + "." + request[2] + ".jpg";
                index = Int32.Parse(request[2].ToString());
            }
            else
            {
                path = "./Image/Food/" + request[0] + "/" + request[0] + "." + request[2] + request[3] + ".jpg";
                index = Int32.Parse(request[2].ToString()) * 10 + Int32.Parse(request[3].ToString());
            }
            Console.WriteLine(path);
            sendImageToClient(sw, stream, path);

            List<FOOD> menuList = new List<FOOD>();
            getMenuFromDatabase(DatabasePath[Int32.Parse(new string(request[0], 1)) - 1], ref menuList);
            int count = 0;
            bool isFound = false;
            foreach (FOOD item in menuList)
            {
                foreach (DISH dish in item.foodList)
                {
                    count++;
                    if (count == index)
                    {
                        sw.WriteLine(dish.description);
                        sw.Flush();
                        isFound = true;
                        break;
                    }
                }
                if (isFound == true) break;
            }
        }

        
        public static void receiveOrder(StreamReader sr, StreamWriter sw)
        {
            ORDER order = new ORDER();
            List<ORDER> orderList = new List<ORDER>();
            int numberOfDish = Int32.Parse(sr.ReadLine());
            getOrderFromDatabase(ref orderList);
            if (orderList != null)
            {
                Console.WriteLine(orderList.Count);
                order.id = (orderList.Count + 1).ToString();
            }
            else
            {
                orderList = new List<ORDER>();
                order.id = "1";
            }

            order.clientName = "Trung map djt";
            order.dateTime = DateTime.Now;
            order.dishOrder = new List<DISH_ORDER>();
            Console.WriteLine(numberOfDish);
            for (int i = 0; i < numberOfDish; i++)
            {
                var newDish = new DISH_ORDER();
                newDish.dish = new DISH();
                newDish.dish.name = sr.ReadLine();
                Console.WriteLine(newDish.dish.name);
                newDish.dish.price = Int32.Parse(sr.ReadLine());
                Console.WriteLine(newDish.dish.price);
                newDish.numberOfDishes = Int32.Parse(sr.ReadLine());
                Console.WriteLine(newDish.numberOfDishes);
                newDish.totalMoney = newDish.numberOfDishes * newDish.dish.price;
                order.dishOrder.Add(newDish);
                order.totalMoney += newDish.totalMoney;
            }
            orderList.Add(order);
            exportOrderToDatabase(orderList);
            sendOrderToClient(sw, order);
          
        }
        public static void ClientLoop(TcpClient client, StreamReader sr, StreamWriter sw, NetworkStream stream)
        {
            while (client.Connected)
            {
                try
                {
                    recvRequest(sr, sw, stream);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Client has disconnected!");
                }
            }
        }
        public static void ServerInit()
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
                    Thread newThread = new Thread(() => ClientLoop(client, sr, sw, stream));
                    newThread.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Something went wrong.");
                }

            }
        }
        public static void recvRequest(StreamReader sr, StreamWriter sw, NetworkStream stream)
        {
            string request;
            request = sr.ReadLine();
            Console.WriteLine(request);
            if (request[0] == '5')
            { 
                receiveOrder(sr, sw);
               
            }
            else
            {
                if (request[2] == '0')
                    sendBackgroundAndMenu(sw, stream, request);
                else
                    sendFoodImageAndDesciption(sw, stream, request);
            }
            //sendPicAndMenu(sw, stream, request);

        }
        public void Run(object sender, RoutedEventArgs e)
        {
            startButton.Visibility = Visibility.Collapsed;
            mainScreen.Background = null;
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
        public string description { get; set; }
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
        public int num;
    }

    [Serializable]
    class ORDER
    {
        public string id { get; set; }
        public string clientName { get; set; }
        public DateTime dateTime { get; set; }
        public List<DISH_ORDER> dishOrder { get; set; }
        public int totalMoney { get; set; }
        public bool isPayed { get; set; }
        public ORDER()
        {
            totalMoney = 0;
            isPayed = false;
        }

    }
    [Serializable]
    public class BANK_CARD
    {
        public string cardNumber { get; set; }
        public int money { get; set; }

    }
}
