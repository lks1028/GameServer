using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class RoomManager
    {
        private Dictionary<int, Room> roomDic = new Dictionary<int, Room>();

        public Dictionary<int, Room> GetRoomDic()
        {
            return roomDic;
        }

        public bool SetRoom(int roomID, Room room)
        {
            if (roomDic.Count != 0 && roomDic.ContainsKey(roomID))
                return false;

            if (room == null)
                return false;

            roomDic.Add(roomID, room);

            return true;
        }

        public string GetRoomName(int roomID)
        {
            if (roomDic.TryGetValue(roomID, out Room room))
                return room.GetRoomName();

            return null;
        }

        public bool FindRoomID(int ID)
        {
            // ID를 가지고 있는 token이 없으면 false, 있으면 true
            return roomDic.ContainsKey(ID);
        }

        public void SendMsgAll(int roomID, string msg, UserToken token)
        {
            if (roomDic.TryGetValue(roomID, out Room room))
            {
                room.SendMsgAll(msg, token);
            }
        }

        public void SendPacketAll(int roomID, COMMAND command, PacketMaker packet)
        {
            if (roomDic.TryGetValue(roomID, out Room room))
            {
                room.SendPacketAll(command, packet);
            }
        }
    }
}
