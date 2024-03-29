﻿using ControlzEx.Standard;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using CsvLib.Models;
using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DbLib.Defs;
using DryIoc;
using ExportLib.Infranstructures;
using ExportLib.Models;
using ImTools;
using LogLib;
using Mapping.Defs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using TakakiLib.Models;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;
using static System.Reflection.Metadata.BlobBuilder;

namespace Mapping.Models
{
    public class ItemMst
    {
        public ItemMst(Dist dist)
        {
            code = dist.CdHimban;
            name = NameLoader.GetHimban(dist.CdHimban);
        }

        public string code = string.Empty;
        public string name = string.Empty;
    }
    public class ShopMst
    {
        public ShopMst(Dist dist, bool bSum=false)
        {
            if (bSum)
            {
                code = dist.CdSumTokuisaki;
                name = NameLoader.GetTokuisaki(dist.CdSumTokuisaki);
            }
            else
            {
                code = dist.CdTokuisaki;
                name = NameLoader.GetTokuisaki(dist.CdTokuisaki);
            }
        }

        public string code = string.Empty;
        public string name = string.Empty;
    }

    class TokusakiComparer : IEqualityComparer<Dist>
    {
        public bool Equals(Dist? x, Dist? y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            return x.CdSumTokuisaki == y.CdSumTokuisaki;
        }

        public int GetHashCode(Dist obj)
        {
            return (obj.CdSumTokuisaki == null ? 0 : obj.CdSumTokuisaki.GetHashCode());
        }
    }
    class CourseRouteComparer : IEqualityComparer<Dist>
    {
        public bool Equals(Dist? x, Dist? y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            return x.CdSumCourse == y.CdSumCourse && x.CdSumRoute == y.CdSumRoute && x.CdSumTokuisaki == y.CdSumTokuisaki;
        }

        public int GetHashCode(Dist obj)
        {
            string hash = obj.CdSumCourse + string.Format("{0:000}",obj.CdSumRoute) + obj.CdSumTokuisaki;

            return (obj.CdSumCourse == null ? 0 : hash.GetHashCode());
        }
    }


    public class MappingManager
    {
        public List<DistGroup> distgroups = new List<DistGroup>();
        public List<Block> blocks = new List<Block>();
        public List<SumTokuisaki> sumtokuisakis = new List<SumTokuisaki>();
        public List<Dist> MappingTokuisaki = new List<Dist>();

        public string DtDelivery = string.Empty;

        public int RackAllocMax = 10;


        public MappingManager()
        {
        }

        public int GetShopCnt(string CdDistGroup)
        {
            int cnt = 0;

            var distgroup = distgroups.Where(x=>x.CdDistGroup == CdDistGroup).FirstOrDefault();

            if (distgroup != null)
            {
//                cnt = distgroup.mappings.Where(x => x.tdunitaddrcode != "").Select(x => x.tdunitaddrcode).Count();
                cnt = distgroup.mappings.Where(x => x.CdTokuisaki != "").Select(x => x.CdTokuisaki).Distinct().Count();
            }
            return cnt;
        }
        public int GetLocCnt(string CdDistGroup)
        {
            int cnt = 0;

            var distgroup = distgroups.Where(x => x.CdDistGroup == CdDistGroup).FirstOrDefault();

            if (distgroup != null)
            {
//                cnt = distgroup.mappings.Where(x => x.tdunitaddrcode != "").Select(x => x.tdunitaddrcode + x.CdBlock).Distinct().Count();
                cnt = distgroup.mappings.Where(x => x.tdunitaddrcode != "").Select(x => x.Maguchi).Sum();
            }
            return cnt;
        }
        public int GetOverCnt(string CdDistGroup)
        {
            int cnt = 0;

            var distgroup = distgroups.Where(x => x.CdDistGroup == CdDistGroup).FirstOrDefault();

            if (distgroup != null)
            {
                cnt = distgroup.dists.Where(x => x.tdunitaddrcode=="").Select(x => x.CdTokuisaki).Distinct().Count();
            }
            return cnt;
        }

