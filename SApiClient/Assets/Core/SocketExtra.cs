using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Stardust
{
    public interface INetComponent
    {
        bool NetReciveMsg(byte[] recivebuffer, int netID);
    }

    public class SocketClient : SocketBase
    {
        public SocketClient(INetComponent netComponent) : base(netComponent)
        {
        }

        public void Connect(string ip, int port)
        {
            Socket.BeginConnect(ip, port, OnConnectCallBack, Socket);
        }

        private void OnConnectCallBack(IAsyncResult ar)
        {
            if (Socket != null && Socket.Connected)
            {
                StartListen(Socket);
            }
            else
            {
                Error = "Connect faild";
            }
        }

        public bool SendMsg(byte[] buffer)
        {
            return SendMsg(buffer, Socket);
        }
    }
    public class SocketServer : SocketBase
    {
        public SocketServer(INetComponent netComponent) : base(netComponent)
        {
        }

        public void Bind(string ip, int port)
        {
            var ipadr = IPAddress.Parse(ip);
            var ipend = new IPEndPoint(ipadr, port);
            Socket.Bind(ipend);
            Socket.Listen(0);
            Socket.BeginAccept(OnClientConnecte, Socket);
        }

        private readonly Dictionary<int, Socket> m_clients = new Dictionary<int, Socket>();

        public delegate void OnConnecteEvent(int cid);

        public OnConnecteEvent OnConnecte;

        private void OnClientConnecte(IAsyncResult ar)
        {
            var socket = (Socket)ar.AsyncState;
            var client = socket.EndAccept(ar);
            var id = StartListen(client);
            m_clients.Add(id, client);
            if (OnConnecte != null)
                OnConnecte(id);
        }

        public bool SendMsg(byte[] buffer, int cid)
        {
            var client = m_clients[cid];
            return SendMsg(buffer, client);
        }

        protected override void OnDisconnect()
        {
            base.OnDisconnect();
            OnConnecte = null;
        }
    }

    public abstract class SocketBase
    {
        private INetComponent m_net;
        protected Socket Socket { get; private set; }

        protected SocketBase(INetComponent netComponent)
        {
            m_net = netComponent;
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
        }

        private int m_idIndex;

        protected int StartListen(Socket socket)
        {
            var res = m_idIndex;
            m_idIndex++;
            var state = new StateObject { WorkSocket = socket, NetID = m_idIndex };
            if (socket != null && socket.Connected)
                socket.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
            return res;
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var state = (StateObject)ar.AsyncState;
                var socket = state.WorkSocket;
                if (!socket.Connected)
                    return;
                var bytesRead = socket.EndReceive(ar);
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
                        m_net.NetReciveMsg(state.Result, state.NetID);
                        state.Result = null;
                    }
                    if (socket.Connected)
                        socket.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
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

        public string DisConnectReason { get; private set; }

        public void Disconnect(string msg)
        {
            OnDisconnect();
            DisConnectReason = msg;
            if (Socket != null && Socket.Connected)
            {
                try
                {
                    Socket.Shutdown(SocketShutdown.Both);
                    Socket.Close();
                }
                catch (Exception)
                {
                    Socket = null;
                }
            }
        }

        protected virtual void OnDisconnect()
        {
        }

        public void Destroy()
        {
            Disconnect("Socket Disconnect:Destroy");
            Socket = null;
            m_net = null;
        }

        protected bool SendMsg(byte[] buffer, Socket socket)
        {
            try
            {
                return socket != null && socket.Send(buffer) > 0;
            }
            catch (Exception e)
            {
                Error = e.Message;
                return false;
            }
        }

        public string Error { get; protected set; }

        public bool Connected
        {
            get
            {
                if (Socket == null)
                    return false;
                return Socket.Connected;
            }
        }

        private class StateObject
        {
            public Socket WorkSocket;

            public const int BufferSize = 1024;
            public readonly byte[] Buffer = new byte[BufferSize];
            public byte[] Result;

            public int NetID;
        }
    }
}