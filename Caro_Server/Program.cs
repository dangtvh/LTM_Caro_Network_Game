using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Caro_Shared;
using System.Linq;

namespace Caro_Server
{
    class Program
    {
        static Dictionary<string, TcpClient> clients = new Dictionary<string, TcpClient>();
        static Dictionary<string, string> userRooms = new Dictionary<string, string>();
        static Dictionary<string, HashSet<string>> rooms = new Dictionary<string, HashSet<string>>();

        static async Task Main(string[] args)
        {
            TcpListener server = new TcpListener(IPAddress.Any, 9999);
            server.Start();
            Console.WriteLine("Server Caro da san sang tai Port 9999...");

            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                _ = Task.Run(() => HandleClient(client));
            }
        }

        static async Task HandleClient(TcpClient client)
        {
            string clientName = null;
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024 * 10];

            try
            {
                while (true)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string json = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    string[] packets = json.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var pStr in packets)
                    {
                        Packet p = JsonConvert.DeserializeObject<Packet>(pStr);
                        if (p == null) continue;

                        if (p.Action == "LOGIN")
                        {
                            clientName = p.Sender;
                            clients[clientName] = client;
                            Console.WriteLine($"{clientName} da tham gia.");
                            BroadcastPlayerList();
                        }
                        // --- ĐOẠN SỬA ĐỔI Ở ĐÂY ---
                        // Cho phép Server chuyển tiếp tất cả các lệnh về nước đi và yêu cầu chơi lại
                        else if (p.Action == "CREATE_ROOM")
                        {
                            CreateRoomAndInvite(p);
                        }
                        else if (p.Action == "INVITE_RESPONSE")
                        {
                            HandleInviteResponse(p);
                        }
                        else if (p.Action == "MOVE" || p.Action == "RESTART_REQUEST" || p.Action == "RESTART_ACCEPT")
                        {
                            Console.WriteLine($"Lenh [{p.Action}] tu {p.Sender}");
                            ForwardPacketInRoom(p);
                        }
                    }
                }
            }
            catch { }
            finally
            {
                if (clientName != null)
                {
                    RemoveFromRoom(clientName);
                    clients.Remove(clientName);
                    Console.WriteLine($"{clientName} da thoat.");
                    BroadcastPlayerList();
                }
                client.Close();
            }
        }

        static void BroadcastPlayerList()
        {
            var availableUsers = clients.Keys.Where(u => !userRooms.ContainsKey(u));
            string list = string.Join(",", availableUsers);
            Packet p = new Packet { Action = "UPDATE_LIST", Message = list };
            BroadcastPacket(p, null);
        }

        static void CreateRoomAndInvite(Packet p)
        {
            if (string.IsNullOrWhiteSpace(p.Sender) || string.IsNullOrWhiteSpace(p.Receiver)) return;
            if (!clients.ContainsKey(p.Receiver)) return;
            if (userRooms.ContainsKey(p.Sender) || userRooms.ContainsKey(p.Receiver)) return;

            string roomId = Guid.NewGuid().ToString("N");
            rooms[roomId] = new HashSet<string> { p.Sender };
            userRooms[p.Sender] = roomId;

            SendToClient(p.Receiver, new Packet
            {
                Action = "INVITE",
                Sender = p.Sender,
                Receiver = p.Receiver,
                RoomId = roomId,
                Message = $"{p.Sender} mời bạn vào phòng."
            });
            BroadcastPlayerList();
        }

        static void HandleInviteResponse(Packet p)
        {
            if (string.IsNullOrWhiteSpace(p.RoomId) || !rooms.ContainsKey(p.RoomId)) return;
            if (!rooms[p.RoomId].Contains(p.Receiver)) return;

            if (p.Message == "ACCEPT")
            {
                rooms[p.RoomId].Add(p.Sender);
                userRooms[p.Sender] = p.RoomId;

                SendToClient(p.Receiver, new Packet { Action = "ROOM_JOINED", RoomId = p.RoomId, Message = "HOST" });
                SendToClient(p.Sender, new Packet { Action = "ROOM_JOINED", RoomId = p.RoomId, Message = "GUEST" });
            }
            else
            {
                SendToClient(p.Receiver, new Packet { Action = "INVITE_DECLINED", Sender = p.Sender });
                rooms.Remove(p.RoomId);
                userRooms.Remove(p.Receiver);
            }

            BroadcastPlayerList();
        }

        static void ForwardPacketInRoom(Packet p)
        {
            if (string.IsNullOrWhiteSpace(p.Sender)) return;
            if (!userRooms.TryGetValue(p.Sender, out string roomId)) return;
            if (!rooms.TryGetValue(roomId, out HashSet<string> users)) return;

            p.RoomId = roomId;
            foreach (var user in users)
            {
                if (user == p.Sender) continue;
                SendToClient(user, p);
            }
        }

        static void RemoveFromRoom(string user)
        {
            if (!userRooms.TryGetValue(user, out string roomId)) return;
            userRooms.Remove(user);
            if (!rooms.TryGetValue(roomId, out HashSet<string> members)) return;

            members.Remove(user);
            foreach (var other in members)
            {
                userRooms.Remove(other);
                SendToClient(other, new Packet { Action = "ROOM_CLOSED", Message = $"{user} đã rời phòng." });
            }
            rooms.Remove(roomId);
        }

        static void SendToClient(string user, Packet p)
        {
            if (!clients.TryGetValue(user, out TcpClient target)) return;
            string json = JsonConvert.SerializeObject(p) + "\n";
            byte[] data = Encoding.UTF8.GetBytes(json);
            try { target.GetStream().Write(data, 0, data.Length); } catch { }
        }

        static void BroadcastPacket(Packet p, string excludeClient)
        {
            string json = JsonConvert.SerializeObject(p) + "\n";
            byte[] data = Encoding.UTF8.GetBytes(json);
            foreach (var item in clients)
            {
                if (item.Key != excludeClient)
                {
                    try { item.Value.GetStream().Write(data, 0, data.Length); } catch { }
                }
            }
        }
    }
}