        public void LoadDatas(string dtdelivery, List<string> seldistgroups)
        {
            DtDelivery = dtdelivery;
            NameLoader.selectDate = dtdelivery;
            sumtokuisakis = MappingLoader.GetSumTokuisakis(DtDelivery);
            blocks = MappingLoader.GetBlocks(DtDelivery);
            distgroups = MappingLoader.GetDistGroups(DtDelivery, seldistgroups);

            foreach (var distgroup in distgroups)
            {
                distgroup.ShopCnt = LoadShopCnt(distgroup, DtDelivery);
            }
        }
        public void Run(string CdDistGroup)
        {
            foreach (var distgroup in distgroups)
            {
                if (CdDistGroup != distgroup.CdDistGroup)
                    continue;

                distgroup.dists = MappingLoader.GetDists(distgroup, DtDelivery);
                if (distgroup.dists.Count == 0)
                {
                    throw new Exception($"マッピング可能な出荷データが１件もありません\n仕分グループ:[{distgroup.CdDistGroup} {distgroup.NmDistGroup}]");
                }
                distgroup.stowages = MappingLoader.GetStowages(distgroup, DtDelivery);
                if (distgroup.stowages.Count == 0)
                {
                    throw new Exception($"マッピング可能な箱数データが１件もありません\n仕分グループ:[{distgroup.CdDistGroup} {distgroup.NmDistGroup}]");
                }

                // distにある出荷バッチ、コースがstowageにあるかチェック
                foreach (var dist in distgroup.dists)
                {
                    if (distgroup.stowages.Find(x => x.CdShukkaBatch == dist.CdShukkaBatch && x.CdCourse == dist.CdCourse) == null)
                    {
                        throw new Exception($"箱数データがない出荷データがあります。\n仕分グループ:[{distgroup.CdDistGroup} {distgroup.NmDistGroup}]\n出荷バッチ:[{dist.CdShukkaBatch}]\nコース:[{dist.CdCourse}]");
                    }
                }

                distgroup.IsSave = false;
                distgroup.IsCancel = false;


                // 集計得意先のコース、配順を設定
                SetSumTokuisakiCourse(distgroup.dists);
                SetSumTokuisakiCourse(distgroup.stowages);

                // 得意先の親子関係をセット
                SetSumTokuisaki(ref distgroup.dists);
                SetSumTokuisaki(ref distgroup.stowages);

                // マッピングする配分データの店舗を抽出しソートする
                distgroup.mappings = ChiceMappingTokuisaki(distgroup);

                // 得意先ソート順ログ出力
                LogOut("得意先ソート結果", distgroup.mappings);

                // ブロック順に座席割り当て
                BlockMapping(distgroup);

                LogOut("マッピング結果", distgroup.mappings);
            }
        }
        public void SetSumTokuisakiCourse(List<Dist> lists)
        {
            foreach (var sumtokuisaki in sumtokuisakis)
            {
                var p = lists.Find(x => x.CdKyoten == sumtokuisaki.CdKyoten && x.CdTokuisaki == sumtokuisaki.CdSumTokuisaki);
                if (p != null)
                {
                    // 親のコース、配順を設定
                    sumtokuisaki.CdSumCourse = p.CdCourse;
                    sumtokuisaki.CdSumRoute = p.CdRoute;
                    continue;
                }
                else
                {
                    foreach (var child in sumtokuisaki.SumTokuisakiChilds)
                    {
                        p = lists.Find(x => x.CdTokuisaki == child);
                        if (p != null)
                        {
                            // 子のコース、配順を設定
                            sumtokuisaki.CdSumCourse = p.CdCourse;
                            sumtokuisaki.CdSumRoute = p.CdRoute;
                            break;
                        }
                    }
                }
            }
        }

