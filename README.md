# LTM Caro Network Game
 Đề tài demo môn Lập trình mạng: xây dựng game **Caro (Gomoku) 20x20** theo mô hình **Client–Server** bằng **C# / .NET 8**.
## 1) Giới thiệu đề tài
**LTM Caro Network Game** là ứng dụng chơi Caro qua mạng LAN, gồm giao diện người chơi (Windows Forms), máy chủ trung gian (TCP), thư viện dữ liệu dùng chung và module AI gợi ý nước đi.
Mục tiêu của đề tài:
- Triển khai mô hình truyền thông **TCP Socket** giữa nhiều client.
- Thiết kế giao thức gói tin JSON đơn giản, dễ mở rộng.
- Đồng bộ trạng thái ván đấu theo thời gian thực.
- Minh họa tích hợp AI heuristic hỗ trợ người chơi.

---

## 2) Kiến trúc hệ thống

Hệ thống gồm 4 project chính:

- `Caro_Server`: Server TCP nhận kết nối, quản lý danh sách người chơi và chuyển tiếp gói tin.
- `Caro_Client`: Ứng dụng WinForms hiển thị bàn cờ, xử lý lượt chơi, đếm giờ, phục thù.
- `Caro_Shared`: Định nghĩa lớp `Packet` dùng chung cho client/server.
- `Caro_AI`: Đưa ra gợi ý nước đi theo chiến lược tấn công/phòng thủ.


## 3)Cấu trúc thư mục

```text
LTM_Caro_Network_Game/
├── Caro_Client/            # UI người chơi (WinForms)
│   ├── Program.cs
│   ├── Form1.cs
│   └── Caro_Client.csproj
├── Caro_Server/            # TCP relay server
│   ├── Program.cs
│   └── Caro_Server.csproj
├── Caro_Shared/            # DTO/gói tin dùng chung
│   ├── Class1.cs
│   └── Caro_Shared.csproj
├── Caro_AI/                # Engine gợi ý nước đi
│   ├── MoveSuggestion.cs
│   ├── MoveSuggestionEngine.cs
│   └── Caro_AI.csproj
└── CaroNetworkGame.sln
```
### 4) Mô hình luồng xử lý
1. Client kết nối tới server qua `IP:9999`.
2. Client gửi `LOGIN` để đăng ký tên người chơi.
3. Server phát `UPDATE_LIST` cho tất cả client.
4. Khi người chơi đánh cờ, client gửi `MOVE`.
5. Server chuyển tiếp `MOVE` cho đối thủ.
6. Hai bên có thể gửi `RESTART_REQUEST` / `RESTART_ACCEPT` để chơi lại.
---
## 5) Công nghệ sử dụng
- **Ngôn ngữ**: C#
- **Nền tảng**: .NET 8
- **UI**: Windows Forms (`net8.0-windows`)
- **Networking**: `TcpListener`, `TcpClient`, `NetworkStream`
- **Serialization**: `Newtonsoft.Json`
- **Mô hình triển khai**: Client–Server trong LAN
---
## 6) Đặc tả chức năng demo

### 6.1 Chức năng phía Server
- Lắng nghe kết nối tại cổng `9999`.
- Quản lý danh sách client đang online theo tên đăng nhập.
- Broadcast danh sách người chơi khi có thay đổi.
- Chuyển tiếp các hành động gameplay:
  - `MOVE`
  - `RESTART_REQUEST`
  - `RESTART_ACCEPT`

### 6.2 Chức năng phía Client
- Kết nối server bằng IP + tên người chơi.
- Hiển thị bàn cờ **20x20**.
- Đánh quân `X`, nhận quân đối thủ `O` theo thời gian thực.
- Kiểm tra thắng thua theo 4 hướng (ngang/dọc/chéo chính/chéo phụ).
- Bộ đếm thời gian mỗi lượt (**30 giây**).
- Cơ chế phục thù (rematch).
- Nút **Suggest Move**: gợi ý nước đi tốt dựa trên AI heuristic.

### 6.3 Thành phần AI (Move Suggestion)
Chiến lược gợi ý:
1. Ưu tiên nước đi thắng ngay.
2. Nếu chưa thắng được, ưu tiên chặn nước thắng của đối thủ.
3. Nếu chưa có tình huống khẩn cấp, chấm điểm các ô ứng viên theo:
   - Mức độ tấn công.
   - Mức độ phòng thủ.
   - Dự báo phản công tốt nhất của đối thủ.
   - Khoảng cách tới trung tâm bàn cờ.
---

