using System;

namespace PCMonitor.Shared
{
    public class PcInfo
    {
        public int PcId { get; set; }
        public string PcName { get; set; }
        public string TimeZone { get; set; }
        public string OsName { get; set; }
        public string DotNetVersion { get; set; }
    }
}
