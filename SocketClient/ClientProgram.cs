using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using SocketClient;

namespace SocketClient
{
    // Класс клиентского приложения (главный класс клиента)
    class ClientProgram
    {
        // Данные для подключения
        static string ip = "127.0.0.1";
        static int port = 11479;

        static void Main(string[] args)
        {
            try
            {
                while (true)
                {
                    // Ввод IP сервера
                    Console.Write("Введите сервер (оставить пустым, default или 0 для выбора стандартного сервера): ");
                    string serverIP = Console.ReadLine();

                    // Установка IP сервера
                    IPEndPoint ipPoint = null;
                    if (serverIP == "default" || serverIP == "0" || serverIP == "")
                        ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                    else
                    {
                        try
                        {
                            ipPoint = new IPEndPoint(IPAddress.Parse(serverIP), port);
                        }
                        catch
                        {
                            ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                        }
                    }

                    // Попытка подключения к серверу
                    TcpClient client = new TcpClient();
                    client.Connect(ipPoint);
                    Console.WriteLine("Подключение к " + ipPoint.ToString());

                    // Инициализация клиентского приложения
                    ClientObject clientObject = new ClientObject(client);
                    clientObject.Sender();
                    client.Close();

                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine(ex.Message);
            }

            // Console.Read();
        }

    }
}
