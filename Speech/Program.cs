using System;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using Speech.Properties;

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

    public class NetServer : SocketExtra.INetComponent
    {
        private readonly Speecher m_speecher;

        private readonly SocketExtra m_socket;

        public NetServer()
        {
            m_speecher = new Speecher();
            m_socket = new SocketExtra(this);
        }

        public void StartServer()
        {
            m_socket.Bind("127.0.0.1", Settings.Default.Port);
        }

        public bool NetSendMsg(byte[] sendbuffer)
        {
            return true;
        }

        public bool NetReciveMsg(byte[] recivebuffer)
        {
            var str = Encoding.Default.GetString(recivebuffer);
            Console.WriteLine(str);
            m_speecher.Speak(str);
            return true;
        }

        public bool Connected { get { return m_socket.Connected; } }
    }

    public class Speecher
    {
        private readonly SpeechSynthesizer m_speaker;

        public Speecher()
        {
            m_speaker = new SpeechSynthesizer();
            var installs = m_speaker.GetInstalledVoices();

            m_speaker.Volume = Settings.Default.SpeakVolume;
            m_speaker.Rate = Settings.Default.SpeakRate;
            var voice = Settings.Default.SpeakVoice;

            var selected = false;
            if (!string.IsNullOrEmpty(voice))
            {
                if (installs.Any(install => install.VoiceInfo.Name == voice))
                {
                    m_speaker.SelectVoice(voice);
                    selected = true;
                }
            }
            if (!selected)
            {
                foreach (var install in installs.Where(install => install.VoiceInfo.Culture.Name == "en-US"))
                {
                    m_speaker.SelectVoice(install.VoiceInfo.Name);
                    break;
                }
            }
        }

        public void Speak(string msg)
        {
            m_speaker.Speak(msg);
        }
    }
}
