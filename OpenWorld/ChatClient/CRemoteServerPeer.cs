using NetService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    class CRemoteServerPeer : IPeer
    {
        public CUserToken token { get; private set; }

        public CRemoteServerPeer(CUserToken token)
        {
            this.token = token;
            this.token.set_peer(this);
        }

        public void disconnect()
        {
            token.socket.Disconnect(false);
        }

        public void on_message(Const<byte[]> buffer)
        {
            CPacket msg = new CPacket(buffer.Value, this);
            PROTOCOL protocol_id = (PROTOCOL)msg.pop_protocol_id();
            switch (protocol_id)
            {
                case PROTOCOL.CHAT_MSG_ACK:
                    string text = msg.pop_string();
                    Console.WriteLine(string.Format("text {0}", text));

                    break;
            }
        }

        public void on_removed()
        {
            Console.WriteLine("Server removed.");
        }

        public void process_user_operation(CPacket msg)
        {
            throw new NotImplementedException();
        }

        public void send(CPacket msg)
        {
            token.send(msg);
        }
    }
}
