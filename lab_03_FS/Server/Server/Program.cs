using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
class Server
{
    static void Main(string[] args)
    {
        int port = 8888;
        string dataDir = $"./server/data/"; 

        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
 
        listener.Bind(localEndPoint);
        listener.Listen(10);

        Console.WriteLine("Server started!");

        while (true)
        {
            Socket handler = listener.Accept();
            byte[] data = new byte[256];
            int bytes = handler.Receive(data);
            string request = Encoding.UTF8.GetString(data, 0, bytes);
            string[] requestData = request.Split(' ');

            string response;

            if (requestData[0].Equals("exit"))
            {
                response = "Server stopped!";
                byte[] exitResponse = Encoding.UTF8.GetBytes(response);
                handler.Send(exitResponse);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                break;
            }

            else if (requestData[0].Equals("PUT"))
            {
                string fileName = requestData[1];
                string fileContent = requestData[2];

                string filePath = $"./server/data/{fileName}";
                if (File.Exists(filePath))
                {
                    File.WriteAllText(filePath, fileContent);
                    response = "403"; // Файл создан 
                }

                else
                {
                    File.WriteAllText(filePath, fileContent);
                    response = "200"; // Файл создан 
                }
            }

            else if (requestData[0].Equals("GET"))
            {
                string fileName = requestData[1];
                string filePath = Path.Combine(dataDir, fileName);

                if (File.Exists(filePath))
                {
                    string fileContent = File.ReadAllText(filePath);
                    response = "200 " + fileContent; // Файл существует
                }

                else
                {
                    response = "404"; // Файл не найден
                }
            }

            else if (requestData[0].Equals("DELETE"))
            {
                string fileName = requestData[1];
                string filePath = Path.Combine(dataDir, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    response = "200"; // Файл удален
                }

                else
                {
                    response = "404"; // Файл не найден
                }
            }

            else
            {
                response = "400"; // Неправильный запрос
            }

            byte[] responseData = Encoding.UTF8.GetBytes(response);
            handler.Send(responseData);
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
    }
}
