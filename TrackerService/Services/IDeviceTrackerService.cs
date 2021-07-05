using PCMonitor.Shared;
using System.Threading.Tasks;

namespace TrackerService.Services
{
    public interface IDeviceTrackerService
    {
        Task Start(PcInfo pcInfo);
    }
}
