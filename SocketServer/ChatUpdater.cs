using System;
using System.Collections.Generic;
using System.Text;

namespace SocketServer
{
    // Класс, реализующий список всех клиентов и метод вызова обновления чата на них
    public class ChatUpdater
    {
        private List<ClientHandler> clients;
        private Object locker = new Object();

        public ChatUpdater(List<ClientHandler> clients)
        {
            this.clients = clients;
        }

        public void Update()
        {
            lock (locker)
            {
                foreach (ClientHandler client in clients)
                    if (client.IsWorking)
                        client.SendUpdateChat();
            }
        }
    }
}
