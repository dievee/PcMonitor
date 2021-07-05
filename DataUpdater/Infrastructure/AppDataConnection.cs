using DataUpdater.Infrastructure.DB;
using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;

namespace DataUpdater.Infrastructure
{
    public class AppDataConnection : DataConnection
    {
        public AppDataConnection(LinqToDbConnectionOptions<AppDataConnection> options)
            : base(options)
        {

        }

        public ITable<Device> Devices => GetTable<Device>();
    }
}