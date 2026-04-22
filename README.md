# LTM_Caro_Network_Game
Game Caro (Gomoku) theo mô hình **Client-Server** viết bằng **C# (.NET Windows Forms)**.

## Tổng quan

Project gồm 3 thành phần chính:

- **Caro_Client**: ứng dụng giao diện người chơi.
- **Caro_Server**: server trung gian xử lý kết nối.
- **Caro_Shared**: thư viện dùng chung giữa client và server.

## Cấu trúc thư mục

```text
LTM_Caro_Network_Game/
├── Caro_Client/
│   ├── Program.cs
│   ├── Form1.cs
│   └── Caro_Client.csproj
├── Caro_Server/
│   ├── Program.cs
│   └── Caro_Server.csproj
├── Caro_Shared/
│   ├── Class1.cs
│   └── Caro_Shared.csproj
└── CaroNetworkGame.sln
```

## Demo code

### 1) Server cơ bản (TCP listener)

```csharp
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class DemoServer
{
    static void Main()
    {
        var listener = new TcpListener(IPAddress.Any, 5000);
        listener.Start();
        Console.WriteLine("Server đang chạy tại cổng 5000...");

        while (true)
        {
            var client = listener.AcceptTcpClient();
            var stream = client.GetStream();

            byte[] buffer = new byte[1024];
            int read = stream.Read(buffer, 0, buffer.Length);
            string message = Encoding.UTF8.GetString(buffer, 0, read);
            Console.WriteLine($"Client gửi: {message}");

            byte[] response = Encoding.UTF8.GetBytes("ACK từ server");
            stream.Write(response, 0, response.Length);

            client.Close();
        }
    }
}
```

### 2) Client gửi nước đi

```csharp
using System;
using System.Net.Sockets;
using System.Text;

class DemoClient
{
    static void Main()
    {
        using var client = new TcpClient("127.0.0.1", 5000);
        using var stream = client.GetStream();

        string move = "MOVE:8,8,X";
        byte[] data = Encoding.UTF8.GetBytes(move);
        stream.Write(data, 0, data.Length);

        byte[] buffer = new byte[1024];
        int read = stream.Read(buffer, 0, buffer.Length);
        Console.WriteLine("Server phản hồi: " + Encoding.UTF8.GetString(buffer, 0, read));
    }
}
```

### 3) Logic kiểm tra thắng đơn giản (5 quân liên tiếp)

```csharp
public static bool CheckWin(char[,] board, int row, int col, char mark)
{
    int[][] directions =
    {
        new[] { 1, 0 },   // dọc
        new[] { 0, 1 },   // ngang
        new[] { 1, 1 },   // chéo chính
        new[] { 1, -1 }   // chéo phụ
    };

    int size = board.GetLength(0);

    foreach (var d in directions)
    {
        int count = 1;

        // tiến
        count += CountDirection(board, row, col, d[0], d[1], mark, size);
        // lùi
        count += CountDirection(board, row, col, -d[0], -d[1], mark, size);

        if (count >= 5) return true;
    }

    return false;
}

static int CountDirection(char[,] board, int r, int c, int dr, int dc, char mark, int size)
{
    int count = 0;
    r += dr;
    c += dc;

    while (r >= 0 && r < size && c >= 0 && c < size && board[r, c] == mark)
    {
        count++;
        r += dr;
        c += dc;
    }

    return count;
}
```

## Cách chạy project

### Bằng Visual Studio

1. Mở file `CaroNetworkGame.sln`.
2. Chạy `Caro_Server` trước.
3. Chạy một hoặc nhiều instance `Caro_Client` để test chơi mạng.

### Bằng .NET CLI

```bash
dotnet build CaroNetworkGame.sln
```

## Gợi ý mở rộng

- Đồng bộ trạng thái bàn cờ theo thời gian thực.
- Thêm tính năng tạo phòng / mời bạn.
- Thêm hệ thống chat in-game.
- Lưu lịch sử trận đấu.

---

Nếu bạn muốn, mình có thể tạo tiếp một bản README chi tiết hơn theo format:
**Giới thiệu → Kiến trúc gói tin → Sequence diagram → Hướng dẫn deploy LAN**.
