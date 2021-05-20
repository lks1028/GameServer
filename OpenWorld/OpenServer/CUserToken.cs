using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OpenServer
{
    public class CUserToken
    {
        public Socket socket { get; set; }

        public SocketAsyncEventArgs receive_event_args { get; private set; }
        public SocketAsyncEventArgs send_event_args { get; private set; }

        // 바이트를 패킷 형식으로 해석해주는 해석기
        CMessageResolver message_resolver;

        // session 객체. 어플리케이션단에서 구현해서 사용
        IPeer peer;

        // 전송할 패킷을 보관하는 큐
        Queue<CPacket> sending_queue;
        // sending_queue lock 처리에 사용되는 객체
        private object cs_sending_queue;

        public CUserToken()
        {
            cs_sending_queue = new object();
            message_resolver = new CMessageResolver();
            peer = null;
            sending_queue = new Queue<CPacket>();
        }

        public void set_peer(IPeer peer)
        {
            this.peer = peer;
        }

        public void set_event_args(SocketAsyncEventArgs receive_event_args, SocketAsyncEventArgs send_event_args)
        {
            this.receive_event_args = receive_event_args;
            this.send_event_args = send_event_args;
        }

        /// <summary>
		///	이 매소드에서 직접 바이트 데이터를 해석해도 되지만 Message resolver클래스를 따로 둔 이유는
		///	추후에 확장성을 고려하여 다른 resolver를 구현할 때 CUserToken클래스의 코드 수정을 최소화 하기 위함이다.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="transfered"></param>
		public void on_receive(byte[] buffer, int offset, int transfered)
        {
            message_resolver.on_receive(buffer, offset, transfered, on_message);
        }

        private void on_message(Const<byte[]> buffer)
        {
            if (peer != null)
                peer.on_message(buffer);
        }

        public void on_removed()
        {
            sending_queue.Clear();

            if (peer != null)
                peer.on_removed();
        }
    }
}
