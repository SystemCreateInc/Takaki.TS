using ControlzEx.Standard;
using CsvHelper;
using CsvHelper.Configuration;
using CsvLib.Models;
using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DbLib.Defs;
using DryIoc;
using ExportLib.Infranstructures;
using ExportLib.Models;
using LogLib;
using Mapping.Defs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            return x.CdSumCourse == y.CdSumCourse && x.CdSumRoute == y.CdSumRoute;
        }

        public int GetHashCode(Dist obj)
        {
            string hash = obj.CdSumCourse + string.Format("{0:000}",obj.CdSumRoute);

            return (obj.CdCourse == null ? 0 : hash.GetHashCode());
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
                cnt = distgroup.mappings.Where(x => x.tdunitaddrcode != "").Select(x => x.tdunitaddrcode).Count();
            }
            return cnt;
        }
        public int GetLocCnt(string CdDistGroup)
        {
            int cnt = 0;

            var distgroup = distgroups.Where(x => x.CdDistGroup == CdDistGroup).FirstOrDefault();

            if (distgroup != null)
            {
                var aa = distgroup.mappings.Where(x => x.tdunitaddrcode != "");

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

                // 集計得意先のコース、配順を設定
                SetSumTokuisakiCourse(distgroup.dists);

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
                ? distgroup.dists.Distinct(new CourseRouteComparer()).ToList()  // 集計する
                : distgroup.dists.Distinct(new TokusakiComparer()).ToList();    // 集計しない

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
                            var shops =
                                distgroup.CdBinSum == (int)BinSumType.Yes ?
                                distgroup.dists.Where(x => x.CdCourse == pp.CdCourse && x.CdRoute == pp.CdRoute).Distinct(new TokusakiComparer()).ToList()
                                : distgroup.dists.Where(x => x.CdCourse == pp.CdCourse && x.CdRoute == pp.CdRoute && x.CdSumTokuisaki == pp.CdSumTokuisaki).Distinct(new TokusakiComparer()).ToList();

                            foreach (var shop in shops)
                            {
                                if (pp.Tokuisakis.Find(x=>x.CdSumTokuisaki == shop.CdSumTokuisaki)==null)
                                {
                                    shop.NmTokuisaki = NameLoader.GetTokuisaki(shop.CdTokuisaki);

                                    pp.Tokuisakis.Add(
                                        new Tokuisaki
                                        {
                                            CdShukkaBatch = syukkabatch.CdShukkaBatch,
                                            NmShukkaBatch = syukkabatch.NmShukkaBatch,
                                            CdLargeGroup = syukkabatch.CdLargeGroup,
                                            NmLargeGroup = syukkabatch.NmLargeGroup,
                                            CdTokuisaki = shop.CdTokuisaki,
                                            NmTokuisaki = shop.NmTokuisaki,
                                            CdCourse = shop.CdCourse,
                                            CdRoute = shop.CdRoute,
                                            CdSumTokuisaki = shop.CdSumTokuisaki,
                                            NmSumTokuisaki = shop.CdSumTokuisaki == shop.CdTokuisaki ? shop.NmTokuisaki : NameLoader.GetTokuisaki(shop.CdSumTokuisaki),
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
                    var p = distgroup.stowages.Find(x => x.CdSumTokuisaki == shop.CdSumTokuisaki && x.StBoxType == (int)BoxType.SmallBox);
                    if (p != null)
                    {
                        d.SmallBox += p.NuBoxCnt;
                    }

                    p = distgroup.stowages.Find(x => x.CdSumTokuisaki == shop.CdSumTokuisaki && x.StBoxType == (int)BoxType.LargeBox);
                    if (p != null)
                    {
                        d.LargeBox += p.NuBoxCnt;
                    }

                    p = distgroup.stowages.Find(x => x.CdSumTokuisaki == shop.CdSumTokuisaki && x.StBoxType == (int)BoxType.BlueBox);
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
                if (itemmsts.Find(x => x.code == dist.CdHimban)==null)
                {
                    itemmsts.Add(new ItemMst(dist));
                }
            }

            // dist,stowageに項目設定
            foreach (var dist in distgroup.dists)
            {
                //var mapping = distgroup.mappings.Find(x => x.CdSumTokuisaki == dist.CdSumTokuisaki);
                var mapping = distgroup.mappings.Find(x => x.CdSumCourse == dist.CdSumCourse && x.CdSumRoute == dist.CdSumRoute);
                if (mapping != null)
                {
                    dist.CdBlock = mapping.CdBlock;
                    dist.tdunitaddrcode = mapping.tdunitaddrcode;
                    dist.NmKyoten = distgroup.NmKyoten;
                    var item = itemmsts.Where(x => x.code == dist.CdHimban).FirstOrDefault();
                    dist.NmHinSeishikimei = item == null ? "" : item.name;
                    dist.Maguchi = mapping.Maguchi;
                    dist.CdBinSum = mapping.Tokuisakis.Count==1 ? (int)BinSumType.No : (int)BinSumType.Yes;

                    var shop = mapping.Tokuisakis.Find(x => x.CdSumTokuisaki == dist.CdSumTokuisaki);
                    if (shop != null)
                    {
                        dist.NmTokuisaki = shop.NmTokuisaki;
                        dist.NmSumTokuisaki = shop.NmSumTokuisaki;
                        dist.CdShukkaBatch = shop.CdShukkaBatch;
                        dist.NmShukkaBatch = shop.NmShukkaBatch;
                        dist.CdLargeGroup = shop.CdLargeGroup;
                        dist.NmLargeGroup = shop.NmLargeGroup;
                    }
                }
            }
            foreach (var stowage in distgroup.stowages)
            {
                //var mapping = distgroup.mappings.Find(x => x.CdSumTokuisaki == stowage.CdSumTokuisaki);
                var mapping = distgroup.mappings.Find(x => x.CdSumCourse == stowage.CdSumCourse && x.CdSumRoute == stowage.CdSumRoute);
                if (mapping != null)
                {
                    stowage.CdBlock = mapping.CdBlock;
                    stowage.tdunitaddrcode = mapping.tdunitaddrcode;
                    stowage.NmShukkaBatch = mapping.NmShukkaBatch;
                    stowage.NmKyoten = distgroup.NmKyoten;
                    stowage.Maguchi = mapping.Maguchi;
                    var shop = mapping.Tokuisakis.Find(x => x.CdSumTokuisaki == stowage.CdSumTokuisaki);
                    if (shop != null)
                    {
                        stowage.NmTokuisaki = shop.NmTokuisaki;
                        stowage.NmSumTokuisaki = shop.NmSumTokuisaki;
                        stowage.CdShukkaBatch = shop.CdShukkaBatch;
                        stowage.NmShukkaBatch = shop.NmShukkaBatch;
                        stowage.CdLargeGroup = shop.CdLargeGroup;
                        stowage.NmLargeGroup = shop.NmLargeGroup;
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
                        var p = distgroup.stowages.Find(x => x.CdTokuisaki == stowage.CdSumTokuisaki && x.StBoxType == (int)boxtype);
                        if (p == null)
                        {
                            // 追加
                            AppendStowages.Add(new Dist(stowage, boxtype));
                        }
                    }
                }
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

                        foreach (var dist in distgroup.dists)
                        {
                            bool over = dist.tdunitaddrcode == "" ? true : false;

                            // オーバーは完了とする
                            if (over)
                            {
                                dist.Rps = dist.Ops;
                            }

                            // dist更新
                            var sql = over 
                                 ? "update TB_DIST set FG_MAPSTATUS=@mapstatus,FG_DSTATUS=@dstatus,NU_LOPS=@ops,NU_DOPS=@ops,NU_DRPS=@ops,updatedAt=@update,DT_WORKDT_DIST=@update where ID_DIST=@id"
                                 : "update TB_DIST set FG_MAPSTATUS=@mapstatus,NU_LOPS=@ops,NU_DOPS=@ops where ID_DIST=@id";

                            con.Execute(sql,
                            new
                            {
                                mapstatus = over ? (int)DbLib.Defs.Status.Inprog : (int)DbLib.Defs.Status.Completed,
                                dstatus = over ? (int)DbLib.Defs.Status.Completed : (int)DbLib.Defs.Status.Ready,
                                ops = dist.Ops,
                                dops = over ? dist.Ops : 0,
                                drps = over ? dist.Ops : 0,
                                update = DateTime.Now,
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
                    DateTime now = DateTime.Now;
                    foreach (var distgroup in distgroups)
                    {
                        if (distgroup.CdDistGroup != cdDistGroup)
                            continue;

                        foreach (var dist in AppendStowages)
                        {
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
                                    CDHENKOSHA = "",
                                    DTTOROKUNICHIJI = now.ToString("yyyyMMddHHmmss"),
                                    DTKOSHINNICHIJI = now.ToString("yyyyMMddHHmmss"),
                                    FGSSTATUS = 0,
                                    STBOXTYPE = (short)dist.StBoxType,
                                    NUOBOXCNT = 0,
                                    NURBOXCNT = 0,
                                    NMHENKOSHA = "",
                                    CreatedAt = DateTime.Now,
                                    UpdatedAt = DateTime.Now,
                                },tr);


                            sql = "INSERT INTO TB_STOWAGE_MAPPING VALUES (IDENT_CURRENT('TB_STOWAGE'),@NMSHUKKABATCH,@NMKYOTEN,@NMTOKUISAKI,@CDBLOCK,@CDDISTGROUP,@NMDISTGROUP,@CDBINSUM,@CDSUMTOKUISAKI,@NMSUMTOKUISAKI,@CDSUMCOURSE,@CDSUMROUTE,@tdunitaddrcode,@NUMAGICHI,@createdAt,@updatedAt);";
                            con.Query<long>(sql,
                                new
                                {
                                    NMSHUKKABATCH = dist.NmShukkaBatch,
                                    NMKYOTEN = dist.NmKyoten,
                                    NMTOKUISAKI = dist.NmTokuisaki,
                                    CDSUMTOKUISAKI = dist.NmHinSeishikimei,
                                    NMSUMTOKUISAKI = dist.NmSumTokuisaki,
                                    CDSUMCOURSE = dist.CdSumCourse,
                                    CDSUMROUTE = (short)dist.CdSumRoute,
                                    CDBINSUM = (short)dist.CdBinSum,
                                    CDBLOCK = dist.CdBlock,
                                    CDDISTGROUP = distgroup.CdDistGroup,
                                    NMDISTGROUP = distgroup.NmDistGroup,
                                    Tdunitaddrcode = dist.NmHinSeishikimei,
                                    NUMAGICHI = dist.Maguchi,

                                    CreatedAt = DateTime.Now,
                                    UpdatedAt = DateTime.Now,
                                },tr);
                        }
                        foreach (var stowage in distgroup.stowages)
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
                foreach (var shop in d.Tokuisakis)
                {
                    Syslog.Info($"{title} Seq[{d.MappingSeq}] CdShukkaBatch[{shop.CdShukkaBatch}] CdSumCourse[{shop.CdSumCourse}] CdSumRoute[{shop.CdSumRoute}] CdCourse[{shop.CdCourse}] CdRoute[{shop.CdRoute}] CdSumTokuisaki[{shop.CdSumTokuisaki}] CdTokuisaki[{shop.CdTokuisaki}] SmallBox[{d.SmallBox}] LargeBox[{d.LargeBox}] Block[{d.CdBlock}] Addr[{d.tdunitaddrcode}] 間口[{d.Maguchi}]");
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
                ShouldQuote = (args) => true,                 // 出力時に値をQuoteで指定した文字で囲むかどうか
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

        public void ExecHulft(string cdDistGroup)
        {
            //
            // C:\HULFT Family\hulft7\binnt\utlsend - f S2AOT001 - sync

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
                        foreach (var dist in distgroup.dists)
                        {
                            var sql = "update TB_DIST set "
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
                            var sql = "update TB_STOWAGE set "
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
    }
}
