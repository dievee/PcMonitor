using Microsoft.Extensions.Configuration;
using PCMonitor.Shared;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TrackerService.Services
{
    public class DeviceTrackerService : IDeviceTrackerService
    {
        private static Dictionary<int, PcInfoWithStatus> devices = new Dictionary<int, PcInfoWithStatus>();
        private static Dictionary<int, CancellationTokenSource> deviceIdsWithCancelTokens = new Dictionary<int, CancellationTokenSource>();
        private static int WaitPingSec { get; set; } 

        private readonly IAzureService _azureService;
        public IConfiguration _configuration { get; private set; }
        public DeviceTrackerService(IConfiguration configuration, IAzureService azureService)
        {
            this._configuration = configuration;
            this._azureService = azureService;
            try
            {
                WaitPingSec = int.Parse(_configuration.GetSection("AppSettings:WaitPingSec").Value);
            }
            catch(Exception e)
            {
                throw new Exception("Something wrong with config file");
            }
            
        }

        /// <summary>
        /// Processing of information received from the client
        /// </summary>
        /// <param name="pcInfo">Device information</param>
        /// <returns></returns>
        public async Task Start(PcInfo pcInfo)
        {
            var pcInfoWithStatus = new PcInfoWithStatus
            {
                PcId = pcInfo.PcId,
                DotNetVersion = pcInfo.DotNetVersion,
                OsName = pcInfo.OsName,
                PcName = pcInfo.PcName,
                TimeZone = pcInfo.TimeZone
            };

            var isChanged = CheckIsPcInfoChanged(pcInfoWithStatus);
            AddCancelToken(pcInfoWithStatus.PcId);
            var cts = deviceIdsWithCancelTokens[pcInfoWithStatus.PcId];
            cts.Cancel();
            cts = UpdateCancelToken(pcInfoWithStatus.PcId);

            //When device status(online/offline) or information changed
            if (isChanged)
            {
                AddOrUpdateDevice(pcInfoWithStatus);
                _azureService.Send(pcInfoWithStatus);
            }

            await Task.Delay(WaitPingSec * 1000, cts.Token).ContinueWith(t =>
            {
                //When device goes offline
                if (!t.IsCanceled)
                {
                    pcInfoWithStatus.Online = false;
                    AddOrUpdateDevice(pcInfoWithStatus);
                    _azureService.Send(pcInfoWithStatus);
                }
            });

        }
        private CancellationTokenSource UpdateCancelToken(int pcId)
        {
            deviceIdsWithCancelTokens[pcId] = new CancellationTokenSource();

            return deviceIdsWithCancelTokens[pcId];
        }

        private void AddCancelToken(int pcId)
        {
            var token = deviceIdsWithCancelTokens.GetValueOrDefault(pcId);

            if (token == null)
            {
                deviceIdsWithCancelTokens.Add(pcId, new CancellationTokenSource());
            }
        }

        /// <summary>
        /// Checks is pc info was changed after last state
        /// </summary>
        /// <param name="pcInfo"></param>
        /// <returns>Return true if a new device was added or pc info was changed</returns>
        private bool CheckIsPcInfoChanged(PcInfoWithStatus pcInfoWithStatus)
        {
            try
            {
                bool result = false;

                if (pcInfoWithStatus?.PcId == null)
                {
                    throw new Exception("PcInfo.Id is invalid.");
                }
                
                var device = devices.GetValueOrDefault(pcInfoWithStatus.PcId);

                if (device != null)
                {
                    result = !device.Equals(pcInfoWithStatus);
                }
                else
                {
                    result = true;
                }

                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        private void AddOrUpdateDevice(PcInfoWithStatus pcInfoWithStatus)
        {
            var device = devices.GetValueOrDefault(pcInfoWithStatus.PcId);

            if (device == null)
            {
                devices.Add(pcInfoWithStatus.PcId, pcInfoWithStatus);
            }
            else
            {
                devices[pcInfoWithStatus.PcId] = pcInfoWithStatus;
            }
        }
    }
}