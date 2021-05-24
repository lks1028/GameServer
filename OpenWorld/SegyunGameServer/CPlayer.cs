using NetworkServiceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegyunGameServer
{
    public class CPlayer
    {
        private CGameUser owner;
        public byte player_index { get; private set; }
        public List<short> viruses { get; private set; }

        public CPlayer(CGameUser user, byte player_index)
        {
            owner = user;
            this.player_index = player_index;
        }

		public void reset()
		{
			viruses.Clear();
		}

		public void add_cell(short position)
		{
			viruses.Add(position);
		}

		public void remove_cell(short position)
		{
			viruses.Remove(position);
		}

		public void send(CPacket msg)
		{
			owner.send(msg);
		}

		public void send_for_broadcast(CPacket msg)
		{
			owner.send(msg);
		}

		public int get_virus_count()
		{
			return viruses.Count;
		}
    }
}
