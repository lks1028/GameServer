using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class SocketAsyncEventArgsPool
    {
        private Stack<SocketAsyncEventArgs> socketPool;

        public SocketAsyncEventArgsPool(int capacity)
        {
            socketPool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        public void Push(SocketAsyncEventArgs item)
        {
            if (item == null)
                throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null");

            lock (socketPool)
            {
                socketPool.Push(item);
            }
        }

        public SocketAsyncEventArgs Pop()
        {
            lock (socketPool)
            {
                return socketPool.Pop();
            }
        }

        public int Count
        {
            get { return socketPool.Count; }
        }
    }
}
