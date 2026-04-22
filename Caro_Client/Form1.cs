using System;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Caro_Shared;

namespace Caro_Client
{
    public partial class Form1 : Form
    {
        TcpClient client;
        NetworkStream stream;
        string myName;
        Button[,] matrix;

        bool isMyTurn = true;
        bool isGameOver = false;

        int timeLeft = 30;
        const int MAX_TIME = 30;

        public Form1()
        {
            InitializeComponent();
            DrawChessBoard();

            // Sử dụng mrCountDown khớp với tên trong Designer của bạn
            mrCountDown.Interval = 1000;
            mrCountDown.Tick += MrCountDown_Tick;
        }

        private void MrCountDown_Tick(object sender, EventArgs e)
        {
            timeLeft--;
            lblTimer.Text = timeLeft + "s";
            prgTimer.Value = (int)((double)timeLeft / MAX_TIME * 100);

            if (timeLeft <= 0)
            {
                mrCountDown.Stop();
                isGameOver = true;
                ToggleBoard(false);
                MessageBox.Show("Hết giờ! Bạn đã thua.");

                if (MessageBox.Show("Bạn muốn phục thù không?", "Kết thúc", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    SendPacket(new Packet { Action = "RESTART_REQUEST", Sender = myName });
                }
            }
        }

        void DrawChessBoard()
        {
            pnlChessBoard.Controls.Clear();
            int rows = 20;
            int cols = 20;
            int size = 30;
            matrix = new Button[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Button btn = new Button()
                    {
                        Width = size,
                        Height = size,
                        Location = new Point(j * size, i * size),
                        Tag = i + "," + j
                    };
                    btn.Click += Btn_Click;
                    pnlChessBoard.Controls.Add(btn);
                    matrix[i, j] = btn;
                }
            }
        }

        void ResetBoard()
        {
            this.Invoke((MethodInvoker)(() => {
                foreach (Button btn in matrix)
                {
                    btn.Text = "";
                    btn.BackColor = default(Color);
                }
                isGameOver = false;
                timeLeft = MAX_TIME;
                prgTimer.Value = 100;
                lblTimer.Text = timeLeft + "s";
                ToggleBoard(true);
            }));
        }

        void ToggleBoard(bool state)
        {
            if (pnlChessBoard.InvokeRequired)
                pnlChessBoard.Invoke((MethodInvoker)(() => pnlChessBoard.Enabled = state));
            else
                pnlChessBoard.Enabled = state;
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            myName = txtPlayerName.Text.Trim();
            string serverIP = txtIP.Text.Trim();

            if (string.IsNullOrEmpty(myName) || string.IsNullOrEmpty(serverIP)) return;

            try
            {
                client = new TcpClient();
                await client.ConnectAsync(serverIP, 9999);
                stream = client.GetStream();
                SendPacket(new Packet { Action = "LOGIN", Sender = myName });

                btnConnect.Enabled = false;
                _ = Task.Run(() => ReceiveData());
            }
            catch { MessageBox.Show("Lỗi kết nối!"); }
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            if (isGameOver || stream == null || !isMyTurn) return;

            Button btn = sender as Button;
            if (btn.Text != "") return;

            btn.Text = "X";
            btn.ForeColor = Color.Red;
            mrCountDown.Stop(); // Dừng mrCountDown

            string[] pos = btn.Tag.ToString().Split(',');
            SendPacket(new Packet { Action = "MOVE", Sender = myName, X = int.Parse(pos[0]), Y = int.Parse(pos[1]) });

            if (isEndGame(btn))
            {
                isGameOver = true;
                ToggleBoard(false);
                MessageBox.Show("Chúc mừng! Bạn thắng!");
            }
            else
            {
                isMyTurn = false;
                ToggleBoard(false);
            }
        }

