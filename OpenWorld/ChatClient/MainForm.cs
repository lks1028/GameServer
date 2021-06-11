using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class MainForm : Form
    {
        Sender sender1;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            sender1 = new Sender();
            sender1.receiveMessageCallback += DisplayText;
            sender1.Start("127.0.0.1", 8888, 10);
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

        private void SendButton_Click(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(ReceiveTextBox.Text);
            builder.Append(SendTextBox.Text);
            builder.Append(Environment.NewLine);

            ReceiveTextBox.Text = builder.ToString();

            sender1.SendMsg(SendTextBox.Text);

            SendTextBox.Text = string.Empty;
        }
    }
}
