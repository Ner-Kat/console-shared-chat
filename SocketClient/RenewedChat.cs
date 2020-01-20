using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SocketClient
{
    // Статический класс для работы с обновляемым текстом в консоли
    static class RenewedChat
    {
        private static int n = 0;
        private static string input = "";
        private static string inMsg = "> ";
        private static Object lockerFirst = new Object();
        private static Object lockerSec = new Object();
        private static int lastInputLength = 0;
        private static ClientObject obj = null;
        private static Thread gettingInput = null;

        private static String allowedSymbols =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz" +
            "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя" +
            "1234567890 `~!@#$%^&*()-_=+\"№;:?/|\\{}[]'.<>";

        public static void UpdWriteLine(string str)
        {
            if (obj != null && gettingInput != null)
            {
                lock (lockerFirst)
                {
                    Console.Write("\r");
                    Console.Write(str);

                    int dif = (lastInputLength + inMsg.Length) - str.Length;
                    if (dif > 0)
                        for (int i = 0; i < dif; i++)
                            Console.Write(" ");

                    Console.Write("\n");
                    Console.Write(inMsg + input);

                    if (lastInputLength > input.Length)
                        lastInputLength = input.Length;
                }
            }
            else
            {
                Console.WriteLine(str);
            }
        }

        private static void UpdInputLine()
        {
            lock (lockerSec)
            {
                Console.Write("\r");
                Console.Write(inMsg + input);

                int dif = lastInputLength - input.Length;
                if (dif > 0)
                    for (int i = 0; i < dif; i++)
                        Console.Write(" ");

                if (lastInputLength > input.Length)
                    lastInputLength = input.Length;

                Console.CursorLeft = input.Length + inMsg.Length;
            }
        }

        private static bool isTextChar(char c)
        {
            /*
            if ((c >= 33 && c <= 126) || (c >= 128 && c <= 175) || (c >= 224 && c <= 241) || c == 252)
                return true;
            */

            if (allowedSymbols.Contains(c))
                return true;

            return false;
        }

        private static void GetInput()
        {
            while (true)
            {
                while (true)
                {
                    ConsoleKeyInfo a = Console.ReadKey();

                    if (a.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                    else if (a.Key == ConsoleKey.Backspace)
                    {
                        if (input.Length >= 1)
                            input = input.Substring(0, input.Length - 1);

                        UpdInputLine();
                        continue;
                    }
                    else if (!isTextChar(a.KeyChar) && a.Key != ConsoleKey.Spacebar)
                        continue;

                    input += a.KeyChar;
                    lastInputLength++;
                }

                string toInput = input;
                input = "";
                UpdInputLine();
                obj.UpdateInput(toInput);
            }
        }

        public static void Start(ClientObject client)
        {
            if (obj == null)
            {
                obj = client;
                Console.Write(inMsg);
                gettingInput = new Thread(new ThreadStart(GetInput));
                gettingInput.Start();
            }
        }

        public static void Stop()
        {
            if (gettingInput != null)
                gettingInput.Abort();
            obj = null;
        }

    }
}
