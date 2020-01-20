using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using SocketServer;
using System.Threading;
using System.Collections.Generic;

namespace SocketTcpServer
{
    // Класс серверного приложения (главный класс сервера)
    class ServerProgram
    {
        // Данные для подключения
        static int port = 11479;
        static TcpListener server;
        static ChatUpdater updater;
        static Object locker = new Object();

        static void RefreshClients(List<ClientHandler> clients)
        {
            lock (locker)
            {
                foreach (ClientHandler client in clients.ToArray())     // ToArray, т.к. "Collection was modified; enumeration operation may not execute"
                {
                    if (!client.IsWorking)
                        clients.Remove(client);
                }
            }
        }

        static void Main(string[] args)
        {
            try
            {
                // Хост по умолчанию
                IPAddress hostIp = IPAddress.Parse("127.0.0.1");

                // Генерация списка ip-адресов активных подключений
                Console.WriteLine("Выберите сетевое подключение, которое будет слушать сервер:");
                String hostName = Dns.GetHostName();
                IPAddress[] ipArray = Dns.GetHostAddresses(hostName);
                int k = 1;
                foreach (IPAddress ip in ipArray)
                {
                    Console.WriteLine(k + ". " + ip.ToString());
                    k++;
                }

                // Выбор пользователем ip-адреса подключения, которое и будет прослушивать сервер
                int addressesSize = ipArray.Length;
                while (true)
                {
                    Console.Write("> ");
                    try
                    {
                        int ch = Int32.Parse(Console.ReadLine());
                        if (ch > 0 && ch < addressesSize)
                        {
                            hostIp = ipArray[ch - 1];
                            break;
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        Console.WriteLine("Неверный ввод");
                    }
                }

                Console.Clear();

                // Запуск сервера
                IPEndPoint ipPoint = new IPEndPoint(hostIp, port);     
                server = new TcpListener(ipPoint);
                server.Start();

                ChatList chat = new ChatList();

                Console.WriteLine("Сервер запущен и слушает адрес " + hostIp.ToString() + ":" + port);

                List<ClientHandler> clients = new List<ClientHandler>();
                updater = new ChatUpdater(clients);

                // Основной рабочий цикл сервера
                bool isServerWorking = true;
                while (isServerWorking)
                {
                    TcpClient client = server.AcceptTcpClient();
                    RefreshClients(clients);
                    ClientHandler clientHandler = new ClientHandler(client, chat);
                    clients.Add(clientHandler);
                    clientHandler.Updater = updater;

                    Console.WriteLine("Подключён клиент: " + clientHandler.GetIP());

                    Thread clientThread = new Thread(new ThreadStart(clientHandler.Handler));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (server != null)
                    server.Stop();
            }
        }   // ENDOF static void Main

    }   // ENDOF class ServerProgram
}