        public void SetSumTokuisaki(ref List<Dist> lists)
        {
            // 得意先の親子関係をセット
            foreach (var p in lists)
            {
                // 親はそのまま
                var sumtokuisaki = sumtokuisakis.Find(x => x.CdSumTokuisaki == p.CdTokuisaki);
                if (sumtokuisaki != null)
                {
                    p.CdSumTokuisaki = sumtokuisaki.CdSumTokuisaki;
                    p.NmSumTokuisaki = sumtokuisaki.NmSumTokuisaki;
                    p.CdSumCourse = sumtokuisaki.CdSumCourse;
                    p.CdSumRoute = sumtokuisaki.CdSumRoute;
                    continue;
                }

                // 子は親を設定
                sumtokuisaki = sumtokuisakis.Find(x => x.SumTokuisakiChilds.Contains(p.CdTokuisaki) == true);
                if (sumtokuisaki != null)
                {
                    p.CdSumTokuisaki = sumtokuisaki.CdSumTokuisaki;
                    p.NmSumTokuisaki = sumtokuisaki.NmSumTokuisaki;
                    p.CdSumCourse = sumtokuisaki.CdSumCourse;
                    p.CdSumRoute = sumtokuisaki.CdSumRoute;
                    continue;
                }

                // そのまま設定
                p.CdSumTokuisaki = p.CdTokuisaki;
                p.NmSumTokuisaki = p.NmTokuisaki;
                p.CdSumCourse = p.CdCourse;
                p.CdSumRoute = p.CdRoute;
            }
        }
        public List<Dist> ChiceMappingTokuisaki(DistGroup distgroup)
        {
            List<Dist> lists =
                distgroup.CdBinSum == (int)BinSumType.Yes 
                ? distgroup.dists.Distinct(new CourseRouteComparer()).OrderBy(x=>x.CdCourse).ThenBy(x=>x.CdRoute).ToList()  // 集計する
                : distgroup.dists.Distinct(new TokusakiComparer()).OrderBy(x => x.CdCourse).ThenBy(x => x.CdRoute).ToList();    // 集計しない

            LogOut("得意先抽出", lists);

            // コース順に連番をつける
            int seq = 0;
            foreach (var course in distgroup.Courses)
            {
                var p = lists.Where(x => x.CdSumCourse == course);
                if (p != null)
                {
                    seq++;
                    foreach (var pp in p)
                    {
                        pp.MappingSeq = seq;

                        foreach (var syukkabatch in distgroup.ShukkaBatchs)
                        {
                            var shops = distgroup.dists
                                .Where(x => x.CdSumCourse == pp.CdSumCourse && x.CdSumRoute == pp.CdSumRoute && x.CdSumTokuisaki == pp.CdSumTokuisaki && x.CdShukkaBatch == syukkabatch.CdShukkaBatch)
                                .Distinct(new TokusakiComparer()).ToList();

                            var shops2 = distgroup.stowages
                                .Where(x => x.CdSumCourse == pp.CdSumCourse && x.CdSumRoute == pp.CdSumRoute && x.CdSumTokuisaki == pp.CdSumTokuisaki && x.CdShukkaBatch == syukkabatch.CdShukkaBatch)
                                .Distinct(new TokusakiComparer()).ToList();

                            shops.AddRange(shops2);

                            foreach (var shop in shops)
                            {
                                if (pp.Tokuisakis.Find(x => x.CdSumTokuisaki == shop.CdSumTokuisaki && x.CdShukkaBatch == shop.CdShukkaBatch) == null)
                                {
                                    //shop.NmTokuisaki = NameLoader.GetTokuisaki(shop.CdTokuisaki);

                                    pp.Tokuisakis.Add(
                                        new Tokuisaki
                                        {
                                            CdShukkaBatch = syukkabatch.CdShukkaBatch,
                                            NmShukkaBatch = syukkabatch.NmShukkaBatch,
                                            CdLargeGroup = syukkabatch.CdLargeGroup,
                                            NmLargeGroup = syukkabatch.NmLargeGroup,
                                            CdTokuisaki = shop.CdTokuisaki,
                                            //NmTokuisaki = shop.NmTokuisaki,
                                            CdCourse = shop.CdCourse,
                                            CdRoute = shop.CdRoute,
                                            CdSumTokuisaki = shop.CdSumTokuisaki,
                                            //NmSumTokuisaki = shop.CdSumTokuisaki == shop.CdTokuisaki ? shop.NmTokuisaki : NameLoader.GetTokuisaki(shop.CdSumTokuisaki),
                                            CdSumCourse = shop.CdSumCourse,
                                            CdSumRoute = shop.CdSumRoute,
                                        }
                                    );
                                }
                            }
                        }
                    }
                }
            }

            // 箱数設定
            foreach (var d in lists)
            {
                foreach (var shop in d.Tokuisakis)
                {
                    var p = distgroup.stowages.Find(x => x.CdShukkaBatch == shop.CdShukkaBatch && x.CdSumTokuisaki == shop.CdSumTokuisaki && x.StBoxType == (int)BoxType.SmallBox);
                    if (p != null)
                    {
                        d.SmallBox += p.NuBoxCnt;
                    }

                    p = distgroup.stowages.Find(x => x.CdShukkaBatch == shop.CdShukkaBatch && x.CdSumTokuisaki == shop.CdSumTokuisaki && x.StBoxType == (int)BoxType.LargeBox);
                    if (p != null)
                    {
                        d.LargeBox += p.NuBoxCnt;
                    }

                    p = distgroup.stowages.Find(x => x.CdShukkaBatch == shop.CdShukkaBatch && x.CdSumTokuisaki == shop.CdSumTokuisaki && x.StBoxType == (int)BoxType.BlueBox);
                    if (p != null)
                    {
                        d.BlueBox += p.NuBoxCnt;
                    }
                }
            }

            // ソート MappingSeq,配順でソート
            return lists.OrderBy(s => s.MappingSeq).ThenBy(s => s.CdRoute).ToList();
        }

