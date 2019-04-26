using System;
using System.Collections.Generic;

namespace OfficeCheckerWPF
{
    public static class MainData
    {
        public static LogConsole LogConsole;
        public static bool ConsoleActive;

        public static readonly Version ActualIscVersion = new Version(2, 2, 0);

        public static List<ProgramInfo> Programs = new List<ProgramInfo>
        {
            new ProgramInfo("Google Chrome", "Google Chrome"),
            new ProgramInfo("Vacuum", "Vacuum-IM"),
            new ProgramInfo("1C:Enterprise 8 Thin client (8.3.10.2667)", "1C8 Thin client"),
            new ProgramInfo("Thunderbird", "Mozilla Thunderbird"),
            new ProgramInfo("TeamViewer", "TeamViewer"),
            new ProgramInfo("LibreOffice", "Libre Office"),
            new ProgramInfo("iSpy", "iSpy"),
            new ProgramInfo("OpenVPN", "OpenVPN"),
            new ProgramInfo("Salt", "Salt"),
            new ProgramInfo("Агент администрирования Kaspersky", "Агент администрирования KSC"),
            new ProgramInfo("Kaspersky Endpoint Security", "Kaspersky Endpoint Security"),
        };

        public static readonly List<ServiceInfo> Services = new List<ServiceInfo>
        {
            new ServiceInfo("MpsSvc", "Брандмауэр Windows"),
            new ServiceInfo("wuauserv", "Центр обновления Windows"),
        };

        public static readonly List<Servers> Servers = new List<Servers>
        {
            new Servers("Google DNS", "8.8.8.8"),
            new Servers("1C8 ТБ", "172.17.203.22"),
        };
    }
}