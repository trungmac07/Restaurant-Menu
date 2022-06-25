using System;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            const string clientIP = "127.0.0.1";
            const int clientPort = 6969;

        connection:
            try
            {
                TcpClient client = new TcpClient(clientIP, clientPort);
                NetworkStream stream = client.GetStream();

                StreamReader sr = new StreamReader(stream);
                StreamWriter sw = new StreamWriter(stream); 

                var response = sr.ReadLine();
                Console.WriteLine(response);
                var numberOfFood = Int32.Parse(sr.ReadLine());
                for (int i = 0; i < numberOfFood; i++)
                {
                    Console.WriteLine();
                    response = sr.ReadLine();
                    Console.WriteLine(response);
                    Console.WriteLine("----------");
                    var numberOfDish = Int32.Parse(sr.ReadLine());
                    for (int j = 0; j < numberOfDish; j++)
                    {
                        response = sr.ReadLine();
                        Console.WriteLine(response);
                        response = sr.ReadLine();
                        Console.WriteLine(response);
                    }
                }
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to connect.");
                goto connection;
            }
        }
    }
}