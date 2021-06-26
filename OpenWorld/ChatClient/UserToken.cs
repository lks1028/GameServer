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
        SERVER_DISCONNECTED,

        SET_USER_ID,
        SET_USER_ID_DONE,

        CREATE_ROOM,
        CREATE_ROOM_DONE,

        GET_ROOM,
        GET_ROOM_DONE,

        JOIN_ROOM,
        JOIN_ROOM_DONE,

        EXIT_ROOM,
        EXIT_ROOM_DONE,

        SEND_CHAT_MSG,
        RECEIVE_CHAT_MSG
    }

    class UserToken
    {
        // Send가 꼬이지 않도록 춘자처리할 큐
        private Queue<PacketMaker> packetQueue;
        private readonly object lockQueueObj;

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
        // 현재 바이트의 위치
        private int currentPos = 0;
        // 남은 바이트 수
        private int remainByteLength = 0;
        // 저장할 버퍼
        private byte[] receiveBuffers = new byte[4096];

        public string userID = string.Empty;

        // 메세지를 받으면 화면에 표시해주기 위한 콜백
        public delegate void ReceiveMessageCallBack(string msg);
        public ReceiveMessageCallBack receiveMessageCallback;

        // 아이디 설정이 됐으면 보내주는 콜백
        public delegate void SettingIDCallBack();
        public SettingIDCallBack settingIDCallback;

        // 룸 리스트 정보를 가져올때 사용하는 콜백
        public delegate void SettingRoomCallBack(string msg);
        public SettingRoomCallBack settingRoomCallback;

        // 룸을 생성한 후콜백
        public delegate void CreateRoomCallBack(string msg);
        public CreateRoomCallBack createRoomCallback;

        public UserToken()
        {
            byte[] buffer = new byte[4096];

            receiveArgs = new SocketAsyncEventArgs();
            receiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(ReceivecComplete);
            receiveArgs.SetBuffer(buffer, 0, buffer.Length);

            sendArgs = new SocketAsyncEventArgs();
            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(SendComplete);

            packetQueue = new Queue<PacketMaker>();
            lockQueueObj = new object();
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

        private void SendComplete(object sender, SocketAsyncEventArgs args)
        {
            // 직전에 보낸놈이 send가 아니면 이상한것이므로
            if (args.LastOperation != SocketAsyncOperation.Send)
            {
                // 다시보낸다
                StartSend();

                return;
            }

            // 완료 되고서 들어올 것이므로 방금 사용한 패킷 제거
            packetQueue.Dequeue();

            lock (lockQueueObj)
            {
                // 큐에 아직 데이터가 남아있다면
                if (packetQueue.Count > 0)
                {
                    // 다시 보내자
                    StartSend();
                }
            }
        }

        // 수신한 byte를 string으로 변환
        private void MessageParser(byte[] buffer, int bytesTransferred)
        {
            remainByteLength = bytesTransferred;

            // 남은 바이트수가 0이 될때까지 지속
            while (remainByteLength > 0)
            {
                // 만약 현재 위치가 헤더 + 커맨드 사이즈보다 적다면 헤더와 커맨드를 먼저 읽어오자
                if (currentByteLength < Defines.HEADERSIZE + Defines.COMMAND)
                {
                    // 헤더 복사
                    Array.Copy(buffer, currentPos, receiveBuffers, currentPos, Defines.HEADERSIZE);
                    currentByteLength += Defines.HEADERSIZE;
                    currentPos += Defines.HEADERSIZE;

                    // 메세지의 크기를 구하자
                    totalByteLength = BitConverter.ToInt32(receiveBuffers, 0);

                    // 커맨드 복사
                    Array.Copy(buffer, currentPos, receiveBuffers, currentPos, Defines.COMMAND);
                    currentByteLength += Defines.COMMAND;
                    currentPos += Defines.COMMAND;

                    // 남은 데이터의 값은
                    remainByteLength = bytesTransferred - currentPos;
                }
                else
                {
                    // 남은 바이트의 길이가 총 읽어올 바이트 길이보다 크다면 
                    int readData = remainByteLength > totalByteLength ? totalByteLength : remainByteLength;

                    // 데이터를 복사
                    Array.Copy(buffer, currentPos, receiveBuffers, currentPos, readData);

                    // 헤더값만큼 뺀걸 더해준다
                    currentByteLength += readData;
                    currentPos += readData;
                    remainByteLength -= readData;

                    // 모든 데이터를 읽어들였다면
                    // remainByteLength로 체크하지 않는 이유는 메세지가 끝나고도 읽어들일 바이트가 남아있을 수 있으므로
                    if (currentByteLength == Defines.HEADERSIZE + Defines.COMMAND + totalByteLength)
                    {
                        // 메세지를 전송하고 초기화한다
                        int command = BitConverter.ToInt32(receiveBuffers, Defines.HEADERSIZE);
                        ReceivePacket((COMMAND)command);

                        totalByteLength = 0;
                        currentByteLength = 0;

                        Array.Clear(receiveBuffers, 0, receiveBuffers.Length);
                    }
                }
            };

            currentPos = 0;

            #region old - 뒤에 데이터가 더 붙어서 오는경우 알수가 없으므로
            //// 만약 현재 위치가 헤더 + 커맨드 사이즈보다 적다면 헤더와 커맨드를 먼저 읽어오자
            //if (currentByteLength < Defines.HEADERSIZE + Defines.COMMAND)
            //{
            //    // 헤더 복사
            //    Array.Copy(buffer, currentByteLength, receiveBuffers, currentByteLength, Defines.HEADERSIZE);
            //    currentByteLength += Defines.HEADERSIZE;

            //    // 메세지의 크기를 구하자
            //    totalByteLength = BitConverter.ToInt32(receiveBuffers, 0);

            //    // 커맨드 복사
            //    Array.Copy(buffer, currentByteLength, receiveBuffers, currentByteLength, Defines.COMMAND);
            //    currentByteLength += Defines.COMMAND;

            //    // 남은 데이터의 값은
            //    remainByteLength = totalByteLength;

            //    // 0보다 크면 남아있는 데이터가 있음
            //    if ((bytesTransferred - (Defines.HEADERSIZE + Defines.COMMAND)) > 0)
            //    {
            //        // 데이터를 복사
            //        Array.Copy(buffer, currentByteLength, receiveBuffers, currentByteLength, (bytesTransferred - (Defines.HEADERSIZE + Defines.COMMAND)));

            //        // 헤더값만큼 뺀걸 더해준다
            //        currentByteLength += (bytesTransferred - (Defines.HEADERSIZE + Defines.COMMAND));
            //        remainByteLength = totalByteLength - currentByteLength + (Defines.HEADERSIZE + Defines.COMMAND);
            //    }
            //}
            //else
            //{
            //    // 남은 데이터의 값이 0이 아니면 받아올 데이터가 남아있음
            //    if (remainByteLength != 0)
            //    {
            //        // 데이터를 복사
            //        Array.Copy(buffer, currentByteLength, receiveBuffers, currentByteLength, bytesTransferred);
            //        currentByteLength += bytesTransferred;
            //        remainByteLength = totalByteLength - currentByteLength + (Defines.HEADERSIZE + Defines.COMMAND);
            //    }
            //}

            //// 데이터를 전부 복사해 남은 값이 없다면
            //if (remainByteLength == 0)
            //{
            //    //byte[] buffer = new byte[4096];
            //    //Array.Copy(args.Buffer, 0, buffer, 0, args.BytesTransferred);

            //    // 메세지로 변환하여 보낸다
            //    int command = BitConverter.ToInt32(receiveBuffers, Defines.HEADERSIZE);
            //    //string msg = Encoding.UTF8.GetString(receiveBuffers, (Defines.HEADERSIZE + Defines.COMMAND), totalByteLength);
            //    //userManager.SendMsgAll(msg, this);

            //    ReceivePacket((COMMAND)command);

            //    totalByteLength = 0;
            //    currentByteLength = 0;
            //    remainByteLength = 0;

            //    Array.Clear(receiveBuffers, 0, receiveBuffers.Length);

            //    //receiveMessageCallback(msg);
            //}
            #endregion

            // 
        }

        // 패킷 전송
        public void SendPacket(COMMAND command, PacketMaker packet)
        {
            lock (lockQueueObj)
            {
                // 큐에 데이터가 없다면 추가하고 패킷을 보내자
                if (packetQueue.Count == 0)
                {
                    packetQueue.Enqueue(packet);
                    StartSend();

                    return;
                }

                // 큐에 데이터가 있다면 넣기만 하자
                packetQueue.Enqueue(packet);
            }
        }

        private void StartSend()
        {
            lock (lockQueueObj)
            {
                bool isComplete = true;

                // 체크를 큐가 들어있는지로 하므로 끝나기 전까지 dequeue 금지
                PacketMaker maker = packetQueue.Peek();

                // 데이터를 보내자
                sendArgs.AcceptSocket = null;
                sendArgs.SetBuffer(maker.dataBuffer, 0, maker.currentPos);
                isComplete = socket.SendAsync(sendArgs);
                if (!isComplete)
                {
                    SendComplete(socket, sendArgs);
                }
            }

            //switch (command)
            //{
            //    case COMMAND.SET_USER_ID:
            //        //userID = Encoding.UTF8.GetString(packet.dataBuffer,
            //        //    Defines.HEADERSIZE + Defines.COMMAND,
            //        //    packet.currentPos - (Defines.HEADERSIZE + Defines.COMMAND));

            //        sendArgs.AcceptSocket = null;
            //        sendArgs.SetBuffer(packet.dataBuffer, 0, packet.currentPos);
            //        socket.SendAsync(sendArgs);

            //        break;

            //    case COMMAND.SEND_CHAT_MSG:
            //        sendArgs.AcceptSocket = null;
            //        sendArgs.SetBuffer(packet.dataBuffer, 0, packet.currentPos);
            //        socket.SendAsync(sendArgs);

            //        break;

            //    case COMMAND.SERVER_DISCONNECTED:
            //        sendArgs.AcceptSocket = null;
            //        sendArgs.SetBuffer(packet.dataBuffer, 0, packet.currentPos);
            //        socket.SendAsync(sendArgs);

            //        break;
            //}
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
                            //MessageBox.Show(msg + " 아이디로 접속되었습니다.");
                            userID = msg;
                            settingIDCallback();

                            // 방 리스트를 가져오자
                            PacketMaker maker = new PacketMaker();
                            maker.SetMsgLength(BitConverter.GetBytes(1).Length);
                            maker.SetCommand((int)COMMAND.GET_ROOM);
                            maker.SetIntData(1);

                            SendPacket(COMMAND.GET_ROOM, maker);
                        }
                        else
                        {
                            MessageBox.Show("동일한 아이디가 존재합니다");

                            // 디스커넥트를 서버에 보내자
                            // 나중에 패킷 메세지도 필요하면 그 때 보내보자
                            PacketMaker maker = new PacketMaker();
                            maker.SetMsgLength(BitConverter.GetBytes(1).Length);
                            maker.SetCommand((int)COMMAND.SERVER_DISCONNECTED);
                            maker.SetIntData(1);

                            SendPacket(COMMAND.SERVER_DISCONNECTED, maker);

                            //socket.Disconnect(false);
                            //SocketAsyncEventArgs dis = new SocketAsyncEventArgs();
                            //dis.Completed += new EventHandler<SocketAsyncEventArgs>(Dis);
                            //socket.DisconnectAsync(dis);
                        }

                        break;
                    }

                case COMMAND.RECEIVE_CHAT_MSG:
                    {
                        string msg = Encoding.UTF8.GetString(receiveBuffers, (Defines.HEADERSIZE + Defines.COMMAND), totalByteLength);

                        receiveMessageCallback(msg);

                        break;
                    }

                case COMMAND.GET_ROOM_DONE:
                    {
                        string msg = Encoding.UTF8.GetString(receiveBuffers, (Defines.HEADERSIZE + Defines.COMMAND), totalByteLength);

                        settingRoomCallback(msg);

                        break;
                    }

                case COMMAND.CREATE_ROOM_DONE:
                    {
                        string msg = Encoding.UTF8.GetString(receiveBuffers, (Defines.HEADERSIZE + Defines.COMMAND), totalByteLength);

                        createRoomCallback(msg);

                        break;
                    }

                case COMMAND.JOIN_ROOM_DONE:
                    {
                        string msg = Encoding.UTF8.GetString(receiveBuffers, (Defines.HEADERSIZE + Defines.COMMAND), totalByteLength);

                        createRoomCallback(msg);

                        break;
                    }

                case COMMAND.EXIT_ROOM_DONE:
                    {
                        // 방 리스트를 가져오자
                        PacketMaker maker = new PacketMaker();
                        maker.SetMsgLength(BitConverter.GetBytes(1).Length);
                        maker.SetCommand((int)COMMAND.GET_ROOM);
                        maker.SetIntData(1);

                        SendPacket(COMMAND.GET_ROOM, maker);

                        // 방 로비 화면으로 넘겨주자
                        settingIDCallback();

                        break;
                    }
            }
        }
    }
}
