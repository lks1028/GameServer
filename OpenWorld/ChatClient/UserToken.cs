using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClient
{
    class Defines
    {
        public static readonly short HEADERSIZE = 4;
        public static readonly short COMMAND = 4;
    }

    public enum COMMAND : int
    {
        SERVER_CONNECTED = 1,

        SET_USER_ID,
        SET_USER_ID_DONE,

        CREATE_ROOM,
        CREATE_ROOM_DONE,

        SEND_CHAT_MSG,
        RECEIVE_CHAT_MSG,

        SERVER_DISCONNECTED
    }

    class UserToken
    {
        // Connet된 소켓 객체
        public Socket socket;
        // 수신할 이벤트 객체
        private SocketAsyncEventArgs receiveArgs;
        // 송신할 이벤트 객체
        private SocketAsyncEventArgs sendArgs;

        // 읽을 전체 바이트 수
        private int totalByteLength = 0;
        // 현재 읽은 바이트 수
        private int currentByteLength = 0;
        // 남은 바이트 수
        private int remainByteLength = 0;
        // 저장할 버퍼
        private byte[] receiveBuffers = new byte[4096];

        public string userID = string.Empty;

        // 메세지를 받으면 화면에 표시해주기 위한 콜백
        public delegate void ReceiveMessageCallBack(string msg);
        public ReceiveMessageCallBack receiveMessageCallback;

        // 아이디 설정이 됐으면 보내주는 콜백
        public delegate void SettingIDCallBack(string msg);
        public SettingIDCallBack settingIDCallback;

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
                //byte[] buffer = new byte[4096];
                //Array.Copy(args.Buffer, 0, buffer, 0, args.BytesTransferred);
                //string msg = Encoding.UTF8.GetString(buffer);

                //receiveMessageCallback(msg);

                MessageParser(args.Buffer, args.BytesTransferred);

                isComplete = socket.ReceiveAsync(receiveArgs);
                if (!isComplete)
                {
                    ReceivecComplete(socket, receiveArgs);
                }
            }
        }

        // 수신한 byte를 string으로 변환
        private void MessageParser(byte[] buffer, int bytesTransferred)
        {
            // 만약 현재 위치가 헤더 + 커맨드 사이즈보다 적다면 헤더와 커맨드를 먼저 읽어오자
            if (currentByteLength < Defines.HEADERSIZE + Defines.COMMAND)
            {
                // 헤더 복사
                Array.Copy(buffer, currentByteLength, receiveBuffers, currentByteLength, Defines.HEADERSIZE);
                currentByteLength += Defines.HEADERSIZE;

                // 메세지의 크기를 구하자
                totalByteLength = BitConverter.ToInt32(receiveBuffers, 0);

                // 커맨드 복사
                Array.Copy(buffer, currentByteLength, receiveBuffers, currentByteLength, Defines.COMMAND);
                currentByteLength += Defines.COMMAND;

                // 남은 데이터의 값은
                remainByteLength = totalByteLength;

                // 0보다 크면 남아있는 데이터가 있음
                if ((bytesTransferred - (Defines.HEADERSIZE + Defines.COMMAND)) > 0)
                {
                    // 데이터를 복사
                    Array.Copy(buffer, currentByteLength, receiveBuffers, currentByteLength, (bytesTransferred - (Defines.HEADERSIZE + Defines.COMMAND)));

                    // 헤더값만큼 뺀걸 더해준다
                    currentByteLength += (bytesTransferred - (Defines.HEADERSIZE + Defines.COMMAND));
                    remainByteLength = totalByteLength - currentByteLength + (Defines.HEADERSIZE + Defines.COMMAND);
                }
            }
            else
            {
                // 남은 데이터의 값이 0이 아니면 받아올 데이터가 남아있음
                if (remainByteLength != 0)
                {
                    // 데이터를 복사
                    Array.Copy(buffer, currentByteLength, receiveBuffers, currentByteLength, bytesTransferred);
                    currentByteLength += bytesTransferred;
                    remainByteLength = totalByteLength - currentByteLength + (Defines.HEADERSIZE + Defines.COMMAND);
                }
            }

            // 데이터를 전부 복사해 남은 값이 없다면
            if (remainByteLength == 0)
            {
                //byte[] buffer = new byte[4096];
                //Array.Copy(args.Buffer, 0, buffer, 0, args.BytesTransferred);

                // 메세지로 변환하여 보낸다
                int command = BitConverter.ToInt32(receiveBuffers, Defines.HEADERSIZE);
                //string msg = Encoding.UTF8.GetString(receiveBuffers, (Defines.HEADERSIZE + Defines.COMMAND), totalByteLength);
                //userManager.SendMsgAll(msg, this);

                ReceivePacket((COMMAND)command);

                totalByteLength = 0;
                currentByteLength = 0;
                remainByteLength = 0;

                Array.Clear(receiveBuffers, 0, receiveBuffers.Length);

                //receiveMessageCallback(msg);
            }

            // 
        }

        // 패킷 전송
        public void SendPacket(COMMAND command, PacketMaker packet)
        {
            switch (command)
            {
                case COMMAND.SET_USER_ID:
                    //userID = Encoding.UTF8.GetString(packet.dataBuffer,
                    //    Defines.HEADERSIZE + Defines.COMMAND,
                    //    packet.currentPos - (Defines.HEADERSIZE + Defines.COMMAND));

                    sendArgs.AcceptSocket = null;
                    sendArgs.SetBuffer(packet.dataBuffer, 0, packet.currentPos);
                    socket.SendAsync(sendArgs);

                    break;

                case COMMAND.SEND_CHAT_MSG:
                    sendArgs.AcceptSocket = null;
                    sendArgs.SetBuffer(packet.dataBuffer, 0, packet.currentPos);
                    socket.SendAsync(sendArgs);

                    break;
            }
        }

        // 패킷 수신
        public void ReceivePacket(COMMAND command)
        {
            switch (command)
            {
                case COMMAND.SET_USER_ID_DONE:
                    {
                        int check = BitConverter.ToInt32(receiveBuffers, (Defines.HEADERSIZE + Defines.COMMAND));
                        string msg = Encoding.UTF8.GetString(receiveBuffers, (Defines.HEADERSIZE + Defines.COMMAND) + 4, totalByteLength);

                        if (check == 1)
                        {
                            MessageBox.Show(msg);
                            userID = msg;
                        }
                        else
                        {
                            MessageBox.Show(msg);
                        }

                        break;
                    }

                case COMMAND.RECEIVE_CHAT_MSG:
                    {
                        string msg = Encoding.UTF8.GetString(receiveBuffers, (Defines.HEADERSIZE + Defines.COMMAND), totalByteLength);

                        receiveMessageCallback(msg);

                        break;
                    }
            }
        }
    }
}
