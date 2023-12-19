using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DbLib.Defs;
using LargeDist.Models;
using System;
using System.Data;

namespace LargeDist.Infranstructures
{
    public class LargeDistRepository : IDisposable
    {
        public IDbConnection Connection { get; set; }
        public IDbTransaction Transaction { get; set; }

        private bool _commited;

        public LargeDistRepository()
        {
            Connection = DbFactory.CreateConnection();
            Transaction = Connection.BeginTransaction();
        }

        public void Commit()
        {
            Transaction.Commit();
            _commited = true;
        }

        public void Rollback()
        {
            Transaction.Rollback();
        }

        public void Dispose()
        {
            if (!_commited)
            {
                Rollback();
            }

            Transaction.Dispose();
            Connection.Dispose();
        }

        public void Save(DistItem item, Person person)
        {
            var result = item.ResultPiece + item.InputPiece;
            var id = item.Id;
            var status = result == item.OrderPiece ? Status.Completed 
                : result == 0 ? Status.Ready
                : Status.Inprog;

            var sql = Sql.Format<TBDISTEntity>($@"update {nameof(TBDISTEntity):T}
                set {nameof(TBDISTEntity.NULRPS):C} = @result
                , {nameof(TBDISTEntity.NUDOPS):C} = @result
                , {nameof(TBDISTEntity.FGLSTATUS):C} = @status
                , {nameof(TBDISTEntity.CDSHAINLARGE):C} = @person
                , {nameof(TBDISTEntity.NMSHAINLARGE):C} = @personName
                , {nameof(TBDISTEntity.DTWORKDTLARGE):C} = @date
                , {nameof(TBDISTEntity.UpdatedAt):C} = getdate()
                where {nameof(TBDISTEntity.IDDIST):C} = @id");

            Connection.Execute(sql, new { status, result, id, person = person.Code, 
                personName = person.Name, date = DateTime.Today.ToString("yyyyMMdd") }, Transaction);
        }
    }
}