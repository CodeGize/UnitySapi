using System.Collections;
using System.Diagnostics;
using System.Text;
using UnityEngine;

public class Speecher : MonoBehaviour, SocketExtra.INetComponent
{
    private SocketExtra m_socket;
    private Process m_process;

    protected void Awake()
    {
        Ins = this;
        m_process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "speech.exe",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            },
        };
        m_process.Start();
    }

    /***测试代码，可删除Start***/
    protected IEnumerator Start()
    {
        yield return StartCoroutine(Connect());
        Speak("test");
    }
    /***测试代码，可删除End***/

    public IEnumerator Connect()
    {
        m_socket = new SocketExtra(this);
        m_socket.Connect("127.0.0.1", 9903);
        while (!m_socket.Connected)
        {
            yield return 1;
        }
    }

    protected void OnDestroy()
    {
        if (m_process != null && !m_process.HasExited)
            m_process.Kill();
        m_process = null;
    }

    public static Speecher Ins;

    public static void Speak(string str)
    {
#if UNITY_EDITOR||UNITY_STANDALONE_WIN
        Ins.Speech(str);
#endif
    }

    public void Speech(string str)
    {
        if (m_socket.Connected)
        {
            var bytes = Encoding.Default.GetBytes(str);
            m_socket.SendMsg(bytes);
        }
    }

    public bool NetReciveMsg(byte[] recivebuffer)
    {
        return true;
    }

    public bool NetSendMsg(byte[] sendbuffer)
    {
        return true;
    }
}