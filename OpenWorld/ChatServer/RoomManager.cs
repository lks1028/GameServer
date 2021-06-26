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
        private int roomCount = 0;

        public Dictionary<int, Room> GetRoomDic()
        {
            return roomDic;
        }

        public bool AddRoom(int roomID, Room room)
        {
            if (roomDic.Count != 0 && roomDic.ContainsKey(roomID))
                return false;

            if (room == null)
                return false;

            roomDic.Add(roomID, room);

            return true;
        }

        public bool RemoveRoom(int roomID)
        {
            if (roomDic.Count != 0 && roomDic.ContainsKey(roomID))
                return false;

            roomDic.Remove(roomID);

            return true;
        }

        public string GetRoomName(int roomID)
        {
            if (roomDic.TryGetValue(roomID, out Room room))
                return room.GetRoomName();

            return null;
        }

        public bool CheckRoomID(int ID)
        {
            // ID를 가지고 있는 token이 없으면 false, 있으면 true
            return roomDic.ContainsKey(ID);
        }

        public int FindRoomID(string roomName)
        {
            foreach (var data in roomDic)
            {
                if (data.Value.GetRoomName().Equals(roomName))
                {
                    return data.Value.GetRoomID();
                }
            }

            return -1;
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

        public int CreateRoomID()
        {
            return roomCount++;
        }
    }
}
