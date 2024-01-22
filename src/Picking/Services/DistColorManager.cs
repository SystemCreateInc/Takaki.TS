using Azure;
using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DbLib.Defs;
using ImTools;
using LogLib;
using Picking.Models;
using SelDistGroupLib.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Transactions;
using System.Windows.Documents;
using System.Windows.Input;
using TdDpsLib.Models;
using WindowLib.Views;
using static DbLib.AppLock;

namespace Picking.Services
{
    public class DistColorManager
    {
        // 状況一覧読み込み
        public static List<DistInfo>? LoadInfs(DistGroup distgroup)
        {
            using (var con = DbFactory.CreateConnection())
            {
                var sql = @"select CD_DIST_GROUP,DT_DELIVERY,TB_DIST.CD_HIMBAN,ST_BOXTYPE,NU_BOXUNIT,max(NM_HIN_SEISHIKIMEI) NM_HIN_SEISHIKIMEI, CD_GTIN13,CD_GTIN14,CD_SHUKKA_BATCH,CD_JUCHU_BIN"
                        + ",sum(NU_OPS) ops,sum(NU_DOPS) dops,sum(NU_DRPS) drps"
                        + ",(case when min(TB_DIST.FG_DSTATUS) = max(TB_DIST.FG_DSTATUS) then max(TB_DIST.FG_DSTATUS) else @dstatus end) dstatus"
                        + ",(case when min(TB_DIST.FG_LSTATUS) = max(TB_DIST.FG_LSTATUS) then max(TB_DIST.FG_LSTATUS) else @dstatus end) lstatus"
                        + ",count(distinct TB_DIST_MAPPING.tdunitaddrcode) order_shop_cnt"
                        + ",count(distinct case when NU_DOPS=NU_DRPS then TB_DIST_MAPPING.tdunitaddrcode else null end) result_shop_cnt"
                        + ",count(distinct case when tdunitzonecode = 1 then TB_DIST_MAPPING.tdunitaddrcode else null end) leftshopcnt"
                        + ",count(distinct case when tdunitzonecode = 2 then TB_DIST_MAPPING.tdunitaddrcode else null end) rightshopcnt"
                        + ",sum(case when tdunitzonecode = 1 then NU_OPS-NU_DRPS else 0 end) leftpscnt"
                        + ",sum(case when tdunitzonecode = 2 then NU_OPS-NU_DRPS else 0 end) rightpscnt"
                        + " from TB_DIST"
                        + " inner join TB_DIST_MAPPING on TB_DIST_MAPPING.ID_DIST=TB_DIST.ID_DIST"
                        + " inner join tdunitaddr on TB_DIST_MAPPING.tdunitaddrcode = tdunitaddr.tdunitaddrcode and tdunitaddr.usageid=@tdunittype"
                        + " where DT_DELIVERY=@dt_delivery and CD_DIST_GROUP=@cd_dist_group and CD_BLOCK=@cd_block and FG_MAPSTATUS=@fg_mapstatus"
                        + " group by CD_DIST_GROUP,DT_DELIVERY,TB_DIST.CD_HIMBAN,ST_BOXTYPE,NU_BOXUNIT,CD_GTIN13,CD_GTIN14,CD_SHUKKA_BATCH,CD_JUCHU_BIN"
                        + " order by (case when min(TB_DIST.FG_DSTATUS) >= @dstatus and max(TB_DIST.FG_DSTATUS) >= @dstatus then @dstatus_completed else @dstatus_ready end),CD_SHUKKA_BATCH,CD_JUCHU_BIN,TB_DIST.CD_HIMBAN";

                return con.Query(sql, new
                {
                    dt_delivery = distgroup.DtDelivery.ToString("yyyyMMdd"),
                    cd_dist_group = distgroup.CdDistGroup,
                    cd_block = distgroup.CdBlock,
                    dstatus = (int)Status.Inprog,
                    dstatus_ready = (int)Status.Ready,
                    dstatus_completed = (int)Status.Completed,
                    fg_mapstatus = (int)Status.Completed,
                    tdunittype = distgroup.TdUnitType,
                })
                     .Select(q => new DistInfo
                     {
                         DtDelivery = q.DT_DELIVERY,
                         CdShukkaBatch = q.CD_SHUKKA_BATCH,
                         CdJuchuBin = q.CD_JUCHU_BIN,
                         CdHimban = q.CD_HIMBAN,
                         NmHinSeishikimei = q.NM_HIN_SEISHIKIMEI,
                         CdGtin13 = q.CD_GTIN13,
                         CdGtin14 = q.CD_GTIN14,
                         StBoxType = q.ST_BOXTYPE,
                         NuBoxUnit = q.NU_BOXUNIT,
                         Csunit = q.NU_BOXUNIT,
                         DStatus = q.dstatus,
                         LStatus = q.lstatus,
                         Ops = q.ops ?? 0,
                         Dops = q.dops ?? 0,
                         Drps = q.drps ?? 0,
                         Ddps = q.ops - q.drps,
                         Order_shop_cnt = q.order_shop_cnt ?? 0,
                         Result_shop_cnt = q.result_shop_cnt ?? 0,
                         Remain_shop_cnt = q.order_shop_cnt - q.result_shop_cnt,
                         Left_shop_cnt = q.leftshopcnt ?? 0,
                         Right_shop_cnt = q.rightshopcnt ?? 0,
                         Left_ps_cnt = q.leftpscnt ?? 0,
                         Right_ps_cnt = q.rightpscnt ?? 0,
                     }).ToList();
            }
        }
        public static List<DistColor>? SetColors()
        {
            List<DistColor> colors = new List<DistColor>();

            string[] color_name = { "赤", "黄", "緑", "白", "青" };

            for (int i = 0; i < color_name.Length; i++)
            {
                colors.Add(
                    new DistColor
                    {
                        DistColor_code = i + 1,
                        DistColor_name = color_name[i],
                        DistColor_Func_name = color_name[i] + string.Format("(F{0})",i+1),
                    }
                );
            }

            return colors;
        }
        public static List<DistDetail>? LoadInfoDetails(DistGroup distgroup, DistBase _dist)
        {
            if (_dist.CdHimban == "")
                return new List<DistDetail>();

            using (var con = DbFactory.CreateConnection())
            {
                var sql = @"select tb_dist.ID_DIST"
                        + ",DT_DELIVERY"
                        + ",CD_DIST_GROUP"
                        + ",NM_DIST_GROUP"
                        + ",CD_COURSE"
                        + ",CD_ROUTE"
                        + ",CD_TOKUISAKI"
                        + ",NM_TOKUISAKI"
                        + ",CD_HIMBAN"
                        + ",CD_GTIN13"
                        + ",NM_HIN_SEISHIKIMEI"
                        + ",NU_BOXUNIT"
                        + ",ST_BOXTYPE"
                        + ",tb_dist_mapping.tdunitaddrcode"
                        + ",tdunitzonecode"
                        + ",FG_DSTATUS"
                        + ",NU_OPS"
                        + ",NU_DOPS"
                        + ",NU_DRPS"
                        + ",NU_LRPS"
                        + " from tb_dist"
                        + " inner join tb_dist_mapping on tb_dist.id_dist=tb_dist_mapping.id_dist"
                        + " inner join tdunitaddr on TB_DIST_MAPPING.tdunitaddrcode = tdunitaddr.tdunitaddrcode and tdunitaddr.usageid=@tdunittype"
                        + " where DT_DELIVERY=@dt_delivery and CD_DIST_GROUP=@cd_dist_group and CD_BLOCK=@cd_block and FG_MAPSTATUS=@fg_mapstatus"
                        + " and CD_HIMBAN=@cd_himban"
                        + " and CD_SHUKKA_BATCH=@cd_shukka_batch and CD_JUCHU_BIN=@cd_juchu_bin"
                        + " order by tb_dist_mapping.tdunitaddrcode";

                return con.Query(sql,
                    new {
                        dt_delivery = distgroup.DtDelivery.ToString("yyyyMMdd"),
                        cd_dist_group = distgroup.CdDistGroup,
                        cd_block = distgroup.CdBlock,
                        cd_himban = _dist.CdHimban,
                        cd_shukka_batch = _dist.CdShukkaBatch,
                        cd_juchu_bin = _dist.CdJuchuBin,
                        fg_mapstatus = (int)Status.Completed,
                        tdunittype = distgroup.TdUnitType,
                    })
                     .Select(q => new DistDetail
                     {
                         IdDist = q.ID_DIST,
                         DtDelivery = q.DT_DELIVERY,
                         CdDistGroup = q.CD_DIST_GROUP,
                         NmDistGroup = q.NM_DIST_GROUP,
                         CdCourse = q.CD_COURSE,
                         CdRoute = q.CD_ROUTE.ToString(),
                         CdTokuisaki = q.CD_TOKUISAKI,
                         NmTokuisaki = q.NM_TOKUISAKI,
                         CdHimban = q.CD_HIMBAN,
                         NmHinSeishikimei = q.NM_HIN_SEISHIKIMEI,
                         CdGtin13 = q.CD_GTIN13,
                         NuBoxUnit = q.NU_BOXUNIT,
                         Csunit = q.NU_BOXUNIT,
                         StBoxType = q.ST_BOXTYPE,
                         Tdunitaddrcode = q.tdunitaddrcode,
                         Tdunitzonecode = q.tdunitzonecode,
                         DStatus = q.FG_DSTATUS,
                         Ops = q.NU_OPS,
                         Dops = q.NU_DOPS,
                         Drps = q.NU_DRPS,
                         Ddps = q.NU_OPS - q.NU_DRPS,
                         Lrps = q.NU_LRPS,
                     }).ToList();
            }
        }

