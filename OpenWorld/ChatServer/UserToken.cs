using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class UserToken
    {
        // Connet된 소켓 객체
        public Socket socket;
        // 수신할 이벤트 객체
        public SocketAsyncEventArgs receiveArgs;
        // 송신할 이벤트 객체
        public SocketAsyncEventArgs sendArgs;

        public void SetSocketArgs(SocketAsyncEventArgs receiveArgs, SocketAsyncEventArgs sendArgs)
        {
            this.receiveArgs = receiveArgs;
            this.sendArgs = sendArgs;
        }
    }
}
