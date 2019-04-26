using System;
using System.ComponentModel;

namespace OfficeCheckerWPF
{
    public partial class LogConsole
    {
        public LogConsole()
        {
            InitializeComponent();
        }

        /// <summary>
        /// добавление лога в консоль
        /// </summary>
        public void AddLog(string message)
        {
            var time = DateTime.Now.ToString("HH:mm:ss.fffffff");
            OutputBlock.Inlines.Add($"{time} {message}\r\n");
        }

        private void LogConsoleOnClosing(object sender, CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }
    }
}