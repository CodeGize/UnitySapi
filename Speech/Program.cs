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

            //try
            //{
            //    var m_speecher = new Speecher();
            //    var m_recognizer = new Recognizer
            //    {
            //        OnRecognized = OnRecognized
            //    };
            //    Console.WriteLine("初始化完成");
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
            while (true)
            {
                var res = Console.ReadLine();
                if (res == "exit")
                    break;
            }
        }

        private static void OnRecognized(string text)
        {
            Console.WriteLine(text);
        }
    }
}
