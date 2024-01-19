using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using Microsoft.IdentityModel.Tokens;
using StowageSvr.Models;
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

        public string? GetPersonName(string person)
        {
            return Connection.Find<TBMSHAINEntity>(s => s
                .AttachToTransaction(Transaction)
                .Where($"{nameof(TBMSHAINEntity.CDSHAIN):C} = {nameof(person):P}")
                .WithParameters(new { person }))
                .Select(x => x.NMSHAIN)
                .FirstOrDefault();
        }

        public IEnumerable<TBSTOWAGEEntity> GetStowageEntitys(string block, string deliveryDate,
            string? distGroup = null, string? tdCode = null, string? batch = null, string? tokuisaki = null)
        {
            return Connection.Find<TBSTOWAGEEntity>(s => s
                .AttachToTransaction(Transaction)
                .Include<TBSTOWAGEMAPPINGEntity>(x => x.InnerJoin())
                .Where(@$"{nameof(TBSTOWAGEEntity.DTDELIVERY):C} = {nameof(deliveryDate):P} and
                        {nameof(TBSTOWAGEMAPPINGEntity.CDBLOCK):of TB_STOWAGE_MAPPING} = {nameof(block):P} and
                        {nameof(TBSTOWAGEMAPPINGEntity.CDDISTGROUP):of TB_STOWAGE_MAPPING} {GetWhereParamSql(distGroup, nameof(distGroup))} and
                        {nameof(TBSTOWAGEMAPPINGEntity.Tdunitaddrcode):of TB_STOWAGE_MAPPING} {GetWhereParamSql(tdCode, nameof(tdCode))} and
                        {nameof(TBSTOWAGEEntity.CDSHUKKABATCH):C} {GetWhereParamSql(batch, nameof(batch))} and
                        {nameof(TBSTOWAGEEntity.CDTOKUISAKI):C} {GetWhereParamSql(tokuisaki, nameof(tokuisaki))}")
                .WithParameters(new { block, deliveryDate, distGroup, tdCode, batch, tokuisaki }));
        }

        // 抽出 or NotNull
        private object GetWhereParamSql(string? param, string paramName)
        {
            return param.IsNullOrEmpty() ? "is not null" : $"= @{paramName}";
        }

        public IEnumerable<TBSTOWAGEEntity> GetStowageEntitys(IEnumerable<long> stowageIds)
        {
            return Connection.Find<TBSTOWAGEEntity>(s => s
                .AttachToTransaction(Transaction)
                .Where($"{nameof(TBSTOWAGEEntity.IDSTOWAGE):C} in {nameof(stowageIds):P}")
                .WithParameters(new { stowageIds }));
        }

        public void UpdateStowageEntity(Stowage stowage)
        {
            var sql = Sql.Format<TBSTOWAGEEntity>($@"update {nameof(TBSTOWAGEEntity):T} set
                {nameof(TBSTOWAGEEntity.NURBOXCNT):C} = {nameof(stowage.ResultBoxCount):P},
                {nameof(TBSTOWAGEEntity.FGSSTATUS):C} = {nameof(stowage.FgSStatus):P},
                {nameof(TBSTOWAGEEntity.DTWORKDTSTOWAGE):C} = {nameof(stowage.WorkDate):P},
                {nameof(TBSTOWAGEEntity.DTKOSHINNICHIJI):C} = {nameof(stowage.UpdatedAtStr):P},
                {nameof(TBSTOWAGEEntity.CDHENKOSHA):C} = {nameof(stowage.Person):P},
                {nameof(TBSTOWAGEEntity.NMHENKOSHA):C} = {nameof(stowage.PersonName):P},
                {nameof(TBSTOWAGEEntity.UpdatedAt):C} = {nameof(stowage.UpdatedAt):P}
                where {nameof(TBSTOWAGEEntity.IDSTOWAGE):C} = {nameof(stowage.Id):P}");

            Connection.Execute(sql, 
                new { stowage.ResultBoxCount, stowage.FgSStatus, stowage.WorkDate, stowage.Person, stowage.PersonName, stowage.UpdatedAt, stowage.UpdatedAtStr, stowage.Id }, 
                Transaction);
        }
    }
}