        public void BlockMapping(DistGroup distgroup)
        {
            int mappingidx = 0;

            foreach (var blockseq in distgroup.DistBlockSeqs)
            {
                // ロケーション設定完了
                if (distgroup.mappings.Count == mappingidx)
                    break;

                Syslog.Info($"ブロック[{blockseq.CdBlock}]　棚範囲{blockseq.CdAddrFrom}～{blockseq.CdAddrTo} 割付開始");

                var block = blocks.Find(x => x.CdBlock == blockseq.CdBlock);
                if (block != null)
                {
                    for(int addridx = 0; addridx < block.addrs.Count;addridx++)
                    {
                        var addr = block.addrs[addridx];

                        if (blockseq.CdAddrFrom.CompareTo(addr.TdUnitAddrCode)<=0 && blockseq.CdAddrTo.CompareTo(addr.TdUnitAddrCode)>=0)
                        {
                            if (addr.StRemove == 0)
                            {
                                distgroup.mappings[mappingidx].CdBlock = blockseq.CdBlock;
                                distgroup.mappings[mappingidx].tdunitaddrcode = addr.TdUnitAddrCode;

                                int maguchi = distgroup.mappings[mappingidx].GetMaguchi(block.NuThreshold);
                                if (RackAllocMax < maguchi)
                                {
                                    maguchi = RackAllocMax;
                                    Syslog.Info($"間口書き換え RackAllocMaxオーバー [{RackAllocMax}] 間口[{maguchi}]");
                                }
                                if (block.addrs.Count() < addridx + maguchi)
                                {
                                    maguchi = block.addrs.Count() - addridx;
                                    Syslog.Info($"間口書き換え ブロック終端 間口[{maguchi}]");
                                }

                                addridx += (maguchi-1);
                                distgroup.mappings[mappingidx].Maguchi = maguchi;

                                mappingidx++;

                                // ロケーション設定完了
                                if (distgroup.mappings.Count == mappingidx)
                                    break;
                            }
                        }
                    }
                }
            }

            // distに品名設定
            List<ItemMst> itemmsts = new List<ItemMst>();
            foreach (var dist in distgroup.dists)
            {
                if (itemmsts.Find(x => x.code == dist.CdHimban) == null)
                {
                    itemmsts.Add(new ItemMst(dist));
                }
            }

            // distに得意先設定
            List<ShopMst> shopmsts = new List<ShopMst>();
            for (int i = 0; i < 2; i++)
            {
                foreach (var dist in i==0?distgroup.dists : distgroup.stowages)
                {
                    if (shopmsts.Find(x => x.code == dist.CdTokuisaki) == null)
                    {
                        shopmsts.Add(new ShopMst(dist));
                    }
                    if (shopmsts.Find(x => x.code == dist.CdSumTokuisaki) == null)
                    {
                        shopmsts.Add(new ShopMst(dist,true));
                    }
                }
            }

            // dist,stowageに項目設定
            for (int i = 0; i < 2; i++)
            {
                foreach (var dist in i==0?distgroup.dists : distgroup.stowages)
                { 
                    var mapping = distgroup.mappings.Find(x => x.CdSumTokuisaki == dist.CdSumTokuisaki && x.CdSumCourse == dist.CdSumCourse && x.CdSumRoute == dist.CdSumRoute);
//                    var mapping = distgroup.mappings.Find(x => x.CdSumCourse == dist.CdSumCourse && x.CdSumRoute == dist.CdSumRoute);
                    if (mapping != null)
                    {
                        dist.CdBlock = mapping.CdBlock;
                        dist.tdunitaddrcode = mapping.tdunitaddrcode;
                        dist.NmKyoten = distgroup.NmKyoten;
                        var item = itemmsts.Where(x => x.code == dist.CdHimban).FirstOrDefault();
                        dist.NmHinSeishikimei = item == null ? "" : item.name;
                        dist.Maguchi = mapping.Maguchi;
                        dist.CdBinSum = (int)BinSumType.No;

                        var shop = mapping.Tokuisakis.Find(x => x.CdShukkaBatch == dist.CdShukkaBatch && x.CdSumTokuisaki == dist.CdSumTokuisaki);
                        if (shop != null)
                        {
                            dist.NmTokuisaki = shopmsts.Where(x => x.code == dist.CdTokuisaki).Select(x => x.name).FirstOrDefault() ?? "";
                            dist.NmSumTokuisaki = shopmsts.Where(x => x.code == dist.CdSumTokuisaki).Select(x => x.name).FirstOrDefault() ?? "";
                            dist.NmShukkaBatch = shop.NmShukkaBatch;
                            dist.CdLargeGroup = shop.CdLargeGroup;
                            dist.NmLargeGroup = shop.NmLargeGroup;
                        }
                    }
                    else
                    {
                        // 配分データになく積み付けにあるデータの場合
                    }
                }

                // 便集約設定
                foreach (var dist in i == 0 ? distgroup.dists : distgroup.stowages)
                {
                    if (dist.tdunitaddrcode != "")
                    {
                        dist.CdBinSum = distgroup.dists.Where(x => x.CdBlock == dist.CdBlock && x.tdunitaddrcode == dist.tdunitaddrcode).Select(x => x.CdShukkaBatch).Distinct().Count() <= 1 ? (int)BinSumType.No : (int)BinSumType.Yes;
                    }
                }
            }
        }

