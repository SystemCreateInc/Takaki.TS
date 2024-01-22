using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DbLib.Defs;
using ImTools;
using Picking.Models;
using SelDistGroupLib.Models;
using System;
using System.Data;
using System.Linq;
using System.Windows.Markup;
using WindowLib.Defs;

namespace Picking.Services
{
    public class DistProgressManager
    {
        // 進捗読み込み
        public static ProgressCnt? GetProgressCnts(DistGroup distgroup)
        {
            using (var con = DbFactory.CreateConnection())
            {
                var sql = @"select"
                        + " count(distinct CD_HIMBAN) itemmax,"
                        + " count(distinct case when TB_DIST.FG_DSTATUS >= @dstatus then CD_HIMBAN else null end) itemvalue,"
                        + " sum(NU_OPS) ops,"
                        + " sum(NU_DRPS) drps"
                        + " from TB_DIST"
                        + " inner join TB_DIST_MAPPING on TB_DIST.ID_DIST = TB_DIST_MAPPING.ID_DIST"
                        + " where DT_DELIVERY=@dt_delivery and CD_DIST_GROUP=@cd_dist_group and CD_BLOCK=@cd_block and FG_MAPSTATUS=@fg_mapstatus";

                return con.Query(sql, new
                {
                    dt_delivery = distgroup.DtDelivery.ToString("yyyyMMdd"),
                    cd_dist_group = distgroup.CdDistGroup,
                    cd_block = distgroup.CdBlock,
                    dstatus = (int)Status.Inprog,
                    fg_mapstatus = (int)Status.Completed,
                })
                     .Select(q => new ProgressCnt
                     {
                         DT_DELIVERY = distgroup.DtDelivery.ToString("yyyyMMdd"),
                         CD_DIST_GROUP = distgroup.CdDistGroup,
                         ItemCntValue = q.itemvalue ?? 0,
                         ItemCntMax = q.itemmax ?? 0,
                         CntMax = q.ops ?? 0,
                         CntValue = q.drps ?? 0,
                     }).FirstOrDefault();
            }
        }

        public static void UpdateDistProgress(DistGroup distgroup, DistColorInfo distcolorinfo, ProgressCnt pcnt, bool bEnd=false)
        {
            // 数量０は更新しない
            if (pcnt.CntMax==0)
                return;

            using (var con = DbFactory.CreateConnection())
            {
                DateTime endtm = DateTime.Now;

                var tr = con.BeginTransaction();
                try
                {
                    bool bCpmpleted = pcnt.CntMax == pcnt.CntValue;

                    var p = GetDistGroupProgress(con, tr, distgroup);
                    if (p==null)
                    {
                        tr.Commit();
                        return;
// マッピングで作成する様に変更
#if false
                        // 新規追加
                        p = new TBDISTGROUPPROGRESSEntity
                        {
                            DTDELIVERY = distgroup.DtDelivery.ToString("yyyyMMdd"),
                            CDKYOTEN = distgroup.CdKyoten,
                            NMKYOTEN = "",
                            CDDISTGROUP = distgroup.CdDistGroup,
                            NMDISTGROUP = distgroup.NmDistGroup,
                            IDPC = distgroup.IdPc,
                            CDBLOCK = distgroup.CdBlock,
                            DTSTART = null,
                            CDSHAIN = null,
                            NMSHAIN = null,
                            NUOITEMCNT = pcnt.ItemCntMax,
                            NURITEMCNT = pcnt.ItemCntValue,
                            NUOPS = pcnt.CntMax,
                            NURPS = pcnt.CntValue,
                            FGDSTATUS = (short)(bCpmpleted ? (short)Status.Completed : (short)Status.Inprog),
                            FGWORKING = (short)Status.Inprog,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                        };
                        con.Insert(p, x => x.AttachToTransaction(tr));
#endif
                    }
                    else
                    {
                        if (p.FGDSTATUS == (int)Status.Completed && bCpmpleted==true)
                        {
                            //既に完了しているので作業者のみ更新
                        }
                        else
                        {
                            // 更新
                            p.IDPC = distgroup.IdPc;

                            if (bCpmpleted == false && p.DTEND != null)
                            {
                                p.DTEND = null;
                            }
                            if ((bCpmpleted && p.DTEND==null) || (bEnd && p.DTEND == null))
                            {
                                p.DTEND = DateTime.Now;
                            }

                            p.NUOITEMCNT = pcnt.ItemCntMax;
                            p.NURITEMCNT = pcnt.ItemCntValue;
                            p.NUOPS = pcnt.CntMax;
                            p.NURPS = pcnt.CntValue;
                            p.FGDSTATUS = (short)(bCpmpleted ? (short)Status.Completed : (short)Status.Inprog);
                        }

                        p.UpdatedAt = DateTime.Now;
                        p.FGWORKING = bEnd ? (short)Status.Ready : (short)Status.Inprog;

                        // 担当者設定
                        p.CDSHAIN = null;
                        p.NMSHAIN = null;
                        foreach (var distcolor in distcolorinfo.DistColors!)
                        {
                            if (distcolor != null)
                            {
                                if (distcolor.ReportShain.DtWorkStart != null)
                                {
                                    p.CDSHAIN = distcolor.ReportShain.CdShain;
                                    p.NMSHAIN = distcolor.ReportShain.NmShain;
                                    if (p.DTSTART == null)
                                    {
                                        p.DTSTART = distcolor.ReportShain.DtWorkStart;
                                    }
                                    break;
                                }
                            }
                        }

                        con.Update(p, x => x.AttachToTransaction(tr));
                    }

                    // 同じ納品日のＰＣは全て未処理へ戻す
                    var sql = "update TB_DIST_GROUP_PROGRESS set FG_WORKING = @status"
                            + " where DT_DELIVERY = @dtdelivery and CD_KYOTEN = @cdkyoten and CD_BLOCK = @cdblock and CD_DIST_GROUP<>''";

                    con.Execute(sql,
                    new
                    {
                        status = (int)Status.Ready,
                        dtdelivery = distgroup.DtDelivery.ToString("yyyyMMdd"),
                        cdkyoten = distgroup.CdKyoten,
                        cdblock = distgroup.CdBlock,
                    }, tr);

                    tr.Commit();
                }
                catch (Exception)
                {
                    tr.Rollback();
                    throw;
                }
            }
        }

        private static TBDISTGROUPPROGRESSEntity? GetDistGroupProgress(IDbConnection con, IDbTransaction tr, DistGroup distgroup)
        {
            return con.Find<TBDISTGROUPPROGRESSEntity>(s => s
                    .Where($"{nameof(TBDISTGROUPPROGRESSEntity.DTDELIVERY):C}=@dtdelivery and {nameof(TBDISTGROUPPROGRESSEntity.CDKYOTEN):C}=@cdkyoten and {nameof(TBDISTGROUPPROGRESSEntity.CDDISTGROUP):C}=@cddistgroup and {nameof(TBDISTGROUPPROGRESSEntity.CDBLOCK):C}=@cdblock")
                    .WithParameters(new { 
                        dtdelivery= distgroup.DtDelivery.ToString("yyyyMMdd"),
                        cdkyoten = distgroup.CdKyoten,
                        cddistgroup = distgroup.CdDistGroup,
                        cdblock = distgroup.CdBlock,
                    })
                    .AttachToTransaction(tr))
                .FirstOrDefault();
        }
    }
}