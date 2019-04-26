using System.Windows;
using System.Windows.Controls;

namespace OfficeCheckerWPF
{
    public class Servers
    {
        public readonly string ServerName;
        public readonly string ServerIpAddress;
        public Label NameLabel;
        public Label PingValueLabel;

        public Servers(string serverName, string serverIpAddress)
        {
            ServerName = serverName;
            ServerIpAddress = serverIpAddress;
            NameLabel = new Label {Content = ServerName};
            PingValueLabel = new Label {FontWeight = FontWeights.Bold, Width = 40, HorizontalContentAlignment = HorizontalAlignment.Center};
        }
    }
}
