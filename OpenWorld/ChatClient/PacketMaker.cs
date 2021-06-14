using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    public class PacketMaker
    {
        private int msgLength;
        private int command;

        public int currentPos = 0;
        public byte[] dataBuffer = new byte[4096];

        public void SetMsgLength(int length)
        {
            msgLength = length;
            byte[] buffer = BitConverter.GetBytes(length);
            Array.Copy(buffer, 0, dataBuffer, 0, Defines.HEADERSIZE);
            currentPos += Defines.HEADERSIZE;
        }

        public void SetCommand(int command)
        {
            this.command = command;
            byte[] buffer = BitConverter.GetBytes(command);
            Array.Copy(buffer, 0, dataBuffer, Defines.HEADERSIZE, Defines.COMMAND);
            currentPos += Defines.COMMAND;
        }

        public void SetStringData(string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            Array.Copy(buffer, 0, dataBuffer, currentPos, buffer.Length);
            currentPos += buffer.Length;
        }

        public void SetIntData(int data)
        {
            byte[] buffer = BitConverter.GetBytes(data);
            Array.Copy(buffer, 0, dataBuffer, currentPos, buffer.Length);
            currentPos += buffer.Length;
        }

        public byte[] GetIntToByte(int length)
        {
            return BitConverter.GetBytes(length);
        }

        public byte[] GetStringToByte(string msg)
        {
            return Encoding.UTF8.GetBytes(msg);
        }


        public int GetByteToInt(byte[] data)
        {
            return BitConverter.ToInt32(data, 0);
        }

        public string GetByteToString(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }
    }
}
