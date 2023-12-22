using DbLib.DbLogging;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common;

namespace DbLib
{
    public class DbFactory : IDbFactory
    {
        public IDbConnection Create(string? serverip = null)
        {
            return CreateConnectionRaw(serverip);
        }
        public IDbConnection CreateConnectionRaw(string? serverip)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("common.json", true, true)
                .Build();

            var conStr = config.GetConnectionString("data");
            var builder = new SqlConnectionStringBuilder(conStr);
            // アプリケーション名をセット
            builder.ApplicationName = AppDomain.CurrentDomain.FriendlyName;
            builder.CommandTimeout = 0;
            SqlClientFactory.Instance.CreateConnection();
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
            var factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
            var con = new LoggedDbConnection(factory.CreateConnection(), new SyslogHook());
            con.ConnectionString = builder.ConnectionString;
            con.Open();
            return con;
        }

        public static IDbConnection CreateConnection(string? serverip=null)
        {
            return new DbFactory().Create(serverip);
        }
    }
}
