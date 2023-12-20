using Dapper.FastCrud;
using DbLib;
using System.Data;

namespace StowageSvr.Reporitories
{
    public class StowageRepository : IStowageRepository
    {
        private IDbConnection Connection { get; set; }
        private IDbTransaction Transaction { get; set; }
        private bool _commited = false;

        public StowageRepository()
        {
            Connection = DbFactory.CreateConnection();
            Transaction = Connection.BeginTransaction();
        }

        public void Dispose()
        {
            if (!_commited)
            {
                Transaction.Rollback();
            }

            Connection?.Close();
        }

        public void Commit()
        {
            if (!_commited)
            {
                Transaction.Commit();
                _commited = true;
            }
        }

        public AppLock Lock()
        {
            return new AppLock(Connection, "StowageSvr");
        }

        public void ResistStock(string labelRegNo)
        {
            return;
        }

        
    }
}
