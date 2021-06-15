﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class UserManager
    {
        private List<UserToken> tokenList;

        // 서버 로그 콜백
        public delegate void ServerLogCallBack(string msg);
        public ServerLogCallBack serverLogCallback;

        public UserManager()
        {
            tokenList = new List<UserToken>();
        }

        public void AddUserToken(UserToken token)
        {
            tokenList.Add(token);
        }

        public void RemoveUserToken(UserToken token)
        {
            if (tokenList.Remove(token))
            {
                serverLogCallback("Client Disconnect");
                serverLogCallback($"Total Client Count : {GetTokenListCount()}");
            }
        }

        public int GetTokenListCount()
        {
            return tokenList.Count;
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

        public bool FindUserID(string ID)
        {
            // ID를 가지고 있는 token이 없으면 false, 있으면 true
            return tokenList.Find(s => s.userID.Equals(ID)) != null;
        }
    }
}
