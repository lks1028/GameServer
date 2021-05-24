using NetworkServiceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegyunGameServer
{
    /// <summary>  
    /// 게임 방 하나를 구성한다. 게임의 로직이 처리되는 핵심 클래스이다.  
    /// </summary> 
    public class CGameRoom
    {
        enum PLAYER_STATE : byte
        {
            // 방에 막 입장한 상태.
            ENTERED_ROOM,

            // 로딩을 완료한 상태.
            LOADING_COMPLETE,

            // 턴 진행 준비 상태.
            READY_TO_TURN,

            // 턴 연출을 모두 완료한 상태.
            CLIENT_TURN_FINISHED
        }

        // 게임을 진행하는 플레이어. 1, 2P가 존재
        private List<CPlayer> players;

        // 플레이어들의 상태를 관리하는 변수
        private Dictionary<byte, PLAYER_STATE> player_state;

        // 현재 턴을 진행하고 있는 플레이어의 인덱스
        private byte current_turn_player;

        // 게임 보드판
        private List<short> gameboard;

        // 0 ~ 49까지의 인덱스를 갖고 있는 보드판 데이터
        private List<short> table_board;

        private static byte COLUMN_COUNT = 7;
        private readonly short EMPTY_SLOT = short.MaxValue;

        public CGameRoom()
        {
            players = new List<CPlayer>();
            player_state = new Dictionary<byte, PLAYER_STATE>();
            current_turn_player = 0;

            // 7*7(총 49칸)모양의 보드판을 구성한다.
            // 초기에는 모두 빈공간이므로 EMPTY_SLOT으로 채운다.
            gameboard = new List<short>();
            table_board = new List<short>();

            for (byte i = 0; i < COLUMN_COUNT * COLUMN_COUNT; ++i)
            {
                gameboard.Add(EMPTY_SLOT);
                table_board.Add(i);
            }
        }

        /// <summary>  
        /// 매칭이 성사된 플레이어들이 게임에 입장한다.  
        /// </summary>  
        /// <param name="player1"></param>  
        /// <param name="player2"></param>  
        public void enter_gameroom(CGameUser user1, CGameUser user2)
        {
            // 플레이어들을 생성하고 1번, 2번 인덱스 부여
            CPlayer player1 = new CPlayer(user1, 1);
            CPlayer player2 = new CPlayer(user2, 2);

            players.Clear();
            players.Add(player1);
            players.Add(player2);

            // 플레이어들의 초기 상태를 지정
            player_state.Clear();
            change_playerstate(player1, PLAYER_STATE.ENTERED_ROOM);
            change_playerstate(player2, PLAYER_STATE.ENTERED_ROOM);

            // 로딩 시작메세지 전송
            //CPacket msg = CPacket.create((short)PROTOCOL.START_LOADING);
            //broadcast(msg);
            players.ForEach(player =>
            {
                CPacket msg = CPacket.create((short)PROTOCOL.START_LOADING);
                // 본인의 플레이어 인덱스를 알림
                msg.push(player.player_index);
                player.send(msg);
            });

            user1.enter_room(player1, this);
            user2.enter_room(player2, this);
        }

        /// <summary>  
        /// 클라이언트에서 로딩을 완료한 후 요청함.  
        /// 이 요청이 들어오면 게임을 시작해도 좋다는 뜻이다.  
        /// </summary>  
        /// <param name="sender">요청한 유저</param>  
        public void loading_complete(CPlayer player)
        {
            // 해당 플레이어를 로딩 완료 상태로 변경
            change_playerstate(player, PLAYER_STATE.LOADING_COMPLETE);

            // 모든 유저가 준비 상태인지 체크한다.  
            if (!allplayers_ready(PLAYER_STATE.LOADING_COMPLETE))
            {
                // 아직 준비가 안된 유저가 있다면 대기한다.  
                return;
            }

            // 모두 준비 되었다면 게임을 시작한다.  
            battle_start();
        }

        /// <summary>  
        /// 모든 유저들에게 메시지를 전송한다.  
        /// </summary>  
        /// <param name="msg"></param>  
        private void broadcast(CPacket msg)
        {
            players.ForEach(player => player.send(msg));
            CPacket.destroy(msg);
        }

        /// <summary>  
        /// 플레이어의 상태를 변경한다.  
        /// </summary>  
        /// <param name="player"></param>  
        /// <param name="state"></param>  
        private void change_playerstate(CPlayer player, PLAYER_STATE state)
        {
            if (player_state.ContainsKey(player.player_index))
            {
                player_state[player.player_index] = state;
            }
            else
            {
                player_state.Add(player.player_index, state);
            }
        }

        /// <summary>  
        /// 모든 플레이어가 특정 상태가 되었는지를 판단한다.  
        /// 모든 플레이어가 같은 상태에 있다면 true, 한명이라도 다른 상태에 있다면 false를 리턴한다.  
        /// </summary>  
        /// <param name="state"></param>  
        /// <returns></returns>  
        private bool allplayers_ready(PLAYER_STATE state)
        {
            foreach (KeyValuePair<byte, PLAYER_STATE> kvp in player_state)
            {
                if (kvp.Value != state)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>  
        /// 게임을 시작한다.  
        /// </summary>  
        private void battle_start()
        {
            // 게임을 새로 시작할 때 마다 초기화해줘야 할 것들.  
            reset_gamedata();

            // 게임 시작 메세지 전송
            CPacket msg = CPacket.create((short)PROTOCOL.GAME_START);

            // 플레이어들의 세균 위치 전송
            msg.push((byte)players.Count);
            players.ForEach(player =>
            {
                // 누구인지 구분하기 위한 인덱스
                msg.push(player.player_index);

                // 플레이어가 소지한 세균들의 전체 갯수
                byte cell_count = (byte)player.viruses.Count;
                msg.push(cell_count);

                // 플레이어의 세균들의 위치정보
                player.viruses.ForEach(position => msg.push(position));
            });

            // 첫 턴을 진행할 플레이어 인덱스
            msg.push(current_turn_player);
            broadcast(msg);

            // 1P부터 시작.  
        }

        /// <summary>
		/// 턴을 시작하라고 클라이언트들에게 알려 준다.
		/// </summary>
		void start_turn()
        {

        }

        /// <summary>  
        /// 게임 데이터를 초기화 한다.  
        /// 게임을 새로 시작할 때 마다 초기화 해줘야 할 것들을 넣는다.  
        /// </summary>  
        private void reset_gamedata()
        {
            // 보드판 데이터 초기화
            for (int i = 0; i < gameboard.Count; i++)
            {
                gameboard[i] = 0;
            }

            // 1번 플레이어의 세균은 왼쪽 위(0, 0) 오른쪽 위(0, 7) 두 군대에 배치
            put_virus(1, 0, 0);
            put_virus(1, 0, 7);

            // 2번 플레이어의 세균은 왼쪽 아래(7, 0) 오른쪽 아래(7, 7) 두 군대에 배치
            put_virus(2, 7, 0);
            put_virus(2, 7, 7);

            // 턴 초기화
            current_turn_player = 1;
        }

        /// <summary>
        /// 보드판에 플레이어의 세균을 배치한다.
        /// </summary>
        /// <param name="player_index"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void put_virus(byte player_index, byte row, byte col)
        {

        }

        /// <summary>  
        /// 보드판에 플레이어의 세균을 배치한다.  
        /// </summary>  
        /// <param name="player_index"></param>  
        /// <param name="position"></param>  
        private void put_virus(byte player_index, short position)
        {
            gameboard[position] = player_index;
            get_player(player_index).add_cell(position);
        }

        /// <summary>  
        /// 배치된 세균을 삭제한다.  
        /// </summary>  
        /// <param name="player_index"></param>  
        /// <param name="position"></param>  
        private void remove_virus(byte player_index, short position)
        {
            gameboard[position] = 0;
            get_player(player_index).remove_cell(position);
        }

        /// <summary>
		/// 플레이어 인덱스에 해당하는 플레이어를 구한다.
		/// </summary>
		/// <param name="player_index"></param>
		/// <returns></returns>
        private CPlayer get_player(byte player_index)
        {
            return this.players.Find(obj => obj.player_index == player_index);
        }

        /// <summary>  
        /// 상대방의 세균을 감염 시킨다.  
        /// </summary>  
        /// <param name="basis_cell"></param>  
        /// <param name="attacker"></param>  
        /// <param name="victim"></param>  
        public void infect(short basis_cell, CPlayer attacker, CPlayer victim)
        {
            // 방어자의 세균중에 기준위치로부터 1칸 반경에 있는 세균들이 감염 대상
            List<short> neighbors = CHelper.find_neighbor_cells(basis_cell, victim.viruses, 1);
            foreach (short position in neighbors)
            {
                // 방어자의 세균을 삭제한다
                remove_virus(victim.player_index, position);

                // 공격자의 세균을 추가한다
                put_virus(attacker.player_index, position);
            }
        }

        /// <summary>
		/// 현재 턴인 플레이어의 상대 플레이어를 구한다.
		/// </summary>
		/// <returns></returns>
		private CPlayer get_opponent_player()
        {
            return this.players.Find(player => player.player_index != this.current_turn_player);
        }


        /// <summary>
        /// 현재 턴을 진행중인 플레이어를 구한다.
        /// </summary>
        /// <returns></returns>
        private CPlayer get_current_player()
        {
            return this.players.Find(player => player.player_index == this.current_turn_player);
        }

        /// <summary>  
        /// 클라이언트의 이동 요청.  
        /// </summary>  
        /// <param name="sender">요청한 유저</param>  
        /// <param name="begin_pos">시작 위치</param>  
        /// <param name="target_pos">이동하고자 하는 위치</param>  
        public void moving_req(CPlayer sender, short begin_pos, short target_pos)
        {
            // sender차례인지 체크.  
            // 체크 이유 : 현재 자신의 차례가 아님에도 불구하고 이동 요청을 보내온다면 게임의 턴이 엉망이 되어 버릴 것입니다.  
            if (current_turn_player != sender.player_index)
            {
                // 현재 턴이 아닌 플레이어가 보낸 요청이라면 무시한다.
                // 이런 비정상적인 상황에서는 화면이나 파일로 로그를 남겨두는것이 좋다.
                return;
            }

            // begin_pos에 sender의 캐릭터가 존재하는지 체크.  
            // 체크 이유 : 없는 캐릭터를 이동하려고 하면 당연히 안되겠죠?  
            if (gameboard[begin_pos] != sender.player_index)
            {
                // 시작 위치에 해당 플레이어의 세균이 존재하지 않는다.
                return;
            }

            // 목적지는 0으로 설정된 빈 공간이어야 한다.  
            // 다른 세균이 자리하고 있는 곳으로는 이동할 수 없다.
            if (gameboard[target_pos] != EMPTY_SLOT)
            {
                // 목적지에 다른 세균이 존재한다.
                return;
            }

            // target_pos가 이동 또는 복제 가능한 범위인지 체크.  
            // 체크 이유: 이동할 수 없는 범위로는 갈 수 없도록 처리해야 합니다.  
            short distance = CHelper.get_distance(begin_pos, target_pos);
            if (distance > 2)
            {
                // 2칸을 초과하는 거리는 이동할 수 없다.
                return;
            }

            if (distance <= 0)
            {
                // 자기 자신의 위치로는 이동할 수 없다
                return;
            }


            // 모든 체크가 정상이라면 이동을 처리한다.  
            // 이동 거리가 한칸일 경우에는 복제를 수행한다.
            if (distance == 1)
            {
                put_virus(sender.player_index, target_pos);
            }
            // 이동 거리가 두칸일 경우에는 이동을 수행한다
            else if (distance == 2)
            {
                // 이전 위치에 있는 세균을 삭제한다
                remove_virus(sender.player_index, begin_pos);

                // 새로운 위치에 세균을 놓는다.
                put_virus(sender.player_index, target_pos);
            }

            // 목적지를 기준으로 주위에 존재하는 상대방 세균을 감염시켜 같은 편으로 만든다.
            CPlayer opponent = get_opponent_player();
            infect(target_pos, sender, opponent);

            // 최종 결과를 모든 클라이언트들에게 전송 한다.  
            CPacket msg = CPacket.create((short)PROTOCOL.PLAYER_MOVED);
            // 누가
            msg.push(sender.player_index);
            // 어디서
            msg.push(begin_pos);
            // 어디로 이동
            msg.push(target_pos);

            broadcast(msg);

            // 턴을 종료 한다.  
            //turn_end();
        }

        /// <summary>  
        /// 클라이언트에서 턴 연출이 모두 완료 되었을 때 호출된다.  
        /// </summary>  
        /// <param name="sender"></param>  
        public void turn_finished(CPlayer sender)
        {
            change_playerstate(sender, PLAYER_STATE.CLIENT_TURN_FINISHED);

            if (!allplayers_ready(PLAYER_STATE.CLIENT_TURN_FINISHED))
            {
                return;
            }

            // 턴을 넘긴다
            turn_end();
        }

        /// <summary>  
        /// 턴을 종료한다. 게임이 끝났는지 확인하는 과정을 수행한다.  
        /// </summary>  
        private void turn_end()
        {
            // 보드판 상태를 확인하여 게임이 끝났는지 검사한다.  
            if (!CHelper.can_play_more(gameboard, get_current_player(), players))
            {
                // game over
                return;
            }

            // 아직 게임이 끝나지 않았다면 다음 플레이어로 턴을 넘긴다.  
            if (current_turn_player < players.Count - 1)
            {
                ++current_turn_player;
            }
            else
            {
                // 다시 첫번째 플레이어의 턴으로
                current_turn_player = players[0].player_index;
            }

            // 턴을 시작
            start_turn();
        }

        private void game_over()
        {
            // 우승자 가리기
            byte win_player_index = byte.MaxValue;
            int count_1p = players[0].get_virus_count();
            int count_2p = players[1].get_virus_count();

            if (count_1p == count_2p)
            {
                // 동점인 경우
                win_player_index = byte.MaxValue;
            }
            else
            {
                if (count_1p > count_2p)
                {
                    win_player_index = players[0].player_index;
                }
                else
                {
                    win_player_index = players[1].player_index;
                }
            }

            CPacket msg = CPacket.create((short)PROTOCOL.GAME_OVER);
            msg.push(win_player_index);
            msg.push(count_1p);
            msg.push(count_2p);
            broadcast(msg);

            // 방 제거
            Program.game_main.room_manager.remove_room(this);
        }

        public void destroy()
        {
            CPacket msg = CPacket.create((short)PROTOCOL.ROOM_REMOVED);
            broadcast(msg);

            players.Clear();
        }
    }
}
