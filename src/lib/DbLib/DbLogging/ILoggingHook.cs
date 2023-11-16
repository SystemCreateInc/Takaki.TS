using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbLib.DbLogging
{
    public interface ILoggingHook
    {
        void ConnectionOpened(DbConnection connection);
        void ConnectionClosed(DbConnection connection);
        void ConnectionDisposed(DbConnection connection);
        void Command(DbCommand command);
        void Begin(DbTransaction tran);
        void Commit(DbTransaction tran);
        void Rollback(DbTransaction tran);
        void StateChange(DbConnection connection, ConnectionState state, ConnectionState original);
    }
}
