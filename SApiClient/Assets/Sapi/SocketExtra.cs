using System;
using System.Net;
using System.Net.Sockets;

public class SocketExtra
{
    /// <summary>
    /// 是否需要策略文件
    /// </summary>
    public SocketExtra(INetComponent netComponent)
    {
        m_net = netComponent;
        m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        m_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        m_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
    }

    private Socket m_socket;
    private INetComponent m_net;


    public string Error { get; private set; }

    public void Connect(string ip, int port)
    {
        m_socket.BeginConnect(ip, port, OnConnectCallBack, m_socket);
    }

    private void OnConnectCallBack(IAsyncResult ar)
    {
        if (m_socket != null && m_socket.Connected)
        {
            StartListen(m_socket);
        }
        else
        {
            Error = "Connect faild";
        }
    }

    private void StartListen(Socket socket)
    {
        var state = new StateObject { WorkSocket = socket };
        if (socket != null && socket.Connected)
            socket.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            var state = (StateObject)ar.AsyncState;
            var client = state.WorkSocket;
            if (!client.Connected)
                return;
            var bytesRead = client.EndReceive(ar);
            if (bytesRead > 0)
            {
                var len = state.Result == null ? 0 : state.Result.Length;
                var res = new byte[len + bytesRead];
                if (state.Result != null)
                    Array.Copy(state.Result, 0, res, 0, len);
                Array.Copy(state.Buffer, 0, res, len, bytesRead);
                state.Result = res;
                if (bytesRead <= StateObject.BufferSize)
                {
                    m_net.NetReciveMsg(state.Result);
                    state.Result = null;
                }
                if (client.Connected)
                    client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
            }
            else
            {
                Disconnect("Socket Disconnect:bytesRead <= 0");
            }
        }
        catch (Exception e)
        {
            Disconnect("Socket Disconnect:" + e.Message);
            Error = e.Message;
        }
    }

    public void Disconnect(string msg)
    {
        DisConnectReason = msg;
        if (m_socket != null && m_socket.Connected)
        {
            try
            {
                m_socket.Shutdown(SocketShutdown.Both);
                m_socket.Close();
            }
            catch (Exception)
            {
                m_socket = null;
            }
        }
    }

    public void Bind(string ip, int port)
    {
        var ipadr = IPAddress.Parse(ip);
        var ipend = new IPEndPoint(ipadr, port);
        m_socket.Bind(ipend);
        m_socket.Listen(0);
        m_socket.BeginAccept(OnClientConnecte, m_socket);

    }

    private void OnClientConnecte(IAsyncResult ar)
    {
        var socket = (Socket)ar.AsyncState;
        var client = socket.EndAccept(ar);
        Console.WriteLine("连接成功");
        StartListen(client);
    }

    public void Destroy()
    {
        Disconnect("Socket Disconnect:Destroy");
        m_socket = null;
        m_net = null;
    }

    public string DisConnectReason { get; private set; }

    public bool SendMsg(byte[] buffer)
    {
        try
        {
            return m_socket != null && m_socket.Send(buffer) > 0;
        }
        catch (Exception e)
        {
            Error = e.Message;
            m_socket = null;
            return false;
        }
    }

    public bool Connected
    {
        get
        {
            if (m_socket == null)
                return false;
            return m_socket.Connected;
        }
    }

    public class StateObject
    {
        public Socket WorkSocket;

        public const int BufferSize = 1024;
        public byte[] Buffer = new byte[BufferSize];
        public byte[] Result;
    }

    public interface INetComponent
    {
        bool NetReciveMsg(byte[] recivebuffer);
        bool NetSendMsg(byte[] sendbuffer);
    }
}