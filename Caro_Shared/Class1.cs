using System;

namespace Caro_Shared
{
    public class Packet
    {
        public string Action { get; set; } // LOGIN, UPDATE_LIST, MOVE
        public string Sender { get; set; }
        public string Message { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}