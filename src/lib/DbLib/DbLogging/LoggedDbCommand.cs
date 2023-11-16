using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace DbLib.DbLogging
{
    public class LoggedDbCommand : DbCommand
    {
        private DbCommand _command;
        private DbConnection? _connection;
        private ILoggingHook _hook;

        public LoggedDbCommand(DbCommand dbCommand, DbConnection loggedDbConnection, ILoggingHook hook)
        {
            _command = dbCommand;
            _connection = loggedDbConnection;
            _hook = hook;
        }

        public override int ExecuteNonQuery()
        {
            try
            {
                return _command.ExecuteNonQuery();
            }
            finally
            {
                _hook.Command(this);
            }
        }

        public override object ExecuteScalar()
        {
            try
            {
                return _command.ExecuteScalar();
            }
            finally
            {
                _hook.Command(this);
            }
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            try
            {
                return _command.ExecuteReader(behavior);
            }
            finally
            {
                _hook.Command(this);
            }
        }

        public override void Cancel() => _command.Cancel();
        public override void Prepare() => _command.Prepare();
        protected override DbParameter CreateDbParameter() => _command.CreateParameter();
        [AllowNull]
        public override string CommandText { get => _command.CommandText; set => _command.CommandText = value; }
        public override int CommandTimeout { get => _command.CommandTimeout; set => _command.CommandTimeout = value; }
        public override CommandType CommandType { get => _command.CommandType; set => _command.CommandType = value; }
        public override bool DesignTimeVisible { get => _command.DesignTimeVisible; set => _command.DesignTimeVisible = value; }
        public override UpdateRowSource UpdatedRowSource { get => _command.UpdatedRowSource; set => _command.UpdatedRowSource = value; }
        protected override DbConnection? DbConnection { get => _connection; set => _connection = value; }

        protected override DbParameterCollection DbParameterCollection => _command.Parameters;

        protected override DbTransaction? DbTransaction 
        { 
            get => _command.Transaction; 
            set
            {
                _command.Transaction = ((LoggedTransaction?)value)?._transaction; 
            }
        }

    }
}
