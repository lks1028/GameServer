using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    class UserToken
    {
        // Connet된 소켓 객체
        public Socket socket;
        // 수신할 이벤트 객체
        private SocketAsyncEventArgs receiveArgs;
        // 송신할 이벤트 객체
        private SocketAsyncEventArgs sendArgs;

        // 메세지를 받으면 화면에 표시해주기 위한 콜백
        public delegate void ReceiveMessageCallBack(string msg);
        public ReceiveMessageCallBack receiveMessageCallback;

        public UserToken()
        {
            byte[] buffer = new byte[4096];

            receiveArgs = new SocketAsyncEventArgs();
            receiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(ReceivecComplete);
            receiveArgs.SetBuffer(buffer, 0, buffer.Length);

            sendArgs = new SocketAsyncEventArgs();
        }

        public void ReceiveStart()
        {
            bool isComplete = true;

            isComplete = socket.ReceiveAsync(receiveArgs);
            if (!isComplete)
            {
                ReceivecComplete(socket, receiveArgs);
            }
        }

        private void ReceivecComplete(object sender, SocketAsyncEventArgs args)
        {
            if (args.LastOperation == SocketAsyncOperation.Receive && args.BytesTransferred > 0)
            {
                bool isComplete = true;

                Socket socket = sender as Socket;
                receiveArgs.AcceptSocket = null;

                // 받은 데이터를 다른 클라들에게 보내자
                byte[] buffer = new byte[4096];
                Array.Copy(args.Buffer, 0, buffer, 0, args.BytesTransferred);
                string msg = Encoding.UTF8.GetString(buffer);

                receiveMessageCallback(msg);

                isComplete = socket.ReceiveAsync(receiveArgs);
                if (!isComplete)
                {
                    ReceivecComplete(socket, receiveArgs);
                }
            }
        }

        public void SendMsg(string msg)
        {
            byte[] lengthBuffer = BitConverter.GetBytes(msg.Length);
            byte[] msgBuffer = Encoding.UTF8.GetBytes(msg);
            byte[] buffer = new byte[lengthBuffer.Length + msgBuffer.Length];
            Array.Copy(lengthBuffer, 0, buffer, 0, lengthBuffer.Length);
            Array.Copy(msgBuffer, 0, buffer, lengthBuffer.Length, msgBuffer.Length);

            sendArgs.AcceptSocket = null;
            sendArgs.SetBuffer(buffer, 0, buffer.Length);
            socket.SendAsync(sendArgs);

            //byte[] buffer = Encoding.UTF8.GetBytes(msg);
            //sendArgs.AcceptSocket = null;
            //sendArgs.SetBuffer(buffer, 0, buffer.Length);
            //socket.SendAsync(sendArgs);
        }
    }
}
