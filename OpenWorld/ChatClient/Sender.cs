using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatClient
{
    public class Sender
    {
        private SocketAsyncEventArgs socketArgs;
        private Socket socket;

        private UserToken token;

        public delegate void ReceiveMessageCallBack(string msg);
        public ReceiveMessageCallBack receiveMessageCallback;

        private object lockObj = new object();

        public void Start(string host, int port, int backlog)
        {
            // 소켓 객체 초기화
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress address = IPAddress.Parse(host);
            IPEndPoint endPoint = new IPEndPoint(address, port);

            // 소캣 bind 후 listen
            //socket.Bind(endPoint);

            //socket.Listen(backlog);

            socketArgs = new SocketAsyncEventArgs();
            socketArgs.Completed += new EventHandler<SocketAsyncEventArgs>(complete);
            socketArgs.RemoteEndPoint = endPoint;

            bool isComplete = false;
            isComplete = socket.ConnectAsync(socketArgs);
            if (!isComplete)
            {
                complete(null, socketArgs);
            }
        }

        private void complete(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success)
                return;

            token = new UserToken();
            token.socket = sender as Socket;
            token.receiveMessageCallback += DisplayText;
            token.ReceiveStart();

            //string test = "SocketAsyncEventArgs Test";
            //byte[] buffer = Encoding.UTF8.GetBytes(test);

            //Console.WriteLine("buffer Length : " + buffer.Length);

            //bool isComplete = false;

            //lock (lockObj)
            //{
            //    //SocketAsyncEventArgs sendEventArgs = new SocketAsyncEventArgs();
            //    //sendEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(ReceiveMsg);
            //    //sendEventArgs.SetBuffer(buffer, 0, buffer.Length);

            //    token = new UserToken();
            //    token.socket = sender as Socket;

            //    isComplete = socket.SendAsync(sendEventArgs);
            //    if (!isComplete)
            //    {
            //        Console.WriteLine(isComplete);
            //    }

            //    Thread.Sleep(50);
            //}
        }

        private void DisplayText(string msg)
        {
            receiveMessageCallback(msg);
        }

        public void SendMsg(string msg)
        {
            token.SendMsg(msg);
        }

        //private void ReceiveMsg(object sender, SocketAsyncEventArgs args)
        //{
        //    lock (lockObj)
        //    {
        //        byte[] buffer = new byte[4096];

        //        SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
        //        receiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(ReceiveComplete);
        //        receiveArgs.SetBuffer(buffer, 0, buffer.Length);

        //        socket.ReceiveAsync(receiveArgs);
        //    }
        //}

        //private void ReceiveComplete(object sender, SocketAsyncEventArgs args)
        //{
        //    if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
        //    {
        //        byte[] buffer = new byte[4096];
        //        Array.Copy(args.Buffer, 0, buffer, 0, args.BytesTransferred);
        //        string result = Encoding.UTF8.GetString(buffer);
        //        Console.WriteLine(result);

        //        //ReceiveMsg(null, args);
        //    }

        //}
    }
}
