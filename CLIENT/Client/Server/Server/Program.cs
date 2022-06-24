using System;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(System.Net.IPAddress.Any, 6969);
            listener.Start();
            while (true)
            {
                Console.WriteLine("Waiting for a connection.");
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client accepted.");
                NetworkStream stream = client.GetStream();
                StreamReader sr = new StreamReader(stream);
                StreamWriter sw = new StreamWriter(stream);
                try
                {
                    string response = sr.ReadLine();
                    Console.WriteLine(response);
                    sw.WriteLine("Hi ban");
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
}