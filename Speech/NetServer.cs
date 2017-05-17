using System;
using Speech.Properties;
using Stardust;

namespace Speech
{
    public class NetServer : INetComponent
    {
        private Speecher m_speecher;
        private Recognizer m_recognizer;
        private readonly SocketServer m_socket;

        public NetServer()
        {
            m_socket = new SocketServer(this);
        }

        public void Init()
        {
            m_speecher = new Speecher();
            m_recognizer = new Recognizer
            {
                OnRecognized = OnRecognized
            };
            Console.WriteLine("初始化完成");
        }

        private void OnRecognized(string text)
        {
            var arg = new ByteInArg();
            arg.Write(text);
            NetSendMsg(arg.GetBuffer());
        }

        public void StartServer()
        {
            m_socket.OnConnecte = OnClientConnecte;
            m_socket.Bind("127.0.0.1", Settings.Default.Port);
        }

        private int m_clientid;
        private void OnClientConnecte(int cid)
        {
            m_clientid = cid;
            Console.WriteLine("已连接客户端:" + cid);
        }

        public bool NetSendMsg(byte[] sendbuffer)
        {
            return m_socket.SendMsg(sendbuffer, m_clientid);
        }

        public bool NetReciveMsg(byte[] recivebuffer, int netID)
        {
            var arg = new ByteOutArg(recivebuffer);
            var cmd = arg.ReadInt32();
            switch ((EmCmd)cmd)
            {
                case EmCmd.Init:
                    Init();
                    break;
                case EmCmd.Speak:
                    var speakcmd= arg.ReadInt32();
                    if (speakcmd == 0)
                    {
                        var str = arg.ReadString();
                        Console.WriteLine(str);
                        m_speecher.Speak(str);
                    }
                    else if (speakcmd == 1)
                    {
                        m_speecher.Pause();
                    }
                    else if(speakcmd==2)
                    {
                        m_speecher.Resume();
                    }
                    return true;
                case EmCmd.Recognize:
                    var scmd = arg.ReadInt32();
                    if (scmd == 1)
                        m_recognizer.BeginRec();
                    if (scmd == 0)
                        m_recognizer.EndRec();
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return false;
        }

        public bool Connected
        {
            get { return m_socket.Connected; }
        }
    }
}