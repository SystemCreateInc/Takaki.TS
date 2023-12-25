using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using System;
using System.Linq;

namespace TakakiLib.Models
{
    // 適用日参照名称取得
    public class NameLoader
    {
        public static string selectDate = DateTime.Today.ToString();

        public static string GetTokuisaki(string code)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBMTOKUISAKIEntity>(s => s
                .Where(@$"{nameof(TBMTOKUISAKIEntity.CDTOKUISAKI):C} = {nameof(code):P} and {CreateTekiyoSql.GetFromDate()}")
                .WithParameters(new { code, selectDate }))
                    .Select(q => q.NMTOKUISAKI)
                    .FirstOrDefault() ?? string.Empty;
            }
        }

        public static string GetKyoten(string code)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBMKYOTENEntity>(s => s
                .Where($"{nameof(TBMKYOTENEntity.CDKYOTEN):C} = {nameof(code):P} and {CreateTekiyoSql.GetFromDate()}")
                .WithParameters(new { code, selectDate }))
                    .Select(q => q.NMKYOTEN)
                    .FirstOrDefault() ?? string.Empty;
            }
        }

        public static string GetNmShukkaBatch(string code)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBMSHUKKABATCHEntity>(s => s
                .Where($"{nameof(TBMSHUKKABATCHEntity.CDSHUKKABATCH):C} = {nameof(code):P} and {CreateTekiyoSql.GetFromDate()}")
                .WithParameters(new { code, selectDate }))
                    .Select(q => q.NMSHUKKABATCH)
                    .FirstOrDefault() ?? string.Empty;
            }
        }
        public static string GetDistGroup(string code)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBDISTGROUPEntity>(s => s
                .Where($"{nameof(TBDISTGROUPEntity.CDDISTGROUP):C} = {nameof(code):P} and {CreateTekiyoSql.GetFromDate()}")
                .WithParameters(new { code, selectDate }))
                    .Select(q => q.NMDISTGROUP)
                    .FirstOrDefault() ?? string.Empty;
            }
        }

        public static string GetLargeGroup(string code)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBLARGEGROUPEntity>(s => s
                .Where($"{nameof(TBLARGEGROUPEntity.CDLARGEGROUP):C} = {nameof(code):P} and {CreateTekiyoSql.GetFromDate()}")
                .WithParameters(new { code, selectDate }))
                    .Select(q => q.CDLARGEGROUPNAME)
                    .FirstOrDefault() ?? string.Empty;
            }
        }
        public static string GetHimban(string code)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBMHIMMOKUEntity>(s => s
                .Where(@$"{nameof(TBMHIMMOKUEntity.CDHIMBAN):C} = {nameof(code):P} and {CreateTekiyoSql.GetFromDate()}")
                .WithParameters(new { code, selectDate }))
                    .Select(q => q.NMHINSEISHIKIMEI)
                    .FirstOrDefault() ?? string.Empty;
            }
        }
    }
}
