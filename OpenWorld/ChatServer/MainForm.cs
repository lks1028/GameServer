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
            listener.receiveMessageCallback += DisplayText;
            listener.serverLogCallback += DisplayLog;
            listener.ListenStart("127.0.0.1", 8888, 10);
        }

        private void DisplayLog(string msg)
        {
            this.Invoke(new Action(
                       delegate ()
                       {
                           StringBuilder builder = new StringBuilder();
                           builder.Append(ServerLogTextBox.Text);
                           builder.Append(msg);

                           ServerLogTextBox.Text = builder.ToString();
                           ServerLogTextBox.Text += Environment.NewLine;
                       }));
        }

        private void DisplayText(string msg)
        {
            this.Invoke(new Action(
                        delegate ()
                        {
                            StringBuilder builder = new StringBuilder();
                            builder.Append(ReceiveTextBox.Text);
                            builder.Append(msg);

                            ReceiveTextBox.Text = builder.ToString();
                            ReceiveTextBox.Text += Environment.NewLine;
                        }));
        }
    }
}
