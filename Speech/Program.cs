using System;
using System.Speech.Synthesis;

namespace Speech
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new NetServer();
            server.StartServer();

            while (true)
            {
                var res = Console.ReadLine();
                if (res == "exit")
                    break;
            }
        }
    }
}
