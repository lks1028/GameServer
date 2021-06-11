
namespace ChatServer
{
    partial class ChatServer
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
            this.ServerLogTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ReceiveTextBox
            // 
            this.ReceiveTextBox.Location = new System.Drawing.Point(12, 260);
            this.ReceiveTextBox.Multiline = true;
            this.ReceiveTextBox.Name = "ReceiveTextBox";
            this.ReceiveTextBox.ReadOnly = true;
            this.ReceiveTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ReceiveTextBox.Size = new System.Drawing.Size(602, 178);
            this.ReceiveTextBox.TabIndex = 0;
            // 
            // ServerLogTextBox
            // 
            this.ServerLogTextBox.Location = new System.Drawing.Point(12, 41);
            this.ServerLogTextBox.Multiline = true;
            this.ServerLogTextBox.Name = "ServerLogTextBox";
            this.ServerLogTextBox.ReadOnly = true;
            this.ServerLogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ServerLogTextBox.Size = new System.Drawing.Size(602, 178);
            this.ServerLogTextBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "서버 로그";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 245);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "메세지 로그";
            // 
            // ChatServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ServerLogTextBox);
            this.Controls.Add(this.ReceiveTextBox);
            this.Name = "ChatServer";
            this.Text = "ChatServer";
            this.Load += new System.EventHandler(this.ChatServer_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ReceiveTextBox;
        private System.Windows.Forms.TextBox ServerLogTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

