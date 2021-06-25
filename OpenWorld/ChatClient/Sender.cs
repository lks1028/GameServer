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

        public delegate void ConnectDoneCallBack();
        public ConnectDoneCallBack connectDoneCallBack;

        public delegate void SettingIDDoneCallBack();
        public SettingIDDoneCallBack settingIDDoneCallBack;

        // 룸 리스트 정보를 가져올때 사용하는 콜백
        public delegate void SettingRoomListCallBack(string msg);
        public SettingRoomListCallBack settingRoomListCallback;

        // 룸을 생성한 후콜백
        public delegate void CreateRoomCallBack(string msg);
        public CreateRoomCallBack createRoomCallback;

        private readonly object lockObj = new object();

        public void Start(string host, int port, int backlog)
        {
            // 소켓 객체 초기화
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress address = IPAddress.Parse(host);
            IPEndPoint endPoint = new IPEndPoint(address, port);

            socketArgs = new SocketAsyncEventArgs();
            socketArgs.Completed += new EventHandler<SocketAsyncEventArgs>(ConnetCompleted);
            socketArgs.RemoteEndPoint = endPoint;

            bool isComplete = false;
            isComplete = socket.ConnectAsync(socketArgs);
            if (!isComplete)
            {
                ConnetCompleted(null, socketArgs);
            }
        }

        private void ConnetCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success)
                return;

            token = new UserToken();
            token.socket = sender as Socket;
            token.receiveMessageCallback += DisplayText;
            token.settingIDCallback += SettingIDCallBack;
            token.settingRoomCallback += SettingRoomCallBack;
            token.createRoomCallback += CreateRoom;
            token.ReceiveStart();

            connectDoneCallBack();
        }

        private void DisplayText(string msg)
        {
            receiveMessageCallback(msg);
        }

        public void SendPacket(COMMAND command, PacketMaker packet)
        {
            token.SendPacket(command, packet);
        }

        private void SettingIDCallBack()
        {
            settingIDDoneCallBack();
        }

        private void SettingRoomCallBack(string msg)
        {
            settingRoomListCallback(msg);
        }

        private void CreateRoom(string msg)
        {
            createRoomCallback(msg);
        }
    }
}