        public static bool UpdateQtyDetail(List<DistDetail> detail)
        {
            bool bTranBegin = false;
            using (var con = DbFactory.CreateConnection())
            {
                var tr = con.BeginTransaction();
                bTranBegin = true;

                try
                {
                    detail.ForEach(x =>
                    {
                        var sql = "update tb_dist set NU_DOPS=@dops,updatedAt=@updt where ID_DIST=@iddist";
                        con.Execute(sql,
                            new
                            {
                                dops = x.Dops + x.Drps,
                                iddist = x.IdDist,
                                updt = DateTime.Now,
                            }, tr);
                    });

                    tr.Commit();
                    bTranBegin = false;
                }
                catch (Exception)
                {
                    if (bTranBegin)
                        tr.Rollback();
                    throw;
                }
            }

            return true;
        }

        public static string GetItemSqls(string having, bool blarge)
        {
            string str="";
            if (having != "")
                str = " having " + having;

            string strlarge = "";
            if (blarge == true)
                strlarge = " and FG_LSTATUS<>@fg_lstatus";

            return @"select CD_DIST_GROUP,DT_DELIVERY,TB_DIST.CD_HIMBAN,ST_BOXTYPE,NU_BOXUNIT,max(NM_HIN_SEISHIKIMEI) NM_HIN_SEISHIKIMEI, CD_GTIN13,CD_GTIN14,CD_SHUKKA_BATCH,CD_JUCHU_BIN"
                + ",sum(NU_OPS) ops,sum(NU_DOPS) dops,sum(NU_DRPS) drps,sum(NU_LRPS) lrps"
                + ",(case when min(TB_DIST.FG_DSTATUS) = max(TB_DIST.FG_DSTATUS) then max(TB_DIST.FG_DSTATUS) else @dstatus end) dstatus"
                + ",(case when min(TB_DIST.FG_LSTATUS) = max(TB_DIST.FG_LSTATUS) then max(TB_DIST.FG_LSTATUS) else @dstatus end) lstatus"
                + ",count(distinct TB_DIST_MAPPING.tdunitaddrcode) order_shop_cnt"
                + ",count(distinct case when NU_DOPS=NU_DRPS then TB_DIST_MAPPING.tdunitaddrcode else null end) result_shop_cnt"
                + ",count(distinct case when tdunitzonecode = 1 then TB_DIST_MAPPING.tdunitaddrcode else null end) leftshopcnt"
                + ",count(distinct case when tdunitzonecode = 2 then TB_DIST_MAPPING.tdunitaddrcode else null end) rightshopcnt"
                + ",sum(case when tdunitzonecode = 1 then NU_OPS-NU_DRPS else 0 end) leftpscnt"
                + ",sum(case when tdunitzonecode = 2 then NU_OPS-NU_DRPS else 0 end) rightpscnt"
                + " from TB_DIST"
                + " left join TB_DIST_MAPPING on TB_DIST_MAPPING.ID_DIST=TB_DIST.ID_DIST"
                + " inner join tdunitaddr on TB_DIST_MAPPING.tdunitaddrcode = tdunitaddr.tdunitaddrcode and tdunitaddr.usageid=@tdunittype"
                + " where DT_DELIVERY=@dt_delivery and CD_DIST_GROUP=@cd_dist_group and CD_BLOCK=@cd_block  and FG_MAPSTATUS=@fg_mapstatus"
                + strlarge
                + " and (TB_DIST.CD_HIMBAN=@scancode or TB_DIST.CD_GTIN13=@scancode or CD_GTIN14=@scancode)"
                + " group by CD_DIST_GROUP,DT_DELIVERY,TB_DIST.CD_HIMBAN,ST_BOXTYPE,NU_BOXUNIT,CD_GTIN13,CD_GTIN14,CD_SHUKKA_BATCH,CD_JUCHU_BIN"
                + str
                + " order by TB_DIST.CD_HIMBAN";
        }
        public static List<DistItemSeq>? GetItems(DistGroup distgroup, string scancode, bool bCheck, bool bExtraction)
        {
            //　スキャンコード読み込み
            using (var con = DbFactory.CreateConnection())
            {
                string having = (bCheck == true || bExtraction == true) ? "0<sum(NU_DRPS)" : "sum(NU_DOPS)<>sum(NU_DRPS)";

                var sql = GetItemSqls(having,true);

                var r =  con.Query(sql, new
                {
                    dt_delivery = distgroup.DtDelivery.ToString("yyyyMMdd"),
                    cd_dist_group = distgroup.CdDistGroup,
                    cd_block = distgroup.CdBlock,
                    dstatus = (int)Status.Inprog,
                    dstatus_ready = (int)Status.Ready,
                    dstatus_completed = (int)Status.Completed,
                    fg_mapstatus = (int)Status.Completed,
                    tdunittype = distgroup.TdUnitType,
                    fg_lstatus = (int)Status.Ready,
                    scancode = scancode,
                })
                     .Select(q => new DistItemSeq
                     {
                         DtDelivery = q.DT_DELIVERY,
                         CdShukkaBatch = q.CD_SHUKKA_BATCH,
                         CdJuchuBin = q.CD_JUCHU_BIN,
                         CdHimban = q.CD_HIMBAN,
                         NmHinSeishikimei = q.NM_HIN_SEISHIKIMEI,
                         CdGtin13 = q.CD_GTIN13,
                         CdGtin14 = q.CD_GTIN14,
                         StBoxType = q.ST_BOXTYPE,
                         NuBoxUnit = q.NU_BOXUNIT,
                         Csunit = q.NU_BOXUNIT,
                         DStatus = q.dstatus,
                         LStatus = q.lstatus,
                         Ops = q.ops ?? 0,
                         Dops = q.dops ?? 0,
                         Drps = q.drps ?? 0,
                         Ddps = q.dops - q.drps,
                         Lrps = q.lrps ?? 0,
                         Order_shop_cnt = q.order_shop_cnt ?? 0,
                         Result_shop_cnt = q.result_shop_cnt ?? 0,
                         Remain_shop_cnt = q.order_shop_cnt - q.result_shop_cnt,
                         Left_shop_cnt = q.leftshopcnt ?? 0,
                         Right_shop_cnt = q.rightshopcnt ?? 0,
                         Left_ps_cnt = q.leftpscnt ?? 0,
                         Right_ps_cnt = q.rightpscnt ?? 0,
                     }).ToList();

                if (r.Count==0)
                {
                    sql = GetItemSqls("",false);
                    var rr = con.Query(sql, new
                    {
                        dt_delivery = distgroup.DtDelivery.ToString("yyyyMMdd"),
                        cd_dist_group = distgroup.CdDistGroup,
                        cd_block = distgroup.CdBlock,
                        dstatus = (int)Status.Inprog,
                        dstatus_ready = (int)Status.Ready,
                        dstatus_completed = (int)Status.Completed,
                        fg_mapstatus = (int)Status.Completed,
                        tdunittype = distgroup.TdUnitType,
                        scancode = scancode,
                    })
                         .Select(q => new DistItemSeq
                         {
                             DtDelivery = q.DT_DELIVERY,
                             CdShukkaBatch = q.CD_SHUKKA_BATCH,
                             CdJuchuBin = q.CD_JUCHU_BIN,
                             CdHimban = q.CD_HIMBAN,
                             NmHinSeishikimei = q.NM_HIN_SEISHIKIMEI,
                             CdGtin13 = q.CD_GTIN13,
                             CdGtin14 = q.CD_GTIN14,
                             StBoxType = q.ST_BOXTYPE,
                             NuBoxUnit = q.NU_BOXUNIT,
                             Csunit = q.NU_BOXUNIT,
                             DStatus = q.dstatus,
                             LStatus = q.lstatus,
                             Ops = q.ops ?? 0,
                             Dops = q.dops ?? 0,
                             Drps = q.drps ?? 0,
                             Ddps = q.dops - q.drps,
                             Lrps = q.lrps ?? 0,
                             Order_shop_cnt = q.order_shop_cnt ?? 0,
                             Result_shop_cnt = q.result_shop_cnt ?? 0,
                             Remain_shop_cnt = q.order_shop_cnt - q.result_shop_cnt,
                             Left_shop_cnt = q.leftshopcnt ?? 0,
                             Right_shop_cnt = q.rightshopcnt ?? 0,
                         }).ToList();

                    if (rr.Count == 0)
                    {
                        throw new Exception($"該当商品が見つかりませんでした。\nスキャンコード:{scancode}");
                    }else
                    {
                        if (rr[0].LStatus == (int)Status.Ready)
                        {
                            throw new Exception($"大仕分けされていない商品です。\n品番:{rr[0].CdHimban}\n品名:{rr[0].NmHinSeishikimei}");
                        }
                        else
                        {
                            if (bCheck || bExtraction)
                            {
                                if (bCheck)
                                    throw new Exception($"まだ仕分けしていないので検品出来ません。\n品番:{rr[0].CdHimban}\n品名:{rr[0].NmHinSeishikimei}");
                                else
                                    throw new Exception($"まだ仕分けしていないので抜き取り出来ません。\n品番:{rr[0].CdHimban}\n品名:{rr[0].NmHinSeishikimei}");
                            }
                            else
                            {
                                throw new Exception($"既に仕分け済みです。\n品番:{rr[0].CdHimban}\n品名:{rr[0].NmHinSeishikimei}");
                            }
                        }
                    }
                }

                return r;
            }
        }
        public static DistItemSeq? RefreshItems(DistGroup distgroup, DistBase item, bool bCheck, bool bExtraction)
        {
            //　スキャンコード読み込み
            using (var con = DbFactory.CreateConnection())
            {
                string having = (bCheck == true || bExtraction == true) ? "0<sum(NU_DRPS)" : "sum(NU_DOPS)<>sum(NU_DRPS)";
                having += $" and CD_SHUKKA_BATCH='{item.CdShukkaBatch}' and CD_JUCHU_BIN='{item.CdJuchuBin}'";

                var sql = GetItemSqls(having, true);

                return con.Query(sql, new
                {
                    dt_delivery = distgroup.DtDelivery.ToString("yyyyMMdd"),
                    cd_dist_group = distgroup.CdDistGroup,
                    cd_block = distgroup.CdBlock,
                    dstatus = (int)Status.Inprog,
                    dstatus_ready = (int)Status.Ready,
                    dstatus_completed = (int)Status.Completed,
                    fg_mapstatus = (int)Status.Completed,
                    tdunittype = distgroup.TdUnitType,
                    fg_lstatus = (int)Status.Ready,
                    scancode = item.CdHimban,
                })
                     .Select(q => new DistItemSeq
                     {
                         DtDelivery = q.DT_DELIVERY,
                         CdShukkaBatch = q.CD_SHUKKA_BATCH,
                         CdJuchuBin = q.CD_JUCHU_BIN,
                         CdHimban = q.CD_HIMBAN,
                         NmHinSeishikimei = q.NM_HIN_SEISHIKIMEI,
                         CdGtin13 = q.CD_GTIN13,
                         CdGtin14 = q.CD_GTIN14,
                         StBoxType = q.ST_BOXTYPE,
                         NuBoxUnit = q.NU_BOXUNIT,
                         Csunit = q.NU_BOXUNIT,
                         DStatus = q.dstatus,
                         LStatus = q.lstatus,
                         Ops = q.ops ?? 0,
                         Dops = q.dops ?? 0,
                         Drps = q.drps ?? 0,
                         Ddps = q.dops - q.drps,
                         Lrps = q.lrps ?? 0,
                         Order_shop_cnt = q.order_shop_cnt ?? 0,
                         Result_shop_cnt = q.result_shop_cnt ?? 0,
                         Remain_shop_cnt = q.order_shop_cnt - q.result_shop_cnt,
                         Left_shop_cnt = q.leftshopcnt ?? 0,
                         Right_shop_cnt = q.rightshopcnt ?? 0,
                         Left_ps_cnt = q.leftpscnt ?? 0,
                         Right_ps_cnt = q.rightpscnt ?? 0,
                     }).FirstOrDefault();
            }
        }

