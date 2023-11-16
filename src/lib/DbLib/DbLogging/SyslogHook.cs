using LogLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbLib.DbLogging
{
    public class SyslogHook : ILoggingHook
    {
        public void ConnectionClosed(DbConnection connection)
        {
            Syslog.Debug($"DB({connection.GetHashCode()}): Connection Closed {connection.Database}@{connection.DataSource}");
        }

        public void ConnectionOpened(DbConnection connection)
        {
            Syslog.Debug($"DB({connection.GetHashCode()}): Connection Opened {connection.Database}@{connection.DataSource}");
        }

        public void ConnectionDisposed(DbConnection connection)
        {
            Syslog.Debug($"DB({connection.GetHashCode()}): Connection Disposed {connection.Database}@{connection.DataSource}");
        }

        public void Command(DbCommand command)
        {
            var prms = GetParameters(command);
            var paramText = string.Join(",", prms.Select(x => $"{x.Key}: {x.Value}"));

            Syslog.Debug($"DB({command.Connection.GetHashCode()}): Command: {command.CommandText} Parameters: {paramText}");
        }

        private Dictionary<string, object> GetParameters(DbCommand command, bool hideValues = false)
        {
            IEnumerable<DbParameter> GetParameters()
            {
                foreach (DbParameter parameter in command.Parameters)
                    yield return parameter;
            }

            return GetParameters()
                .ToDictionary(
                    k => k.ParameterName,
                    v => hideValues
                        ? "?"
                        : v.Value == null || v.Value is DBNull
                            ? "<null>"
                            : v.Value);
        }

        public void Begin(DbTransaction tran)
        {
            Syslog.Debug($"DB({tran.Connection.GetHashCode()}): Begin Transaction");
        }

        public void Commit(DbTransaction tran)
        {
            Syslog.Debug($"DB({tran.Connection.GetHashCode()}): Commit Transaction");
        }

        public void Rollback(DbTransaction tran)
        {
            Syslog.Debug($"DB({tran.Connection.GetHashCode()}): Rollback Transaction");
        }

        public void StateChange(DbConnection connection, ConnectionState state, ConnectionState original)
        {
            Syslog.Debug($"DB({connection.GetHashCode()}): State Change {original} -> {state}");
        }
    }
}
