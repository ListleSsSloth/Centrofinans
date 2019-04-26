using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace OfficeCheckerWPF
{
    public static class Utils
    {
        /// <summary>
        /// получение имени метода из которого запущен метод
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod()
        {
            var sf = new StackTrace().GetFrame(1);
            return sf.GetMethod().Name;
        }

        /// TODO убрать копипаст
        /// <summary>
        /// проверка установки отдельной программы программы в реестре
        /// </summary>
        public static bool CheckProgramInstall(string pName)
        {
            var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            var key = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            if (key == null)
            {
                MessageBox.Show("Test", "Ошибка чтения реестра");
                return false;
            }
            foreach (var keyName in key.GetSubKeyNames())
            {
                var subKey = (string)key.OpenSubKey(keyName)?.GetValue("DisplayName");
                if (subKey == null)
                    continue;
                if (subKey.Contains(pName))
                    return true;
            }
            localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            key = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            if (key == null)
            {
                MessageBox.Show("Test", "Ошибка чтения реестра");
                return false;
            }
            foreach (var keyName in key.GetSubKeyNames())
            {
                var subKey = (string)key.OpenSubKey(keyName)?.GetValue("DisplayName");
                if (subKey == null)
                    continue;
                if (subKey.Contains(pName))
                    return true;
            }
            return false;
        }

        #region Check Soft

        /// <summary>
        /// получение списка сеарч имен проверяемых на установку программ
        /// </summary>
        /// <param name="prList"></param>
        /// <returns></returns>
        public static List<string> GetProgramNames(List<ProgramInfo> prList)
        {
            return prList.Select(pr => pr.SearchName).ToList();
        }

        /// <summary>
        /// проверка списка софта в обоих ветках методом CheckSoft
        /// </summary>
        public static void CheckSofrware(List<ProgramInfo> softList, List<string> programSearchNames)
        {
            var list = CheckSoft(softList, programSearchNames);
            if (list != null)
                CheckSoft(softList, programSearchNames, RegistryView.Registry64);
        }

        /// <summary>
        /// основной метотод проверки списка софта на установку
        /// </summary>
        private static List<string> CheckSoft(List<ProgramInfo> softList, List<string> programSearchNames,
            RegistryView registryType = RegistryView.Registry32)
        {
            var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryType);
            var key = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            if (key == null)
                return programSearchNames;

            foreach (var keyName in key.GetSubKeyNames())
            {
                var subKey = (string)key.OpenSubKey(keyName)?.GetValue("DisplayName");
                if (subKey == null)
                    continue;
                foreach (var progSearchName in programSearchNames)
                {
                    if (!subKey.Contains(progSearchName))
                        continue;
                    foreach (var prog in softList)
                    {
                        if (prog.SearchName != progSearchName) continue;
                        prog.Installed = true;
                        break;
                    }
                    programSearchNames.Remove(progSearchName);
                    break;
                }
                if (programSearchNames.Count == 0)
                    return null;
            }
            return programSearchNames;
        }

        #endregion

        #region Check Servers / Internet speed

        /// <summary>
        /// эхо запрос по указанному адресу
        /// </summary>
        public static int PingingHostInt(string ipAddress)
        {
            const string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            var buffer = Encoding.ASCII.GetBytes(data);
            var pingSender = new Ping();
            var reply = pingSender.Send(ipAddress, 1000, buffer);
            return reply != null && reply.Status == IPStatus.Success ? (int)reply.RoundtripTime : -1;
        }

        /// <summary>
        /// эхо запрос по указанному адресу
        /// </summary>
        public static bool PingingHostBool(string ipAddress)
        {
            try
            {
                var pingSender = new Ping();
                var reply = pingSender.Send(ipAddress, 1000);
                return reply != null && reply.Status == IPStatus.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// подсчет скорости загрузки указанного файла
        /// </summary>
        public static double SpeedTest(string filePath)
        {
            var watch = new Stopwatch();
            byte[] data;
            using (var client = new System.Net.WebClient())
            {
                watch.Start();
                try
                {
                    data = client.DownloadData(filePath);
                }
                catch (Exception)
                {
                    return 0;
                }
                watch.Stop();
            }

            return data.LongLength / watch.Elapsed.TotalSeconds / 1000000 * 8;
        }

        #endregion

        #region Pc Info

        public static string GetPcName()
        {
            return Dns.GetHostName();
        }

        public static string GetOsInformation()
        {
            var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_OperatingSystem");
            foreach (var queryObj in searcher.Get())
                return queryObj["Caption"].ToString();
            return "";
        }

        public static string GetIs64Or32()
        {
            return Environment.Is64BitOperatingSystem ? "64" : "32";
        }

        public static string GetCpuName()
        {
            var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
            foreach (var queryObj in searcher.Get())
                return queryObj["Name"].ToString();
            return "";

        }

        public static string GetCpuCoresCount()
        {
            var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
            foreach (var queryObj in searcher.Get())
                return queryObj["NumberOfCores"].ToString();
            return "";
        }

        public static string GetRamInfo()
        {
            var resultStr = "";
            var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PhysicalMemory");
            foreach (var queryObj in searcher.Get())
            {
                resultStr +=
                    $"RAM: {Math.Round(Convert.ToDouble(queryObj["Capacity"]) / 1024 / 1024 / 1024, 2)} ГБ, {queryObj["Speed"]} МГц\n";
            }
            return resultStr;
        }

        #endregion

        #region Other Check

        /// <summary>
        /// проверка начилия приложения ISC
        /// </summary>
        /// <returns></returns>
        public static bool CheckExistIscfIle()
        {
            return File.Exists(@"C:\scripts\iSpyController.exe");
        }

        /// <summary>
        /// проверка актуальности версии приложения ISC
        /// </summary>
        public static bool CheckIscVersionActuality(bool gui)
        {
            var versionInfo = GetIscVersionAsString();

            if (Version.TryParse(versionInfo, out var version))
                return version.CompareTo(MainData.ActualIscVersion) == 0 ||
                       version.CompareTo(MainData.ActualIscVersion) > 0;
            if (gui)
            {
                Messages.ErrorMessage("Ошибка парса версии ISC");
            }
            return false;
        }

        /// <summary>
        /// получение имени пользователя сертификата впн
        /// </summary>
        public static string GetVpnCertName()
        {
            var certName = "Отсутствует";
            if (!File.Exists(@"C:\Program Files\OpenVPN\config\client.ovpn"))
                return certName;
            foreach (var line in File.ReadLines(@"C:\Program Files\OpenVPN\config\client.ovpn"))
                if (line.Length > 9)
                {
                    if (line.Substring(0, 4) == "cert")
                        certName = line.Substring(5, line.Length - 9);
                }
            return certName;
        }

        /// <summary>
        /// проверка имени пк на корректность
        /// совпадение с именем впн сертификата, не более 14 символов, апперкейс
        /// </summary>
        public static bool CheckPcNameOnСorrectness(string pcName, string vpnCertName)
        {
            return string.Equals(pcName, vpnCertName, StringComparison.OrdinalIgnoreCase) && pcName.Length <= 14 &&
                   string.Equals(pcName.ToUpper(), pcName, StringComparison.Ordinal);
        }

        /// <summary>
        /// проверка наличия пользователя "user"
        /// </summary>
        public static bool CheckUserUser()
        {
            return GetUserNamesList().Any(user => user == "user");
        }

        /// <summary>
        /// проверка наличия пользователя "Центрофинанс"
        /// </summary>
        public static bool CheckUserCentrofinans()
        {
            return GetUserNamesList().Any(user => user == "Центрофинанс");
        }

        /// <summary>
        /// получение списка нестандартных пользователей
        /// в формате [user1] [user2]... 
        /// </summary>
        public static string GetIncorrectUsers()
        {
            var incorrectUsers = new List<string>();
            foreach (var user in GetUserNamesList())
            {
                switch (user)
                {
                    case "user":
                        break;
                    case "Центрофинанс":
                        break;
                    case "DefaultAccount":
                        break;
                    case "WDAGUtilityAccount":
                        break;
                    case "defaultuser0":
                        break;
                    case "Администратор":
                        break;
                    case "Гость":
                        break;
                    default:
                        incorrectUsers.Add(user);
                        break;
                }
            }
            return incorrectUsers.ListToString();
        }

        /// <summary>
        /// получение списка пользователей пк
        /// </summary>
        private static IEnumerable<string> GetUserNamesList()
        {
            var query = new SelectQuery("Win32_UserAccount");
            var searcher = new ManagementObjectSearcher(query);
            return (from ManagementBaseObject envVar in searcher.Get() select envVar["Name"].ToString()).ToList(); 
        }

        #region NO GUI

        /// <summary>
        /// проверка таски ISC в шедулере для nogui режима
        /// </summary>
        /// <returns></returns>
        public static bool CheckIscTask()
        {
            var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            var key = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Schedule\TaskCache\Tree");
            if (key == null)
                return false;

            foreach (var keyName in key.GetSubKeyNames())
            {
                if (keyName == "iSpyController")
                    return true;
            }
            return false;
        }

        /// <summary>
        /// получение статуса сервиса в формате строки для nogui режима
        /// </summary>
        public static string GetServiceStatus(string serviceName)
        {
            var sc = new ServiceController(serviceName);

            switch (sc.Status)
            {
                case ServiceControllerStatus.Running:
                    return "Running";
                case ServiceControllerStatus.Stopped:
                    return "Stopped";
                case ServiceControllerStatus.Paused:
                    return "Paused";
                case ServiceControllerStatus.StopPending:
                    return "Stopping";
                case ServiceControllerStatus.StartPending:
                    return "Starting";
                default:
                    return "Status Changing";
            }
        }

        /// <summary>
        /// приведение версии приложения ISC для nogui режима
        /// </summary>
        public static string GetIscVersionAsString()
        {
            const string path = @"C:\scripts\iSpyController.exe";
            if (!File.Exists(path))
                return "no file";
            var versionInfo = FileVersionInfo.GetVersionInfo(path);
            return !Version.TryParse(versionInfo.ProductVersion, out var version) ? "parse error" : version.ToString();
        }

        #endregion

        #endregion

        #region .NET Framework Info

        private static int GetFrameworkReleaseKey()
        {
            using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
                .OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\")) 
            {
                var releaseKey = Convert.ToInt32(ndpKey?.GetValue("Release"));
                return releaseKey;
            }
        }

        public static string GetFrameworkReleaseVersion()
        {
            var releaseKey = GetFrameworkReleaseKey();
            if (releaseKey >= 461808)
                return "4.7.2 or later";
            if (releaseKey >= 461308)
                return "4.7.1 or later";
            if (releaseKey >= 460798)
                return "4.7 or later";
            if (releaseKey >= 394802)
                return "4.6.2 or later";
            if (releaseKey >= 394254)
                return "4.6.1 or later";
            if (releaseKey >= 393295)
                return "4.6 or later";
            if (releaseKey >= 393273)
                return "4.6 RC or later";
            if (releaseKey >= 379893)
                return "4.5.2 or later";
            if (releaseKey >= 378675)
                return "4.5.1 or later";
            if (releaseKey >= 378389)
                return "4.5 or later";
            return "No 4.5 or later version detected";
        }

        #endregion
    }
}