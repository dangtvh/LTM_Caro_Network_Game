using System;

namespace Caro_Shared
{
    public class Packet
    {
        public string Action { get; set; } // LOGIN, UPDATE_LIST, INVITE, MOVE...
        public string Sender { get; set; } // Dùng để xác định người gửi, hoặc người chơi thực hiện nước đi
        public string Receiver { get; set; } // Dùng để gửi lời mời, phản hồi lời mời, hoặc chuyển tiếp nước đi
        public string Message { get; set; } // Dùng để gửi danh sách người chơi, thông báo, hoặc các dữ liệu khác

        public string RoomId { get; set; } // Dùng để xác định phòng chơi
        public int X { get; set; }
        public int Y { get; set; }
    }
}