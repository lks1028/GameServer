using NetworkServiceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegyunGameServer
{
    class CGameRoomManager
    {
        private List<CGameRoom> rooms;
        // 방 리스트 Key는 방의 번호이다.
        private Dictionary<byte, CGameRoom> dicRoom;

        public CGameRoomManager()
        {
            rooms = new List<CGameRoom>();
        }

        /// <summary>  
        /// 매칭을 요청한 유저들을 넘겨 받아 게임 방을 생성한다.  
        /// </summary>  
        /// <param name="user1"></param>  
        /// <param name="user2"></param>  
        public void create_room(CGameUser user1, CGameUser user2)
        {
            // 게임 방을 생성하여 입장 시킴
            CGameRoom battleroom = new CGameRoom();
            battleroom.enter_gameroom(user1, user2);

            // 방 리스트에 추가하여 관리
            rooms.Add(battleroom);
        }

        /// <summary>
        /// 방 생성 요청을 처리한다.
        /// </summary>
        /// <param name="user"></param>
        public void create_room(CGameUser user)
        {
            //byte create_room_number = 0;
            byte maxClient = 100;

            CGameRoom battleroom = new CGameRoom();
            for (byte create_room_number = 0; create_room_number <= maxClient; ++create_room_number)
            {
                // 이미 있는 방 번호라면 넘긴다.
                // 이거 containskey 쓴는거 맞냐?;;; 나중에 집가서 찾아보자
                if (dicRoom.ContainsKey(create_room_number))
                    continue;

                // 방에 입장하고 생성된 방에 추가한다.
                battleroom.enter_gameroom(user);
                dicRoom.Add(create_room_number, battleroom);
            }
        }

        /// <summary>
        /// 생성된 방에 입장한다.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="room_number"></param>
        public void enter_room(CGameUser user, byte room_number)
        {
            if (dicRoom.TryGetValue(room_number, out CGameRoom battleroom))
            {
                battleroom.enter_gameroom(user);
            }
        }


        /// <summary>
        /// 방 제거
        /// </summary>
        /// <param name="room"></param>
        public void remove_room(CGameRoom room)
        {
            room.destroy();
            rooms.Remove(room);
        }

        /// <summary>
        /// 매칭 대기중인 방
        /// </summary>
        public void match_read_room(CGameUser user)
        {
            // 대기중인 방에 대한 패킷을 여기서 보내주는게 맞겠지?
            CPacket msg = CPacket.create((short)PROTOCOL.GET_WAITING_ROOM);

            var data = dicRoom.Where(s => s.Value.getplayercount() != 2);
            //Dictionary<byte, CGameRoom> data = dicRoom.Where(s => s.Value.getplayercount() != 2);

            foreach (CGameRoom room in dicRoom.Values)
            {

            }

            //var data = dicRoom.Select(s => s.Value.getplayercount() != 2).ToDictionary();
        }
    }
}
