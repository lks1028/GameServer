using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatServer
{
    public partial class ChatServer : Form
    {
        public ChatServer()
        {
            InitializeComponent();
        }

        private void ChatServer_Load(object sender, EventArgs e)
        {
            Listener listener = new Listener();
            listener.ListenStart("127.0.0.1", 8888, 10);
        }
    }
}
