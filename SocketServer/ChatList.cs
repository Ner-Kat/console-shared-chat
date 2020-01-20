using System;
using System.Collections.Generic;
using System.Text;

namespace SocketServer
{
    // Класс глоабльного текстового чата
    public class ChatList
    {
        private List<ChatMessage> chat = new List<ChatMessage>();
        public int Count {
            get
            {
                return chat.Count;
            }
        }

        public ChatList()
        {
        }

        public List<string> GetMessages()
        {
            List<string> result = new List<string>();
            foreach (ChatMessage entry in chat)
            {
                result.Add(entry.message);
            }

            return result;
        }

        public List<string> GetNames()
        {
            List<string> result = new List<string>();
            foreach (ChatMessage entry in chat)
            {
                result.Add(entry.name);
            }

            return result;
        }


        public SortedSet<string> GetUniqueNames()
        {
            SortedSet<string> result = new SortedSet<string>();
            foreach (ChatMessage entry in chat)
            {
                result.Add(entry.name);
            }

            return result;
        }

        public bool Add(string name, string msg)
        {
            if (ChatMessage.CanBeName(name) && ChatMessage.CanBeMessage(msg))
            {
                chat.Add(new ChatMessage(name, msg));
                return true;
            }

            return false;
        }

        public ChatMessage Get(int index)
        {
            if (index > chat.Count)
                return null;

            return chat[index];
        }

        public ChatMessage this[int index]
        {
            get
            {
                return chat[index];
            }
            set
            {
                chat[index] = value;
            }
        }
    }
}
