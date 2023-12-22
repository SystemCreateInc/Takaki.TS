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

        public IEnumerable<TBSTOWAGEEntity> GetStowageEntitys(string block, string deliveryDate,
            string? distGroup = null, string? tdCode = null, string? batch = null, string? tokuisaki = null)
        {
            var kyotenCd = GetKyotenCode();

            // 抽出 or NotNull
            var whereDistGroup = distGroup.IsNullOrEmpty() ? "is not null" : "= @distGroup";
            var whereTdCode = tdCode.IsNullOrEmpty() ? "is not null" : "= @tdCode";
            var whereBatch = batch.IsNullOrEmpty() ? "is not null" : "= @batch";
            var whereTokuisaki = tokuisaki.IsNullOrEmpty() ? "is not null" : "= @tokuisaki";

            return Connection.Find<TBSTOWAGEEntity>(s => s
                .AttachToTransaction(Transaction)
                .Include<TBSTOWAGEMAPPINGEntity>(x => x.InnerJoin())
                .Where(@$"{nameof(TBSTOWAGEEntity.CDKYOTEN):C} = {nameof(kyotenCd):P} and
                        {nameof(TBSTOWAGEEntity.DTDELIVERY):C} = {nameof(deliveryDate):P} and
                        {nameof(TBSTOWAGEMAPPINGEntity.CDBLOCK):of TB_STOWAGE_MAPPING} = {nameof(block):P} and
                        {nameof(TBSTOWAGEMAPPINGEntity.CDDISTGROUP):of TB_STOWAGE_MAPPING} {whereDistGroup} and
                        {nameof(TBSTOWAGEMAPPINGEntity.Tdunitaddrcode):of TB_STOWAGE_MAPPING} {whereTdCode} and
                        {nameof(TBSTOWAGEEntity.CDSHUKKABATCH):C} {whereBatch} and
                        {nameof(TBSTOWAGEEntity.CDTOKUISAKI):C} {whereTokuisaki}")
                .WithParameters(new { kyotenCd, block, deliveryDate, distGroup, tdCode, batch, tokuisaki }));
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
            // 数量変更
            if (stowage.IsChangeCount)
            {
                var sql = Sql.Format<TBSTOWAGEEntity>($@"update {nameof(TBSTOWAGEEntity):T} set
                {nameof(TBSTOWAGEEntity.NURBOXCNT):C} = {nameof(stowage.ResultBoxCount):P},
                {nameof(TBSTOWAGEEntity.FGSSTATUS):C} = {nameof(stowage.FgSStatus):P},
                {nameof(TBSTOWAGEEntity.DTWORKDTSTOWAGE):C} = {nameof(stowage.WorkDate):P},
                {nameof(TBSTOWAGEEntity.UpdatedAt):C} = {nameof(stowage.UpdatedAt):P}
                where {nameof(TBSTOWAGEEntity.IDSTOWAGE):C} = {nameof(stowage.Id):P}");

                Connection.Execute(sql, new { stowage.ResultBoxCount, stowage.FgSStatus, stowage.WorkDate, stowage.UpdatedAt, stowage.Id }, Transaction);
            }
            else
            {
                var sql = Sql.Format<TBSTOWAGEEntity>($@"update {nameof(TBSTOWAGEEntity):T} set
                {nameof(TBSTOWAGEEntity.FGSSTATUS):C} = {nameof(stowage.FgSStatus):P},
                {nameof(TBSTOWAGEEntity.DTWORKDTSTOWAGE):C} = {nameof(stowage.WorkDate):P},
                {nameof(TBSTOWAGEEntity.UpdatedAt):C} = {nameof(stowage.UpdatedAt):P}
                where {nameof(TBSTOWAGEEntity.IDSTOWAGE):C} = {nameof(stowage.Id):P}");

                Connection.Execute(sql, new { stowage.FgSStatus, stowage.WorkDate, stowage.UpdatedAt, stowage.Id }, Transaction);
            }
        }

        private string GetKyotenCode()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("common.json", true, true)
                .Build();

            var kyotenCd = config.GetSection("pc")?["cdkyoten"];

            if (string.IsNullOrEmpty(kyotenCd))
            {
                throw new Exception("拠点コードが設定されていません");
            }

            return kyotenCd;
        }
    }
}
