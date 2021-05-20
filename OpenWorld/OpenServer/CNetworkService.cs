using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenServer
{
    public class CNetworkService
    {
        CListener client_listner;

        SocketAsyncEventArgsPool receive_event_args_pool;
        SocketAsyncEventArgsPool send_event_args_pool;
        BufferManager buffer_manager;

        public delegate void SessionHandler(CUserToken token);
        public SessionHandler session_created_callback { get; set; }

        int max_connections;
        int buffer_size;
        readonly int pre_alloc_count = 2;

        public void listen(string host, int port, int backlog)
        {
            CListener listener = new CListener();
            listener.callback_on_newclient += on_new_client;
            listener.start(host, port, backlog);
        }

        public void initialize()
        {
            max_connections = 10000;
            buffer_size = 1024;

            receive_event_args_pool = new SocketAsyncEventArgsPool(max_connections);
            send_event_args_pool = new SocketAsyncEventArgsPool(max_connections);
            buffer_manager = new BufferManager(max_connections * buffer_size * pre_alloc_count, buffer_size);

            SocketAsyncEventArgs arg;

            for (int i = 0; i < max_connections; i++)
            {
                CUserToken token = new CUserToken();

                // receive pool
                {
                    arg = new SocketAsyncEventArgs();
                    arg.Completed += new EventHandler<SocketAsyncEventArgs>(receive_completed);
                    arg.UserToken = token;

                    buffer_manager.SetBuffer(arg);

                    receive_event_args_pool.Push(arg);
                }

                // send pool
                {
                    arg = new SocketAsyncEventArgs();
                    arg.Completed += new EventHandler<SocketAsyncEventArgs>(send_completed);
                    arg.UserToken = token;

                    buffer_manager.SetBuffer(arg);

                    receive_event_args_pool.Push(arg);
                }
            }
        }

        private void on_new_client(Socket client_socket, object token)
        {
            // 풀에서 하나 꺼내와 사용
            SocketAsyncEventArgs receive_args = receive_event_args_pool.Pop();
            SocketAsyncEventArgs send_args = send_event_args_pool.Pop();

            // SocketAsyncEventArgs를 생성할 때 만들어 두었던 CUserToken을 꺼내와서   
            // 콜백 매소드의 파라미터로 넘겨줍니다.  
            if (session_created_callback != null)
            {
                CUserToken user_token = receive_args.UserToken as CUserToken;
                session_created_callback(user_token);
            }

            // 클라이언트에서 데이터 수신 준비
            begin_receive(client_socket, receive_args, send_args);
        }

        private void begin_receive(Socket socket, SocketAsyncEventArgs receive_args, SocketAsyncEventArgs send_args)
        {
            // receive_args, send_args 아무곳에서나 꺼내와도 된다. 둘다 동일한 CUserToken을 물고 있다.
            CUserToken token = receive_args.UserToken as CUserToken;
            token.set_event_args(receive_args, send_args);

            // 생성된 클라이언트 소켓을 보관해 놓고 통신할때 사용
            token.socket = socket;

            // 데이터를 받을 수 있도록 소켓 매소드를 호출해준다.  
            // 비동기로 수신할 경우 워커 스레드에서 대기중으로 있다가 Completed에 설정해놓은 매소드가 호출된다.  
            // 동기로 완료될 경우에는 직접 완료 매소드를 호출해줘야 한다.  
            bool pending = socket.ReceiveAsync(receive_args);
            if (!pending)
            {
                process_receive(receive_args);
            }
        }

        private void receive_completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.LastOperation == SocketAsyncOperation.Receive)
            {
                process_receive(e);
                return;
            }

            throw new ArgumentException("The last operation completed on the socket was not a receive.");
        }

        private void process_receive(SocketAsyncEventArgs e)
        {
            CUserToken token = e.UserToken as CUserToken;
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                // 이후의 작업은 CUserToken에게
                token.on_receive(e.Buffer, e.Offset, e.BytesTransferred);

                // 다음 메세지 수신을 위해 다시 ReceiveAsync 호출
                bool pending = token.socket.ReceiveAsync(e);
                if (!pending)
                {
                    process_receive(e);
                }
            }
            else
            {
                Console.WriteLine(string.Format("error {0},  transferred {1}", e.SocketError, e.BytesTransferred));
                close_clientsocket(token);
            }
        }
    }
}
