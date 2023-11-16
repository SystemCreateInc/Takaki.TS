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
    class LoggedTransaction : DbTransaction
    {
        public readonly DbTransaction _transaction;
        private readonly DbConnection _connection;
        private readonly ILoggingHook _hook;

        public LoggedTransaction(DbConnection con, DbTransaction transaction, ILoggingHook hook)
        {
            _connection = con;
            _transaction = transaction;
            _hook = hook;
            _hook.Begin(this);
        }

        public override IsolationLevel IsolationLevel => _transaction.IsolationLevel;

        protected override DbConnection DbConnection => _connection;

        public override void Commit()
        {
            _transaction.Commit();
            _hook.Commit(this);
        }

        public override void Rollback()
        {
            _transaction.Rollback();
            _hook.Rollback(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _transaction?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
