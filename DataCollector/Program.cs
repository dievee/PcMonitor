using DataCollector.Jobs;
using Microsoft.Extensions.Configuration;
using PCMonitor.Shared;
using Quartz;
using Quartz.Impl;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DataCollector
{
    class Program
    {
        public static int PcId { get; set; }
        public static string ServiceDomain { get; set; }
        public static int CronIntervalSec { get; set; }

        static async Task Main(string[] args)
        {
            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();

                ServiceDomain = configuration.GetSection("ServiceDomain").Value;
                PcId = Int32.Parse(configuration.GetSection("PcId").Value);
                CronIntervalSec = Int32.Parse(configuration.GetSection("CronIntervalSec").Value);

            }
            catch(Exception e)
            {
                throw new Exception("Something wrong with config file");
            }

            await ConfigureQuartz();

            Console.ReadKey();
        }

       public static PcInfo GetPcInfo()
        {
            //This code wasn't tested on macOS/Linux
            var pcInfo = new PcInfo()
            {
                PcId = PcId,
                PcName = Environment.MachineName,
                TimeZone = TimeZoneInfo.Local.ToString(),
                OsName = Environment.OSVersion.VersionString,
                DotNetVersion = Environment.Version.ToString(),
            };

            return pcInfo;
        }

       private static async Task ConfigureQuartz()
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<SendPcInfoJob>()
                .WithIdentity("sendPcInfoJob", "sendPcInfoGroup")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("sendPcInfoTrigger", "sendPcInfoGroup")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(CronIntervalSec)
                    .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
