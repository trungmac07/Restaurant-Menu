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
                string message = "Hello";

                int byteCount = Encoding.ASCII.GetByteCount(message + 1);
                byte[] sendData = new byte[byteCount];
                sendData = Encoding.ASCII.GetBytes(message);

                NetworkStream stream = client.GetStream();
                stream.Write(sendData, 0, sendData.Length);

                StreamReader sr = new StreamReader(stream);
                string response = sr.ReadLine();
                Console.WriteLine("Serer: " + response);

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