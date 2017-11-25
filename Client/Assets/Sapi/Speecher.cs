using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Stardust;
using UnityEngine;
using UnityEngine.Events;

public class Speecher : MonoBehaviour, INetComponent
{
    private SocketClient m_socket;
    private Process m_process;

    [Header("是否显示Speech.exe")]
    public bool ServerDisplay;

    protected void Awake()
    {
        Ins = this;
        m_process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "speech.exe",
                CreateNoWindow = !ServerDisplay,
                WindowStyle = ServerDisplay ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden
            },
        };
        m_process.Start();
    }

    /// <summary>
    /// 开始或者结束语音识别功能
    /// </summary>
    /// <param name="tf"></param>
    public void Recognize(bool tf)
    {
        var arg = new ByteInArg();
        arg.Write(3);
        arg.Write(tf ? 1 : 0);
        NetSendMsg(arg.GetBuffer());
    }

    public IEnumerator Connect()
    {
        m_socket = new SocketClient(this);
        m_socket.Connect("127.0.0.1", 9903);
        while (!m_socket.Connected)
        {
            yield return 1;
        }
    }

    public IEnumerator InitServer()
    {
        var arg = new ByteInArg();
        arg.Write(1);
        NetSendMsg(arg.GetBuffer());
        yield return 1;
    }

    protected void OnDestroy()
    {
        if (m_process != null && !m_process.HasExited)
        {
            var res = m_process.CloseMainWindow();
            if (!res)
                //m_process.Close();
                m_process.Kill();
        }
        m_process = null;
    }

    public static Speecher Ins { get; private set; }

    //    public void Speak(string str)
    //    {
    //#if UNITY_EDITOR||UNITY_STANDALONE_WIN
    //        Ins.Speech(str);
    //#endif
    //    }

    public void Speech(string str)
    {
        if (m_socket.Connected)
        {
            var arg = new ByteInArg();
            arg.Write(2);
            arg.Write(0);
            arg.Write(str);
            //var bytes = Encoding.Default.GetBytes(str);
            NetSendMsg(arg.GetBuffer());
        }
    }

    public bool NetReciveMsg(byte[] recivebuffer, int netID)
    {
        var arg = new ByteOutArg(recivebuffer);
        var str = arg.ReadString();
        ResList.Enqueue(str);
        //UnityEngine.Debug.Log(str);
        return true;
    }

    public Queue<string> ResList = new Queue<string>();

    public void Update()
    {
        if (ResList.Count > 0)
        {
            var str = ResList.Dequeue();
            if (OnGetRecognize != null)
                OnGetRecognize.Invoke(str);
        }
    }

    public bool NetSendMsg(byte[] sendbuffer)
    {
        return m_socket.SendMsg(sendbuffer);
    }

    public delegate void GetRecognize(string msg);
    public GetRecognize OnGetRecognize;

    public UnityEvent<string> OnGetRecognizeAct;

    public IEnumerator Pause()
    {
        var arg = new ByteInArg();
        arg.Write(2);
        arg.Write(1);
        NetSendMsg(arg.GetBuffer());
        yield return 1;
    }

    public IEnumerator Resume()
    {
        var arg = new ByteInArg();
        arg.Write(2);
        arg.Write(2);
        NetSendMsg(arg.GetBuffer());
        yield return 1;
    }
}