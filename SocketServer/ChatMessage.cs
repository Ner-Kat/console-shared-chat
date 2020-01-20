using System;
using System.Collections.Generic;
using System.Text;

namespace SocketServer
{
    // Класс, описывающий одно сообщение чата
    public class ChatMessage
    {
        public readonly string message = "неизвестное сообщение";
        public readonly string name = "неизвестное имя";

        public static bool CanBeName(string name)
        {
            return (name != null && !name.Equals(""));
        }

        public static bool CanBeMessage(string message)
        {
            return (message != null && !message.Equals(""));
        }

        public ChatMessage(string name, string message)
        {
            if (CanBeName(name) && CanBeMessage(message))
            {
                this.name = name;
                this.message = message;
            }
        }

        public override string ToString()
        {
            return (name + ": " + message);
        }
    }
}
