using Customer.Models;
using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using TakakiLib.Models;

namespace Customer.Loader
{
    public class CustomerLoader
    {
        // メイン画面
        public static IEnumerable<SumCustomer> Get()
        {
            var sql = "SELECT max(t1.ID_SUM_TOKUISAKI) ID_SUM_TOKUISAKI"
                      + ",t1.CD_KYOTEN"
                      + ",t1.CD_SUM_TOKUISAKI"
                      + ",max(t1.DT_TEKIYOMUKO) DT_TEKIYOMUKO"
                      + ",max(t1.CD_HENKOSHA) CD_HENKOSHA"
                      + ",max(t1.NM_HENKOSHA) NM_HENKOSHA"
                      + ",max(t2.NM_TOKUISAKI) NM_TOKUISAKI"
                      + " FROM TB_SUM_TOKUISAKI t1"
                      +$" left join TB_MTOKUISAKI t2 on t2.CD_TOKUISAKI = t1.CD_SUM_TOKUISAKI and {CreateTekiyoSql.GetFromDate("t2.")}"
                      + " group by t1.CD_KYOTEN, t1.CD_SUM_TOKUISAKI"
                      + " order by t1.CD_KYOTEN, t1.CD_SUM_TOKUISAKI";

            using (var con = DbFactory.CreateConnection())
            {
                return con.Query(sql, new { selectDate = DateTime.Now.ToString("yyyyMMdd") }).Select(q => new SumCustomer
                {
                    CdKyoten = q.CD_KYOTEN,
                    CdSumTokuisaki = q.CD_SUM_TOKUISAKI,
                    NmSumTokuisaki = q.NM_TOKUISAKI,
                });
            }
        }

        // 集約得意先、適用日から取得(入力DLG)
        public static SumCustomer? GetFromKey(string cdSumTokuisaki, string dtTekiyoKaishi)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBSUMTOKUISAKIEntity>(s => s
                .Include<TBSUMTOKUISAKICHILDEntity>()
                .Where(@$"{nameof(TBSUMTOKUISAKIEntity.CDSUMTOKUISAKI):C} = {nameof(cdSumTokuisaki):P} and
                            {nameof(TBSUMTOKUISAKIEntity.DTTEKIYOKAISHI):C} = {nameof(dtTekiyoKaishi):P}")
                .WithParameters(new { cdSumTokuisaki, dtTekiyoKaishi }))
                    .Select(q => CreateSumcustomer(q))
                    .FirstOrDefault();
            }
        }

        // 同一得意先
        public static SumCustomer? GetSameCustomer(IEnumerable<string> targetCustomers, string startDate, string endDate, long? excludeId)
        {
            // 更新時、自ID対象外
            var updateId = excludeId?.ToString() ?? "-1";

            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBSUMTOKUISAKIEntity>(s => s
                .Include<TBSUMTOKUISAKICHILDEntity>()
                .Where(@$"({nameof(TBSUMTOKUISAKIEntity.CDSUMTOKUISAKI):C} in {nameof(targetCustomers):P} or 
                            {nameof(TBSUMTOKUISAKICHILDEntity.CDTOKUISAKICHILD):of TB_SUM_TOKUISAKI_CHILD} in {nameof(targetCustomers):P}) and 
                            {nameof(TBSUMTOKUISAKIEntity.IDSUMTOKUISAKI):of TB_SUM_TOKUISAKI} <> {nameof(updateId):P} and
                            {CreateTekiyoSql.GetFromRange()}")

                .WithParameters(new { targetCustomers, startDate, endDate, updateId }))
                    .Select(q => CreateSumcustomer(q))
                    .FirstOrDefault();
            }
        }

        // 得意先名称取得
        public static string GetName(string code, string selectDate)
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

        private static SumCustomer CreateSumcustomer(TBSUMTOKUISAKIEntity entity)
        {
            return new SumCustomer
            {
                SumTokuisakiId = entity.IDSUMTOKUISAKI,
                CdKyoten = entity.CDKYOTEN,
                CdSumTokuisaki = entity.CDSUMTOKUISAKI,
                // 入力欄側の検索処理で表示
                //NmSumTokuisaki = 
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,

                Tekiyokaishi = entity.DTTEKIYOKAISHI,
                TekiyoMuko = entity.DTTEKIYOMUKO,

                ChildCustomers = entity.TBSUMTOKUISAKICHILD!.Select(x => new ChildCustomer
                {
                    CdTokuisakiChild = x.CDTOKUISAKICHILD,
                    // 入力欄側の検索処理で表示
                    //NmTokuisaki = 
                }).ToList(),
            };
        }
    }
}