        public void Saves(string cdDistGroup)
        {
            List<Dist> AppendStowages = new List<Dist>();

            foreach (var distgroup in distgroups)
            {
                if (distgroup.CdDistGroup != cdDistGroup)
                    continue;

                // 足りないStBoxTypeを追加
                foreach (var stowage in distgroup.stowages)
                {
                    foreach (BoxType boxtype in Enum.GetValues(typeof(BoxType)))
                    {
                        var p = distgroup.stowages.Find(x => x.CdTokuisaki == stowage.CdTokuisaki && x.StBoxType == (int)boxtype);
                        if (p == null)
                        {
                            if(AppendStowages.Find(x => x.CdShukkaBatch == stowage.CdShukkaBatch && x.CdKyoten == stowage.CdKyoten && x.CdTokuisaki == stowage.CdTokuisaki && x.StBoxType == (int)boxtype)==null)
                            {
                                // 追加
                                AppendStowages.Add(new Dist(stowage, boxtype));
                            }
                        }
                    }
                }
            }

            DateTime now = DateTime.Now;
            string now_tostring = now.ToString("yyyyMMddHHmmss");
            string cd_henkousha = Environment.MachineName;
            if (10<cd_henkousha.Count())
            {
                cd_henkousha = cd_henkousha.Substring(0, 10);
            }

            // データ保存
            using (var con = DbFactory.CreateConnection())
            using (var tr = con.BeginTransaction())
            {
                try
                {
                    // distmapping新規追加
                    foreach (var distgroup in distgroups)
                    {
                        if (distgroup.CdDistGroup != cdDistGroup)
                            continue;

                        var sql = "delete from TB_DIST_GROUP_PROGRESS"
                            + " where DT_DELIVERY=@dtdelivery"
                            + " and CD_KYOTEN=@cdkyoten"
                            + " and CD_DIST_GROUP=@cddistgroup;";
                        con.Query<long>(sql,
                            new
                            {
                                dtdelivery = DtDelivery,
                                cdkyoten = distgroup.CdKyoten,
                                cddistgroup = distgroup.CdDistGroup,
                            }, tr);

                        // 商品数、総数取得
                        var blocks = distgroup.dists.Where(x => x.tdunitaddrcode != "").Select(x => x.CdBlock).Distinct().ToList();

                        foreach (var block in blocks)
                        {
                            int itemcnt = distgroup.dists.Where(x => x.CdBlock == block).Select(x => x.CdHimban).Distinct().Count();
                            int pscnt = distgroup.dists.Where(x => x.CdBlock == block).Select(x => x.Ops).Sum();

                            // 新規追加
                            TBDISTGROUPPROGRESSEntity distgorupprogress = new TBDISTGROUPPROGRESSEntity
                            {
                                DTDELIVERY = DtDelivery,
                                CDKYOTEN = distgroup.CdKyoten,
                                NMKYOTEN = "",
                                CDDISTGROUP = distgroup.CdDistGroup,
                                NMDISTGROUP = distgroup.NmDistGroup,
                                IDPC = null,
                                CDBLOCK = block,
                                DTSTART = null,
                                CDSHAIN = null,
                                NMSHAIN = null,
                                NUOITEMCNT = itemcnt,
                                NURITEMCNT = 0,
                                NUOPS = pscnt,
                                NURPS = 0,
                                FGDSTATUS = 0,
                                FGWORKING = 0,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now,
                            };
                            con.Insert(distgorupprogress, x => x.AttachToTransaction(tr));
                        }

                        foreach (var dist in distgroup.dists)
                        {
                            dist.DtTorokuNichiji = dist.createdAt.ToString("yyyyMMddHHmmss");
                            dist.DtKoshinNichiji = now_tostring;
                            dist.CdHenkosha = cd_henkousha;

                            bool over = dist.tdunitaddrcode == "" ? true : false;

                            // オーバーは完了とする
                            if (over)
                            {
                                dist.Rps = dist.Ops;
                            }

                            // dist更新
                            sql = over 
                                 ? "update TB_DIST set FG_MAPSTATUS=@mapstatus,FG_DSTATUS=@dstatus,NU_LOPS=@ops,NU_DOPS=@ops,NU_DRPS=@ops,updatedAt=@update,DT_WORKDT_DIST=@update,CD_HENKOSHA=@cd_henkousha,DT_TOROKU_NICHIJI=@dt_torokuNichiji,DT_KOSHIN_NICHIJI=@dt_koshinNichiji where ID_DIST=@id"
                                 : "update TB_DIST set FG_MAPSTATUS=@mapstatus,NU_LOPS=@ops,NU_DOPS=@ops,CD_HENKOSHA=@cd_henkousha,DT_TOROKU_NICHIJI=@dt_torokuNichiji,DT_KOSHIN_NICHIJI=@dt_koshinNichiji where ID_DIST=@id";

                            con.Execute(sql,
                            new
                            {
                                mapstatus = over ? (int)DbLib.Defs.Status.Inprog : (int)DbLib.Defs.Status.Completed,
                                dstatus = over ? (int)DbLib.Defs.Status.Completed : (int)DbLib.Defs.Status.Ready,
                                ops = dist.Ops,
                                dops = over ? dist.Ops : 0,
                                drps = over ? dist.Ops : 0,
                                update = DateTime.Now,
                                now_tostring = now_tostring,
                                cd_henkousha = cd_henkousha,
                                nm_henkousha = cd_henkousha,
                                dt_torokuNichiji = dist.DtTorokuNichiji,
                                dt_koshinNichiji = dist.DtKoshinNichiji,
                                id = dist.Id,
                            }, tr);

                            var distmapping = new TBDISTMAPPINGEntity
                            {
                                IDDIST = dist.Id,
                                NMSHUKKABATCH = dist.NmShukkaBatch,
                                NMKYOTEN = dist.NmKyoten,
                                NMTOKUISAKI = dist.NmTokuisaki,
                                NMHINSEISHIKIMEI = dist.NmHinSeishikimei,
                                CDSUMTOKUISAKI = dist.CdSumTokuisaki,
                                NMSUMTOKUISAKI = dist.NmSumTokuisaki,
                                CDSUMCOURSE = dist.CdSumCourse,
                                CDSUMROUTE = (short)dist.CdSumRoute,
                                CDBINSUM = (short)dist.CdBinSum,
                                CDBLOCK = dist.CdBlock,
                                CDDISTGROUP = distgroup.CdDistGroup,
                                NMDISTGROUP = distgroup.NmDistGroup,
                                CDLARGEGROUP = dist.CdLargeGroup,
                                NMLARGEGROUP = dist.NmLargeGroup,
                                Tdunitaddrcode = dist.tdunitaddrcode,
                                NUMAGICHI = dist.Maguchi,

                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now,
                            };
                            con.Insert(distmapping, x => x.AttachToTransaction(tr));
                        }
                    }

                    // 積み付け新規追加
                    foreach (var distgroup in distgroups)
                    {
                        if (distgroup.CdDistGroup != cdDistGroup)
                            continue;

                        foreach (var dist in AppendStowages)
                        {
                            if (dist.NmTokuisaki=="")
                            {
                                Syslog.Info($"出荷データにない箱データは無視 CdSumCourse[{dist.CdSumCourse}] CdSumRoute[{dist.CdSumRoute}] CdCourse[{dist.CdCourse}] CdRoute[{dist.CdRoute}] CdSumTokuisaki[{dist.CdSumTokuisaki}]");
                                continue;
                            }

                            dist.DtTorokuNichiji = dist.createdAt.ToString("yyyyMMddHHmmss");
                            dist.DtKoshinNichiji = now_tostring;
                            dist.CdHenkosha = cd_henkousha;

                            string sql = "INSERT INTO TB_STOWAGE VALUES (@DTDELIVERY,@CDSHUKKABATCH,@CDKYOTEN,@CDHAISHOBIN,@CDCOURSE,@CDROUTE,@CDTOKUISAKI,@CDHENKOSHA,@DTTOROKUNICHIJI,@DTKOSHINNICHIJI,@FGSSTATUS,@STBOXTYPE,@NUOBOXCNT,@NURBOXCNT,@NMHENKOSHA,NULL,NULL,@createdAt,@updatedAt);";
                            var id = con.Query<long>(sql,
                                new
                                {
                                    DTDELIVERY = dist.DtDelivery,
                                    CDSHUKKABATCH = dist.CdShukkaBatch,
                                    CDKYOTEN = dist.CdKyoten,
                                    CDHAISHOBIN = dist.CdHaiShobin,
                                    CDCOURSE = dist.CdCourse,
                                    CDROUTE = dist.CdRoute,
                                    CDTOKUISAKI = dist.CdTokuisaki,
                                    CDHENKOSHA = dist.CdHenkosha,
                                    DTTOROKUNICHIJI = dist.DtTorokuNichiji,
                                    DTKOSHINNICHIJI = dist.DtKoshinNichiji,
                                    FGSSTATUS = 0,
                                    STBOXTYPE = (short)dist.StBoxType,
                                    NUOBOXCNT = 0,
                                    NURBOXCNT = 0,
                                    NMHENKOSHA = dist.CdHenkosha,
                                    CreatedAt = DateTime.Now,
                                    UpdatedAt = DateTime.Now,
                                },tr);

                            // アドレスなしは設定しません
                            if (dist.tdunitaddrcode != "")
                            {
                                sql = "INSERT INTO TB_STOWAGE_MAPPING VALUES (IDENT_CURRENT('TB_STOWAGE'),@NMSHUKKABATCH,@NMKYOTEN,@NMTOKUISAKI,@CDBLOCK,@CDDISTGROUP,@NMDISTGROUP,@CDBINSUM,@CDSUMTOKUISAKI,@NMSUMTOKUISAKI,@CDSUMCOURSE,@CDSUMROUTE,@tdunitaddrcode,@NUMAGICHI,@createdAt,@updatedAt);";
                                con.Query<long>(sql,
                                    new
                                    {
                                        NMSHUKKABATCH = dist.NmShukkaBatch,
                                        NMKYOTEN = dist.NmKyoten,
                                        NMTOKUISAKI = dist.NmTokuisaki,
                                        CDSUMTOKUISAKI = dist.CdSumTokuisaki,
                                        NMSUMTOKUISAKI = dist.NmSumTokuisaki,
                                        CDSUMCOURSE = dist.CdSumCourse,
                                        CDSUMROUTE = (short)dist.CdSumRoute,
                                        CDBINSUM = (short)dist.CdBinSum,
                                        CDBLOCK = dist.CdBlock,
                                        CDDISTGROUP = distgroup.CdDistGroup,
                                        NMDISTGROUP = distgroup.NmDistGroup,
                                        Tdunitaddrcode = dist.tdunitaddrcode,
                                        NUMAGICHI = dist.Maguchi,

                                        CreatedAt = DateTime.Now,
                                        UpdatedAt = DateTime.Now,
                                    }, tr);
                            }
                        }
                        foreach (var stowage in distgroup.stowages)
                        {
                            if (stowage.NmTokuisaki == "")
                            {
                                Syslog.Info($"出荷データにない箱データは無視 CdSumCourse[{stowage.CdSumCourse}] CdSumRoute[{stowage.CdSumRoute}] CdCourse[{stowage.CdCourse}] CdRoute[{stowage.CdRoute}] CdSumTokuisaki[{stowage.CdSumTokuisaki}]");
                                continue;
                            }

                            stowage.DtTorokuNichiji = stowage.createdAt.ToString("yyyyMMddHHmmss");
                            stowage.DtKoshinNichiji = now_tostring;
                            stowage.CdHenkosha = cd_henkousha;

                            var sql = "update TB_STOWAGE set updatedAt=@update,CD_HENKOSHA=@cd_henkousha,NM_HENKOSHA=@nm_henkousha,DT_TOROKU_NICHIJI=@dt_torokuNichiji,DT_KOSHIN_NICHIJI=@dt_koshinNichiji where ID_STOWAGE=@id";

                            con.Execute(sql,
                            new
                            {
                                update = DateTime.Now,
                                dt_torokuNichiji = stowage.DtTorokuNichiji,
                                dt_koshinNichiji = stowage.DtKoshinNichiji,
                                cd_henkousha = cd_henkousha,
                                nm_henkousha = cd_henkousha,
                                id = stowage.Id,
                            }, tr);

                            if (stowage.tdunitaddrcode != "")
                            {
                                var stowagemapping = new TBSTOWAGEMAPPINGEntity
                                {
                                    IDSTOWAGE = stowage.Id,
                                    NMSHUKKABATCH = stowage.NmShukkaBatch,
                                    NMKYOTEN = stowage.NmKyoten,
                                    NMTOKUISAKI = stowage.NmTokuisaki,
                                    CDSUMTOKUISAKI = stowage.CdSumTokuisaki,
                                    NMSUMTOKUISAKI = stowage.NmSumTokuisaki,
                                    CDSUMCOURSE = stowage.CdSumCourse,
                                    CDSUMROUTE = (short)stowage.CdSumRoute,
                                    CDBINSUM = 0,
                                    CDBLOCK = stowage.CdBlock,
                                    CDDISTGROUP = distgroup.CdDistGroup,
                                    NMDISTGROUP = distgroup.NmDistGroup,
                                    Tdunitaddrcode = stowage.tdunitaddrcode,
                                    NUMAGICHI = stowage.Maguchi,

                                    CreatedAt = DateTime.Now,
                                    UpdatedAt = DateTime.Now,
                                };
                                con.Insert(stowagemapping, x => x.AttachToTransaction(tr));
                            }
                        }
                    }

                    // 実績作成
                    Export(cdDistGroup);

                    // 実績をＨＵＬＦＴ送信
                    ExecHulft(cdDistGroup);

                    tr.Commit();
                }
                catch (Exception)
                {
                    tr.Rollback();
                    throw;
                }
            }
        }

