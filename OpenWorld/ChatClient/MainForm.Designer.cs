
namespace ChatClient
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.ReceiveTextBox = new System.Windows.Forms.TextBox();
            this.SendTextBox = new System.Windows.Forms.TextBox();
            this.SendButton = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.UserListBox = new System.Windows.Forms.TextBox();
            this.RoomCountAndRoomName = new System.Windows.Forms.Label();
            this.CreateRoomButton = new System.Windows.Forms.Button();
            this.JoinRoomButton = new System.Windows.Forms.Button();
            this.RoomUserCount = new System.Windows.Forms.Label();
            this.RoomListBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ReceiveTextBox
            // 
            this.ReceiveTextBox.Enabled = false;
            this.ReceiveTextBox.Location = new System.Drawing.Point(12, 38);
            this.ReceiveTextBox.Multiline = true;
            this.ReceiveTextBox.Name = "ReceiveTextBox";
            this.ReceiveTextBox.ReadOnly = true;
            this.ReceiveTextBox.Size = new System.Drawing.Size(776, 389);
            this.ReceiveTextBox.TabIndex = 0;
            this.ReceiveTextBox.Visible = false;
            // 
            // SendTextBox
            // 
            this.SendTextBox.Enabled = false;
            this.SendTextBox.Location = new System.Drawing.Point(12, 443);
            this.SendTextBox.Name = "SendTextBox";
            this.SendTextBox.Size = new System.Drawing.Size(615, 21);
            this.SendTextBox.TabIndex = 1;
            this.SendTextBox.Visible = false;
            // 
            // SendButton
            // 
            this.SendButton.Enabled = false;
            this.SendButton.Location = new System.Drawing.Point(646, 434);
            this.SendButton.Name = "SendButton";
            this.SendButton.Size = new System.Drawing.Size(142, 30);
            this.SendButton.TabIndex = 2;
            this.SendButton.Text = "전송";
            this.SendButton.UseVisualStyleBackColor = true;
            this.SendButton.Visible = false;
            this.SendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(250, 216);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(108, 21);
            this.textBox1.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(382, 210);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(142, 30);
            this.button1.TabIndex = 4;
            this.button1.Text = "접속";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(248, 201);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "서버접속아이디";
            // 
            // UserListBox
            // 
            this.UserListBox.Enabled = false;
            this.UserListBox.Location = new System.Drawing.Point(794, 38);
            this.UserListBox.Multiline = true;
            this.UserListBox.Name = "UserListBox";
            this.UserListBox.ReadOnly = true;
            this.UserListBox.Size = new System.Drawing.Size(169, 389);
            this.UserListBox.TabIndex = 6;
            this.UserListBox.Visible = false;
            // 
            // RoomCountAndRoomName
            // 
            this.RoomCountAndRoomName.AutoSize = true;
            this.RoomCountAndRoomName.Location = new System.Drawing.Point(12, 23);
            this.RoomCountAndRoomName.Name = "RoomCountAndRoomName";
            this.RoomCountAndRoomName.Size = new System.Drawing.Size(101, 12);
            this.RoomCountAndRoomName.TabIndex = 7;
            this.RoomCountAndRoomName.Text = "방개수또는방이름";
            // 
            // CreateRoomButton
            // 
            this.CreateRoomButton.Enabled = false;
            this.CreateRoomButton.Location = new System.Drawing.Point(646, 470);
            this.CreateRoomButton.Name = "CreateRoomButton";
            this.CreateRoomButton.Size = new System.Drawing.Size(142, 30);
            this.CreateRoomButton.TabIndex = 8;
            this.CreateRoomButton.Text = "방 생성";
            this.CreateRoomButton.UseVisualStyleBackColor = true;
            this.CreateRoomButton.Visible = false;
            this.CreateRoomButton.Click += new System.EventHandler(this.CreateRoom_Click);
            // 
            // JoinRoomButton
            // 
            this.JoinRoomButton.Enabled = false;
            this.JoinRoomButton.Location = new System.Drawing.Point(646, 506);
            this.JoinRoomButton.Name = "JoinRoomButton";
            this.JoinRoomButton.Size = new System.Drawing.Size(142, 30);
            this.JoinRoomButton.TabIndex = 9;
            this.JoinRoomButton.Text = "방 접속";
            this.JoinRoomButton.UseVisualStyleBackColor = true;
            this.JoinRoomButton.Visible = false;
            this.JoinRoomButton.Click += new System.EventHandler(this.JoinRoomButton_Click);
            // 
            // RoomUserCount
            // 
            this.RoomUserCount.AutoSize = true;
            this.RoomUserCount.Location = new System.Drawing.Point(801, 23);
            this.RoomUserCount.Name = "RoomUserCount";
            this.RoomUserCount.Size = new System.Drawing.Size(97, 12);
            this.RoomUserCount.TabIndex = 10;
            this.RoomUserCount.Text = "접속한 유저 수 : ";
            // 
            // RoomListBox
            // 
            this.RoomListBox.Enabled = false;
            this.RoomListBox.Location = new System.Drawing.Point(12, 38);
            this.RoomListBox.Multiline = true;
            this.RoomListBox.Name = "RoomListBox";
            this.RoomListBox.ReadOnly = true;
            this.RoomListBox.Size = new System.Drawing.Size(776, 389);
            this.RoomListBox.TabIndex = 11;
            this.RoomListBox.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(985, 551);
            this.Controls.Add(this.RoomListBox);
            this.Controls.Add(this.RoomUserCount);
            this.Controls.Add(this.JoinRoomButton);
            this.Controls.Add(this.CreateRoomButton);
            this.Controls.Add(this.RoomCountAndRoomName);
            this.Controls.Add(this.UserListBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.SendButton);
            this.Controls.Add(this.SendTextBox);
            this.Controls.Add(this.ReceiveTextBox);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ReceiveTextBox;
        private System.Windows.Forms.TextBox SendTextBox;
        private System.Windows.Forms.Button SendButton;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox UserListBox;
        private System.Windows.Forms.Label RoomCountAndRoomName;
        private System.Windows.Forms.Button CreateRoomButton;
        private System.Windows.Forms.Button JoinRoomButton;
        private System.Windows.Forms.Label RoomUserCount;
        private System.Windows.Forms.TextBox RoomListBox;
    }
}

