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
                    response = sr.ReadLine();
                    Console.WriteLine(response);
                }
                
                Console.WriteLine(sr.ReadLine());
                //Read and Write food name 
                Console.Write(sr.ReadLine());
                string message = Console.ReadLine();
                sw.WriteLine(message);
                sw.Flush();
                // Read and Write number of food
                Console.Write(sr.ReadLine());
                message = Console.ReadLine();
                sw.WriteLine(message);
                sw.Flush();
                // Reading order
                Console.WriteLine(sr.ReadLine());
                Console.WriteLine(sr.ReadLine());
                Console.WriteLine(sr.ReadLine());

                stream.Close();
                client.Close();
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