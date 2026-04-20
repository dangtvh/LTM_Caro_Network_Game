namespace Caro_Client
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pnlChessBoard = new Panel();
            txtIP = new TextBox();
            txtPlayerName = new TextBox();
            btnConnect = new Button();
            lsvPlayers = new ListBox();
            lblTimer = new Label();
            tmCooldown = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // pnlChessBoard
            // 
            pnlChessBoard.Location = new Point(12, 100);
            pnlChessBoard.Name = "pnlChessBoard";
            pnlChessBoard.Size = new Size(600, 600);
            pnlChessBoard.TabIndex = 0;
            // 
            // txtIP
            // 
            txtIP.Location = new Point(78, 42);
            txtIP.Name = "txtIP";
            txtIP.Size = new Size(157, 31);
            txtIP.TabIndex = 1;
            txtIP.Text = "127.0.0.1";
            // 
            // txtPlayerName
            // 
            txtPlayerName.Location = new Point(321, 42);
            txtPlayerName.Name = "txtPlayerName";
            txtPlayerName.Size = new Size(150, 31);
            txtPlayerName.TabIndex = 2;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(584, 42);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(112, 34);
            btnConnect.TabIndex = 3;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // lsvPlayers
            // 
            lsvPlayers.FormattingEnabled = true;
            lsvPlayers.ItemHeight = 25;
            lsvPlayers.Location = new Point(630, 100);
            lsvPlayers.Name = "lsvPlayers";
            lsvPlayers.Size = new Size(150, 304);
            lsvPlayers.TabIndex = 4;
            // 
            // lblTimer
            // 
            lblTimer.AutoSize = true;
            lblTimer.Location = new Point(500, 45);
            lblTimer.Name = "lblTimer";
            lblTimer.Size = new Size(82, 25);
            lblTimer.TabIndex = 5;
            lblTimer.Text = "thời gian";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 720);
            Controls.Add(lblTimer);
            Controls.Add(lsvPlayers);
            Controls.Add(btnConnect);
            Controls.Add(txtPlayerName);
            Controls.Add(txtIP);
            Controls.Add(pnlChessBoard);
            Name = "Form1";
            Text = "Caro Network Game";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        // CÁC DÒNG KHAI BÁO DƯỚI ĐÂY LÀ BẮT BUỘC ĐỂ KHÔNG BỊ LỖI
        private Panel pnlChessBoard;
        private TextBox txtIP;
        private TextBox txtPlayerName;
        private Button btnConnect;
        private ListBox lsvPlayers;
        private Label lblTimer;
        private System.Windows.Forms.Timer tmCooldown;
    }
}