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

        private UserManager userManager;

        private AutoResetEvent auto;

        // 서버 로그 콜백
        public delegate void ServerLogCallBack(string msg);
        public ServerLogCallBack serverLogCallback;

        // 메세지 콜백
        public delegate void ReceiveMessageCallBack(string msg);
        public ReceiveMessageCallBack receiveMessageCallback;

        public void ListenStart(string host, int port, int backlog)
        {
            auto = new AutoResetEvent(false);
            userManager = new UserManager();

            // 소켓 객체 초기화
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress address = IPAddress.Parse(host);
            IPEndPoint endPoint = new IPEndPoint(address, port);

            // 소캣 bind 후 listen
            socket.Bind(endPoint);
            socket.Listen(backlog);

            serverLogCallback("Start Chat Server");

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


            // 토큰 정보 셋팅
            UserToken token = new UserToken();
            token.socket = args.AcceptSocket;
            token.ReceiveStart();
            token.SendMsg("Server Connet");
            token.SetUserManager(userManager);
            token.receiveMessageCallback += DisplayText;


            // 유저 컨트롤 매니저에 토큰 등록
            userManager.AddUserToken(token);

            serverLogCallback("Client Accept");
            serverLogCallback($"Total Client Count : {userManager.GetTokenListCount()}");

            auto.Set();
        }   

        private void DisplayText(string msg)
        {
            receiveMessageCallback(msg);
        }
    }
}
