﻿using NetworkServiceLibrary;
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
            token.socket.Disconnect(false);
        }

        public void on_message(Const<byte[]> buffer)
        {
            byte[] clone = new byte[1024];
            Array.Copy(buffer.Value, clone, buffer.Value.Length);
            CPacket msg = new CPacket(clone, this);
            Program.game_main.enqueue_packet(msg, this);
        }

        public void on_removed()
        {
            Console.WriteLine("The client disconnected.");

            Program.remove_user(this);
        }

        public void process_user_operation(CPacket msg)
        {
            PROTOCOL protocol = (PROTOCOL)msg.pop_protocol_id();
            Console.WriteLine("protocol id " + protocol);
            switch (protocol)
            {
                // 대기방 조회
                case PROTOCOL.GET_WAITING_ROOM:
                    Program.game_main.GetMatchingRoom(this);

                    break;

                // 방 생성
                case PROTOCOL.CREATE_ROOM:
                    Program.game_main.CreateRoom(this);

                    break;

                // 플레이어가 레디함
                case PROTOCOL.PLAYER_READY:
                    battle_room.PlayerReady(player);

                    break;

                #region 책에 있던 소스들
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
                #endregion
            }
        }

        public void send(CPacket msg)
        {
            token.send(msg);
        }

        public void enter_room(CPlayer player, CGameRoom room)
        {
            this.player = player;
            battle_room = room;
        }
    }
}
