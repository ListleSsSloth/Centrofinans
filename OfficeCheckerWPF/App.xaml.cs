using System;111111
using System.Windows;

namespace OfficeCheckerWPF
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MainData.ConsoleActive = CheckConsoleActive();

            var args = e.Args;
            if (args.Length > 0 && args[0] == "-noguigpi")
            {
                AlternateRunsGpi.Run();
                Environment.Exit(0);
            }
            else
            {
                base.OnStartup(e);
            }
        }

        /// <summary>
        /// проверка параметра -console
        /// </summary>
        /// <returns></returns>
        private static bool CheckConsoleActive()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length <= 1)
                return false;
            return args.Length > 0 && args[1] == "-console";
        }
    }
}
