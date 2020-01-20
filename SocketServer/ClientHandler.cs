using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace SocketServer
{
    // Класс, реализующий взаимодействие с одним клиентом на сервере
    public class ClientHandler
    {
        private TcpClient client;
        private string clientIP;
        private ChatList chat;
        private Object locker = new Object();
        private Object lockerGet = new Object();
        private Object lockerSend = new Object();
        private NetworkStream stream = null;
        private bool isWorking = false;

        public ChatUpdater Updater { get; set; }

        public bool IsWorking
        {
            get
            {
                return isWorking;
            }
        }

        public ClientHandler(TcpClient client, ChatList chat)
        {
            this.client = client;
            this.chat = chat;

            string ipWithPort = client.Client.RemoteEndPoint.ToString();
            clientIP = ipWithPort.Substring(0, ipWithPort.IndexOf(':'));

            Updater = null;
        }

        public string GetIP()
        {
            return client.Client.RemoteEndPoint.ToString();
        }

        private void SendMessage(string message)
        {
            lock (lockerSend)
            {
                byte[] buffer = Encoding.Unicode.GetBytes(message);
                stream.Write(buffer);
            }
        }

        private string GetMessage()
        {
            StringBuilder response = new StringBuilder();

            lock (lockerGet)
            {
                int size = 0;
                byte[] buffer = new byte[256];

                do
                {
                    size = stream.Read(buffer, 0, buffer.Length);
                    response.Append(Encoding.Unicode.GetString(buffer, 0, size));
                }
                while (stream.DataAvailable);
            }

            return response.ToString();
        }


        private void ClientHello()
        {
            // Ожидание приветствия
            string msg = GetMessage();

            // Отправка числа сообщений чата
            int count = chat.Count;
            int startIndex = count - 10;
            if (count < 10)
                startIndex = 0;
            SendMessage((count - startIndex).ToString());

            // Ожидание подтвеждения парсинга числа сообщений чата
            msg = GetMessage();
            if (msg == "/success")
            {
                // Отправка чата
                for (int i = startIndex; i < count; i++)
                {
                    SendMessage(chat[i].ToString());
                    if (GetMessage() == "/stop")
                        break;
                }
            }
        }

        public void SendUpdateChat()
        {
            SendMessage(chat[chat.Count - 1].ToString());
        }

        public void Handler()
        {
            isWorking = true;

            try
            {
                stream = client.GetStream();

                ClientHello();

                // Отправка сообщения о готовности к обмену
                SendMessage("/ready");

                while (true)
                {
                    // Приём сообщения
                    string getted = GetMessage();
                    if (getted == "/stop")
                        break;
                    else if (getted == null || getted == "")
                    {
                        // Либо что-то сломалось с getted, либо сервер получил пакет длины 0 => соединение разорвано
                        throw new Exception();
                    }
                    else
                    {
                        lock (locker)
                        {
                            Console.WriteLine(DateTime.Now.ToShortTimeString() + $" (от { GetIP() }): " + getted);
                            chat.Add(GetIP(), getted);
                            if (Updater != null)
                                Updater.Update();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Соединение с " + GetIP() + " разорвано");
                //Console.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();

                isWorking = false;
            }
        }

    }
}