        public static bool DistUpdate(DistColor distcolor)
        {
            using (var con = DbFactory.CreateConnection())
            {
                DateTime endtm = DateTime.Now;

                var tr = con.BeginTransaction();
                try
                {
                    using (var locker = new AppLock(con, tr, "distlock", LockMode.Exclusive, LockOwner.Session))
                    {
                        foreach (var itemseq in distcolor.ItemSeqs)
                        {
                            if (itemseq.Details == null || itemseq.Details.Count == 0)
                            {
                                continue;
                            }

                            // 配分のみ更新
                            if (distcolor.DistWorkMode == (int)Defs.DistWorkMode.Dist)
                            {
                                foreach (var d in itemseq.Details)
                                {
                                    int status = d.Ops == d.Drps ? (short)DbLib.Defs.Status.Completed : (short)DbLib.Defs.Status.Inprog;

                                    var sql = "update TB_DIST set NU_DOPS=NU_LRPS,NU_DRPS=@drps,FG_DSTATUS=@status,CD_SHAIN_DIST=@cdshain,NM_SHAIN_DIST=@nmshain,DT_WORKDT_DIST=@dtwork,updatedAt=@updt where ID_DIST=@iddist";

                                    con.Execute(sql,
                                    new
                                    {
                                        status = status,
                                        drps = d.Drps,
                                        cdshain = distcolor.CdShain,
                                        nmshain = distcolor.NmShain,
                                        iddist = d.IdDist,
                                        dtwork = DateTime.Now,
                                        updt = DateTime.Now,
                                    }, tr);
                                }
                            }

                            // 未処理へ戻す
                            if (distcolor.DistWorkMode == (int)Defs.DistWorkMode.Extraction)
                            {
                                foreach (var d in itemseq.Details)
                                {
                                    // 押下したもののみ更新
                                    if (d.TdUnitPushTm != null)
                                    {
                                        var sql = "update TB_DIST set NU_DRPS=@drps,FG_DSTATUS=@status,CD_SHAIN_DIST=NULL,NM_SHAIN_DIST=NULL,DT_WORKDT_DIST=NULL,updatedAt=@updt where ID_DIST=@iddist";

                                        con.Execute(sql,
                                        new
                                        {
                                            status = (short)DbLib.Defs.Status.Ready,
                                            drps = 0,
                                            iddist = d.IdDist,
                                            updt = DateTime.Now,
                                        }, tr);
                                    }
                                }
                            }

                            itemseq.Clear();
                            itemseq.Details = null;
                        }

                        tr.Commit();
                    }

                    distcolor.DistColorClear();
                }
                catch (Exception)
                {
                    tr.Rollback();
                    throw;
                }
                return true;
            }
        }
        private static TBDISTEntity? GetDist(IDbConnection con, IDbTransaction tr, long iddist)
        {
            return con.Find<TBDISTEntity>(s => s
                    .Where($"{nameof(TBDISTEntity.IDDIST)}=@iddist")
                    .WithParameters(new { iddist })
                    .AttachToTransaction(tr))
                .FirstOrDefault();
        }
        public static void WorkReportAppend(DistGroupEx distgroup, List<ReportShain> reportshains)
        {
            using (var con = DbFactory.CreateConnection())
            {
                DateTime endtm = DateTime.Now;

                var tr = con.BeginTransaction();
                try
                {
                    DateTime startdt = reportshains.Select(x => x.DtWorkStart).Min().GetValueOrDefault();
                    DateTime enddt = reportshains.Select(x => x.DtWorkEnd).Max().GetValueOrDefault();

                    foreach (var report in reportshains)
                    {
                        TBREPORTEntity r = new TBREPORTEntity()
                        {
                            DTDELIVERY = distgroup.DtDelivery.ToString("yyyyMMdd"),
                            CDDISTGROUP = distgroup.CdDistGroup,
                            NMDISTGROUP = distgroup.NmDistGroup,
                            CDBLOCK = distgroup.CdBlock,
                            DTSTART = startdt,
                            DTEND = enddt,
                            NMIDLE = 0,
                            CDSYAIN = report.CdShain,
                            NMSYAIN = report.NmShain,
                            DTWORKSTART = (DateTime)report.DtWorkStart!,
                            DTWORKEND = (DateTime)report.DtWorkEnd!,
                            NMWORKTIME = (int)report.WorkTime,
                            NMITEMCNT = report.ItemCnt,
                            NMSHOPCNT = report.ShopCnt,
                            NMDISTCNT = report.DistCnt,
                            NMCHECKCNT = report.CheckCnt,
                            NMCHECKTIME = (int)report.CheckTime,
                            CreatedAt = DateTime.Now,
                        };

                        con.Insert(r, x => x.AttachToTransaction(tr));
                    }

                    tr.Commit();
                }
                catch (Exception)
                {
                    tr.Rollback();
                    throw;
                }
            }
        }
    }
}