## 7) Thiết kế gói tin (Packet)
Dữ liệu trao đổi được chuẩn hóa qua lớp `Packet`:
- `Action`: loại hành động (`LOGIN`, `UPDATE_LIST`, `MOVE`, ...)
- `Sender`: tên người gửi
- `Message`: dữ liệu chuỗi bổ sung (ví dụ danh sách player)
- `X`, `Y`: tọa độ nước đi
Ví dụ gói tin JSON:
```json
{"Action":"MOVE","Sender":"playerA","X":8,"Y":12}
```

---
## 8) Demo code
### 8.1 Server nhận và chuyển tiếp packet
```csharp
if (p.Action == "LOGIN")
{
    clientName = p.Sender;
    clients[clientName] = client;
    BroadcastPlayerList();
}
else if (p.Action == "MOVE" || p.Action == "RESTART_REQUEST" || p.Action == "RESTART_ACCEPT")
{
    BroadcastPacket(p, clientName);
}
```

### 8.2 Client gửi nước đi
```csharp
string[] pos = btn.Tag.ToString().Split(',');
SendPacket(new Packet
{
    Action = "MOVE",
    Sender = myName,
    X = int.Parse(pos[0]),
    Y = int.Parse(pos[1])
});
```
### 8.3 Kiểm tra thắng theo 4 hướng
```csharp
bool isEndGame(Button btn)
return isWinHorizontal(btn)
        || isWinVertical(btn)
        || isWinPrimary(btn)
        || isWinSub(btn);
### 8.4 AI gợi ý nước đi
```csharp
MoveSuggestion? suggestion = MoveSuggestionEngine.SuggestMove(boardState, "X", "O");
if (suggestion == null)
    return new Point(-1, -1);
    return new Point(suggestion.Row, suggestion.Col);
```
### 8.5 Logic kiểm tra thắng (5 quân liên tiếp)

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
> Các đoạn code trên được trích từ code thực tế trong project để phục vụ trình bày báo cáo/demo.

---

## 9) Hướng dẫn chạy demo

## Yêu cầu môi trường
- Windows + .NET SDK 8.0+
- Visual Studio 2022 (khuyến nghị) hoặc .NET CLI

### Cách 1: Chạy bằng Visual Studio
1. Mở file `CaroNetworkGame.sln`.
2. Đặt `Caro_Server` làm startup và chạy trước.
3. Chạy 1–2 instance `Caro_Client` để mô phỏng 2 người chơi.
4. Nhập IP server (LAN hoặc `127.0.0.1`) và tên người chơi để kết nối.

### Cách 2: Chạy bằng .NET CLI
```bash
dotnet restore CaroNetworkGame.sln
dotnet build CaroNetworkGame.sln

# Terminal 1: chạy server
dotnet run --project Caro_Server/Caro_Server.csproj

# Terminal 2,3: chạy client (Windows)
dotnet run --project Caro_Client/Caro_Client.csproj
---

## 8) Kịch bản demo 
1. Khởi động server và 2 client.
2. Cả hai người chơi đăng nhập thành công, danh sách online cập nhật.
3. Thực hiện vài nước đi để thể hiện đồng bộ realtime.
4. Bấm **Suggest Move** để trình bày AI hỗ trợ.
5. Kết thúc ván, thực hiện **phục thù** để trình bày luồng `RESTART_*`.
6. Trình bày ngắn về kiến trúc, packet và ưu/nhược điểm.
```

## Tính năng AI gợi ý nước đi (trend AI thông minh hơn)

- Trong `Caro_Client`, có nút **"Gợi ý nước đi"** ở panel bên phải.
- Logic AI đã được tách sang project riêng `Caro_AI` để dễ mở rộng và test độc lập.
- Khi tới lượt bạn, bấm nút này để AI đề xuất một ô nên đánh.
- Cách AI chọn nước đi:
  1. Ưu tiên nước giúp bạn (`X`) thắng ngay.
  2. Nếu chưa có, ưu tiên chặn nước thắng ngay của đối thủ (`O`).
  3. Nếu chưa có tình huống khẩn cấp, AI dùng heuristic nâng cao:
     - Chấm điểm theo mẫu chuỗi quân (open 2/open 3/open 4, v.v.).
     - Giới hạn tập ứng viên ở các ô gần quân đã đánh để tính nhanh hơn.
     - Mô phỏng phản đòn tốt nhất của đối thủ và trừ điểm rủi ro.
     - Ưu tiên nhẹ khu vực trung tâm bàn cờ.
- Ô được gợi ý sẽ được tô **xanh nhạt** để bạn dễ nhận biết.