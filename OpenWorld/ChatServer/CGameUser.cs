using NetService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    /// <summary>
	/// 하나의 session객체를 나타낸다.
	/// </summary>
    class CGameUser : IPeer
    {
        CUserToken token;

        public CGameUser(CUserToken token)
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
            PROTOCOL protocol = (PROTOCOL)msg.pop_protocol_id();
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("protocol id " + protocol);
            switch (protocol)
            {
                case PROTOCOL.CHAT_MSG_REQ:
                    string text = msg.pop_string();
                    Console.WriteLine(string.Format("text {0}", text));

                    CPacket response = CPacket.create((short)PROTOCOL.CHAT_MSG_ACK);
                    response.push(text);
                    send(response);
                    break;
            }
        }

        public void on_removed()
        {
            Console.WriteLine("The client disconnected.");

            Program.remove_user(this);
        }

        public void process_user_operation(CPacket msg)
        {
            throw new NotImplementedException();
        }

        public void send(CPacket msg)
        {
            //token.send(msg);

            foreach (var otherUserToken in Program.userlist)
            {
                if (otherUserToken.token.Equals(token))
                    continue;

                otherUserToken.token.send(msg);
            }
        }
    }
}
