using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;

namespace OfficeCheckerWPF
{
    public partial class SpyWindow
    {
        private readonly DispatcherTimer _dt = new DispatcherTimer {Interval = new TimeSpan(0, 0, 5)};
        private readonly string _xmlPath = Environment.ExpandEnvironmentVariables(@"%APPDATA%\iSpy\XML\");
        private readonly Dictionary<string, Camera> _camModels = new Dictionary<string, Camera>();

        public SpyWindow()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            InitDictionary();
            CamModelBox.ItemsSource = _camModels.Keys;
            EnableMainElements();
            _dt.Tick += DtTick;
            _dt.IsEnabled = true;
        }

        #region MAIN

        /// <summary>
        /// активация основных элементов
        /// </summary>
        private void EnableMainElements(bool enable = false)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                CamModelBox.IsEnabled = enable;
                IpBox1.IsEnabled = enable;
                IpBox2.IsEnabled = enable;
                IpBox3.IsEnabled = enable;
                IpBox4.IsEnabled = enable;
                CamLoginTextBox.IsEnabled = enable;
                CamPasswordTextBox.IsEnabled = enable;
                StartISpyButton.IsEnabled = enable;
                LookSourceButon.IsEnabled = enable;
                XmlSettingButton.IsEnabled = enable;
                BackupSettingButton.IsEnabled = enable;
                BackupRestoreSettingButton.IsEnabled = enable;
            });
        }

        /// <summary>
        /// проверка процесса айспая
        /// </summary>
        /// <returns></returns>
        private static bool ProcessISpyIsRunningCheck()
        {
            return Process.GetProcessesByName("iSpy").Length != 0;
        }

        /// <summary>
        /// Закрытие айспая
        /// </summary>
        /// <returns></returns>
        private static bool CloseISpy()
        {
            var count = 3;
            while (ProcessISpyIsRunningCheck() && count != 0)
            {
                count--;
                var ps1 = Process.GetProcessesByName("iSpyMonitor");
                foreach (var p1 in ps1)
                {
                    p1.Kill();
                    p1.WaitForExit(1000);
                }
                

                ps1 = Process.GetProcessesByName("iSpy");

                foreach (var p1 in ps1)
                {
                    p1.Kill();
                    p1.WaitForExit(1000);
                }
               // Thread.Sleep(1000);
            }
            return count != 0;
        }

        /// <summary>
        /// сборка IP адреса из полей формы
        /// </summary>
        /// <returns></returns>
        private string AssembleIpAddress()
        {
            var assembleIp = IpBox1.Text + "." + IpBox2.Text + "." + IpBox3.Text + "." + IpBox4.Text;
            return assembleIp;
        }

        /// <summary>
        /// конфигурация потока в xml файле согласно выбранным параметрам
        /// </summary>
        private void XmlConfiguration()
        {
            Camera camera = null;
            var camIp = "";
            var uLogin = "";
            var uPass = "";

            Application.Current.Dispatcher.Invoke(() =>
            {
                camera = _camModels[(string)CamModelBox.SelectedValue];
                camIp = AssembleIpAddress();
                uLogin = CamLoginTextBox.Text;
                uPass = CamPasswordTextBox.Text;
            });

            //Запись параметров в config.xml

            var sr = new StreamReader(_xmlPath + "config.xml", true);
            var doc = new XmlDocument();
            try
            {
                doc.Load(sr);
                var node = doc.SelectSingleNode("//configuration/MJPEGURL");
                if (node != null)
                    node.InnerText = camera.ConfigXmlMjpegurl.Replace("[[IPADDRESS]]", camIp)
                        .Replace("[[LOGINUSER]]", uLogin).Replace("[[LOGINPASS]]", uPass);

                node = doc.SelectSingleNode("//configuration/AVIFileName");
                if (node != null)
                    node.InnerText = camera.ConfigXmlAviFileName.Replace("[[IPADDRESS]]", camIp)
                        .Replace("[[LOGINUSER]]", uLogin).Replace("[[LOGINPASS]]", uPass);

                node = doc.SelectSingleNode("//configuration/FeatureSet");
                if (node != null)
                    node.InnerText = camera.ConfigXmlFeatureSet;

                node = doc.SelectSingleNode("//configuration/RecentFileList");
                if (node != null)
                    node.InnerText = camera.ConfigXmlRecentFileList.Replace("[[IPADDRESS]]", camIp)
                        .Replace("[[LOGINUSER]]", uLogin).Replace("[[LOGINPASS]]", uPass);

                sr.Close();
                doc.Save(_xmlPath + "config.xml");
            }
            catch (Exception e)
            {
                Messages.ErrorMessage($"Ошибка записи конфигурации в config.xml\r\n{e}");
            }

            //Запись параметров в objects.xml

            sr = new StreamReader(_xmlPath + "objects.xml", true);
            doc = new XmlDocument();
            try
            {
                doc.Load(sr);
                var node = doc.SelectSingleNode("//camera[@id = '1']/settings/sourceindex");
                if (node != null)
                    node.InnerText = camera.ObjectsXmlSourceindex;

                node = doc.SelectSingleNode("//camera[@id = '1']/settings/videosourcestring");
                if (node != null)
                    node.InnerText = camera.ObjectsXmlVideoSourceString.Replace("[[IPADDRESS]]", camIp)
                        .Replace("[[LOGINUSER]]", uLogin).Replace("[[LOGINPASS]]", uPass);

                node = doc.SelectSingleNode("//camera[@id = '1']/settings/login");
                if (node != null)
                    node.InnerText = uLogin;
                sr.Close();
                doc.Save(_xmlPath + "objects.xml");
            }
            catch (Exception e)
            {
                Messages.ErrorMessage("Ошибка записи конфигурации в objects.xml\r\n" + e);
            }
        }

        /// <summary>
        /// включение-выключение прогрессбара длительного процесса из отдельного потока
        /// </summary>
        /// <param name="show"></param>
        private void EnableRunningProgressBar(bool show = true)
        {
            EnableMainElements(show.ReverseBool());
            var vis = show ? Visibility.Visible : Visibility.Hidden;
            Application.Current.Dispatcher.Invoke(() => { RunningProgressBar.Visibility = vis; });
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// клик по кнопке проверки установки iSpy
        /// </summary>
        private void BaseTestButtonClick(object sender, RoutedEventArgs e)
        {
            if (!Utils.CheckProgramInstall("iSpy"))
            {
                Messages.ErrorMessage("iSpy не установлен!\r\n" +
                                      "Установите его из стандартного пакета:\r\n" +
                                      "iSpy_config_120дн_250ГБ.exe");
                return;
            }
            BaseTestButton.Visibility = Visibility.Collapsed;
            StartISpyButton.Visibility = Visibility.Visible;
            EnableMainElements(true);
        }

        /// <summary>
        /// клик по кнопек запуска ispy.exe
        /// </summary>
        private void StartISpyButtonClick(object sender, RoutedEventArgs e)
        {
            var iSpyPath1 = Environment.ExpandEnvironmentVariables(@"%PROGRAMFILES(x86)%\iSpy\iSpy\iSpy.exe");
            const string iSpyPath2 = @"C:\Program Files\iSpy\iSpy.exe";

            if (!Utils.CheckProgramInstall("iSpy"))
            {
                Messages.ErrorMessage("iSpy не установлен!\r\n" +
                                      "Установите его из стандартного пакета:\r\n" +
                                      "iSpy_config_120дн_250ГБ.exe");
                return;
            }
            if (ProcessISpyIsRunningCheck())
            {
                Messages.InfoMessage("iSpy уже запущен");
                return;
            }

            if (!File.Exists(iSpyPath1) && !File.Exists(iSpyPath2))
            {
                Messages.ErrorMessage("Не удается запустить iSpy\r\n" +
                                "Скорее всего iSpy установлен не в стандартную директорию");
                return;
            }

            var iSpyPath = File.Exists(iSpyPath1) ? iSpyPath1 : iSpyPath2;
            var proc = new Process { StartInfo = { FileName = iSpyPath } };
            proc.Start();
            proc.WaitForExit(5000);
        }

        /// <summary>
        /// клик по кнопке просмотра текущего потока
        /// </summary>
        private void LookSourceButonClick(object sender, RoutedEventArgs e)
        {
            var wnd = new StreamWindow { Owner = this };
            wnd.ShowDialog();
        }

        /// <summary>
        /// клик по кнопке настроки потока
        /// </summary>
        private void XmlSettingButtonClick(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    EnableRunningProgressBar();

                    if (!CloseISpy())
                    {
                        EnableRunningProgressBar(false);
                        Messages.ErrorMessage("Ошибка закрытия iSpy");
                        return;
                    }

                    if (!File.Exists(_xmlPath + "config.xml") || !File.Exists(_xmlPath + "objects.xml"))
                    {
                        EnableRunningProgressBar(false);
                        Messages.ErrorMessage("Конфигурационные файлы отсутствуют", "Настройка не возможна");
                        return;
                    }

                    XmlConfiguration();
                    EnableRunningProgressBar(false);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Messages.InfoMessage("Настройки iSpy для камеры " + CamModelBox.SelectedValue + " завершены!",
                            "OK");
                    });
                }
                catch (Exception ex)
                {
                    EnableRunningProgressBar(false);
                    Messages.ErrorMessage($"Проблема запуска\r\n{ex}");
                }
            });
        }

        /// <summary>
        /// клик по кнопке бэкапа настроек
        /// </summary>
        private void BackupSettingButtonClick(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    EnableRunningProgressBar();

                    if (!CloseISpy())
                    {
                        EnableRunningProgressBar(false);
                        Messages.ErrorMessage("Ошибка закрытия iSpy");
                        return;
                    }

                    if (!File.Exists(_xmlPath + "config.xml") || !File.Exists(_xmlPath + "objects.xml"))
                    {
                        EnableRunningProgressBar(false);
                        Messages.ErrorMessage("Конфигурационные файлы отсутствуют", "Копирование не возможно");
                        return;
                    }

                    File.Copy(_xmlPath + "config.xml", _xmlPath + "config.xml.backup", true);
                    File.Copy(_xmlPath + "objects.xml", _xmlPath + "objects.xml.backup", true);

                    EnableRunningProgressBar(false);
                    Messages.InfoMessage("Копии настроек сохранены", "OK");
                }
                catch (Exception ex)
                {
                    EnableRunningProgressBar(false);
                    Messages.ErrorMessage($"Проблема запуска\r\n{ex}");
                }
            });
        }

        /// <summary>
        /// клик по кнопке восстановления настроек
        /// </summary>
        private void BackupRestoreSettingButtonClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Восстановить настройки?", "Предупреждение", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    EnableRunningProgressBar();

                    if (!CloseISpy())
                    {
                        EnableRunningProgressBar(false);
                        Messages.ErrorMessage("Ошибка закрытия iSpy");
                        return;
                    }

                    if (!File.Exists(_xmlPath + "config.xml.backup") || !File.Exists(_xmlPath + "objects.xml.backup"))
                    {
                        EnableRunningProgressBar(false);
                        Messages.ErrorMessage("Резервные копии отсутствуют", "Восстановление не возможно");
                        return;
                    }

                    try
                    {
                        File.Copy(_xmlPath + "config.xml.backup", _xmlPath + "config.xml", true);
                        File.Copy(_xmlPath + "objects.xml.backup", _xmlPath + "objects.xml", true);
                        EnableRunningProgressBar(false);
                        Messages.InfoMessage("Настройки восстановлены из копии", "OK");
                    }
                    catch (Exception exception)
                    {
                        EnableRunningProgressBar(false);
                        Messages.ErrorMessage(exception.ToString());
                    }
                }
                catch (Exception ex)
                {
                    EnableRunningProgressBar(false);
                    Messages.ErrorMessage($"Проблема запуска\r\n{ex}");
                }
            });
        }

        /// <summary>
        /// тик таймера периодического пинга камеры
        /// </summary>
        private void DtTick(object sender, EventArgs e)
        {

            if (Utils.PingingHostBool(AssembleIpAddress()))
            {
                CameraAccessLabel.Foreground = Brushes.Green;
                CameraAccessLabel.Content = "Камера доступна";
            }
            else
            {
                CameraAccessLabel.Foreground = Brushes.Red;
                CameraAccessLabel.Content = "Камера не доступна";
            }
        }

        #endregion

        #region Cam Source Dictionary

        /* Словарь потоков
        [[IPADDRESS]]
        [[LOGINUSER]]
        [[LOGINPASS]]
        ConfigXmlMjpegurl
        ConfigXmlAviFileName
        ConfigXmlFeatureSet
        ConfigXmlRecentFileList
        ObjectsXmlSourceindex
        ObjectsXmlVideoSourceString
        */
        private void InitDictionary()
        {
            _camModels.Add("Vstarcam",
                new Camera(
                    "http://[[IPADDRESS]]/videostream.cgi?loginuse=[[LOGINUSER]]&loginpas=[[LOGINPASS]]",
                    "",
                    "106824102",
                    "",
                    "1",
                    "http://[[IPADDRESS]]/videostream.cgi?loginuse=[[LOGINUSER]]&loginpas=[[LOGINPASS]]"
                ));
            _camModels.Add("Vstarcam C8824WIP",
                new Camera(
                    "",
                    "rtsp://[[LOGINUSER]]:[[LOGINPASS]]@[[IPADDRESS]]:10554/udp/av0_0",
                    "106824166",
                    "rtsp://[[LOGINUSER]]:[[LOGINPASS]]@[[IPADDRESS]]:10554/udp/av0_0",
                    "2",
                    "rtsp://[[LOGINUSER]]:[[LOGINPASS]]@[[IPADDRESS]]:10554/udp/av0_0"
                ));
            _camModels.Add("Polyvision",
                new Camera(
                    "",
                    "rtsp://[[IPADDRESS]]:554/user=[[LOGINUSER]]&amp;password=[[LOGINPASS]]&amp;channel=1&amp;stream=0?.sdp",
                    "106824166",
                    "rtsp://[[IPADDRESS]]:554/user=[[LOGINUSER]]&amp;password=[[LOGINPASS]]&amp;channel=1&amp;stream=0?.sdp",
                    "2",
                    "rtsp://[[IPADDRESS]]:554/user=[[LOGINUSER]]&password=[[LOGINPASS]]&channel=1&stream=0?.sdp"
                ));
            _camModels.Add("FalconEyeHD",
                new Camera(
                    "",
                    "rtsp://[[LOGINUSER]]:[[LOGINPASS]]@[[IPADDRESS]]/12",
                    "106824166",
                    "rtsp://[[LOGINUSER]]:[[LOGINPASS]]@[[IPADDRESS]]/12",
                    "2",
                    "rtsp://[[LOGINUSER]]:[[LOGINPASS]]@[[IPADDRESS]]/12"
                ));
            _camModels.Add("FalconEye",
                new Camera(
                    "http://[[IPADDRESS]]/vjpeg.v?user=[[LOGINUSER]]&pwd=[[LOGINPASS]]",
                    "",
                    "106824102",
                    "",
                    "1",
                    "http://[[IPADDRESS]]/vjpeg.v?user=[[LOGINUSER]]&pwd=[[LOGINPASS]]"
                ));
            _camModels.Add("Foscam",
                new Camera(
                    "http://[[IPADDRESS]]/videostream.cgi?user=[[LOGINUSER]]&pwd=[[LOGINPASS]]",
                    "",
                    "106824102",
                    "",
                    "1",
                    "http://[[IPADDRESS]]/videostream.cgi?user=[[LOGINUSER]]&pwd=[[LOGINPASS]]"
                ));
            _camModels.Add("D-Link DCS-5030L",
                new Camera(
                    "http://[[IPADDRESS]]/VIDEO.CGI",
                    "",
                    "106824102",
                    "",
                    "1",
                    "http://[[IPADDRESS]]/VIDEO.CGI"
                ));
            _camModels.Add("TP-Link NC450",
                new Camera(
                    "http://[[IPADDRESS]]/videostream.cgi?user=[[LOGINUSER]]&pwd=[[LOGINPASS]]",
                    "",
                    "106824102",
                    "",
                    "1",
                    "http://[[IPADDRESS]]/videostream.cgi?user=[[LOGINUSER]]&pwd=[[LOGINPASS]]"
                ));
        }

        #endregion

        #region Ip Box Control

        /// <summary>
        /// ограничение ввода
        /// </summary>
        private void IpBoxInputControl(object sender, TextCompositionEventArgs e)
        {
            var result = Regex.IsMatch(e.Text, @"\D");
            if (result)
            {
                e.Handled = true;
                return;
            }
            var box = (TextBox)sender;
            var str = box.Text + e.Text;
            if (int.Parse(str) > 255)
            {
                box.Text = "255";
                return;
            }

            if (str.Length >= 2 && str.Substring(0, 1) == "0")
            {
                e.Handled = true;
                return;
            }
            e.Handled = false;
        }

        /// <summary>
        /// очистка при фокусе
        /// </summary>
        private void IpBoxGotFocus(object sender, RoutedEventArgs e)
        {
            var box = (TextBox)sender;
            if (box.Text == "")
                return;
            box.Text = "";
        }

        #endregion
    }
}