using PCMonitor.Shared;

namespace TrackerService.Services
{
    public interface IAzureService
    {
        void Send(PcInfoWithStatus pcInfoWithStatus);
    }
}
