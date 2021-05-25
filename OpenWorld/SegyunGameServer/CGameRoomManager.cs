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
        private Dictionary<short, CGameRoom> roomDic;

        public CGameRoomManager()
        {
            rooms = new List<CGameRoom>();
            roomDic = new Dictionary<short, CGameRoom>();
        }

        #region 책에 있던 소스
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
        /// 방 제거
        /// </summary>
        /// <param name="room"></param>
        public void remove_room(CGameRoom room)
        {
            room.destroy();
            rooms.Remove(room);
        }
        #endregion

        /// <summary>
        /// 방 생성 요청을 처리한다.
        /// </summary>
        /// <param name="user"></param>
        public void CreateRoom(CGameUser user)
        {
            //byte create_room_number = 0;
            short maxClient = 100;

            // 방 생성
            CGameRoom room = new CGameRoom();
            for (short createRoomNumber = 0; createRoomNumber <= maxClient; ++createRoomNumber)
            {
                // 이미 있는 방 번호라면 넘긴다.
                // 이거 containskey 쓰는거 맞냐?;;; 나중에 집가서 찾아보자
                if (roomDic.ContainsKey(createRoomNumber))
                    continue;

                // 방에 입장하고 생성된 방에 추가한다.
                room.EnterGameRoom(user, createRoomNumber);
                roomDic.Add(createRoomNumber, room);
            }
        }

        /// <summary>
        /// 생성된 방에 입장한다.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="room_number"></param>
        public void EnterRoom(CGameUser user, short roomNumber)
        {
            if (roomDic.TryGetValue(roomNumber, out CGameRoom battleroom))
            {
                battleroom.EnterGameRoom(user, roomNumber);
            }
        }

        /// <summary>
        /// 매칭 대기중인 방
        /// </summary>
        public void GetMatchingRoom(CGameUser user)
        {
            // 플레이어의 수가 2미만인 방의 리스트 조회
            List<CGameRoom> roomlist = roomDic.Select(s => s.Value).Where(o => o.GetPlayerCount() < 2).ToList();

            // 대기중인 방에 대한 패킷을 여기서 보내주는게 맞겠지?
            CPacket msg = CPacket.create((short)PROTOCOL.GET_WAITING_ROOM);

            // 총 방의 수를 넣고
            msg.push(roomlist.Count);

            // 각 방의 데이터를 넣는다
            roomlist.ForEach(room =>
            {
                // 방 번호라던가, 방 제목, 플레이어의 아이디 등등....
                // 현재는 방번호만 있으므로
                msg.push(room.roomNumber);
            });

            // 됐다면 해당 유저에게 다시 보내주자
            user.send(msg);
        }
    }
}
