using System;
using System.Collections.Generic;
using System.Net;

namespace OfficeCheckerWPF
{
     public static class AlternateRunsGpi
    {
        /// <summary>
        /// запуск процесса get pc info
        /// </summary>
        public static void Run()
        {
            var pcName = Utils.GetPcName();
            var osInfo = Utils.GetOsInformation();
            var is6432 = Utils.GetIs64Or32();
            var correctPcName = Utils.CheckPcNameOnСorrectness(pcName, Utils.GetVpnCertName()).ToString();
            var userUser = Utils.CheckUserUser().ToString();
            var userCentrofinans = Utils.CheckUserCentrofinans().ToString();
            var incorrectUsers = Utils.GetIncorrectUsers();
            var notInstalledSoftware = GetNotInstalledSoftware();
            var timezone = TimeZoneInfo.Local.ToString();
            var wuauservServiceStatus = Utils.GetServiceStatus("wuauserv");
            var iscVersion = Utils.GetIscVersionAsString();
            var frameworkVerson = Utils.GetFrameworkReleaseVersion();
            var iscTask = Utils.CheckIscTask().ToString();

            SendToApi(pcName, osInfo, is6432, correctPcName, userUser, userCentrofinans, incorrectUsers,
                notInstalledSoftware, timezone, wuauservServiceStatus, iscVersion, frameworkVerson, iscTask);
        }

        /// <summary>
        /// веб запрос к апи
        /// </summary>
        private static void SendToApi(
            string pcName, string osInfo, string is6432, string correctPcName, string userUser, string userCentrofinans,
            string incorrectUsers, string notInstalledSoftware, string timezone, string wuauservServiceStatus,
            string iscVersion, string frameworkVerson, string iscTask
            )
        {
            var sendData = $@"http://172.17.203.91:12345/api/gpi";
            sendData += $"?pcName={pcName}";
            sendData += $"&osInfo={osInfo}";
            sendData += $"&is6432={is6432}";
            sendData += $"&correctPcName={correctPcName}";
            sendData += $"&userUser={userUser}";
            sendData += $"&userCentrofinans={userCentrofinans}";
            sendData += $"&incorrectUsers={incorrectUsers}";
            sendData += $"&notInstalledSoftware={notInstalledSoftware}";
            sendData += $"&timezone={timezone}";
            sendData += $"&wuauservServiceStatus={wuauservServiceStatus}";
            sendData += $"&iscVersion={iscVersion}";
            sendData += $"&frameworkVerson={frameworkVerson}";
            sendData += $"&iscTask={iscTask}";

            var request = WebRequest.Create(sendData);
            request.ContentType = "application/text";
            try
            {
                request.GetResponse();
            }
            catch (Exception)
            {
                Environment.Exit(-1);
            }

            //using (var doc = new StreamWriter(@"D:\asdasd.txt",true))
            //{
            //    doc.WriteLine(sendData);
            //}
        }

        /// <summary>
        /// получить перечень не установленного софта из основной коллекции
        /// </summary>
        private static string GetNotInstalledSoftware()
        {
            Utils.CheckSofrware(MainData.Programs, Utils.GetProgramNames(MainData.Programs));
            var temList = new List<string>();
            foreach (var prog in MainData.Programs)
            {
                if (!prog.Installed)
                {
                    temList.Add(prog.TitleName);
                }
            }
            return temList.ListToString();
        }
    }
}