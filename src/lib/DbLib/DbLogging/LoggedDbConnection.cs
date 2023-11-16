using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace DbLib.DbLogging
{
    public class LoggedDbConnection : DbConnection
    {
        private readonly DbConnection _connection;
        private readonly ILoggingHook _hook;

        public LoggedDbConnection(DbConnection connection, ILoggingHook hook)
        {
            _connection = connection;
            _hook = hook;

            _connection.StateChange += LoggedDbConnection_StateChange;
        }

        private void LoggedDbConnection_StateChange(object sender, StateChangeEventArgs e)
        {
            _hook.StateChange(this, e.CurrentState, e.OriginalState);
        }

        [AllowNull]
        public override string ConnectionString
        {
            get => _connection.ConnectionString;
            set => _connection.ConnectionString = value;
        }

        public override void Close()
        {
            try
            {
                _connection.Close();
            }
            finally
            {
                _hook.ConnectionClosed(this);
            }

        }

        public override void Open()
        {
            try
            {
                _connection.Open();
            }
            finally
            {
                _hook.ConnectionOpened(this);
            }
        }

        protected override DbCommand CreateDbCommand()
        {
            return new LoggedDbCommand(_connection.CreateCommand(), this, _hook);
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return new LoggedTransaction(this, _connection.BeginTransaction(), _hook);
        }

        public override string Database => _connection.Database;

        public override string DataSource => _connection.DataSource;

        public override string ServerVersion => _connection.ServerVersion;

        public override ConnectionState State => _connection.State;

        public override void ChangeDatabase(string databaseName) => _connection.ChangeDatabase(databaseName);

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    _connection?.Dispose();
                }

                base.Dispose(disposing);
            }
            finally
            {
                _hook.ConnectionDisposed(this);
            }
        }
    }
}
