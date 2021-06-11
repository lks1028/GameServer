using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class UserManager
    {
        private List<UserToken> tokenList;

        public UserManager()
        {
            tokenList = new List<UserToken>();
        }

        public void AddUserToken(UserToken token)
        {
            tokenList.Add(token);
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
    }
}
