using Customer.Models;
using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;

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
                      + " left join TB_MTOKUISAKI t2 on t2.CD_TOKUISAKI = t1.CD_SUM_TOKUISAKI"
                      + " group by t1.CD_KYOTEN, t1.CD_SUM_TOKUISAKI";

            using (var con = DbFactory.CreateConnection())
            {
                return con.Query(sql).Select(q => new SumCustomer
                {
                    CdKyoten = q.CD_KYOTEN,
                    CdSumTokuisaki = q.CD_SUM_TOKUISAKI,
                    NmSumTokuisaki = q.NM_TOKUISAKI,
                });
            }
        }

        // 参照日選択取得(入力DLG)
        public static SumCustomer? GetFromReferenceDate(string cdKyoten, string cdSumTokuisaki, DateTime referenceDate)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBSUMTOKUISAKIEntity>(s => s
                .Include<TBSUMTOKUISAKICHILDEntity>()
                .Where(@$"{nameof(TBSUMTOKUISAKIEntity.CDKYOTEN):C} = {nameof(cdKyoten):P} and
                            {nameof(TBSUMTOKUISAKIEntity.CDSUMTOKUISAKI):C} = {nameof(cdSumTokuisaki):P} and 
                            {nameof(referenceDate):P} between {nameof(TBSUMTOKUISAKIEntity.DTTEKIYOKAISHI):C} and {nameof(TBSUMTOKUISAKIEntity.DTTEKIYOMUKO):C}")
                .WithParameters(new { cdKyoten, cdSumTokuisaki, referenceDate }))
                    .Select(q => new SumCustomer
                    {
                        SumTokuisakiId = q.IDSUMTOKUISAKI,
                        CdKyoten = q.CDKYOTEN,
                        CdSumTokuisaki = q.CDSUMTOKUISAKI,
                        // 入力欄側の検索処理で表示
                        //NmSumTokuisaki = 
                        CreatedAt = q.CreatedAt,
                        UpdatedAt = q.UpdatedAt,

                        Tekiyokaishi = q.DTTEKIYOKAISHI,
                        TekiyoMuko = q.DTTEKIYOMUKO,

                        ChildCustomers = q.TBSUMTOKUISAKICHILD!.Select(x => new ChildCustomer
                        {
                            CdTokuisakiChild = x.CDTOKUISAKICHILD,
                            // 入力欄側の検索処理で表示
                            //NmTokuisaki = 
                        }).ToList(),
                    }).FirstOrDefault();
            }
        }

        // 拠点、集約得意先、適用日から取得(入力DLG)
        public static SumCustomer? GetFromKey(string cdKyoten, string cdSumTokuisaki, string dtTekiyoKaishi)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBSUMTOKUISAKIEntity>(s => s
                .Include<TBSUMTOKUISAKICHILDEntity>()
                .Where(@$"{nameof(TBSUMTOKUISAKIEntity.CDKYOTEN):C} = {nameof(cdKyoten):P} and
                                            {nameof(TBSUMTOKUISAKIEntity.CDSUMTOKUISAKI):C} = {nameof(cdSumTokuisaki):P} and
                                            {nameof(TBSUMTOKUISAKIEntity.DTTEKIYOKAISHI):C} = {nameof(dtTekiyoKaishi):P}")
                .WithParameters(new { cdKyoten, cdSumTokuisaki, dtTekiyoKaishi }))
                    .Select(q => new SumCustomer
                    {
                        SumTokuisakiId = q.IDSUMTOKUISAKI,
                        CdKyoten = q.CDKYOTEN,
                        CdSumTokuisaki = q.CDSUMTOKUISAKI,
                        // 入力欄側の検索処理で表示
                        //NmSumTokuisaki = 
                        CreatedAt = q.CreatedAt,
                        UpdatedAt = q.UpdatedAt,

                        Tekiyokaishi = q.DTTEKIYOKAISHI,

                        ChildCustomers = q.TBSUMTOKUISAKICHILD!.Select(x => new ChildCustomer
                        {
                            CdTokuisakiChild = x.CDTOKUISAKICHILD,
                            // 入力欄側の検索処理で表示
                            //NmTokuisaki = 
                        }).ToList(),
                    }).FirstOrDefault();
            }
        }

        // 得意先名称取得
        public static string GetName(string code)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBMTOKUISAKIEntity>(s => s
                .Where($"{nameof(TBMTOKUISAKIEntity.CDTOKUISAKI):C} = {nameof(code):P}")
                .WithParameters(new { code }))
                    .Select(q => q.NMTOKUISAKI)
                    .FirstOrDefault() ?? string.Empty;
            }
        }
    }
}
