namespace OfficeCheckerWPF
{
    internal class PingData
    {
        public string ServerName;
        public int AvgPing;
        public int PacketLost;

        public PingData(string serverName, int avgPing, int packetLost)
        {
            ServerName = serverName;
            AvgPing = avgPing;
            PacketLost = packetLost;
        }

        public override string ToString()
        {
            return $"{ServerName} - PingAVG: {AvgPing} - Lost: {PacketLost}\n";
        }
    }
}
