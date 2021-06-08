using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServer
{
    public class Listener
    {
        //private SocketAsyncEventArgs receiveArgs;
        //private SocketAsyncEventArgs sendArgs;

        private SocketAsyncEventArgs socketArgs;
        private Socket socket;

        private AutoResetEvent auto;

        UserToken token;

        private int count;

        private object lockObj = new object();

        public void ListenStart(string host, int port, int backlog)
        {
            byte[] buffer = new byte[4096];
            SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
            sendArgs.SetBuffer(buffer, 0, buffer.Length);

            SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
            receiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(ReceiveComplete);
            receiveArgs.SetBuffer(buffer, 0, buffer.Length);

            auto = new AutoResetEvent(false);
            token = new UserToken();

            token.SetSocketArgs(receiveArgs, sendArgs);



            // 소켓 객체 초기화
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress address = IPAddress.Parse(host);
            IPEndPoint endPoint = new IPEndPoint(address, port);

            // 소캣 bind 후 listen
            socket.Bind(endPoint);
            socket.Listen(backlog);

            // 수신할 이벤특 객체
            socketArgs = new SocketAsyncEventArgs();
            socketArgs.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptComplete);

            Thread thread = new Thread(DoListen);
            thread.Start();
        }

        private void DoListen()
        {
            // 계속 수신
            while (true)
            {
                bool isComplete = true;
                socketArgs.AcceptSocket = null;
                isComplete = socket.AcceptAsync(socketArgs);
                if (!isComplete)
                {
                    AcceptComplete(null, socketArgs);
                }

                auto.WaitOne();
            }
        }

        private void AcceptComplete(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success)
                throw new Exception("args SocketError not Success");


            token.socket = args.AcceptSocket;
            ReceiveMsg(token.socket);
            SendMsg(token.socket);

            auto.Set();
        }   
        
        private void SendMsg(Socket clientSocket)
        {
            string text = "Connect Server";
            byte[] buffer = Encoding.UTF8.GetBytes(text);

            bool isComplete = true;

            lock(lockObj)
            {
                token.sendArgs.SetBuffer(buffer, 0, buffer.Length);
                isComplete = clientSocket.SendAsync(token.sendArgs);
                if (!isComplete)
                {

                }
            }
        }

        private void ReceiveMsg(Socket clientSocket)
        {
            lock (lockObj)
            {
                clientSocket.ReceiveAsync(token.receiveArgs);
            }
        }

        private void ReceiveComplete(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
            {
                byte[] buffer = new byte[4096];
                Array.Copy(args.Buffer, 0, buffer, 0, args.BytesTransferred);
                string result = Encoding.UTF8.GetString(buffer);
                Console.WriteLine(result);

                ReceiveMsg(token.socket);
            }
        }
    }
}
