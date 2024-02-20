using DbLib.DbEntities;
using DbLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.FastCrud;
using System.Windows.Controls;
using DbLib.Defs;
using TakakiLib.Models;
using System.Windows.Documents;
using LogLib;
using System.Windows;
using Mapping.Defs;
using System.Collections.Concurrent;
using System.Runtime.Intrinsics.X86;

namespace Mapping.Models
{
    public class MappingLoader
    {
        public static List<SumTokuisaki> GetSumTokuisakis(string dtdelivery)
        {
            using (var con = DbFactory.CreateConnection())
            {
                var r = con.Find<TBSUMTOKUISAKIEntity>(s => s
                    .Where($"{nameof(TBSUMTOKUISAKIEntity.DTTEKIYOKAISHI):C} <= @dtdelivery and @dtdelivery < {nameof(TBSUMTOKUISAKIEntity.DTTEKIYOMUKO):C}")
                    .WithParameters(new { dtdelivery })
                    .OrderBy($"{nameof(TBSUMTOKUISAKIEntity.CDSUMTOKUISAKI)}"))
                    .Select((x, index) => new SumTokuisaki
                    {
                        IdSumTokuisaki = x.IDSUMTOKUISAKI,
                        CdKyoten = x.CDKYOTEN,
                        CdSumTokuisaki = x.CDSUMTOKUISAKI,
                        NmSumTokuisaki = NameLoader.GetTokuisaki(x.CDSUMTOKUISAKI),
                    }).ToList();

                if (r.Count() != 0)
                {
                    foreach (var p in r)
                    {
                        p.SumTokuisakiChilds = con.Find<TBSUMTOKUISAKICHILDEntity>(s => s
                            .Where($"{nameof(TBSUMTOKUISAKICHILDEntity.IDSUMTOKUISAKI):C}=@IdSumTokuisaki")
                            .WithParameters(new { p.IdSumTokuisaki })
                            .OrderBy($"{nameof(TBSUMTOKUISAKICHILDEntity.CDTOKUISAKICHILD)}"))
                            .Select(x => x.CDTOKUISAKICHILD).ToList();
                    }
                }

                return r;
            }
        }
        public static List<Block> GetBlocks(string dtdelivery)
        {
            using (var con = DbFactory.CreateConnection())
            {
                var r = con.Find<TBBLOCKEntity>(s => s
                    .Where($"{nameof(TBBLOCKEntity.DTTEKIYOKAISHI):C} <= @dtdelivery and @dtdelivery < {nameof(TBBLOCKEntity.DTTEKIYOMUKO):C}")
                    .WithParameters(new { dtdelivery })
                    .OrderBy($"{nameof(TBBLOCKEntity.CDBLOCK)}"))
                    .Select((x, index) => new Block
                    {
                        CdKyoten = x.CDKYOTEN,
                        CdBlock = x.CDBLOCK,
                        StTdUnitType = x.STTDUNITTYPE,
                        NuTdUnitCnt = x.NUTDUNITCNT,
                        NuThreshold = x.NUTHRESHOLD,
                    }).ToList();

                if (r.Count() != 0)
                {
                    foreach (var p in r)
                    {
                        var sql = "select * from tdunitaddr"
                                + " left join TB_LOCPOS on TB_LOCPOS.CD_BLOCK = @CdBlock and TB_LOCPOS.tdunitaddrcode = tdunitaddr.tdunitaddrcode"
                                + " where usageid = @StTdUnitType"
                                + " order by tdunitaddr.tdunitaddrcode";

                        p.addrs = con.Query(sql, new
                        {
                            @CdBlock = p.CdBlock,
                            @StTdUnitType = p.StTdUnitType,
                        })
                             .Select(q => new Addr
                             {
                                 TdUnitAddrCode = q.tdunitaddrcode,
                                 StRemove = q.ST_REMOVE ?? 0,
                             }).ToList();

                    }
                }

                return r;
            }
        }
        public static List<DistGroup> GetDistGroups(string dtdelivery, List<string> seldistgroups)
        {
            using (var con = DbFactory.CreateConnection())
            {
                var distgroups = con.Find<TBDISTGROUPEntity>(s => s
                    .Where($"{nameof(TBDISTGROUPEntity.DTTEKIYOKAISHI):C} <= @dtdelivery and @dtdelivery < {nameof(TBDISTGROUPEntity.DTTEKIYOMUKO):C} and {nameof(TBDISTGROUPEntity.CDDISTGROUP):C} in @seldistgroups")
                    .WithParameters(new { dtdelivery, seldistgroups })
                    .OrderBy($"{nameof(TBDISTGROUPEntity.CDDISTGROUP)}"))
                    .Select((x) => new DistGroup
                    {
                        CdKyoten = x.CDKYOTEN,
                        NmKyoten = NameLoader.GetKyoten(x.CDKYOTEN),
                        IdDistGroup = x.IDDISTGROUP,
                        CdDistGroup = x.CDDISTGROUP,
                        NmDistGroup = x.NMDISTGROUP,
                        CdBinSum = x.CDBINSUM,
                    }).ToList();

                foreach (var distgroup in distgroups)
                {
                    // 出荷バッチ
                    distgroup.ShukkaBatchs = con.Find<TBDISTGROUPSHUKKABATCHEntity>(s => s
                        .Where($"{nameof(TBDISTGROUPSHUKKABATCHEntity.IDDISTGROUP):C}=@IdDistGroup")
                        .WithParameters(new { distgroup.IdDistGroup })
                        .OrderBy($"{nameof(TBDISTGROUPSHUKKABATCHEntity.NUSHUKKABATCHSEQ)}"))
                     .Select((x) => new ShukkaBatch
                     {
                         NuShukkaBatchSeq = x.NUSHUKKABATCHSEQ,
                         CdShukkaBatch = x.CDSHUKKABATCH,
                     }).ToList();

                    distgroup.Courses = new List<string>();
                    foreach (var shukkabatch in distgroup.ShukkaBatchs)
                    {
                        // 出荷バッチコース取得
                        shukkabatch.NmShukkaBatch = NameLoader.GetNmShukkaBatch(shukkabatch.CdShukkaBatch);

                        // 先頭の出荷バッチのコースを使用
                        if (distgroup.Courses.Count == 0)
                        {
                            distgroup.Courses = con.Find<TBDISTGROUPCOURSEEntity>(s => s
                                .Where($"{nameof(TBDISTGROUPCOURSEEntity.IDDISTGROUP):C}=@IdDistGroup and {nameof(TBDISTGROUPCOURSEEntity.CDSHUKKABATCH):C}=@cdshukkabatch")
                                .WithParameters(new { @IdDistGroup = distgroup.IdDistGroup, @cdshukkabatch = shukkabatch.CdShukkaBatch })
                                .OrderBy($"{nameof(TBDISTGROUPCOURSEEntity.NUCOURSESEQ)}"))
                                .Select(x => x.CDCOURSE)
                                .ToList();
                        }
                    }

                    // 大仕分け
                    var largegroup = con.Find<TBDISTGROUPLARGEGROUPEntity>(s => s
                         .Where($"{nameof(TBDISTGROUPLARGEGROUPEntity.IDDISTGROUP):C}=@IdDistGroup")
                         .WithParameters(new { distgroup.IdDistGroup })
                         .OrderBy($"{nameof(TBDISTGROUPLARGEGROUPEntity.NULARGEGROUPSEQ)}"))
                      .Select((x) => new DistLargeGroup
                      {
                          CdLargeGroup = x.CDLARGEGROUP,
                          NmLargeGroup = NameLoader.GetLargeGroup(x.CDLARGEGROUP),
                      }).ToList();

                    for (int i = 0; i < distgroup.ShukkaBatchs.Count; i++)
                    {
                        if (i < largegroup.Count)
                        {
                            distgroup.ShukkaBatchs[i].CdLargeGroup = largegroup[i].CdLargeGroup;
                            distgroup.ShukkaBatchs[i].NmLargeGroup = largegroup[i].NmLargeGroup;
                        }
                    }

                    // ブロック順
                    var sql = "select * from TB_DIST_BLOCK"
                            + " inner join TB_DIST_BLOCK_SEQ on TB_DIST_BLOCK.ID_DIST_BLOCK = TB_DIST_BLOCK_SEQ.ID_DIST_BLOCK"
                            + " where CD_DIST_GROUP = @CdDistGroup"
                            + " and CD_KYOTEN = @cdkyoten"
                            + " and DT_TEKIYOKAISHI <= @dtdelivery and @dtdelivery < DT_TEKIYOMUKO"
                            + " order by NU_BLOCK_SEQ";

                    distgroup.DistBlockSeqs = con.Query(sql, new
                    {
                        @cdkyoten = distgroup.CdKyoten,
                        @dtdelivery = dtdelivery,
                        @CdDistGroup = distgroup.CdDistGroup,
                    })
                         .Select(q => new DistBlockSeq
                         {
                             CdBlock = q.CD_BLOCK,
                             CdAddrFrom = q.CD_ADDR_FROM,
                             CdAddrTo = q.CD_ADDR_TO,
                         }).ToList();

                }

                return distgroups;
            }
        }
        public static List<Dist> GetDists(DistGroup distgroup, string dtdelivery)
        {
            string whereCourse = string.Empty;
            foreach (var syukkabatch in distgroup.ShukkaBatchs)
            {
                whereCourse += whereCourse == string.Empty ? " and (" : " or ";
                whereCourse += $" (CD_SHUKKA_BATCH='" + syukkabatch.CdShukkaBatch + "')";
            }
            if (whereCourse != string.Empty)
                whereCourse += ")";

            if (distgroup.Courses.Count() != 0)
            {
                whereCourse += $" and CD_COURSE in ('" + String.Join("','", distgroup.Courses) + "')";
            }


            using (var con = DbFactory.CreateConnection())
            {
                var dists = con.Find<TBDISTEntity>(s => s
                .Where($"{nameof(TBDISTEntity.CDKYOTEN):C}=@cdkyoten and {nameof(TBDISTEntity.DTDELIVERY):C} = @dtdelivery {whereCourse}")
                .WithParameters(new { cdkyoten = distgroup.CdKyoten, dtdelivery })
                .OrderBy($"{nameof(TBDISTEntity.IDDIST)}"))
                .Select((x) => new Dist
                {
                    Id = x.IDDIST,
                    DtDelivery = x.DTDELIVERY,
                    CdKyoten = x.CDKYOTEN,
                    CdShukkaBatch = x.CDSHUKKABATCH,
                    CdCourse = x.CDCOURSE,
                    CdRoute = x.CDROUTE,
                    CdTokuisaki = x.CDTOKUISAKI,
                    CdHimban = x.CDHIMBAN,
                    CdGtin13 = x.CDGTIN13,
                    Ops = x.NUOPS,
                    CdHaishoBin = x.CDHAISHOBIN,
                    CdJuchuBin = x.CDJUCHUBIN,
                    CdGtin14 = x.CDGTIN14,
                    Rps = 0,
                    DtTorokuNichiji = x.DTTOROKUNICHIJI,
                    DtKoshinNichiji = x.DTKOSHINNICHIJI,
                    CdHenkosha = x.CDHENKOSHA,
                    createdAt = x.CreatedAt,

                }).ToList();

                return dists;
            }
        }
        public static List<Dist> GetStowages(DistGroup distgroup, string dtdelivery)
        {
            string whereCourse = string.Empty;

            foreach (var syukkabatch in distgroup.ShukkaBatchs)
            {
                whereCourse += whereCourse == string.Empty ? " and (" : " or ";
                whereCourse += $" (CD_SHUKKA_BATCH='" + syukkabatch.CdShukkaBatch + "')";
            }
            if (whereCourse != string.Empty)
                whereCourse += ")";

            if (distgroup.Courses.Count() != 0)
            {
                whereCourse += $" and CD_COURSE in ('" + String.Join("','", distgroup.Courses) + "')";
            }

            using (var con = DbFactory.CreateConnection())
            {
                var stowages = con.Find<TBSTOWAGEEntity>(s => s
                .Where($"{nameof(TBSTOWAGEEntity.CDKYOTEN):C}=@cdkyoten and {nameof(TBSTOWAGEEntity.DTDELIVERY):C} = @dtdelivery {whereCourse}")
                .WithParameters(new { distgroup.CdKyoten, dtdelivery })
                .OrderBy($"{nameof(TBSTOWAGEEntity.IDSTOWAGE)}"))
                .Select((x) => new Dist
                {
                    Id = x.IDSTOWAGE,
                    DtDelivery = x.DTDELIVERY,
                    CdKyoten = x.CDKYOTEN,
                    CdShukkaBatch = x.CDSHUKKABATCH,
                    CdCourse = x.CDCOURSE,
                    CdRoute = x.CDROUTE,
                    CdTokuisaki = x.CDTOKUISAKI,
                    StBoxType = x.STBOXTYPE,
                    NuBoxCnt = x.NUOBOXCNT,
                    CdHaiShobin = x.CDHAISHOBIN,
                    createdAt = x.CreatedAt,

                }).ToList();

                return stowages;
            }
        }
        public static int GetRackAllocMax()
        {
            using (var con = DbFactory.CreateConnection())
            using (var tr = con.BeginTransaction())
            {
                var settings = new Settings(tr);
                return settings.GetInt("RACKALLOCMAX", 10);
            }
        }
        public static void SetRackAllocMax(int rackallocmax)
        {
            using (var con = DbFactory.CreateConnection())
            using (var tr = con.BeginTransaction())
            {
                try
                {
                    var settings = new Settings(tr);
                    settings.Set("RACKALLOCMAX", rackallocmax);
                    tr.Commit();
                }
                catch (Exception)
                {
                    tr.Rollback();
                    throw;
                }

            }
        }
        public static IEnumerable<DpsOtherInfo> GetDpsOther(string dtDelivery)
        {
            using (var con = DbFactory.CreateConnection())
            {
                var sql = "select "
                    + "TB_DIST.CD_SHUKKA_BATCH, "
                    + "max(NM_SHUKKA_BATCH) NM_SHUKKA_BATCH, "
                    + "CD_COURSE, "
                    + "count(distinct CD_TOKUISAKI) shopcnt "
                    + "from TB_DIST "
                    + "left join(select* from (select CD_SHUKKA_BATCH, NM_SHUKKA_BATCH, row_number() over(partition by CD_SHUKKA_BATCH order by updatedAt desc) no "
                    + "from TB_MSHUKKA_BATCH where TB_MSHUKKA_BATCH.DT_TEKIYOKAISHI <= @dtDelivery and @dtDelivery<TB_MSHUKKA_BATCH.DT_TEKIYOMUKO) t3 "
                    + "where no = 1) v3 on TB_DIST.CD_SHUKKA_BATCH = v3.CD_SHUKKA_BATCH "
                    + "where TB_DIST.FG_MAPSTATUS = @mapStatus and TB_DIST.DT_DELIVERY = @dtDelivery "
                    + "group by TB_DIST.CD_SHUKKA_BATCH,CD_COURSE "
                    + "order by TB_DIST.CD_SHUKKA_BATCH,CD_COURSE ";

                return con.Query(sql, new
                {
                    @mapStatus = (int)Status.Ready,
                    @dtDelivery = dtDelivery,
                })
                    .Select(q => new DpsOtherInfo
                    {
                        CdShukkaBatch = q.CD_SHUKKA_BATCH,
                        NmShukkaBatch = q.NM_SHUKKA_BATCH,
                        CdCourse = q.CD_COURSE,
                        ShopCnt = q.shopcnt,
                    });
            }
        }    }
}
