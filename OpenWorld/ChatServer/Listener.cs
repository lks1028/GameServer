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
        private SocketAsyncEventArgs socketArgs;
        private Socket socket;

        public void ListenStart(string host, int port, int backlog)
        {
            // 소켓 객체 초기화
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress address = IPAddress.Parse(host);
            IPEndPoint endPoint = new IPEndPoint(address, port);

            // 소캣 bind 후 listen
            socket.Bind(endPoint);
            socket.Listen(backlog);

            socketArgs = new SocketAsyncEventArgs();
            socketArgs.Completed += new EventHandler<SocketAsyncEventArgs>(complete);

            Thread thread = new Thread(DoListen);
            thread.Start();
        }

        private void DoListen()
        {
            bool isComplete = false;
            isComplete = socket.AcceptAsync(socketArgs);
            if (!isComplete)
            {
                complete(null, socketArgs);
            }
        }

        private void complete(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success)
                return;

            bool isComplete = false;

            byte[] buffer = new byte[4096];

            Socket clientSocket = args.AcceptSocket;
            SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
            receiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(receiveComplete);
            receiveArgs.SetBuffer(buffer, 0, 1024);
            isComplete = clientSocket.ReceiveAsync(receiveArgs);
            if (!isComplete)
            {
                receiveComplete(null, args);
            }
        }

        private void receiveComplete(object sender, SocketAsyncEventArgs args)
        {
            if (args.LastOperation != SocketAsyncOperation.Receive)
                return;

            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                Console.WriteLine("buffer Length : " + args.BytesTransferred);
                byte[] buffer = new byte[1024];
                Array.Copy(args.Buffer, 0, buffer, 0, args.BytesTransferred);

                string test = Encoding.UTF8.GetString(buffer);
                Console.WriteLine(test);
            }
        }
    }
}
