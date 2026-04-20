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
        Button[,] matrix; // Ma trận để quản lý các ô cờ dễ dàng hơn

        public Form1()
        {
            InitializeComponent();
            DrawChessBoard();
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

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            myName = txtPlayerName.Text.Trim();
            if (string.IsNullOrEmpty(myName)) return;

            try
            {
                client = new TcpClient();
                await client.ConnectAsync("127.0.0.1", 9999);
                stream = client.GetStream();

                SendPacket(new Packet { Action = "LOGIN", Sender = myName });

                btnConnect.Enabled = false;
                txtPlayerName.ReadOnly = true;
                _ = Task.Run(() => ReceiveData());
            }
            catch { MessageBox.Show("Lỗi kết nối Server!"); }
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Text != "" || stream == null) return;

            // Đánh quân mình (X)
            btn.Text = "X";
            btn.ForeColor = Color.Red;

            string[] pos = btn.Tag.ToString().Split(',');
            int x = int.Parse(pos[0]);
            int y = int.Parse(pos[1]);

            // Gửi tọa độ lên Server
            SendPacket(new Packet { Action = "MOVE", Sender = myName, X = x, Y = y });

            // Kiểm tra xem mình đánh nước này xong có thắng không
            if (isEndGame(btn))
            {
                MessageBox.Show("Bạn đã thắng!");
            }
        }

        async Task ReceiveData()
        {
            byte[] buffer = new byte[1024 * 10];
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
                    }
                }
                catch { break; }
            }
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

        void MarkEnemy(int x, int y)
        {
            this.Invoke((MethodInvoker)(() => {
                Button btn = matrix[x, y];
                btn.Text = "O";
                btn.ForeColor = Color.Blue;

                if (isEndGame(btn))
                {
                    MessageBox.Show("Đối thủ đã thắng rồi!");
                }
            }));
        }

        #region Logic Kiểm tra thắng thua
        bool isEndGame(Button btn)
        {
            return isWinHorizontal(btn) || isWinVertical(btn) || isWinPrimary(btn) || isWinSub(btn);
        }

        Point GetPoint(Button btn)
        {
            string[] pos = btn.Tag.ToString().Split(',');
            return new Point(int.Parse(pos[1]), int.Parse(pos[0]));
        }

        bool isWinHorizontal(Button btn) // Kiểm tra hàng ngang
        {
            Point point = GetPoint(btn);
            int count = 0;
            for (int i = point.X; i >= 0; i--)
            {
                if (matrix[point.Y, i].Text == btn.Text) count++;
                else break;
            }
            for (int i = point.X + 1; i < 20; i++)
            {
                if (matrix[point.Y, i].Text == btn.Text) count++;
                else break;
            }
            return count >= 5;
        }

        bool isWinVertical(Button btn) // Kiểm tra hàng dọc
        {
            Point point = GetPoint(btn);
            int count = 0;
            for (int i = point.Y; i >= 0; i--)
            {
                if (matrix[i, point.X].Text == btn.Text) count++;
                else break;
            }
            for (int i = point.Y + 1; i < 20; i++)
            {
                if (matrix[i, point.X].Text == btn.Text) count++;
                else break;
            }
            return count >= 5;
        }

        bool isWinPrimary(Button btn) // Đường chéo chính
        {
            Point point = GetPoint(btn);
            int count = 0;
            for (int i = 0; i <= point.X && i <= point.Y; i++)
            {
                if (matrix[point.Y - i, point.X - i].Text == btn.Text) count++;
                else break;
            }
            for (int i = 1; i < 20 - point.X && i < 20 - point.Y; i++)
            {
                if (matrix[point.Y + i, point.X + i].Text == btn.Text) count++;
                else break;
            }
            return count >= 5;
        }

        bool isWinSub(Button btn) // Đường chéo phụ
        {
            Point point = GetPoint(btn);
            int count = 0;
            for (int i = 0; i <= point.X && point.Y + i < 20; i++)
            {
                if (matrix[point.Y + i, point.X - i].Text == btn.Text) count++;
                else break;
            }
            for (int i = 1; i <= point.Y && point.X + i < 20; i++)
            {
                if (matrix[point.Y - i, point.X + i].Text == btn.Text) count++;
                else break;
            }
            return count >= 5;
        }
        #endregion
    }
}