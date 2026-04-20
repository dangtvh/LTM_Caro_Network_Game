using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Caro_Shared;

namespace Caro_Server
{
    class Program
    {
        static Dictionary<string, TcpClient> clients = new Dictionary<string, TcpClient>();

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
                        else if (p.Action == "MOVE")
                        {
                            Console.WriteLine($"{p.Sender} danh tai: {p.X},{p.Y}");
                            // Gửi nước đi cho tất cả mọi người TRỪ người vừa đánh
                            BroadcastPacket(p, clientName);
                        }
                    }
                }
            }
            catch { }
            finally
            {
                if (clientName != null)
                {
                    clients.Remove(clientName);
                    Console.WriteLine($"{clientName} da thoat.");
                    BroadcastPlayerList();
                }
                client.Close();
            }
        }

        static void BroadcastPlayerList()
        {
            string list = string.Join(",", clients.Keys);
            Packet p = new Packet { Action = "UPDATE_LIST", Message = list };
            BroadcastPacket(p, null);
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