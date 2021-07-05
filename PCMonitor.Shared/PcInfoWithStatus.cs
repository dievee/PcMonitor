namespace PCMonitor.Shared
{
    public class PcInfoWithStatus : PcInfo
    {
        public bool Online { get; set; } = true;

        public override bool Equals(object obj)
        {
            if (!(obj is PcInfoWithStatus))
                return false;

            var other = obj as PcInfoWithStatus;

            if (DotNetVersion != other.DotNetVersion ||
                OsName != other.OsName ||
                PcName != other.PcName ||
                TimeZone != other.TimeZone ||
                Online != other.Online)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hashDotNetVersion = DotNetVersion == null ? 0 : DotNetVersion.GetHashCode();
            int hashOsName = OsName == null ? 0 : OsName.GetHashCode();
            int hashPcName = PcName == null ? 0 : PcName.GetHashCode();
            int hashTimeZone = TimeZone == null ? 0 : TimeZone.GetHashCode();
            int hashOnline = Online ? 1 : 0;

            return hashDotNetVersion ^ 
                hashOsName ^
                hashPcName ^
                hashTimeZone ^
                hashOnline;
        }
    }
}