        public void LogOut(string title, List<Dist> lists)
        {
            foreach (var d in lists)
            {
                if (d.Tokuisakis.Count==0)
                {
                    Syslog.Info($"{title} CdSumCourse[{d.CdSumCourse}] CdSumRoute[{d.CdSumRoute}] CdCourse[{d.CdCourse}] CdRoute[{d.CdRoute}] CdSumTokuisaki[{d.CdSumTokuisaki}] CdTokuisaki[{d.CdTokuisaki}] Block[{d.CdBlock}] Addr[{d.tdunitaddrcode}] 間口[{d.Maguchi}]");
                }
                else
                {

                    foreach (var shop in d.Tokuisakis)
                    {
                        Syslog.Info($"{title} Seq[{d.MappingSeq}] CdShukkaBatch[{shop.CdShukkaBatch}] CdSumCourse[{shop.CdSumCourse}] CdSumRoute[{shop.CdSumRoute}] CdCourse[{shop.CdCourse}] CdRoute[{shop.CdRoute}] CdSumTokuisaki[{shop.CdSumTokuisaki}] CdTokuisaki[{shop.CdTokuisaki}] SmallBox[{d.SmallBox}] LargeBox[{d.LargeBox}] Block[{d.CdBlock}] Addr[{d.tdunitaddrcode}] 間口[{d.Maguchi}]");
                    }
                }
            }
        }

