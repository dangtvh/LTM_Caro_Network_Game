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
            label1 = new Label();
            label2 = new Label();
            mrCountDown = new System.Windows.Forms.Timer(components);
            prgTimer = new ProgressBar();
            btnSuggestMove = new Button();
            btnInvite = new Button();
            lblRoom = new Label();
            SuspendLayout();
            // 
            // pnlChessBoard
            // 
            pnlChessBoard.Location = new Point(16, 128);
            pnlChessBoard.Margin = new Padding(4);
            pnlChessBoard.Name = "pnlChessBoard";
            pnlChessBoard.Size = new Size(780, 768);
            pnlChessBoard.TabIndex = 0;
            // 
            // txtIP
            // 
            txtIP.Location = new Point(69, 56);
            txtIP.Margin = new Padding(4);
            txtIP.Name = "txtIP";
            txtIP.Size = new Size(203, 39);
            txtIP.TabIndex = 1;
            txtIP.Text = "127.0.0.1";
            // 
            // txtPlayerName
            // 
            txtPlayerName.Location = new Point(353, 58);
            txtPlayerName.Margin = new Padding(4);
            txtPlayerName.Name = "txtPlayerName";
            txtPlayerName.Size = new Size(128, 39);
            txtPlayerName.TabIndex = 2;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(516, 58);
            btnConnect.Margin = new Padding(4);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(130, 37);
            btnConnect.TabIndex = 3;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // lsvPlayers
            // 
            lsvPlayers.FormattingEnabled = true;
            lsvPlayers.Location = new Point(803, 411);
            lsvPlayers.Margin = new Padding(4);
            lsvPlayers.Name = "lsvPlayers";
            lsvPlayers.Size = new Size(195, 388);
            lsvPlayers.TabIndex = 4;
            // 
            // lblTimer
            // 
            lblTimer.AutoSize = true;
            lblTimer.Location = new Point(845, 160);
            lblTimer.Margin = new Padding(4, 0, 4, 0);
            lblTimer.Name = "lblTimer";
            lblTimer.Size = new Size(109, 32);
            lblTimer.TabIndex = 5;
            lblTimer.Text = "thời gian";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(15, 58);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(33, 32);
            label1.TabIndex = 6;
            label1.Text = "IP";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(293, 58);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(52, 32);
            label2.TabIndex = 7;
            label2.Text = "Tên";
            // 
            // mrCountDown
            // 
            mrCountDown.Interval = 1000;
            // 
            // prgTimer
            // 
            prgTimer.Location = new Point(803, 255);
            prgTimer.Margin = new Padding(4);
            prgTimer.Name = "prgTimer";
            prgTimer.Size = new Size(195, 44);
            prgTimer.TabIndex = 8;
            // 
            // btnSuggestMove
            // 
            btnSuggestMove.Location = new Point(803, 334);
            btnSuggestMove.Name = "btnSuggestMove";
            btnSuggestMove.Size = new Size(195, 46);
            btnSuggestMove.TabIndex = 9;
            btnSuggestMove.Text = "Gợi ý nước đi";
            btnSuggestMove.UseVisualStyleBackColor = true;
            btnSuggestMove.Click += btnSuggestMove_Click;
            // 
            // btnInvite
            // 
            btnInvite.Location = new Point(674, 58);
            btnInvite.Name = "btnInvite";
            btnInvite.Size = new Size(122, 37);
            btnInvite.TabIndex = 10;
            btnInvite.Text = "Mời Vào ";
            btnInvite.UseVisualStyleBackColor = true;
            btnInvite.Click += btnInvite_Click;
            // 
            // lblRoom
            // 
            lblRoom.AutoSize = true;
            lblRoom.Location = new Point(777, 9);
            lblRoom.Name = "lblRoom";
            lblRoom.Size = new Size(251, 32);
            lblRoom.TabIndex = 11;
            lblRoom.Text = "Phòng: Chưa tham gia";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1040, 922);
            Controls.Add(lblRoom);
            Controls.Add(btnInvite);
            Controls.Add(btnSuggestMove);
            Controls.Add(prgTimer);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(lblTimer);
            Controls.Add(lsvPlayers);
            Controls.Add(btnConnect);
            Controls.Add(txtPlayerName);
            Controls.Add(txtIP);
            Controls.Add(pnlChessBoard);
            Margin = new Padding(4);
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
        private Label label1;
        private Label label2;
        private System.Windows.Forms.Timer mrCountDown;
        private ProgressBar prgTimer;
        private Button btnSuggestMove;
        private Button btnInvite;
        private Label lblRoom;
    }
}