using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class Room
    {
        // 방에 접속한 유저 리스트
        private List<UserToken> tokenList = new List<UserToken>();
        // 방 이름
        private string roomName;
        // 방 ID
        private int roomID;

        public int GetRoomID()
        {
            return roomID;
        }

        public string GetRoomName()
        {
            return roomName;
        }

        public List<UserToken> GetRoomUserList()
        {
            return tokenList;
        }

        public void AddUser(UserToken token)
        {
            tokenList.Add(token);
        }

        public void RemoveUser(UserToken token)
        {
            tokenList.Remove(token);
        }

        public void SetRoomName(string roomName)
        {
            this.roomName = roomName;
        }

        public void SetRoomID(int roomID)
        {
            this.roomID = roomID;
        }

        public void SendMsgAll(string msg, UserToken token)
        {
            foreach (var user in tokenList)
            {
                // 같으면 넘긴다
                if (user == token)
                    continue;

                user.SendMsg(msg);
            }
        }

        public void SendPacketAll(COMMAND command, PacketMaker packet)
        {
            foreach (var user in tokenList)
            {
                //// 같으면 넘긴다
                //if (user == token)
                //    continue;

                user.SendPacket(command, packet);
            }
        }
    }
}