        // 出荷実績データ作成
        public void Export(string cdDistGroup)
        {
            List<CsvDist> csvdatas = new List<CsvDist>();

            foreach (var distgroup in distgroups)
            {
                if (distgroup.CdDistGroup != cdDistGroup)
                    continue;

                foreach (var dist in distgroup.dists)
                {
                    csvdatas.Add(new CsvDist(dist));
                }
            }

            string tmppath = Path.GetTempFileName();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var config = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                HasHeaderRecord = false,                      // ヘッダー行の有無(default:true)
                Encoding = Encoding.GetEncoding("Shift_JIS"), // 文字コード指定(default:UTF-8)
                TrimOptions = TrimOptions.Trim,               // 値のトリムオプション
                Quote = '"',                                  // 値を囲む文字(default:'"')
                ShouldQuote = args => args.FieldType == typeof(string),// 出力時に値をQuoteで指定した文字で囲むかどうか
            };
            using (var writer = new StreamWriter(tmppath, false, Encoding.GetEncoding("Shift_JIS")))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(csvdatas);
            }

            var path = GetExportPath();

            File.Copy(tmppath, path, true);
            Syslog.SLCopy(path);
        }

        // ※ハルフト送信を止める
        public void ExecHulft(string cdDistGroup)
        {
#if false
            var config = new ConfigurationBuilder()
                .AddJsonFile("common.json", true, true)
                .Build();

            var hulftpath = config.GetSection("hulft")?["path"] ?? "";

            string hulftid = GetExportHulftId();
            if (hulftid == "" || hulftpath=="")
            {
                Syslog.Warn($"ExecHulft:Skip");
                return;
            }

            var app = new ProcessStartInfo();
            app.FileName = hulftpath;
            app.Arguments = hulftid;

            Syslog.Warn($"ExecHulft:[{app.FileName} {app.Arguments}]");
            Process.Start(app);
#endif
            foreach (var distgroup in distgroups)
            {
                if (distgroup.CdDistGroup != cdDistGroup)
                    continue;

                // 書き込みフラグＯＦＦ
                distgroup.IsSave = false;
                distgroup.IsCancel = false;
            }
        }


        public string GetExportPath()
        {
            using (var repo = new ExportRepository())
            {
                if (repo.GetInterfaceFile(DbLib.Defs.DataType.PickResult) is InterfaceFile interfaceFile)
                {
                    return interfaceFile.FileName;
                }
            }
            return "";
        }
        public string GetExportHulftId()
        {
            using (var repo = new ExportRepository())
            {
                if (repo.GetInterfaceFile(DbLib.Defs.DataType.PickResult) is InterfaceFile interfaceFile)
                {
                    return interfaceFile.HulftId??"";
                }
            }
            return "";
        }


        // データ初期化
        public void ClearDatas(string dtdelivery, List<string> seldistgroups)
        {
            DtDelivery = dtdelivery;
            NameLoader.selectDate = dtdelivery;
            var sumtokuisakis = MappingLoader.GetSumTokuisakis(DtDelivery);
            var blocks = MappingLoader.GetBlocks( DtDelivery);
            var distgroups = MappingLoader.GetDistGroups(DtDelivery, seldistgroups);

            // 該当データを抽出
            foreach (var distgroup in distgroups)
            {
                distgroup.dists = MappingLoader.GetDists(distgroup, DtDelivery);
                distgroup.stowages = MappingLoader.GetStowages(distgroup, DtDelivery);
            }

            using (var con = DbFactory.CreateConnection())
            using (var tr = con.BeginTransaction())
            {
                try
                {
                    // distmapping新規追加
                    foreach (var distgroup in distgroups)
                    {
                        var sql = "delete from TB_DIST_GROUP_PROGRESS"
                            + " where DT_DELIVERY=@dtdelivery"
                            + " and CD_KYOTEN=@cdkyoten"
                            + " and CD_DIST_GROUP=@cddistgroup;";
                        con.Query<long>(sql,
                            new
                            {
                                dtdelivery = dtdelivery,
                                cdkyoten = distgroup.CdKyoten,
                                cddistgroup = distgroup.CdDistGroup,
                            }, tr);

                        foreach (var dist in distgroup.dists)
                        {
                            sql = "update TB_DIST set "
                                + "NU_LOPS=NU_OPS"
                                + ",NU_LRPS=0"
                                + ",NU_DOPS=NU_OPS"
                                + ",NU_DRPS=0"
                                + ",FG_MAPSTATUS=0"
                                + ",FG_LSTATUS=0"
                                + ",FG_DSTATUS=0"
                                + ",CD_SHAIN_LARGE=null"
                                + ",NM_SHAIN_LARGE=null"
                                + ",DT_WORKDT_LARGE=null"
                                + ",CD_SHAIN_DIST=null"
                                + ",NM_SHAIN_DIST=null"
                                + ",DT_WORKDT_DIST=null"
                                + ",DT_SENDDT_DIST=null"
                                + ",updatedAt=@UpdatedAt"
                                + " where ID_DIST=@id;";

                            sql += "delete from TB_DIST_MAPPING where ID_DIST=@id;";

                            con.Query<long>(sql,
                                new
                                {
                                    id = dist.Id,
                                    UpdatedAt = DateTime.Now,
                                }, tr);
                        }

                        foreach (var dist in distgroup.stowages)
                        {
                            sql = "update TB_STOWAGE set "
                                + "NU_RBOXCNT=0"
                                + ",FG_SSTATUS=0"
                                + ",DT_WORKDT_STOWAGE=null"
                                + ",DT_SENDDT_STOWAGE=null"
                                + ",updatedAt=@UpdatedAt"
                                + " where ID_STOWAGE=@id;";

                            sql += "delete from TB_STOWAGE_MAPPING where ID_STOWAGE=@id;";

                            con.Query<long>(sql,
                                new
                                {
                                    id = dist.Id,
                                    UpdatedAt = DateTime.Now,
                                }, tr);

                        }

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
        public static int LoadShopCnt(DistGroup distgroup, string dtdelivery)
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
                var sql = $"select count(distinct CD_TOKUISAKI) cnt from TB_DIST where CD_KYOTEN=@cdkyoten and DT_DELIVERY=@dtdelivery {whereCourse}";

                var entity = con.Query(sql,
                    new
                    {
                        @cdkyoten = distgroup.CdKyoten,
                        @dtdelivery = dtdelivery,
                    })
                    .FirstOrDefault();

                return entity != null ? entity.cnt : 0;
            }
        }
    }
}
