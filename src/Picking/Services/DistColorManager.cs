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
                        + ",(case when min(TB_DIST.FG_DSTATUS) >= @dstatus and max(TB_DIST.FG_DSTATUS) >= @dstatus then @dstatus_completed else @dstatus_ready end) dstatus"
                        + ",count(distinct TB_DIST.CD_TOKUISAKI) order_shop_cnt"
                        + ",count(distinct case when TB_DIST.FG_DSTATUS >= @dstatus then TB_DIST.CD_TOKUISAKI else null end) result_shop_cnt"
                        + ",count(distinct case when tdunitzonecode = 1 then CD_TOKUISAKI else null end) leftshopcnt"
                        + ",count(distinct case when tdunitzonecode = 2 then CD_TOKUISAKI else null end) rightshopcnt"
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
                         Ops = q.ops ?? 0,
                         Dops = q.dops ?? 0,
                         Drps = q.drps ?? 0,
                         Ddps = q.dops ?? 0 - q.drps ?? 0,
                         Order_shop_cnt = q.order_shop_cnt ?? 0,
                         Result_shop_cnt = q.result_shop_cnt ?? 0,
                         Remain_shop_cnt = q.order_shop_cnt ?? 0 - q.result_shop_cnt ?? 0,
                         Left_shop_cnt = q.leftshopcnt ?? 0,
                         Right_shop_cnt = q.rightshopcnt ?? 0,
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
                                dops = x.Dops,
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
                + ",sum(NU_OPS) ops,sum(NU_DOPS) dops,sum(NU_DRPS) drps"
                + ",(case when min(TB_DIST.FG_DSTATUS) >= @dstatus and max(TB_DIST.FG_DSTATUS) >= @dstatus then @dstatus_completed else @dstatus_ready end) dstatus"
                + ",(case when min(TB_DIST.FG_LSTATUS) >= @dstatus and max(TB_DIST.FG_LSTATUS) >= @dstatus then @dstatus_completed else @dstatus_ready end) lstatus"
                + ",count(distinct TB_DIST.CD_TOKUISAKI) order_shop_cnt"
                + ",count(distinct case when TB_DIST.FG_DSTATUS >= @dstatus then TB_DIST.CD_TOKUISAKI else null end) result_shop_cnt"
                + ",count(distinct case when tdunitzonecode = 1 then CD_TOKUISAKI else null end) leftshopcnt"
                + ",count(distinct case when tdunitzonecode = 2 then CD_TOKUISAKI else null end) rightshopcnt"
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
        public static List<DistItemSeq>? GetItems(DistGroup distgroup, string scancode, bool bCheck)
        {
            //　スキャンコード読み込み
            using (var con = DbFactory.CreateConnection())
            {
                string having = bCheck == true ? "0<sum(NU_DRPS)" : "sum(NU_OPS)<>sum(NU_DRPS)";

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
                         Ddps = q.dops ?? 0 - q.drps ?? 0,
                         Order_shop_cnt = q.order_shop_cnt ?? 0,
                         Result_shop_cnt = q.result_shop_cnt ?? 0,
                         Remain_shop_cnt = q.order_shop_cnt ?? 0 - q.result_shop_cnt ?? 0,
                         Left_shop_cnt = q.leftshopcnt ?? 0,
                         Right_shop_cnt = q.rightshopcnt ?? 0,
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
                             Ddps = q.dops ?? 0 - q.drps ?? 0,
                             Order_shop_cnt = q.order_shop_cnt ?? 0,
                             Result_shop_cnt = q.result_shop_cnt ?? 0,
                             Remain_shop_cnt = q.order_shop_cnt ?? 0 - q.result_shop_cnt ?? 0,
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
                            if (bCheck)
                            {
                                throw new Exception($"まだ仕分けしていないので検品出来ません。\n品番:{rr[0].CdHimban}\n品名:{rr[0].NmHinSeishikimei}");
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
#if false
                                    var dist = GetDist(con, tr, d.IdDist);
                                    if (dist != null)
                                    {
                                        int order = d.Ops;
                                        if (d.DStatus == (int)DbLib.Defs.Status.Completed)
                                        {
                                            dist.NUDRPS = d.Drps;
                                            dist.FGDSTATUS = d.Ops == d.Drps ? (short)DbLib.Defs.Status.Completed : (short)DbLib.Defs.Status.Inprog;
                                        }
                                        else
                                        {
                                            dist.FGDSTATUS = (short)DbLib.Defs.Status.Inprog;
                                        }
                                        dist.UpdatedAt = DateTime.Now;

                                        var sql = "update dist set NU_DRPS=@drps,FGDSTATUS=@status,updatedAt=@updt where ID_DIST=@iddist";

                                        con.Execute(sql,
                                        new
                                        {
                                            status = dist.FGDSTATUS,
                                            drps = dist.NUDRPS,
                                            iddist = dist.IDDIST,
                                            updt = DateTime.Now,
                                        }, tr);

                                        // 大仕分けで更新する可能性があるので全項目更新は止める
                                        //con.Update(dist, s => s.AttachToTransaction(tr));
                                    }
#else

                                    int status = d.Ops == d.Drps ? (short)DbLib.Defs.Status.Completed : (short)DbLib.Defs.Status.Inprog;

                                    var sql = "update TB_DIST set NU_DRPS=@drps,FG_DSTATUS=@status,CD_SHAIN_DIST=@cdshain,NM_SHAIN_DIST=@nmshain,DT_WORKDT_DIST=@dtwork,updatedAt=@updt where ID_DIST=@iddist";

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
#endif
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

#if false
        // ゾーン一覧読み込み
        public static List<DistColor>? LoadZones()
        {
            using (var con = DbFactory.CreateConnection())
            {
                var tablets = con.Find<TabletEntity>(s => s
                                   .OrderBy($"{nameof(TabletEntity.Zone)}"))
                                   .Select((q, index) => new TabletEntity
                                   {
                                       Tabletid = q.Tabletid,
                                       Zone = q.Zone,
                                       Saddr1 = q.Saddr1,
                                       Eaddr1 = q.Eaddr1,
                                       Saddr2 = q.Saddr2,
                                       Eaddr2 = q.Eaddr2,
                                       Saddr3 = q.Saddr3,
                                       Eaddr3 = q.Eaddr3,
                                   }).ToList();

                if (tablets == null || tablets.Count == 0)
                    return null;

                List<DistColor> distzones = new List<DistColor>();

                int zone = 0;
                int zoneidx = 0;
                DistColor? distzone = null;
                foreach (var tablet in tablets)
                {
                    if (zone != tablet.Zone)
                    {
                        zone = (int)tablet.Zone!;

                        // ゾーン設定
                        distzone = null;
                        distzone = new DistColor();
                        distzone.Zoneno = (int)tablet.Zone!;
                        distzone.Zoneidx = ++zoneidx;
                        distzone.Tdledcolor = (int)zoneidx % 2 == 0 ? TdLedColor.Green : TdLedColor.White;
                        distzone.Zonestatus = ZoneStatus.Wait;
                        distzone.Ops = 0;
                        distzone.Dops = 0;
                        distzone.Drps = 0;
                        distzone.Order_shop_cnt = 0;
                        distzone.Result_shop_cnt = 0;
                        distzone.Remain_shop_cnt = 0;
                        distzones.Add(distzone);
                    }

                    // ゾーンアドレス範囲設定
                    tablet.Saddr1?.Trim();
                    if (tablet.Saddr1 != null && tablet.Saddr1 != "")
                    {
                        DistZoneAddr zoneaddr = new DistZoneAddr()
                        {
                            Tabletid = tablet.Tabletid,
                            Saddr = tablet.Saddr1!,
                            Eaddr = tablet.Eaddr1!,
                        };
                        distzone?.Zoneaddrs.Add(zoneaddr);
                    }
                    if (tablet.Saddr2 != null && tablet.Saddr2 != "")
                    {
                        DistZoneAddr zoneaddr = new DistZoneAddr()
                        {
                            Tabletid = tablet.Tabletid,
                            Saddr = tablet.Saddr2!,
                            Eaddr = tablet.Eaddr2!,
                        };
                        distzone?.Zoneaddrs.Add(zoneaddr);
                    }
                    if (tablet.Saddr3 != null && tablet.Saddr3 != "")
                    {
                        DistZoneAddr zoneaddr = new DistZoneAddr()
                        {
                            Tabletid = tablet.Tabletid,
                            Saddr = tablet.Saddr3!,
                            Eaddr = tablet.Eaddr3!,
                        };
                        distzone?.Zoneaddrs.Add(zoneaddr);
                    }
                }
                return distzones;
            }
        }

        // 実績進捗読み込み
        public static ProgressCnt? GetResultProgressCnts()
        {
            using (var con = DbFactory.CreateConnection())
            {
                var sql = @"select min(shipdt.shipdt) shipdt ,count(distinct packno) packcntmax"
                        + ",count(distinct case when dsenddt is not null then packno else null end) packcntvalue from shipdt"
                        + " left join dist on dist.shipdt=shipdt.shipdt"
                        + " where shipdt.useshipdt=@useshipdt";

                return con.Query(sql, new
                {
                    useshipdt = 1,
                })
                     .Select(q => new ProgressCnt
                     {
                         Shipdt = DateTime.ParseExact(q.shipdt, "yyyyMMdd", null),
                         CntValue = q.packcntvalue,
                         CntMax = q.packcntmax,
                     }).FirstOrDefault();
            }
        }

        // 状況一覧読み込み
        public static List<DistInfo>? LoadInfs(DispType disptype)
        {
            using (var con = DbFactory.CreateConnection())
            {
                var sql = @"select dist.shipdt,dist.workkey,dist.shop,max(dist.shopnm) shopnm,sstore,dist.packno,sum(dist.ops) ops,sum(dist.dops) dops,sum(dist.drps) drps"
                        + ",(case when min(dist.dstatus) >= @dstatus and max(dist.dstatus) >= @dstatus and (zonestatus is null or zonestatus = @zonestatus) then @dstatus_completed else @dstatus_ready end) dstatus"
                        + ",(case when max(dist.dsenddt) is null then 0 else 1 end) sendstatus"
                        + ",count(distinct item) order_item_cnt"
                        + ",count(distinct case when dist.dstatus >= @dstatus then item else null end) result_item_cnt"
                        + ", inseq,workingzone,max(dist.boxappend) boxappend"
                        + " from shipdt"
                        + " inner join dist on dist.shipdt=shipdt.shipdt"
                        + " left join distprog on dist.packno=distprog.packno"
                        + " where shipdt.useshipdt=@useshipdt"
                        + " group by dist.shipdt,dist.workkey,dist.shop,sstore,dist.packno,inseq,workingzone,zonestatus";

                if (disptype == DispType.Untreated)
                {
                    sql += " having (case when min(dist.dstatus) = max(dist.dstatus) then min(dist.dstatus) else 1 end)=0";
                }
                sql += " order by (case when min(dist.dstatus) >= @dstatus and max(dist.dstatus) >= @dstatus and (zonestatus is null or zonestatus = @zonestatus) then @dstatus_completed else @dstatus_ready end),workkey";

                return con.Query(sql, new { useshipdt = 1, 
                                            zonestatus = (int)ZoneStatus.Completed, 
                                            dstatus=(int)Status.Inprog,
                                            dstatus_ready = (int)Status.Ready,
                                            dstatus_completed = (int)Status.Completed,
                })
                     .Select(q => new DistInfo
                     {
                         InSeq = q.inseq,
                         WorkingZone = q.workingzone==0 ? null: q.workingzone,
                         DT_DELIVERY = q.shipdt,
                         Workkey = q.workkey,
                         DStatus = q.dstatus,
                         SendStatus = q.sendstatus,
                         CD_TOKUISAKI = q.shop,
                         NM_TOKUISAKI = q.shopnm,
                         Sstore = q.sstore,
                         Packno = q.packno,
                         Ops = q.ops,
                         Dops = q.dops,
                         Drps = q.drps,
                         Ddps = q.dops - q.drps,
                         Order_item_cnt = q.order_item_cnt,
                         Result_item_cnt = q.result_item_cnt,
                         Remain_item_cnt = q.order_item_cnt - q.result_item_cnt,
                         Boxappend = q.boxappend,
                     }).ToList();
            }
        }
        public static List<DistDetail>? LoadInfoDetails(DistInfo _distinfo)
        {
            using (var con = DbFactory.CreateConnection())
            {
                var sql = @"select dist.* from shipdt"
                        + " inner join dist on dist.shipdt=shipdt.shipdt"
                        + " where shipdt.useshipdt=@useshipdt and dist.packno=@packno"
                        + " order by tdunitaddrcode";

                return con.Query(sql, new { useshipdt = 1, packno = _distinfo.Packno })
                     .Select(q => new DistDetail
                     {
                         Distid = q.distid,
                         DT_DELIVERY = q.shipdt,
                         Chain = q.chain,
                         Chainnm = q.chainnm,
                         CD_TOKUISAKI = q.shop,
                         NM_TOKUISAKI = q.shopnm,
                         Packno = q.packno,
                         Rack = q.rack,
                         Item = q.item,
                         Itemnm = q.itemnm,
                         Delivdt = q.delivdt,
                         Workno = q.workno,
                         Pickseq = q.pickseq,
                         Course = q.course,
                         Route = q.route,
                         Slipno = q.slipno,
                         Slipnocd = q.slipnocd,
                         Sliprow = q.slipnocw,
                         Losstype = q.lanstype,
                         Boxappend = q.boxappend,
                         Workkey = q.workkey,
                         Csunit = q.csunit,
                         Csunittype = q.csunittype,
                         Tdunitaddrcode = q.tdunitaddrcode,
                         DStatus = q.dstatus,
                         Ops = q.ops,
                         Dops = q.dops,
                         Drps = q.drps,
                         Ddps = q.ops - q.drps,
                     }).ToList();
            }
        }
        public static List<DistDetail>? LoadDetails(ref DistColor _distzone)
        {
            using (var con = DbFactory.CreateConnection())
            {
                int zone = _distzone.Zoneno;
                // 担当者取得
                var distprogzone = con.Find<DistprogzoneEntity>(s => s
                        .Where($"{nameof(DistprogzoneEntity.Zone)}=@zoneno")
                        .WithParameters(new { zoneno = zone }))
                    .FirstOrDefault();

                var person = con.Find<PersonEntity>(s => s
                        .Where($"{nameof(PersonEntity.Personid)}=@personid")
                        .WithParameters(new { personid = distprogzone?.Personid }))
                    .FirstOrDefault();

                _distzone.CD_SHAIN = person?.Personid ?? "";
                _distzone.NM_SHAIN = person?.Personnm ?? "";

                StringBuilder sb = new StringBuilder();
                foreach (var zoneaddr in _distzone.Zoneaddrs)
                {
                    if (sb.Length==0)
                    {
                        sb.Append('(');
                    }
                    else
                    {
                        sb.Append(" or ");
                    }
                    sb.AppendFormat($"tdunitaddrcode between '{zoneaddr.Saddr}' and '{zoneaddr.Eaddr}'");
                }
                if (0 < sb.Length)
                {
                    sb.Append(')');
                }

                var sql = @"select dist.* from shipdt"
                        + " inner join dist on dist.shipdt=shipdt.shipdt"
                        + " where shipdt.useshipdt=@useshipdt and dist.packno=@packno"
                        + " and " + sb.ToString()
                        + " order by tdunitaddrcode";

                return con.Query(sql, new
                {
                    useshipdt = 1,
                    packno = _distzone.Packno,
                })
                     .Select(q => new DistDetail
                     {
                         Distid = q.distid,
                         DT_DELIVERY = q.shipdt,
                         Chain = q.chain,
                         Chainnm = q.chainnm,
                         CD_TOKUISAKI = q.shop,
                         NM_TOKUISAKI = q.shopnm,
                         Packno = q.packno,
                         Rack = q.rack,
                         Item = q.item,
                         Itemnm = q.itemnm,
                         Delivdt = q.delivdt,
                         Workno = q.workno,
                         Pickseq = q.pickseq,
                         Course = q.course,
                         Route = q.route,
                         Slipno = q.slipno,
                         Slipnocd = q.slipnocd,
                         Sliprow = q.slipnocw,
                         Losstype = q.lanstype,
                         Boxappend = q.boxappend,
                         Workkey = q.workkey,
                         Csunit = q.csunit,
                         Csunittype = q.csunittype,
                         Tdunitaddrcode = q.tdunitaddrcode,
                         DStatus = q.dstatus,
                         Ops = q.ops,
                         Dops = q.dops,
                         Drps = q.drps,
                         Ddps = q.dops - q.drps,
                     }).ToList();
            }
        }
        public static bool UpdateDetails(DistColor _distzone)
        {
            bool bTranBegin = false;
            using (var con = DbFactory.CreateConnection())
            {
                var distprogzone = con.Find<DistprogzoneEntity>(s => s
                        .Where($"{nameof(DistprogzoneEntity.Zone)}=@zoneno")
                        .WithParameters(new { zoneno = _distzone.Zoneno }))
                    .FirstOrDefault();

                var tr = con.BeginTransaction();
                bTranBegin = true;

                string personid = distprogzone?.Personid??"";

                try
                {
                    if (_distzone.Datas != null)
                    {
                        foreach (var data in _distzone.Datas)
                        {
                            var sql = "update dist set drps=@drps,dstatus=@dstatus,dworkdt=@dworkdt,dpersonid=@dpersonid where distid=@distid";
                            con.Execute(sql, 
                                new { 
                                    drps = data.Drps, 
                                    dstatus = data.DStatus, 
                                    dworkdt = DateTime.Now, 
                                    dpersonid = personid, 
                                    distid = data.Distid }, tr);
                        }
                    }

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
        public static bool UpdateDetail(DistColor _distzone, DistDetail detail)
        {
            bool bTranBegin = false;
            using (var con = DbFactory.CreateConnection())
            {
                var distprogzone = con.Find<DistprogzoneEntity>(s => s
                        .Where($"{nameof(DistprogzoneEntity.Zone)}=@zoneno")
                        .WithParameters(new { zoneno = _distzone.Zoneno }))
                    .FirstOrDefault();

                var tr = con.BeginTransaction();
                bTranBegin = true;

                string personid = distprogzone?.Personid ?? "";

                try
                {
                    var sql = "update dist set drps=@drps,dstatus=@dstatus,dworkdt=@dworkdt,dpersonid=@dpersonid where distid=@distid";
                    con.Execute(sql,
                        new
                        {
                            drps = detail.Drps,
                            dstatus = detail.DStatus,
                            dworkdt = DateTime.Now,
                            dpersonid = personid,
                            distid = detail.Distid
                        }, tr);

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
        public static bool IsSendWorkKey(string workkey)
        {
            Syslog.Info($"IsSendWorkKey::workkey:[{workkey}]");

            string workkey_where = "";
            if (workkey!="")
            {
                workkey_where = "and dist.workkey = @workkey";
            }

            using (var con = DbFactory.CreateConnection())
            {
                var sql = "select dist.workkey from shipdt"
                    + " inner join dist on dist.shipdt = shipdt.shipdt"
                    + " left join distprog on dist.packno = distprog.packno"
                    + " where shipdt.useshipdt=@useshipdt and dsenddt is null " + workkey_where
                    + " group by dist.workkey"
                    + " having min(dstatus)>=@dstatus"
                    + " and (min(zonestatus)=@zonestatus or min(zonestatus) is null)";

                var r = con.Query(sql, new
                {
                    useshipdt = 1,
                    dstatus = (int)Status.Inprog,
                    workkey = workkey,
                    zonestatus = (int)ZoneStatus.Completed,
                })
                     .Select(q => new DistBase
                     {
                         Workkey = q.Workkey,
                     }).FirstOrDefault();

                return r != null;
            }
        }
        public static bool IsSpecialprocessing(int zone)
        {
            Syslog.Info($"IsSpecialprocessing::zone:[{zone}]");

            using (var con = DbFactory.CreateConnection())
            {
                var distprogzone = con.Find<DistprogzoneEntity>(s => s
                    .Where($"{nameof(DistprogzoneEntity.Zone)}=@zone")
                    .WithParameters(new { zone }))
                .FirstOrDefault();

                // レコードなしはOKとする
                if (distprogzone == null)
                {
                    return false;
                }
                else
                {
                    return distprogzone.Specialprocessing == 1 ? true : false;
                }
            }
        }
        public static void DeleteLossAddr(string Tdunitaddrcode = "")
        {
            Syslog.Info($"DeleteLossAddr::addr:[{Tdunitaddrcode}]");

            using (var con = DbFactory.CreateConnection())
            {
                // 全削除
                var sql = @"delete from lossaddr";
                if (Tdunitaddrcode != "")
                {
                    sql += $" where tdunitaddrcode='{Tdunitaddrcode}'";
                }
                con.Execute(sql);
            }
        }
        public static void AppendLossAddr(string Tdunitaddrcode)
        {
            Syslog.Info($"AppendLossAddr::addr:[{Tdunitaddrcode}]");

            DeleteLossAddr(Tdunitaddrcode);

            using (var con = DbFactory.CreateConnection())
            {
                LossaddrEntity lossaddr = new LossaddrEntity()
                {
                    Tdunitaddrcode = Tdunitaddrcode,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                con.Insert(lossaddr);
            }
        }
#endif
        }
    }