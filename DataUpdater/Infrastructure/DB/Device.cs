using LinqToDB.Mapping;
using PCMonitor.Shared;
using System;

namespace DataUpdater.Infrastructure.DB
{
    [Table(Name = "Devices")]
    public class Device
    {
        [PrimaryKey, Identity]
        public int RecordId { get; set; }

        [Column(Name = "DeviceId"), NotNull]
        public int DeviceId { get; set; }

        [Column(Name = "DeviceName"), NotNull]
        public string DeviceName { get; set; }

        [Column(Name = "TimeZone"), NotNull]
        public string TimeZone { get; set; }

        [Column(Name = "OsName"), NotNull]
        public string OsName { get; set; }

        [Column(Name = "DotNetVersion"), NotNull]
        public string DotNetVersion { get; set; }

        [Column(Name = "Online"), NotNull]
        public bool Online { get; set; }


        public bool Equals(PcInfoWithStatus other)
        {
            if (DotNetVersion != other.DotNetVersion ||
                OsName != other.OsName ||
                DeviceName != other.PcName ||
                TimeZone != other.TimeZone ||
                Online != other.Online)
                return false;

            return true;
        }
    }
}
