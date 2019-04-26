using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Windows;

namespace OfficeCheckerWPF
{
    public static class Messages
    {
        public static void ErrorMessage(string message, string header = "Ошибка")
        {
            MessageBox.Show(message, header, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void WarningMessage(string message, string header = "Внимание")
        {
            MessageBox.Show(message, header, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void InfoMessage(string message, string header = "Информация")
        {
            MessageBox.Show(message, header, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// отправка исключения почтой на список адресов
        /// </summary>
        public static void ExaptionMailMessage(string exaption)
        {
            var eMails = new List<MailAddress>
            {
                new MailAddress("d.klishin@centrofinans.ru"),
            };

            var smtp = new SmtpClient("smtp.centrofinans.ru", 25)
            {
                Credentials = new NetworkCredential("tpbot@centrofinans.ru", "asd123ASD"),
                EnableSsl = true
            };

            foreach (var mailTo in eMails)
            {
                var msg = new MailMessage(new MailAddress("OfficeCheckerExaption@centrofinans.ru", $"OfficeCheckerExaption"), mailTo)
                {
                    Subject = $"OfficeCheckerExaption@centrofinans.ru",
                    Body = $"PCname: {Environment.MachineName}\r\n" +
                           $"Timestamp: {DateTime.Now:O}\r\n" +
                           $"Exaption: {exaption}"
                };
                try
                {
                    smtp.Send(msg);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    }
}