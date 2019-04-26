using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml;

namespace OfficeCheckerWPF
{
    public partial class StreamWindow
    {
        public StreamWindow()
        {
            InitializeComponent();
        }

        private void StreamWindowLoaded(object sender, RoutedEventArgs e)
        {
            StreamLabel.Content = GetCurrentStreamString();
            StreamImage.Source = new BitmapImage(new Uri(@"http://127.0.0.1:8080/livefeed?oid=2"));
        }

        /// <summary>
        /// получение строки потока из конфига xml
        /// </summary>
        /// <returns></returns>
        private static string GetCurrentStreamString()
        {
            var str = "";
            var xmlPath = Environment.ExpandEnvironmentVariables(@"%APPDATA%\iSpy\XML\objects.xml");
            if (!File.Exists(xmlPath))
                return str;
            var sr = new StreamReader(xmlPath, true);
            var doc = new XmlDocument();
            doc.Load(sr);
            var node = doc.SelectSingleNode("//camera[@id = '1']/settings/videosourcestring");
            if (node != null)
                str = node.InnerText;
            sr.Close();
            return str;
        }
    }
}