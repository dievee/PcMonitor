using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DataUpdater.Infrastructure;
using DataUpdater.Infrastructure.DB;
using LinqToDB;
using PCMonitor.Shared;
using System.Linq;

namespace DataUpdater
{
    public class UpdatePcStatus
    {
        private readonly AppDataConnection _connection;

        public UpdatePcStatus(AppDataConnection connection)
        {
            _connection = connection;
        }

        [FunctionName("UpdatePcStatus")]
        public  async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var pcInfoWithStatus = JsonConvert.DeserializeObject<PcInfoWithStatus>(requestBody);

                var deviceFromDb = _connection.Devices
                    .Where(x => x.DeviceId == pcInfoWithStatus.PcId)
                    .SingleOrDefault();

                if (deviceFromDb == null)
                {
                    _connection.Insert(new Device
                    {
                        DeviceId = pcInfoWithStatus.PcId,
                        DeviceName = pcInfoWithStatus.PcName,
                        DotNetVersion = pcInfoWithStatus.DotNetVersion,
                        Online = pcInfoWithStatus.Online,
                        OsName = pcInfoWithStatus.OsName,
                        TimeZone = pcInfoWithStatus.TimeZone
                    });
                }
                else
                {
                    if (!deviceFromDb.Equals(pcInfoWithStatus))
                    {
                        _connection.Devices
                            .Where(x => x.DeviceId == pcInfoWithStatus.PcId)
                            .Set(x => x.DeviceName, pcInfoWithStatus.PcName)
                            .Set(x => x.DotNetVersion, pcInfoWithStatus.DotNetVersion)
                            .Set(x => x.Online, pcInfoWithStatus.Online)
                            .Set(x => x.OsName, pcInfoWithStatus.OsName)
                            .Set(x => x.TimeZone, pcInfoWithStatus.TimeZone)
                            .Update();
                    }
                }              

                return new OkObjectResult(null);
            }
            catch(Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
            
        }
    }
}
