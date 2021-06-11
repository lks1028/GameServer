
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
            this.SuspendLayout();
            // 
            // ReceiveTextBox
            // 
            this.ReceiveTextBox.Location = new System.Drawing.Point(12, 12);
            this.ReceiveTextBox.Multiline = true;
            this.ReceiveTextBox.Name = "ReceiveTextBox";
            this.ReceiveTextBox.ReadOnly = true;
            this.ReceiveTextBox.Size = new System.Drawing.Size(776, 389);
            this.ReceiveTextBox.TabIndex = 0;
            // 
            // SendTextBox
            // 
            this.SendTextBox.Location = new System.Drawing.Point(12, 417);
            this.SendTextBox.Name = "SendTextBox";
            this.SendTextBox.Size = new System.Drawing.Size(615, 21);
            this.SendTextBox.TabIndex = 1;
            // 
            // SendButton
            // 
            this.SendButton.Location = new System.Drawing.Point(646, 408);
            this.SendButton.Name = "SendButton";
            this.SendButton.Size = new System.Drawing.Size(142, 30);
            this.SendButton.TabIndex = 2;
            this.SendButton.Text = "전송";
            this.SendButton.UseVisualStyleBackColor = true;
            this.SendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
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
    }
}

