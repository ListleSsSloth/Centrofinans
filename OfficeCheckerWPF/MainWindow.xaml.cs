using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Win32;
using Brushes = System.Windows.Media.Brushes;

namespace OfficeCheckerWPF  
{
    public partial class MainWindow
    {
        private readonly Dictionary<NetworkType, string> _speedTestFiles = new Dictionary<NetworkType, string>
        {
            {NetworkType.Global, @"http://dl.google.com/googletalk/googletalk-setup.exe"},
            {NetworkType.Local, @"http://web.centrofinans.ru/uploaded/oc-speedtest.exe"},
        };

        private static readonly string ProgramVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public MainWindow()
        {
            InitializeComponent();
            StartWindow.Title = $"Office Checker {ProgramVersion}";

            if (MainData.ConsoleActive)
            {
                MainData.LogConsole = new LogConsole();
                ConsoleMenuItem.Visibility = Visibility.Visible;
            }
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.Start);

            WriteConsoleLog("Запрос пароля");
            var wnd = new LoginWindow {Owner = this};
            wnd.ShowDialog();
            WriteConsoleLog("Аунтификация пройдена");

            WriteConsoleLog("Инициализация элементов");
            InitCheckBoxesStack();
            InitServiceStack();
            InitNetworkStack();
            WriteConsoleLog("Инициализация элементов завершена");

            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.End);
        }

        #region Initializers
        //отрисовка элеметов программ, сервисов и блока теста сети
        private void InitCheckBoxesStack()
        {
            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.Start);

            SoftCheckBoxStack.Children.Clear();
            foreach (var prog in MainData.Programs)
            {
                prog.CheckBox.IsChecked = false;
                SoftCheckBoxStack.Children.Add(prog.CheckBox);
                WriteConsoleLog($"Добавлен элемент {prog.TitleName}");
            }

            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.End);
        }

        private void InitServiceStack()
        {
            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.Start);
            

            foreach (var serv in MainData.Services)
            {
                var stack = new StackPanel { Orientation = Orientation.Vertical };
                stack.Children.Add(serv.CheckBox);
                stack.Children.Add(serv.StartServiceButton);
                ServiceStack.Children.Add(stack);
                WriteConsoleLog($"Добавлен элемент {serv.TitleName}");
            }

            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.End);
        }

        private void InitNetworkStack()
        {
            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.Start);

            NetworkProgressBar.Maximum = MainData.Servers.Count * 10 + 12;
            NetworkProgressBar.Value = 0;
            NetworkStatusLabel.Content = "Ожидание";
            NetworkInfoTextBlock.Text = "";

            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.End);
        }

        #endregion

        #region MAIN

        /// <summary>
        /// проверка списка служб по ключам реестра, авто изменение папаметров запуска
        /// </summary>
        private static void CheckServices(IEnumerable<ServiceInfo> list)
        {
            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.Start, true);

            foreach (var service in list)
            {
                var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                var key = localMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\" + service.SearchName);
                if (key == null)
                    return;
                var subKey = (int)key.GetValue("Start");
                if (subKey != 2)
                {
                    var process = new Process
                    {
                        StartInfo =
                        {
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            FileName = "cmd.exe",
                            Arguments = "/C " + "sc config " + service.SearchName + " start= auto"
                        }
                    };
                    process.Start();
                    WriteConsoleLog($"Установлен автозапуск для {service.TitleName}", true);
                }
                using (var sc = new ServiceController(service.SearchName))
                {
                    if (sc.Status == ServiceControllerStatus.Running)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            service.CheckBox.IsChecked = true;
                        });
                        if (service.SearchName == "MpsSvc")
                        {
                            var process = new Process
                            {
                                StartInfo =
                                {
                                    RedirectStandardOutput = true,
                                    UseShellExecute = false,
                                    CreateNoWindow = true,
                                    FileName = "cmd.exe",
                                    Arguments = "/C " + "netsh advfirewall set allprofiles state off"
                                }
                            };
                            process.Start();
                            WriteConsoleLog("Отключена защита фаервола", true);
                        }
                    }
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    service.StartServiceButton.Visibility = Visibility.Visible;
                });
                WriteConsoleLog($"Активирована кнопка контроля для {service.TitleName}", true);
            }
            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.End, true);
        }

        /// <summary>
        /// получение данных эхо запросов списка серверов
        /// </summary>
        /// <returns>список объектов PingData</returns>
        private IEnumerable<PingData> PingingServers(IEnumerable<Servers> serverses)
        {
            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.Start, true);
            Application.Current.Dispatcher.Invoke(() =>
            {
                NetworkStatusLabel.Content = "Пинг серверов";
            });

            var pingDataList = new List<PingData>();
            foreach (var server in serverses)
            {
                WriteConsoleLog($"Пинг {server.ServerName}", true);
                var error = 10;
                var lost = 0;
                var totalPing = 0;
                var iterationsCount = 0;
                for (var i = 0; i < 10; i++)
                {
                    iterationsCount++;
                    var ping = Utils.PingingHostInt(server.ServerIpAddress);
                    WriteConsoleLog($"{server.ServerName} результат эхо запроса - {ping}", true);
                    if (ping == -1)
                    {
                        i--;
                        error--;
                        lost++;
                        if (error == 0)
                        {
                            MessageBox.Show($"Сервер {server.ServerName} не доступен!");
                            ResetNetwork();
                            break;
                        }
                        continue;
                    }
                    totalPing += ping;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NetworkProgressBar.Value++;
                    });

                }
                pingDataList.Add(new PingData(server.ServerName, totalPing / 10, lost));
                WriteConsoleLog(
                    $"\r\n***Общий результат сервера {server.ServerName}***\r\n" +
                    $"Средний пинг: {totalPing / 10}\r\n" +
                    $"Потери: {lost}\r\n" +
                    $"Ошибки: {10 - error}\r\n" +
                    $"Итерации: {iterationsCount}"
                    ,true);
                WriteConsoleLog($"Создание объекта PingData для сервера {server.ServerName} на основе данных", true);
                WriteConsoleLog($"Добавление объекта PingData для сервера {server.ServerName} в список", true);
            }
            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.End, true);
            return pingDataList;
        }

        /// <summary>
        /// измерение скорости загруки для указанного типа сети, среднее из 5 загрузок
        /// </summary>
        private double GetNetworkSpeed(NetworkType networkType)
        {
            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.Start, true);
            WriteConsoleLog($"Тест скорости соединения для {networkType} типа сети", true);

            var str = networkType == NetworkType.Global ? "Диагностика глобальной сети" : "Диагностика внутренней сети";
            Application.Current.Dispatcher.Invoke(() =>
            {
                NetworkStatusLabel.Content = str;
            });
            double res = 0;
            for (var i = 0; i < 6; i++)
            {
                WriteConsoleLog($"Итерация {i}", true);
                var s = Utils.SpeedTest(_speedTestFiles[networkType]);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    NetworkProgressBar.Value++;
                });
                if (i == 0)
                    continue;
                res += s;
            }
            WriteConsoleLog($"Тест скорости соединения для {networkType} Завершен", true);

            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.End, true);
            return res / 5;
        }

        /// <summary>
        /// анчек чекбоксов
        /// </summary>
        private static void UncheckAll()
        {
            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.Start, true);

            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var programInfo in MainData.Programs)
                    programInfo.CheckBox.IsChecked = false;
                foreach (var serviceInfo in MainData.Services)
                    serviceInfo.CheckBox.IsChecked = false;
            });

            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.End, true);
        }

        /// <summary>
        /// сброс панели диагностики сети
        /// </summary>
        private void ResetNetwork()
        {
            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.Start, true);

            Application.Current.Dispatcher.Invoke(() =>
            {
                NetworkStartTestButton.Visibility = Visibility.Visible;
                NetworkProgressBar.Value = 0;
                NetworkStatusLabel.Content = "Ожидание";
            });

            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.End, true);
        }

        #endregion

        #region Texboxes filling

        /// <summary>
        /// заполненние информации о пк
        /// </summary>
        private void PcInfoTextBlockFilling()
        {
            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.Start, true);

            WriteConsoleLog("GetOsInformation", true);
            var osVers = Utils.GetOsInformation();

            WriteConsoleLog("GetIs64Or32", true);
            var is64 = Utils.GetIs64Or32();

            WriteConsoleLog("GetCpuName", true);
            var cpuName = Utils.GetCpuName();

            WriteConsoleLog("GetCpuCoresCount", true);
            var cpuCOres = Utils.GetCpuCoresCount();

            WriteConsoleLog("GetRamInfo", true);
            var ramInfo = Utils.GetRamInfo();

            WriteConsoleLog("Заполнение полученной информации", true);
            Application.Current.Dispatcher.Invoke(() =>
            {
                PcInfoTextBlock.Text += $"Версия ОС: {osVers}\n";
                PcInfoTextBlock.Text += $"Разрядность ОС: {is64}\n";
                PcInfoTextBlock.Text += $"CPU: {cpuName}\n";
                PcInfoTextBlock.Text += $"Количество ядер CPU: {cpuCOres}\n";
                PcInfoTextBlock.Text += ramInfo;
            });

            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.End, true);
        }

        /// <summary>
        /// заполнение основного информационного блока
        /// </summary>
        private void InfoTextBlockFilling()
        {
            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.Start);

            Brush goodColor = Brushes.Blue;
            Brush badColor = Brushes.Red;
            var color = badColor;

            WriteConsoleLog("GetVpnCertName");
            var certName = Utils.GetVpnCertName();
            if (certName != "Отсутствует")
                color = goodColor;
            var run = new Run($"Текущий VPN сертификат: {certName}\r\n") { Foreground = color };
            InfoTextBlock.Inlines.Add(run);

            WriteConsoleLog("GetPcName");
            var pcName = Utils.GetPcName();
            string subString;
            if (Utils.CheckPcNameOnСorrectness(pcName, certName))
            {
                subString = " соответствует ";
                color = goodColor;
            }
            else
            {
                subString = " НЕ СООТВЕТСТВУЕТ ";
                color = badColor;
            }
            run = new Run($"Имя ПК: {pcName} - {subString} стандартам\r\n") { Foreground = color };
            InfoTextBlock.Inlines.Add(run);

            WriteConsoleLog("Проверка текущего пользователя");
            var currentUsername = Environment.UserName;
            color = currentUsername == "Центрофинанс" ? goodColor : badColor;
            run = new Run($"Текущий пользователь: {currentUsername}\r\n") { Foreground = color };
            InfoTextBlock.Inlines.Add(run);

            WriteConsoleLog("CheckUserDirOnCorrect");
            if (CheckUserDirOnCorrect())
            {
                color = goodColor;
                subString = "создан корректно";
            }
            else
            {
                color = badColor;
                subString = "ПЕРЕИМЕНОВАН!";
            }
            run = new Run($"Пользователь {subString}\r\n") { Foreground = color };
            InfoTextBlock.Inlines.Add(run);

            //switch (Environment.UserName)
            //{
            //    case "user":
            //        color = goodColor;
            //        subString =
            //            "Пользователь user должен иметь пароль и предназначен не для работы офиса\r\n" +
            //            "Перенастройте пользователей!\r\n" +
            //            "Стандартное наименование пользоваателя \"Центрофинанс\"";
            //        break;
            //    case "Центрофинанс":
            //        color = goodColor;
            //        subString =
            //            "Пользователь соответствует стандарту\r\n" +
            //            "На всякий случай проверьте наличие пользователя \"user\" с паролем";
            //        break;
            //    default:
            //        color = badColor;
            //        subString = "Настройте пользователей на ПК в соответствии со стандартами:\r\n" +
            //                    "1-й пользователь \"user\" со стандартным паролем для удаленного управления\r\n" +
            //                    "2-й пользователь \"Центрофинанс\" без пароля для работы офиса";
            //        break;
            //}
            //run = new Run($"Текущий пользователь: {Environment.UserName}\n{subString}\n") { Foreground = color };
            //InfoTextBlock.Inlines.Add(run);

            WriteConsoleLog("CheckUserUser");
            if (Utils.CheckUserUser())
            {
                color = goodColor;
                subString = "Пользователь \"user\" есть";
            }
            else
            {
                color = badColor;
                subString = "Пользователя \"user\" нет";
            }
            run = new Run($"{subString}\r\n") { Foreground = color };
            InfoTextBlock.Inlines.Add(run);

            WriteConsoleLog("GetIncorrectUsers");
            var incorrectUsers = Utils.GetIncorrectUsers();
            if (!string.IsNullOrEmpty(incorrectUsers))
            {
                color = badColor;
                run = new Run($"Нестандартные пользователи {incorrectUsers}\r\n") { Foreground = color };
                InfoTextBlock.Inlines.Add(run);
            }

            WriteConsoleLog("CheckExistIscfIle");
            if (Utils.CheckExistIscfIle())
            {
                run = new Run("iSpyController установлен\n") { Foreground = goodColor };
                InfoTextBlock.Inlines.Add(run);

                run = Utils.CheckIscVersionActuality(true)
                    ? new Run("Версия iSpyController актуальна\n") { Foreground = goodColor }
                    : new Run("Версия iSpyController не актуальна\n") { Foreground = badColor };
                InfoTextBlock.Inlines.Add(run);
            }
            else
            {
                run = new Run("iSpyController не установлен\n") { Foreground = badColor };
                InfoTextBlock.Inlines.Add(run);
            }

            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.End);
        }

        /// <summary>
        /// чек боксов программ на основе свойства объектов
        /// </summary>
        private static void RenderingProgramsCheckBoxes()
        {
            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.Start, true);

            foreach (var prog in MainData.Programs)
            {
                prog.CheckBox.IsChecked = prog.Installed;
                WriteConsoleLog($"Отметка {prog.TitleName} - {prog.Installed}", true);
            }

            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.End, true);
        }

        /// <summary>
        /// проверка корректности создания пользователя
        /// </summary>
        private static bool CheckUserDirOnCorrect()
        {
            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.Start);

            var userName = Environment.UserName;
            var path = Environment.ExpandEnvironmentVariables(@"%APPDATA%");
            var userDir = Regex.Match(path, @"Users\\(?<dir>.*)\\AppData");

            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.End);
            return userName == userDir.Groups["dir"].Value;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// клик по кнопки основной проверки
        /// </summary>
        private void StartButtonClick(object sender, RoutedEventArgs e)
        {
            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.Start);

            GoButton.Visibility = Visibility.Collapsed;
            MainCheckProgressBar.Visibility = Visibility.Visible;

            WriteConsoleLog("Task.Factory.StartNew");
            Task.Factory.StartNew(() =>
            {
                try
                {
                    WriteConsoleLog("--StartNew(StartButtonClick) STARTED", true);

                    //UncheckAll();
                    WriteConsoleLog("Начало проверки программ", true);
                    Utils.CheckSofrware(MainData.Programs, Utils.GetProgramNames(MainData.Programs));
                    WriteConsoleLog("Проверка программ завершена", true);

                    Application.Current.Dispatcher.Invoke(RenderingProgramsCheckBoxes);
                    CheckServices(MainData.Services);
                    PcInfoTextBlockFilling();
                    Application.Current.Dispatcher.Invoke(InfoTextBlockFilling);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MainCheckProgressBar.Visibility = Visibility.Collapsed;
                    });

                    WriteConsoleLog("--StartNew(StartButtonClick) ENDED", true);
                }
                catch (Exception ex)
                {
                    Messages.ErrorMessage($"Проблема запуска\r\n{ex}");
                }
            });

            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.End);
        }

        /// <summary>
        /// пункт меня about
        /// </summary>
        private void MenuItemAboutOnClick(object sender, RoutedEventArgs e)
        {
            Messages.InfoMessage("Office Checker\r\nVersion " + ProgramVersion + "\r\nCreated by Dmitrii Klishin", "О программе");
        }

        /// <summary>
        /// клик по кнопке настройки потока ispy в меню, открывает помошник настроки ispy
        /// </summary>
        private void OptionsISpyOnClick(object sender, RoutedEventArgs e)
        {
            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.Start);

            var wnd = new SpyWindow { Owner = this };
            wnd.ShowDialog();

            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.End);
        }

        //private void MenuItemNewInPatchClick(object sender, RoutedEventArgs e)
        //{
        //    var wnd = new HistoryOfChangesWindow { Owner = this };
        //    wnd.ShowDialog();
        //}

        /// <summary>
        /// клик по кнопки диагностики качества сети и скорости соединения
        /// </summary>
        private void NetworkStartTestButtonClick(object sender, RoutedEventArgs e)
        {
            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.Start);

            WriteConsoleLog("Task.Factory.StartNew");
            Task.Factory.StartNew(() =>
            {
                try
                {
                    NetworkStartTestButtonRun();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Запуск не удался:\r\n{ex}");
                }
            });

            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.End);
        }

        /// <summary>
        /// дополнительный инструмент очистки остаточных файлов ispy в
        /// директориях других пользователей
        /// </summary>
        private void ToolsClearAllAppDataClick(object sender, RoutedEventArgs e)
        {
            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.Start);

            if (MessageBox.Show("Очистить файлы iSpy в AppData всех пользователей?", "Подтверждение", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

            var dirPathList = Directory.GetDirectories(@"C:\users");
            foreach (var path in dirPathList)
            {
                DeleteIspyDir(path);
            }

            Messages.InfoMessage("Данные удалены", "ОК");

            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.End);
        }

        /// <summary>
        /// дополнительный инструмент консоль
        /// доступен при параметре запуска -console
        /// </summary>
        private void ToolsConsoleMenuItemOnClick(object sender, RoutedEventArgs e)
        {
            MainData.LogConsole.Show();
        }

        #endregion

        #region Event Handlers Support

        /// <summary>
        /// основной метод проверки связи
        /// </summary>
        private void NetworkStartTestButtonRun()
        {
            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.Start, true);

            ResetNetwork();
            Application.Current.Dispatcher.Invoke(() =>
            {
                NetworkStartTestButton.Visibility = Visibility.Collapsed;
            });

            var pingDataList = PingingServers(MainData.Servers);
            var localSpeed = GetNetworkSpeed(NetworkType.Local);
            if (localSpeed <= 0)
            {
                MessageBox.Show("Ошибка диагностики\nотключите антивирус", "Ошибка");
                ResetNetwork();
                return;
            }
            var globalSpeed = GetNetworkSpeed(NetworkType.Global);
            if (globalSpeed <= 0)
            {
                MessageBox.Show("Ошибка диагностики\nОтключите антивирус", "Ошибка");
                ResetNetwork();
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                NetworkInfoTextBlock.Text = "";
            });
            Brush goodColor = Brushes.Blue;
            Brush badColor = Brushes.Red;
            Brush color;
            foreach (var pingData in pingDataList)
            {
                if (pingData.AvgPing > 150 || pingData.PacketLost > 5)
                {
                    color = badColor;
                }
                else
                {
                    color = goodColor;
                }
                var color1 = color;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var run = new Run(pingData.ToString()) { Foreground = color1 };
                    NetworkInfoTextBlock.Inlines.Add(run);
                });
            }

            color = globalSpeed < 1.0 ? badColor : goodColor;
            Application.Current.Dispatcher.Invoke(() =>
            {
                var run = new Run($"Скорость внешней сети: {globalSpeed:N1} Мбит/с\n") { Foreground = color };
                NetworkInfoTextBlock.Inlines.Add(run);
            });

            color = localSpeed < 1.0 ? badColor : goodColor;
            Application.Current.Dispatcher.Invoke(() =>
            {
                var run = new Run($"Скорость внутренней сети: {localSpeed:N1} Мбит/с\n") { Foreground = color };
                NetworkInfoTextBlock.Inlines.Add(run);
            });

            Application.Current.Dispatcher.Invoke(() =>
            {
                NetworkStatusLabel.Content = "Диагностика завершена";
            });

            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.Start, true);
        }

        /// <summary>
        /// удаление каталога айспай в ападате
        /// </summary>
        private static void DeleteIspyDir(string path)
        {
            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.Start);

            var realPath = path + @"\AppData\Roaming\iSpy";

            if (!Directory.Exists(realPath))
                return;
            try
            {
                Directory.Delete(realPath, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления\r\n" + realPath + "\r\n" + ex, "Ошибка");
            }

            WriteStartStopMethodConsoleLog(Utils.GetCurrentMethod(), MethodAction.End);
        }

        #endregion

        #region ***Console Logs

        /// <summary>
        /// запись лога в консоль
        /// </summary>
        private static void WriteConsoleLog(string message, bool anotherThread = false)
        {
            if (!MainData.ConsoleActive)
                return;
            if (!anotherThread)
            {
                MainData.LogConsole.AddLog(message);
                MainData.LogConsole.Scroller.ScrollToEnd();
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MainData.LogConsole.AddLog(message);
                    MainData.LogConsole.Scroller.ScrollToEnd();
                });
            }
        }

        /// <summary>
        /// запись начала или конца метода в консоль
        /// </summary>
        private static void WriteStartStopMethodConsoleLog(string methodName, MethodAction methodAction,  bool anotherThread = false)
        {
            if (!MainData.ConsoleActive)
                return;
            var action = methodAction == MethodAction.Start ? "STARTED" : "ENDED";
            if (!anotherThread)
            {
                MainData.LogConsole.AddLog($"---{methodName} is {action}");
                MainData.LogConsole.Scroller.ScrollToEnd();
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MainData.LogConsole.AddLog($"---{methodName} is {action}");
                    MainData.LogConsole.Scroller.ScrollToEnd();
                });
            }
        }

        #endregion

        /// <summary>
        /// полное закрытие с кодом возврата 0 при закрытии основной формы
        /// </summary>
        private void MainWindowOnClosing(object sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}