using NetService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegyunGameServer
{
    /// <summary>
	/// 하나의 session객체를 나타낸다.
	/// </summary>
    public class CGameUser : IPeer
    {
        CUserToken token;

        public CGameRoom battle_room { get; private set; }
        CPlayer player;

        public CGameUser(CUserToken token)
        {
            this.token = token;
            token.set_peer(this);
        }

        public void disconnect()
        {
            throw new NotImplementedException();
        }

        public void on_message(Const<byte[]> buffer)
        {
            throw new NotImplementedException();
        }

        public void on_removed()
        {
            throw new NotImplementedException();
        }

        public void process_user_operation(CPacket msg)
        {
            PROTOCOL protocol = (PROTOCOL)msg.pop_protocol_id();
            Console.WriteLine("protocol id " + protocol);
            switch (protocol)
            {
                case PROTOCOL.ENTER_GAME_ROOM_REQ:
                    Program.game_main.matching_req(this);

                    break;

                case PROTOCOL.LOADING_COMPLETED:
                    battle_room.loading_complete(player);

                    break;

                case PROTOCOL.MOVING_REQ:
                    short begin_pos = msg.pop_int16();
                    short target_pos = msg.pop_int16();
                    battle_room.moving_req(player, begin_pos, target_pos);

                    break;

                case PROTOCOL.TURN_FINISHED_REQ:
                    battle_room.turn_finished(player);

                    break;
            }
        }

        public void send(CPacket msg)
        {
            throw new NotImplementedException();
        }

        public void enter_room(CPlayer player, CGameRoom room)
        {
            this.player = player;
            battle_room = room;
        }
    }
}
