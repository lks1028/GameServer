using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class PacketMaker
    {
        public byte[] GetIntToByte(int length)
        {
            return BitConverter.GetBytes(length);
        }

        //public byte[] GetCommandToByte(short command)
        //{
        //    return BitConverter.GetBytes(command);
        //}

        public byte[] GetStringToByte(string msg)
        {
            return Encoding.UTF8.GetBytes(msg);
        }


        public int GetByteToInt(byte[] data)
        {
            return BitConverter.ToInt32(data, 0);
        }

        //public byte[] GetCommandToByte(short command)
        //{
        //    return BitConverter.GetBytes(command);
        //}

        public string GetByteToString(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }
    }
}
