using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;

public class Y2Client
{

    private const int BUFFER_SIZE = 1024;
    private const int PORT_NUMBER = 6969;

    static ASCIIEncoding encoding = new ASCIIEncoding();

    public static void Main()
    {

        try
        {
            TcpClient client = new TcpClient();

            // 1. connect
            client.Connect("127.0.0.1", PORT_NUMBER);
            Stream stream = client.GetStream();

            string str = Console.ReadLine();

            // 2. send
            byte[] data = encoding.GetBytes(str);

            stream.Write(data, 0, data.Length);

            // 3. receive
            data = new byte[BUFFER_SIZE];
            stream.Read(data, 0, BUFFER_SIZE);

            Console.WriteLine(encoding.GetString(data));

            // 4. Close
            stream.Close();
            client.Close();
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }

        Console.Read();
    }
}