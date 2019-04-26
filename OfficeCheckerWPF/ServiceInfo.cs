using System.Diagnostics;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace OfficeCheckerWPF
{
    public class ServiceInfo
    {
        public readonly string SearchName;
        public readonly string TitleName;
        public readonly CheckBox CheckBox;
        public readonly Button StartServiceButton;
      

        public ServiceInfo(string searchName, string titleName, bool createCheckBox = true, bool createStartServiceButton = true)
        {
            SearchName = searchName;
            TitleName = titleName;

            if (createCheckBox)
            {
                CheckBox = new CheckBox
                {
                    Content = titleName,
                    IsHitTestVisible = false,
                    Margin = new Thickness(3)
                };
            }

            if (createStartServiceButton)
            {
                StartServiceButton = new Button
                {
                    IsEnabled = true,
                    Visibility = Visibility.Collapsed,
                    Padding = new Thickness(2),
                    Margin = new Thickness(4,1,0,1),
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                SetButtonTrigger(StartServiceButton, CheckBox);
                StartServiceButton.Click += StartStopService;
            }
        }

        /// <summary>
        /// обработчик для кнопки службы
        /// </summary>
        /// <param name="sedner"></param>
        /// <param name="e"></param>
        private void StartStopService(object sedner, RoutedEventArgs e)
        {
            using (var sc = new ServiceController(SearchName))
            {
                Debug.Assert(CheckBox.IsChecked != null, "CheckBox.IsChecked != null");
                if ((bool) CheckBox.IsChecked)
                {
                    if (!sc.CanStop)
                    {
                        MessageBox.Show($"Службу {TitleName}\nне возможно остановить в данный момент!","Остановка не возможна");
                        return;
                    }
                        sc.Stop();
                        sc.WaitForStatus(ServiceControllerStatus.Stopped);
                        CheckBox.IsChecked = sc.Status != ServiceControllerStatus.Stopped;
                }
                else
                {
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running);
                    CheckBox.IsChecked = sc.Status == ServiceControllerStatus.Running;
                    if (SearchName == "MpsSvc")
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
                    }
                }
            }
        }

        /// <summary>
        /// привязка тригеров к кнопке службы
        /// </summary>
        /// <param name="button"></param>
        /// <param name="source"></param>
        private static void SetButtonTrigger(FrameworkElement button, CheckBox source)
        {
            var style = new Style {TargetType = typeof(Button)};
            style.Triggers.Clear();
            var trigger = new DataTrigger
            {
                Binding = new Binding
                {
                    Source = source,
                    Path = new PropertyPath("IsChecked")
                },
                Value = false
            };
            var setter = new Setter
            {
                Property = ContentControl.ContentProperty,
                Value = "Запустить службу"
            };
            trigger.Setters.Add(setter);
            style.Triggers.Add(trigger);
            trigger = new DataTrigger
            {
                Binding = new Binding
                {
                    Source = source,
                    Path = new PropertyPath("IsChecked")
                },
                Value = true
            };

            setter = new Setter
            {
                Property = ContentControl.ContentProperty,
                Value = "Остановить службу"
            };
            trigger.Setters.Add(setter);
            style.Triggers.Add(trigger);
            button.Style = style;
        }
    }
}