        async Task ReceiveData()
        {
            byte[] buffer = new byte[10240];
            while (true)
            {
                try
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string json = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    string[] packets = json.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var pStr in packets)
                    {
                        Packet p = JsonConvert.DeserializeObject<Packet>(pStr);
                        if (p.Action == "UPDATE_LIST") UpdateList(p.Message);
                        if (p.Action == "MOVE") MarkEnemy(p.X, p.Y);
                        if (p.Action == "RESTART_REQUEST")
                        {
                            this.Invoke((MethodInvoker)(() => {
                                if (MessageBox.Show($"Đối thủ [{p.Sender}] muốn chơi lại?", "Mời", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    SendPacket(new Packet { Action = "RESTART_ACCEPT" });
                                    ResetBoard(); isMyTurn = false;
                                }
                            }));
                        }
                        if (p.Action == "RESTART_ACCEPT")
                        {
                            this.Invoke((MethodInvoker)(() => {
                                MessageBox.Show("Ván mới bắt đầu!");
                                ResetBoard(); isMyTurn = true; mrCountDown.Start();
                            }));
                        }
                    }
                }
                catch { break; }
            }
        }

        void MarkEnemy(int x, int y)
        {
            this.Invoke((MethodInvoker)(() => {
                if (isGameOver) return;
                Button btn = matrix[x, y];
                btn.Text = "O";
                btn.ForeColor = Color.Blue;

                if (isEndGame(btn))
                {
                    isGameOver = true;
                    mrCountDown.Stop();
                    ToggleBoard(false);
                    if (MessageBox.Show("Thua rồi! Phục thù không?", "Kết thúc", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        SendPacket(new Packet { Action = "RESTART_REQUEST", Sender = myName });
                }
                else
                {
                    isMyTurn = true;
                    ToggleBoard(true);
                    timeLeft = MAX_TIME;
                    mrCountDown.Start();
                }
            }));
        }

        void SendPacket(Packet p)
        {
            try
            {
                string json = JsonConvert.SerializeObject(p) + "\n";
                byte[] data = Encoding.UTF8.GetBytes(json);
                stream.Write(data, 0, data.Length);
            }
            catch { }
        }

        void UpdateList(string list)
        {
            this.Invoke((MethodInvoker)(() => {
                lsvPlayers.Items.Clear();
                foreach (var user in list.Split(','))
                    if (user != myName && !string.IsNullOrEmpty(user)) lsvPlayers.Items.Add(user);
            }));
        }

        #region Logic Win
        bool isEndGame(Button btn) { return isWinHorizontal(btn) || isWinVertical(btn) || isWinPrimary(btn) || isWinSub(btn); }
        Point GetPoint(Button btn) { string[] pos = btn.Tag.ToString().Split(','); return new Point(int.Parse(pos[1]), int.Parse(pos[0])); }
        bool isWinHorizontal(Button btn) { Point p = GetPoint(btn); int c = 0; for (int i = p.X; i >= 0 && matrix[p.Y, i].Text == btn.Text; i--) c++; for (int i = p.X + 1; i < 20 && matrix[p.Y, i].Text == btn.Text; i++) c++; return c >= 5; }
        bool isWinVertical(Button btn) { Point p = GetPoint(btn); int c = 0; for (int i = p.Y; i >= 0 && matrix[i, p.X].Text == btn.Text; i--) c++; for (int i = p.Y + 1; i < 20 && matrix[i, p.X].Text == btn.Text; i++) c++; return c >= 5; }
        bool isWinPrimary(Button btn) { Point p = GetPoint(btn); int c = 0; for (int i = 0; p.Y - i >= 0 && p.X - i >= 0 && matrix[p.Y - i, p.X - i].Text == btn.Text; i++) c++; for (int i = 1; p.Y + i < 20 && p.X + i < 20 && matrix[p.Y + i, p.X + i].Text == btn.Text; i++) c++; return c >= 5; }
        bool isWinSub(Button btn) { Point p = GetPoint(btn); int c = 0; for (int i = 0; p.Y + i < 20 && p.X - i >= 0 && matrix[p.Y + i, p.X - i].Text == btn.Text; i++) c++; for (int i = 1; p.Y - i >= 0 && p.X + i < 20 && matrix[p.Y - i, p.X + i].Text == btn.Text; i++) c++; return c >= 5; }
        #endregion

        private void lsvPlayers_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}