using Dapper;
using LogLib;
using System;
using System.Data;
using System.Data.Common;

namespace DbLib
{
    public class AppLock : IDisposable
    {
        public enum LockMode
        {
            Shared,
            Update,
            Intentshared,
            Exclusive,
        }

        public enum LockOwner
        {
            Session,
            Transaction,
        }

        public string Resource { get; private set; }
        public LockMode Mode { get; set; }
        public LockOwner Owner { get; set; }
        public IDbConnection Connection;
        public bool DisposeConnection { get; set; }
        public IDbTransaction Transaction { get; set; }

        public AppLock(IDbConnection con, string resource, LockMode lockMode = LockMode.Exclusive, LockOwner lockOwner = LockOwner.Session, bool disposeConnection = false)
            : this(con, null, resource, lockMode, lockOwner, disposeConnection)
        {
        }

        public AppLock(IDbConnection con, IDbTransaction transaction, string resource, LockMode lockMode = LockMode.Exclusive, LockOwner lockOwner = LockOwner.Transaction, bool disposeConnection = false)
        {
            Connection = con;
            Mode = lockMode;
            Owner = lockOwner;
            Resource = resource;
            DisposeConnection = disposeConnection;

            con.Execute("sp_getapplock", new
            {
                @Resource = resource,
                @LockMode = lockMode.ToString(),
                @LockOwner = lockOwner.ToString(),
            }, 
            commandType: System.Data.CommandType.StoredProcedure,
            transaction: transaction);

            Transaction = transaction;
        }

        public void Dispose()
        {
            if (Connection == null)
                return;

            try
            {
                Connection.Execute("sp_releaseapplock", new
                {
                    @Resource = Resource,
                    @LockOwner = Owner.ToString(),
                }, 
                commandType: System.Data.CommandType.StoredProcedure,
                transaction: Transaction);
            }
            catch (InvalidOperationException ex)
            {
                Syslog.Warn($"AppLock: EXCEPTION: {ex.Message}");
            }

            if (DisposeConnection)
            {
                Connection.Dispose();
            }

            Connection = null;
        }
    }
}
