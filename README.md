Azure Function Project with connection string to Azure SQL(MS SQL) db has already published. To test solution you have to run WebService project (TrackerService) and Console app (DataCollector).

AppSettings:

*Console app (DataCollector) => appsettings.json => CronIntervalSec - the interval at which the application sends data to service.

*WebService project (TrackerService) => appsettings.json => AppSettings:WaitPingSec - the time after which the client that terminated the connection is considered turned off
