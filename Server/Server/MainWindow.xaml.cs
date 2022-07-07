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
using System.Net;




namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class Client
    {
        public TcpClient client;
        public NetworkStream stream;
        public StreamReader sr;
        public StreamWriter sw;
        public ORDER order;
        public DateTime timeClock;
        public Client()
        {
            order = null;
            timeClock = DateTime.Now;
        }

        public void setStream()
        {
            stream = client.GetStream();
            sr = new StreamReader(stream);
            sw = new StreamWriter(stream);
        }
    }


    public partial class MainWindow : Window
    {

        public bool isStart;
        public string[] DatabasePath = { "../../../MAIN_DISHES.json", "../../../SOUP.json", "../../../DESSERT.json", "../../../DRINKS.json" };

        int[] space = { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                        1, 1, 1, 1, 3, 3, 3, 2, 3, 3, 3, 3, 3, 3, 3, 3, 2, 4, 3, 3, 2, 3, 3, 3, 3, 3, 3, 4, 3, 3,
                        3, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 3, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3,
                        2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};

        void getMenuFromDatabase(string filePath, ref List<FOOD> menuList)
        {
            var jsonText = File.ReadAllText(filePath);
            menuList = JsonConvert.DeserializeObject<List<FOOD>>(jsonText);
        }

        void sendMenuToClient(Client client, List<FOOD> menuList)
        {
            if (menuList == null)
            {
                client.sw.WriteLine(0);
                client.sw.Flush();
                return;
            }
            client.sw.WriteLine(menuList.Count);
            client.sw.Flush();
            foreach (var menuItem in menuList)
            {
                client.sw.WriteLine(menuItem.name);
                client.sw.Flush();
                client.sw.WriteLine(menuItem.foodList.Count);
                client.sw.Flush();
                foreach (var item in menuItem.foodList)
                {

                    int count = 0;
                    for (int i = 0; i < item.name.Length; ++i)
                        count += space[item.name[i]];
                    string price = item.price.ToString();
                    for (int i = 0; i < price.Length; ++i)
                        count += space[price[i] - 48];

                    string s = item.name;
                    for (int i = 0; i < 61 - count; ++i)
                        s += '.';
                    s += price;
                    client.sw.WriteLine(s);
                    client.sw.Flush();
                }
            }
        }
        public bool isCardValid(string clientCard, int money)
        {
            List<BANK_CARD> cardList;
            var jsonText = File.ReadAllText("../../../BANK_CARD.json");
            cardList = JsonConvert.DeserializeObject<List<BANK_CARD>>(jsonText);

            foreach (var card in cardList)
            {
                if (card.cardNumber == clientCard)
                {
                    if (card.money < money)
                    {
                        return false;
                    }
                    else
                    {
                        card.money -= money;
                        File.WriteAllText("../../../BANK_CARD.json", string.Empty);
                        File.WriteAllText("../../../BANK_CARD.json", "[");
                        int count = 0;
                        foreach (var Card in cardList)
                        {
                            if (count != 0) File.AppendAllText("../../../BANK_CARD.json", ",");
                            File.AppendAllText("../../../BANK_CARD.json", JsonConvert.SerializeObject(Card));
                            count++;
                        }
                        File.AppendAllText("../../../BANK_CARD.json", "]");
                        return true;

                    }
                }
            }
            return false;
        }


        void exportOrderToDatabase(List<ORDER> orderList)
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

        void getOrderFromDatabase(ref List<ORDER> orderList)
        {
            var jsonText = File.ReadAllText("../../../ORDERS.json");
            orderList = JsonConvert.DeserializeObject<List<ORDER>>(jsonText);
        }

        void sendOrderToClient(Client client)
        {
            client.sw.WriteLine(client.order.id);
            client.sw.Flush();
            client.sw.WriteLine(client.order.dateTime);
            client.sw.Flush();
            client.sw.WriteLine(client.order.dishOrder.Count);
            client.sw.Flush();
            foreach (var dishItem in client.order.dishOrder)
            {
                client.sw.WriteLine(dishItem.dish.name);
                client.sw.Flush();
                client.sw.WriteLine(dishItem.dish.price);
                client.sw.Flush();
                client.sw.WriteLine(dishItem.numberOfDishes);
                client.sw.Flush();
                client.sw.WriteLine(dishItem.totalMoney);
                client.sw.Flush();
            }
            client.sw.WriteLine(client.order.totalMoney);
            client.sw.Flush();
            //client.sw.WriteLine(client.order.payment);
            //client.sw.Flush();
        }



        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        public void sendImageToClient(Client client, string link)
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(link);
            byte[] a = ImageToByteArray(img);
            Console.WriteLine(a.Length);
            int len = a.Length;
            client.sw.WriteLine(len); //send size of image for client to create a buffer
            client.sw.Flush();
            Console.WriteLine(a.Length);
            Console.WriteLine(len);
            client.stream.Write(a, 0, len);  //send bytes
            client.stream.Flush();
        }

        public void sendBackgroundAndMenu(Client client, string request)
        {
            //Backround
            sendImageToClient(client, "./Image/Background/" + request[0] + ".jpg");
            // Send menu list 
            List<FOOD> menuList = new List<FOOD>();

            getMenuFromDatabase(DatabasePath[Int32.Parse(new string(request[0], 1)) - 1], ref menuList);
            sendMenuToClient(client, menuList);
        }

        public void sendFoodImageAndDesciption(Client client, string request)
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
            sendImageToClient(client, path);

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
                        client.sw.WriteLine(dish.description);
                        client.sw.Flush();
                        isFound = true;
                        break;
                    }
                }
                if (isFound == true) break;
            }
        }
        public void getPayment(Client client, string request, ref ORDER order)
        {
            List<ORDER> orderList = new List<ORDER>();
            getOrderFromDatabase(ref orderList);
            if (request[2] == '1')
            {

                order.payment = "cash";
                order.bankCard = null;
                order.isPayed = true;

                client.sw.WriteLine("1");
                client.sw.Flush();
            }
            else if (request[2] == '0')
            {
                order.payment = "banking";
                order.bankCard = client.sr.ReadLine();
                if (isCardValid(order.bankCard, order.totalMoney) == false)
                {
                    order.isPayed = false;
                    client.sw.WriteLine("0");
                    client.sw.Flush();
                }
                else
                {
                    order.isPayed = true;
                    client.sw.WriteLine("1");
                    client.sw.Flush();
                }
            }
            foreach (ORDER Order in orderList)
            {
                if (Order.id == order.id)
                {
                    Order.payment = order.payment;
                    Order.bankCard = order.bankCard;
                    Order.isPayed = order.isPayed;
                    break;
                }
            }
            exportOrderToDatabase(orderList);
        }
        public void receiveNewOrder(ref Client client)
        {
            client.order = new ORDER();
            List<ORDER> orderList = new List<ORDER>();
            int numberOfDish = Int32.Parse(client.sr.ReadLine());
            getOrderFromDatabase(ref orderList);
            if (orderList != null)
            {
                Console.WriteLine(orderList.Count);
                client.order.id = "HKT#" + (orderList.Count + 1).ToString();
            }
            else
            {
                orderList = new List<ORDER>();
                client.order.id = "HKT#1";
            }

            client.order.dateTime = DateTime.Now;
            client.order.dishOrder = new List<DISH_ORDER>();
            Console.WriteLine(numberOfDish);
            for (int i = 0; i < numberOfDish; i++)
            {
                var newDish = new DISH_ORDER();
                newDish.dish = new DISH();
                newDish.dish.name = client.sr.ReadLine();
                Console.WriteLine(newDish.dish.name);
                newDish.dish.price = Int32.Parse(client.sr.ReadLine());
                Console.WriteLine(newDish.dish.price);
                newDish.numberOfDishes = Int32.Parse(client.sr.ReadLine());
                Console.WriteLine(newDish.numberOfDishes);
                newDish.totalMoney = newDish.numberOfDishes * newDish.dish.price;
                client.order.dishOrder.Add(newDish);
                client.order.totalMoney += newDish.totalMoney;
            }
            orderList.Add(client.order);
            exportOrderToDatabase(orderList);
            sendOrderToClient(client);


            //drawUI(order);

        }
        public void receiveExistedOrder(ref Client client)
        {
            List<ORDER> orderList = new List<ORDER>();
            int numberOfDish = Int32.Parse(client.sr.ReadLine());
            getOrderFromDatabase(ref orderList);

            Console.WriteLine(client.order.id);
            Console.WriteLine(numberOfDish);
            foreach (ORDER oRDER in orderList)
            {
                if (oRDER.id == client.order.id)
                {
                    int lastMoney = oRDER.totalMoney;
                    for (int i = 0; i < numberOfDish; i++)
                    {
                        var newDish = new DISH_ORDER();
                        newDish.dish = new DISH();
                        newDish.dish.name = client.sr.ReadLine();
                        Console.WriteLine(newDish.dish.name);
                        newDish.dish.price = Int32.Parse(client.sr.ReadLine());
                        Console.WriteLine(newDish.dish.price);
                        newDish.numberOfDishes = Int32.Parse(client.sr.ReadLine());
                        Console.WriteLine(newDish.numberOfDishes);
                        newDish.totalMoney = newDish.numberOfDishes * newDish.dish.price;
                        client.order.dishOrder.Add(newDish);
                        oRDER.dishOrder.Add(newDish);
                        client.order.totalMoney += newDish.totalMoney;
                        oRDER.totalMoney += newDish.totalMoney;
                    }
                    exportOrderToDatabase(orderList);
                    sendOrderToClient(client);
                    client.sw.WriteLine(lastMoney - client.order.totalMoney);
                    client.sw.Flush();
                    break;
                }
            }
            Console.WriteLine("Order sended");
            //drawUI(order);

        }
        bool drawColor = true;
        public void drawUI(ORDER dishOrder)
        {
            this.Dispatcher.Invoke(() =>
            {



                Border motherBorder = new Border();
                motherBorder.Width = 670;
                motherBorder.HorizontalAlignment = HorizontalAlignment.Center;
                motherBorder.BorderThickness = new Thickness(2, 2, 2, 2);
                motherBorder.BorderBrush = new SolidColorBrush(Colors.Black);

                DockPanel whole = new DockPanel();

                Button tableNum = new Button();
                tableNum.BorderThickness = new Thickness(0, 0, 2, 0);
                tableNum.Width = 70;
                tableNum.FontSize = 37;
                tableNum.Content = "7";

                StackPanel viewDish = new StackPanel();
                viewDish.Width = 500;
                if (drawColor == true)
                    whole.Background = new SolidColorBrush(Colors.LightBlue);

                whole.Children.Add(tableNum);

                foreach (var dish in dishOrder.dishOrder)
                {
                    DockPanel dock = new DockPanel();



                    TextBlock name = new TextBlock();
                    name.Height = 45;
                    name.Margin = new Thickness(45, 0, 0, 0);
                    name.Text = dish.dish.name + "  x" + dish.numberOfDishes;
                    name.FontSize = 23;

                    Button done = new Button();
                    done.Background = new SolidColorBrush(System.Windows.Media.Colors.LawnGreen);
                    done.Width = 45;
                    done.HorizontalAlignment = HorizontalAlignment.Right;
                    done.BorderThickness = new Thickness(0, 0, 0, 0);

                    try
                    {
                        ImageBrush imageBrush = new ImageBrush();
                        imageBrush.ImageSource = new BitmapImage(new Uri("./Image/tick.png", UriKind.Relative));
                        done.Background = imageBrush;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }


                    dock.Children.Add(name);
                    dock.Children.Add(done);

                    viewDish.Children.Add(dock);
                }



                whole.Children.Add(viewDish);

                motherBorder.Child = whole;

                menuArea.Children.Add(motherBorder);


                drawColor = true ^ drawColor;
            });
        }
        public void ClientLoop(Client client)
        {
            while (client.client.Connected)
            {
                try
                {
                    recvRequest(client);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Client has disconnected!");
                }
            }
        }


        public void ServerInit()
        {

            const int serverPort = 6969;
            TcpListener listener = new TcpListener(System.Net.IPAddress.Any, serverPort);
            listener.Start();

            List<Client> clientList = new List<Client>();

            while (true)
            {
                Console.WriteLine("Waiting for a connection.");
                Client customer = new Client();
                customer.client = listener.AcceptTcpClient();
                customer.setStream();
                Console.WriteLine("Client accepted");
                clientList.Add(customer);

                try
                {
                    Thread newThread = new Thread(o =>
                    {
                        ClientLoop(clientList[clientList.Count - 1]);
                        System.Windows.Threading.Dispatcher.Run();
                    });
                    newThread.SetApartmentState(ApartmentState.STA);
                    newThread.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Something went wrong.");
                }

            }
        }
        public bool getBillID(ref Client client, string billID, StreamWriter sw)
        {
            List<ORDER> orderList = new List<ORDER>();
            getOrderFromDatabase(ref orderList);
            foreach (ORDER Order in orderList)
            {
                if (Order.id == billID)
                {
                    sw.WriteLine("1");
                    sw.Flush();
                    client.order = Order;
                    Console.WriteLine(client.order.id);
                    DateTime timePast = DateTime.Now;
                    if (timePast.AddHours(-2) > client.order.dateTime)
                    {
                        client.timeClock = DateTime.Now;
                        client.sw.WriteLine("0");
                        client.sw.Flush();
                        return false;
                    }
                    client.sw.WriteLine("1");
                    client.sw.Flush();
                    //sendOrderToClient(client, order);
                    return true;
                }
            }
            sw.WriteLine("0");
            sw.Flush();
            return false;
        }
        public void recvRequest(Client client)
        {
            while (client.client.Connected)
            {
                try
                {
                    string request;
                    request = client.sr.ReadLine();
                    Console.WriteLine(request);
                    if (request[0] == '5')
                    {
                        if (client.order == null)
                        {
                            receiveNewOrder(ref client);
                        }
                        else
                        {
                            Console.WriteLine(client.order.id);
                            receiveExistedOrder(ref client);
                        }
                    }
                    else if (request[0] == '6')
                    {
                        getPayment(client, request, ref client.order);
                    }
                    else if (request[0] == '7')
                    {
                        string billID = client.sr.ReadLine();
                        getBillID(ref client, billID, client.sw);
                    }
                    else
                    {
                        if (request[2] == '0')
                            sendBackgroundAndMenu(client, request);
                        else
                            sendFoodImageAndDesciption(client, request);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Client has disconnected!");

                }
            }

        }

        public void Run(object sender, RoutedEventArgs e)
        {
            startButton.Visibility = Visibility.Collapsed;
            mainScreen.Background = null;
            if (isStart == false)
            {
                isStart = true;
                Thread mainThread = new Thread(o =>
                {
                    ServerInit();
                    System.Windows.Threading.Dispatcher.Run();
                });
                mainThread.SetApartmentState(ApartmentState.STA);
                //mainThread.SetApartmentState(ApartmentState.STA);
                mainThread.Start();
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
    public class FOOD
    {
        public string name { get; set; }
        public List<DISH> foodList { get; set; }
        public int num;
    }

    [Serializable]
    public class ORDER
    {
        public string id { get; set; }
        public System.DateTime dateTime { get; set; }
        public List<DISH_ORDER> dishOrder { get; set; }
        public int totalMoney { get; set; }
        public bool isPayed { get; set; }
        public string payment { get; set; }
        public string bankCard { get; set; }
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
