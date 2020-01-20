using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketClient
{
    // Класс, реализующий работу сокета клиента
    public class ClientObject
    {
        private TcpClient client;
        private string toSend;
        private NetworkStream stream = null;
        private Object locker = new Object();
        private Object lockerSend = new Object();
        private bool ready = false;

        public ClientObject(TcpClient client)
        {
            this.client = client;
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

            lock (locker)
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


        private void ServerHello()
        {
            // Отправка приветствия
            SendMessage("hello");

            // Получение числа сообщений чата
            string helloResponse = GetMessage();
            int count = 0;
            try
            {
                count = int.Parse(helloResponse);
                SendMessage("/success");
            }
            catch (Exception ex)
            {
                Console.WriteLine("\tНе удалось загрузить данные чата: " + ex.Message);
                SendMessage("/stop");
            }

            // Получение чата
            for (int i = 0; i < count; i++)
            {
                string msg = GetMessage();
                Console.WriteLine("\t" + msg);
                SendMessage("Получение подтверждено");
            }
        }

        public void UpdateInput(string msg)
        {
            if (msg == "/stop")
            {
                //ready = false;
                stream.Close();
                client.Close();
                return;
            }

            toSend = msg;
            Thread sending = new Thread(new ThreadStart(SendInput));
            sending.Start();
        }

        public void SendInput()
        {
            SendMessage(toSend);
        }

        public void Sender()
        {
            try
            {
                stream = client.GetStream();

                ServerHello();

                // Проверка готовности к обмену сообщениями
                ready = (GetMessage() == "/ready");

                RenewedChat.Start(this);

                while (ready)
                {
                    string message = GetMessage();
                    RenewedChat.UpdWriteLine(message);
                }
            }
            catch (Exception ex)
            {
                RenewedChat.Stop();
                Console.WriteLine("Соединение с сервером разорвано");
                //Console.WriteLine(ex.Message);
            }
            finally
            {
                RenewedChat.Stop();

                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }

    }
}
