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
        string userID = string.Empty;

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
            sender1.settingRoomListCallback += SettingRoomList;
            sender1.createRoomCallback += CreateRoom;
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
            builder.Append(userID);
            builder.Append(" : ");
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

            userID = textBox1.Text;

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

                            CreateRoomButton.Enabled = true;
                            CreateRoomButton.Visible = true;

                            JoinRoomButton.Enabled = true;
                            JoinRoomButton.Visible = true;

                            //SendButton.Enabled = true;
                            //SendButton.Visible = true;
                        }));
        }

        private void SettingRoomList(string msg)
        {
            Invoke(new Action(
                        delegate ()
                        {
                            string[] data = msg.Split('#');

                            // rooms의 0번째는 무조건 총 방의 개수이다.
                            RoomCountAndRoomName.Text = "현재 생성된 방의 수 : " + data[0];

                            if (data[0] != "0")
                            {
                                for (int i = 1; i < data.Length; i++)
                                {
                                    ReceiveTextBox.AppendText(data[i]);
                                }
                            }
                        }));
        }

        private void CreateRoom_Click(object sender, EventArgs e)
        {
            PacketMaker maker = new PacketMaker();
            maker.SetMsgLength(Encoding.UTF8.GetByteCount(SendTextBox.Text));
            maker.SetCommand((int)COMMAND.CREATE_ROOM);
            maker.SetStringData(SendTextBox.Text);

            sender1.SendPacket(COMMAND.CREATE_ROOM, maker);

            SendTextBox.Text = string.Empty;
        }

        private void CreateRoom(string msg)
        {
            Invoke(new Action(
                        delegate ()
                        {
                            string[] data = msg.Split('#');

                            // rooms의 0번째는 무조건 총 방의 개수이다.
                            RoomCountAndRoomName.Text = msg;

                            CreateRoomButton.Enabled = false;
                            CreateRoomButton.Visible = false;

                            JoinRoomButton.Enabled = false;
                            JoinRoomButton.Visible = false;

                            SendButton.Enabled = true;
                            SendButton.Visible = true;
                        }));
        }

        private void JoinRoomButton_Click(object sender, EventArgs e)
        {
            PacketMaker maker = new PacketMaker();
            maker.SetMsgLength(BitConverter.GetBytes(int.Parse(SendTextBox.Text)).Length);
            maker.SetCommand((int)COMMAND.JOIN_ROOM);
            maker.SetIntData(int.Parse(SendTextBox.Text));
            //maker.SetStringData(SendTextBox.Text);

            sender1.SendPacket(COMMAND.JOIN_ROOM, maker);

            SendTextBox.Text = string.Empty;
        }
    }
}
