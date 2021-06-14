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
            sender1.connectDoneCallBack += ConnectDone;
            sender1.settingIDDoneCallBack += SettingIDDone;
            //sender1.Start("127.0.0.1", 8888, 10);
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

            PacketMaker maker = new PacketMaker();
            maker.SetMsgLength(Encoding.UTF8.GetByteCount(SendTextBox.Text));
            maker.SetCommand((int)COMMAND.SEND_CHAT_MSG);
            maker.SetStringData(SendTextBox.Text);

            sender1.SendPacket(COMMAND.SEND_CHAT_MSG, maker);

            SendTextBox.Text = string.Empty;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sender1.Start("127.0.0.1", 8888, 10);
        }

        private void ConnectDone()
        {
            PacketMaker maker = new PacketMaker();
            maker.SetMsgLength(Encoding.UTF8.GetByteCount(textBox1.Text));
            maker.SetCommand((int)COMMAND.SET_USER_ID);
            maker.SetStringData(textBox1.Text);

            sender1.SendPacket(COMMAND.SET_USER_ID, maker);
        }

        private void SettingIDDone()
        {
            Invoke(new Action(
                        delegate ()
                        {
                            label1.Visible = false;
                            textBox1.Visible = false;
                            button1.Visible = false;

                            ReceiveTextBox.Enabled = true;
                            ReceiveTextBox.Visible = true;

                            SendTextBox.Enabled = true;
                            SendTextBox.Visible = true;

                            SendButton.Enabled = true;
                            SendButton.Visible = true;
                        }));
        }
    }
}
