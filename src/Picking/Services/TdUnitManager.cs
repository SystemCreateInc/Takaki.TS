using DbLib.DbEntities;
using DbLib.Defs;
using DbLib.Extensions;
using ImTools;
using LogLib;
using MaterialDesignColors.Recommended;
using Picking.Defs;
using Picking.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using TdDpsLib.Models;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Picking.Models
{
    internal class TdUnitManager
    {
        private static readonly object _tdLock = new object(); // ロック用のオブジェクト
        // 作業中かチェックする
        public static bool TdLightCheck(DistColor distcolor)
        {
#if false
            if (distcolor.Zonestatus != DbLib.Defs.ZoneStatus.Wait)
            {
                // エラー
                throw new Exception("作業中の色の為、処理出来ません。");
            }
#endif
            return true;
        }

        public static bool TdLight(ref DistColor distcolor, bool IsDistWorkNormal, TdDpsManager tddps)
        {
            // 初期化
            distcolor.Clear();
            distcolor.Tdunitdisplay.Clear();
            // 開始時刻設定
            distcolor.DistStartTm = DateTime.Now;
            distcolor.Distitem_cnt = 0;
            distcolor.DistType = distcolor.DistWorkMode == (int)DistWorkMode.Dist ? (int)DistTypeStatus.DistWorking : (int)DistTypeStatus.CheckWorking;

            List<TdUnitDisplay> tmotdunit = new List<TdUnitDisplay>();

            foreach (var itemseq in distcolor.ItemSeqs)
            {
                // データなしは処理しない
                if (itemseq.Details == null || itemseq.Details.Count == 0)
                {
                    continue;
                }

                // 検品残数再設定
                if (distcolor.DistWorkMode != (int)DistWorkMode.Dist)
                {
                    itemseq.Dops = itemseq.Drps;
                    itemseq.Drps = 0;
                    itemseq.Ddps = itemseq.Dops - itemseq.Drps;
                    itemseq.Remain_shop_cnt = itemseq.Result_shop_cnt;
                    itemseq.Result_shop_cnt = 0;
                }

                distcolor.Distitem_cnt++;

                Dictionary<string, int> psmap = new Dictionary<string, int>();

                foreach (var detail in itemseq.Details)
                {
                    int ps = distcolor.DistWorkMode == (int)DistWorkMode.Dist ? detail.Dops - detail.Drps : detail.Drps;

                    distcolor.Ops += detail.Ops;
                    distcolor.Dops += ps;
                    distcolor.Drps += detail.Drps;
                    distcolor.Ddps += ps;

                    if (detail.Tdunitaddrcode != null)
                    {
                        if (!psmap.ContainsKey(detail.Tdunitaddrcode))
                        {
                            psmap.Add(detail.Tdunitaddrcode, 0);
                        }

                        psmap[detail.Tdunitaddrcode] += ps;
                    }
                }

                foreach (KeyValuePair<string, int> item in psmap)
                {
                    string tdunitaddr = item.Key;
                    int ttlps = item.Value;

                    int ttl_cs = ttlps / itemseq.GetCsunit;
                    int ttl_ps = ttlps % itemseq.GetCsunit;

                    string strcs = string.Format($"{ttl_cs}");
                    if (ttl_cs == 0)
                        strcs = "  ";

                    string strps = string.Format($"{ttl_ps}");

                    string tddisplay = string.Format(" {0:D1}{1,2}{2,2}"
                        , itemseq.InSeq!, strcs, strps
                    );
                    // 桁オーバーは０を設定
                    if (2 < strcs.Count() || 2 < strps.Count())
                    {
                        tddisplay = string.Format(" {0:D1}0000"
                            , itemseq.InSeq!
                        );
                    }

                    int zone = 0, seq = 0;
                    TdAddrData? addrdata;
                    tddps.GetTdAddr(out addrdata, tdunitaddr);
                    if (addrdata!=null)
                    {
                        zone = addrdata.TdUnitZoneCode;
                        seq = addrdata.TdUnitSeq;
                    }

                    if (ttlps != 0)
                    {
                        distcolor.Tdunitdisplay.Add(new TdUnitDisplay() { Tdunitaddrcode = tdunitaddr, Status = (int)DbLib.Defs.Status.Ready, InSeq = (int)itemseq.InSeq!, TdDisplay = tddisplay,Zone=zone, TdUnitSeq = seq });
                    }

                    // 件数取得用
                    tmotdunit.Add(new TdUnitDisplay() { Tdunitaddrcode = tdunitaddr, Status = (int)DbLib.Defs.Status.Ready, InSeq = (int)itemseq.InSeq!, TdDisplay = tddisplay, Zone = zone });
                }
            }

            var querys = distcolor.Tdunitdisplay
                 .Where(x => x.Status == (int)DbLib.Defs.Status.Ready)
                 .GroupBy(x => x.Tdunitaddrcode)
                 .Select(x => x.First());

            // 各表示の最後はＥを設定
            foreach (var query in querys)
            {
                TdUnitDisplay? last = distcolor.Tdunitdisplay.FindLast(x => x.Tdunitaddrcode == query.Tdunitaddrcode);
                if (last != null)
                {
                    last.TdDisplay = 'E' + last.TdDisplay.Remove(0, 1);
                }
            }

#if false
            for (int i = 0; i < distcolor.Tdunitdisplay.Count; i++)
            {
                TdUnitDisplay? last = distcolor.Tdunitdisplay[distcolor.Tdunitdisplay.Count() - 1];

                // 最終の商品なら先頭をＥに置き換え
                if (last != null)
                {
                    last.TdDispaly.Remove(0, 1);
                    last.TdDispaly.Insert(0, "E");
                }
            }
#endif


            if (IsDistWorkNormal)
            {
                // 一斉仕分け
                foreach (var query in querys)
                {
                    TdUnitLight(query.Tdunitaddrcode, tddps, true, false, distcolor.DistColor_code, query.TdDisplay, true);
                }
            }
            else
            {
                // 追いかけはスタートＢＯＸを点灯
                TdUnitLightChaseStartBox(tddps, distcolor.Tdunitdisplay, StartBoxMode.Go, distcolor.DistColor_code);
            }

            // 表示器数
            distcolor.Order_shop_cnt = querys.Count();
            distcolor.Plan_shop_cnt = querys.Count();
            distcolor.Remain_shop_cnt = querys.Count();

            return true;
        }

        // 表示器消灯
        public static bool TdLightOff(ref DistColor distcolor, TdDpsManager tddps, bool bAll = false)
        {
            lock (_tdLock)
            {
                var querys = distcolor.Tdunitdisplay
                 .GroupBy(x => x.Tdunitaddrcode)
                 .Select(x => x.First());

                foreach (var query in querys)
                {
                    TdAddrData? addrdata;
                    tddps.GetTdAddr(out addrdata, query.Tdunitaddrcode);

                    if (addrdata != null)
                    {
                        TdUnitLight(query.Tdunitaddrcode, tddps, false, false, (int)distcolor.DistColor_code, "", true);
                    }
                }

                // スタートＢＯＸの消灯
                TdUnitLightChaseStartBox(tddps, distcolor.Tdunitdisplay, StartBoxMode.Off, distcolor.DistColor_code);
            }
            return true;
        }

        // 表示器点灯
        public static bool TdUnitLight(string tdunitaddrcode, TdDpsManager tddps, bool bOn, bool bBlink, int color, string display, bool bQuick)
        {
            TdAddrData? addrdata;
            tddps.GetTdAddr(out addrdata, tdunitaddrcode);

            // 表示器の現在の状態取得(点灯中の場合は点滅へ移行)
            if (addrdata != null)
            {
                int lightbtn = addrdata.GetLightButton();
                if (bOn && lightbtn != -1)
                {
                    var led = addrdata.GetLedButton(lightbtn);
                    bBlink = true;
                    color = lightbtn;
                    if (led!=null)
                    {
                        display = led.Text!;
                    }
                }

                tddps.Light(ref addrdata, bOn, bBlink, color, display, bQuick);
                // endタイマークリア
                if (bOn == true && color != (int)TdLedColor.Claim)
                {
                    if (display != TdAddrBase.END_DISPLAY)
                    {
                        addrdata.EndDisplay(false);
                    }
                }
            }

            return true;
        }

        // 表示器押下
        public static bool TdUnitRcv(TdDpsManager tddps, DistColorInfo distcolorinfo, int stno, int group, int addr, int color, ref bool bDistEnd)
        {
            Syslog.Info($"TdUnitRcv:: stno[{stno}], group[{group}], addr[{addr}], color[{color}]");
            // 休憩中は無視
            if(tddps.IsIdle())
            {
                Syslog.Info($"TdUnitRcv::休憩中");
                return true;
            }

            DateTime nowtm = DateTime.Now;

            TdAddrData? addrdata;
            tddps.GetTdAddr(out addrdata, stno, group, addr);

            if (addrdata != null)
            {
                // ゾーン取得
                int zone = addrdata.TdUnitZoneCode;

                // スタートＢＯＸ押下
                if (addrdata.IsUnitStartBox()==true)
                {
                    var ledbox = addrdata.GetLedButton(addrdata.TddButton);

                    if (ledbox!=null)
                    {
                        if (ledbox.Text == EnumExtensions.GetDescription(StartBoxMode.Go))
                        {
                            // 表示器点灯へ
                            TdUnitChaseFirstLight(ref distcolorinfo, tddps, addrdata.TddButton, zone);
                        }
                    }
                    return true;
                }

                 bool bLoss = false;

                // 欠品処理
                if (color == (int)TdLedColor.Claim)
                {
                    int blinkcolor = addrdata.GetBlinkButton();
                    if (blinkcolor != -1)
                    {
                        // 欠品
                        bLoss = true;
                        color = blinkcolor;
                    }
                    else
                    {
                        Syslog.Info($"TdUnitRcv::欠品押下したがい色が点滅していない{blinkcolor}");
                        return false;
                    }
                }

                // １ボタンなのでボタン押下は点滅色に書き換え
                color = addrdata.GetLightButton();
                if (color == -1)
                    return false;

                // 点灯してない表示は無視
                var led = addrdata.GetLedButton(color);
                if (led == null)
                {
                    int blinkcolor = addrdata.GetBlinkButton();
                    if (blinkcolor != -1)
                    {
                        tddps.Light(ref addrdata, true, false, blinkcolor, "", true);
                    }
                    Syslog.Info($"TdUnitRcv::color err{color}");
                    return false;
                }

                if (!led.IsLight)
                {
                    int blinkcolor = addrdata.GetBlinkButton();
                    if (blinkcolor != -1)
                    {
                        tddps.Light(ref addrdata, true, false, blinkcolor, "", true);
                    }
                    Syslog.Info($"TdUnitRcv::非点灯{color}");
                    return false;
                }

                bool bBreak = false;
                var distcolors = distcolorinfo.DistColors!.Where(x => x.DistColor_code == color).ToList();

                foreach (var distcolor in distcolors)
                {
                    TdUnitDisplay? now =
                        distcolorinfo.IsDistWorkNormal == true ?
                            distcolor.Tdunitdisplay.Find(x => x.Tdunitaddrcode == addrdata.TdUnitAddrCode && x.Status == (int)DbLib.Defs.Status.Ready)
                            : distcolor.Tdunitdisplay.Find(x => x.Tdunitaddrcode == addrdata.TdUnitAddrCode && x.Status == (int)DbLib.Defs.Status.Ready && x.Zone==zone);
                    if (now == null)
                    {
                        break;
                    }
                    bBreak = true;

                    if (led.IsLight == true)
                    {
                        if (now != null)
                        {
                            now.Status = bLoss ? (int)DbLib.Defs.Status.Inprog : (int)DbLib.Defs.Status.Completed;

                            TdUnitDisplay? next =
                                distcolorinfo.IsDistWorkNormal == true ?
                                    distcolor.Tdunitdisplay.Find(x => x.Tdunitaddrcode == addrdata.TdUnitAddrCode && x.Status == (int)DbLib.Defs.Status.Ready)
                                    : distcolor.Tdunitdisplay.Find(x => x.Tdunitaddrcode == addrdata.TdUnitAddrCode && x.Status == (int)DbLib.Defs.Status.Ready && x.Zone == zone);

                            if (next == null)
                            {
                                distcolor.Remain_shop_cnt--;
                                distcolor.Result_shop_cnt++;
                            }

                            var itemseq = distcolor.ItemSeqs[now.InSeq - 1];

                            var details = itemseq.Details!.Where(x => x.Tdunitaddrcode == addrdata.TdUnitAddrCode);

                            foreach (var detail in details)
                            {
                                if (bLoss == false)
                                {
                                    if (distcolor.DistWorkMode == (int)DistWorkMode.Dist)
                                    {
                                        // 配分
                                        int ps = detail.Ddps;
                                        distcolor.Drps += ps;
                                        distcolor.Ddps -= ps;
                                        detail.Drps += ps;
                                        detail.Ddps -= ps;
                                        detail.TdUnitPushTm = nowtm;
                                        detail.DStatus = detail.Drps == detail.Dops ? (int)DbLib.Defs.Status.Completed : (int)DbLib.Defs.Status.Inprog;
                                        now.Status = (int)DbLib.Defs.Status.Completed;

                                        // 作業報告書開始
                                        distcolor.ReportCountUp(addrdata.TdUnitAddrCode, ps);
                                    }
                                    else
                                    {
                                        // 検品
                                        int ps = detail.Ddps;
                                        distcolor.Drps += ps;
                                        distcolor.Ddps -= ps;
                                        detail.Drps += ps;
                                        detail.Ddps -= ps;
                                    }
                                    itemseq.Remain_shop_cnt--;
                                    itemseq.Result_shop_cnt++;
                                }
                                else
                                {
                                    detail.TdUnitPushTm = nowtm;
                                    detail.DStatus = (int)DbLib.Defs.Status.Inprog;
                                    now.Status = (int)DbLib.Defs.Status.Inprog;
                                }
                            }

                            // 消灯か次の商品表示
                            if (led.IsLight == true)
                            {
                                if (next != null)
                                {
                                    // 次の商品表示(別色が待機中なら点滅表示)
                                    string text = next != null ? next.TdDisplay : "";
                                    var distnextcolor = distcolorinfo.GetNextDistSeq(distcolor.DistSeq[zone], addrdata!.TdUnitAddrCode);
                                    bool blink = false;
                                    if (distcolorinfo.IsDistWorkNormal && distnextcolor != null)
                                    {
                                        blink = true;
                                    }
                                    tddps.Light(ref addrdata, true, blink, color, text, true);
                                }
                                else
                                {
                                    if(distcolorinfo.IsDistWorkNormal)
                                    {
                                        // 次に投入した色の数量があれば表示する
                                        var distnextcolor = distcolorinfo.GetNextDistSeq(distcolor.DistSeq[zone], addrdata!.TdUnitAddrCode);
                                        if (distnextcolor == null)
                                        {
                                            // 消灯
                                            tddps.Light(ref addrdata, false, false, color, "", true);
                                        }
                                        else
                                        {
                                            next = distnextcolor.Tdunitdisplay.Find(x => x.Tdunitaddrcode == addrdata.TdUnitAddrCode);

                                            var distnextnextcolor = distcolorinfo.GetNextDistSeq(distnextcolor.DistSeq[zone], addrdata!.TdUnitAddrCode);
                                            bool blink = false;
                                            if (distcolorinfo.IsDistWorkNormal && distnextnextcolor != null)
                                            {
                                                blink = true;
                                            }

                                            string text = next != null ? next.TdDisplay : "";
                                            tddps.Light(ref addrdata, false, false, color, text, true);
                                            tddps.Light(ref addrdata, true, blink, distnextcolor.DistColor_code, text, true);
                                        }
                                    }
                                    else
                                    {
                                        // 消灯
                                        tddps.Light(ref addrdata, false, false, color, "", true);

                                        TdUnitChaseLight(ref distcolorinfo, tddps, color, zone, distcolor.DistSeq[zone]);
                                    }
                                }
                            }

                            TdUnitDisplay? end =
                                distcolorinfo.IsDistWorkNormal == true ?
                                distcolor.Tdunitdisplay.Find(x => x.Status == (int)DbLib.Defs.Status.Ready)
                                 : distcolor.Tdunitdisplay.Find(x => x.Status == (int)DbLib.Defs.Status.Ready && x.Zone == zone);
                            if (end == null)
                            {
                                bDistEnd = true;

                                // END表示
                                addrdata.EndDisplay(true);
                                tddps.Light(ref addrdata, false, false, (int)TdLedColor.Claim, addrdata.GetNowDisplay(), true);

                                // 追っかけのスタートＢＯＸ消灯
                                if (distcolorinfo.IsDistWorkNormal == false)
                                {
                                    TdUnitLightChaseStartBox(tddps, null, StartBoxMode.Off, distcolor.DistColor_code,zone);
                                }

                                // END表示タスク起動
                                Task.Run(() =>
                                {
                                    while (addrdata.EndDispTime.IsRunning)
                                    {
                                        System.Threading.Thread.Sleep(500);

                                        if (addrdata.IsEndTimeOver() == true)
                                        {
                                            addrdata.EndDisplay(false);
                                            if (addrdata.nowDisplay == TdAddrBase.END_DISPLAY)
                                            {
                                                color = addrdata.GetLightButton();
                                                if (color == -1)
                                                {
                                                    tddps.Light(ref addrdata, false, false, (int)TdLedColor.Claim, "", true);
                                                }
                                                else
                                                {
                                                    var led = addrdata.GetLedButton(color);
                                                    tddps.Light(ref addrdata, true, led!.IsBlink, color, led!.Text!, true);
                                                }
                                            }
                                        }
                                    }

                                });

                                // 全ゾーン処理終了で書き込み
                                end = distcolor.Tdunitdisplay.Find(x => x.Status == (int)DbLib.Defs.Status.Ready);
                                if (end == null)
                                {
                                    // 作業報告書開始
                                    distcolor.ReportEnd();
                                    distcolorinfo.ReportUpdate(distcolor.ReportShain, distcolor.DistWorkMode);

                                    // 終了処理へ
                                    DistColorManager.DistUpdate(distcolor);
                                }
                            }

                            break;
                        }
                    }

                    if (bBreak)
                    {
                        break;
                    }
                }
            }
            else
            {
                Syslog.Info($"TdUnitRcv::存在しない表示器 stno[{stno}], group[{group}], addr[{addr}]");
            }

            return true;
        }

        // 追いかけ受信処理
        public static bool TdUnitRcvChase(TdDpsManager tddps, DistColorInfo distcolorinfo, int stno, int group, int addr, int color)
        {
            return true;
        }

        // 追いかけ処理のスタートＢＯＸ点灯
        public static bool TdUnitLightChaseStartBox(TdDpsManager tddps, List<TdUnitDisplay>? tdunits, StartBoxMode startboxmode, int color, int zone=0)
        {
            Syslog.Info($"TdUnitLightChaseStartBox:Start:color={color} zone={zone} startmode={startboxmode}");

            bool bOn;
            bool bBlink;
            string START_TEXT_GO = EnumExtensions.GetDescription(StartBoxMode.Go);
            string START_TEXT_SPACE = EnumExtensions.GetDescription(StartBoxMode.Off);
            string START_TEXT_IN = EnumExtensions.GetDescription(StartBoxMode.In);

            switch (startboxmode)
            {
                case StartBoxMode.In:
                    bOn = true;
                    bBlink = false;
                    break;
                case StartBoxMode.Go:
                    bOn = true;
                    bBlink = true;
                    break;
                default:
                    bOn = false;
                    bBlink = false;
                    break;
            }

            HashSet<int> zones = new HashSet<int>();
            TdAddrData? addrdata;

            if (zone != 0)
            {
                zones.Add(zone);
            }
            else
            { 
                foreach (var p in tdunits!)
                {
                    tddps.GetTdAddr(out addrdata, p.Tdunitaddrcode);
                    if (addrdata != null)
                    {
                        if (zone == 0 || (zone != 0 && addrdata.TdUnitZoneCode == zone))
                        {
                            zones.Add(addrdata.TdUnitZoneCode);
                        }
                    }
                }
            }

            foreach (var z in zones)
            {
                if (zone != 0 && zone != z)
                    continue;

                // ゾーン先頭のＳＥＱを取得
                var topaddr = tddps.TdAddrs!
                    .Where(x => x.TdUnitZoneCode == z && x.IsUnitNormal() == true)
                    .OrderBy(x => x.TdUnitSeq).FirstOrDefault();
                int topseq = topaddr != null ? topaddr.TdUnitSeq : 0;

                // 点灯してる最小ＳＥＱを取得
                var addr = tddps.TdAddrs!
                    .Where(x => x.TdUnitZoneCode == z && x.GetLightButton() != -1 && x.IsUnitNormal() == true)
                    .OrderBy(x => x.TdUnitSeq).FirstOrDefault();
                int mintdunitseq = addr != null ? addr.TdUnitSeq - (int)StartBoxMode.SpaceCnt : 99999;

                Syslog.Info($"TdUnitLightChaseStartBox:topseq={topseq} mintdunitseq={mintdunitseq} zone={zone}");

                // 棚が１つでも空いていれば入れる様にする
                string gotext = topseq < mintdunitseq ? START_TEXT_GO : START_TEXT_SPACE;

                tddps.GetTdStartBoxAddr(out addrdata, z, color);
                if (addrdata != null)
                {
                    if (bOn)
                    {
                        var led = addrdata.GetLedButton(color);
                        if (led != null)
                        {
                            string text = bBlink ? gotext: START_TEXT_IN;

                            // 点灯
                            if (led.Text != text || led.IsBlink != bBlink)
                            {
                                tddps.Light(ref addrdata, bOn, bBlink, color, text, true);
                            }
                        }
                    }
                    else
                    {
                        if (addrdata.IsLedButtonLight(color) == true)
                        {
                            // 消灯
                            tddps.Light(ref addrdata, bOn, bBlink, color, START_TEXT_SPACE, true);
                        }
                    }
                }


                // 別色の状態を変更
                for (int i = (int)TdLedColor.Red; i < (int)TdLedColor.Claim; i++)
                {
                    if (i == color)
                        continue;

                    tddps.GetTdStartBoxAddr(out addrdata, z, i);

                    // 空きあればGO なければ空白
                    if (addrdata != null)
                    {
                        var led = addrdata.GetLedButton(i);
                        if (led != null)
                        {
                            // 点滅(GO)か？
                            if (led.IsBlink == true)
                            {
                                // minが１つでもあいていればＧＯを表示あいてなければ空白
                                if (led.Text != gotext)
                                {
                                    tddps.Light(ref addrdata, true, true, i, gotext, true);
                                }
                            }
                        }
                    }
                }
            }

            Syslog.Info($"TdUnitLightChaseStartBox:End:color={color} zone={zone} startmode={startboxmode}");
            return true;
        }

        // 追いかけ処理のスタートＢＯＸ押下による表示器点灯
        public static bool TdUnitChaseFirstLight(ref DistColorInfo distcolorinfo, TdDpsManager tddps, int color, int zone)
        {
            Syslog.Info($"TdUnitChaseFirstLight:Start:color={color} zone={zone}");

            int distseq = distcolorinfo.DistSeq;

            // 該当ゾーンで点灯している最小のDistSeqを取得
#if true
                // 点灯してる最小ＳＥＱを取得
                var addr = tddps.TdAddrs!
                    .Where(x => x.TdUnitZoneCode == zone && x.GetLightButton() != -1 && x.IsUnitNormal() == true)
                    .OrderBy(x => x.TdUnitSeq).FirstOrDefault();
                int mintdunitseq = addr != null ? addr.TdUnitSeq - (int)StartBoxMode.SpaceCnt : 99999;

                Syslog.Info($"TdUnitChaseFirstLight:mintdunitseq={mintdunitseq} zone={zone}");
#else
            var colors = distcolorinfo.DistColors!.Where(x => x.DistSeq[zone] != 0 && x.DistSeq[zone] < distseq)
                .OrderBy(x => x.DistSeq).ToList();

            int mintdunitseq = 99999;
            foreach (var distcolor in colors)
            {
                int min = distcolor.Tdunitdisplay
                  .Where(x => x.Status == (int)DbLib.Defs.Status.Ready && x.bLight == true && x.Zone == zone)
                  .Min(x => x.TdUnitSeq);

                if (mintdunitseq == 99999)
                    mintdunitseq = min;
                else
                {
                    if (min < mintdunitseq)
                    {
                        mintdunitseq = min;
                    }
                }
            }
#endif
            // 最小のアドレスの１つ前まで点灯させる
            var distcolors = distcolorinfo.DistColors!.Where(x => x.DistColor_code == color).ToList();
            foreach (var distcolor in distcolors)
            {
                var querys = distcolor.Tdunitdisplay
                     .Where(x => x.Status == (int)DbLib.Defs.Status.Ready && x.bLight == false && x.Zone == zone && x.TdUnitSeq < mintdunitseq)
                     .GroupBy(x => x.Tdunitaddrcode)
                     .Select(x => x.First());

                if (querys.Count() != 0)
                {
                    // 点灯
                    foreach (var query in querys)
                    {
                        TdAddrData? addrdata;
                        tddps.GetTdAddr(out addrdata, query.Tdunitaddrcode);

                        if (addrdata != null)
                        {
                            TdUnitLight(query.Tdunitaddrcode, tddps, true, true, distcolor.DistColor_code, query.TdDisplay, true);
                            query.bLight = true;
                        }
                    }
                }

                // スタートＢＯＸ表示を変更
                distcolor.DistSeq[zone] = ++distcolorinfo.DistSeq;
                // INへ変更
                TdUnitLightChaseStartBox(tddps, null, StartBoxMode.In, distcolor.DistColor_code, zone);
            }

            Syslog.Info($"TdUnitChaseFirstLight:end:color={color} zone={zone}");

            return true;
        }

        // 追っ駆け配分で消灯した表示器より前の表示器を点灯させる
        public static bool TdUnitChaseLight(ref DistColorInfo distcolorinfo, TdDpsManager tddps, int color, int zone, int distseq)
        {
            Syslog.Info($"TdUnitChaseLight:Start:color={color} zone={zone} distseq={distseq}");

            int mintdunitseq = 99999;
            // 点灯対象のdistseqを取得
            var colors = distcolorinfo.DistColors!.Where(x => x.DistSeq[zone] != 0 && x.DistSeq[zone] <= distseq)
                .OrderBy(x => x.DistSeq[zone])
                .Select(x => x.DistColor_code)
                .ToList();
            foreach (var col in colors)
            {
                // 点灯してる最小ＳＥＱを取得
                var addr = tddps.TdAddrs!
                .Where(x => x.TdUnitZoneCode == zone && x.GetLightButton() == col && x.IsUnitNormal() == true)
                .OrderBy(x => x.TdUnitSeq).FirstOrDefault();
                if (addr != null)
                {
                    int seq = addr.TdUnitSeq - (int)StartBoxMode.SpaceCnt;

                    if (seq < mintdunitseq)
                    {
                        mintdunitseq = seq;
                    }
                }
            }

            Syslog.Info($"TdUnitChaseLight:mintdunitseq={mintdunitseq}");

            // 点灯対象のdistseqを取得
            var distcolors = distcolorinfo.DistColors!.Where(x => x.DistSeq[zone] != 0 && distseq < x.DistSeq[zone])
                .OrderBy(x => x.DistSeq[zone]).ToList();

            foreach (var distcolor in distcolors)
            {
                var querys = distcolor.Tdunitdisplay
                     .Where(x => x.Status == (int)DbLib.Defs.Status.Ready && x.bLight == false && x.Zone == zone && x.TdUnitSeq < mintdunitseq)
                     .GroupBy(x => x.Tdunitaddrcode)
                     .Select(x => x.First());

                // 点灯
                foreach (var query in querys)
                {
                    TdAddrData? addrdata;
                    tddps.GetTdAddr(out addrdata, query.Tdunitaddrcode);

                    if (addrdata != null)
                    {
                        TdUnitLight(query.Tdunitaddrcode, tddps, true, true, distcolor.DistColor_code, query.TdDisplay, true);
                        query.bLight = true;
                        if (query.TdUnitSeq < mintdunitseq)
                            mintdunitseq = query.TdUnitSeq - (int)StartBoxMode.SpaceCnt;
                    }
                }
            }

            // INへ変更&別色が使用可能になったらＧＯを表示
            TdUnitLightChaseStartBox(tddps, null, StartBoxMode.In, color, zone);

            Syslog.Info($"TdUnitChaseLight:End:color={color} zone={zone} distseq={distseq}");

            return true;
        }


        // レスポンス
        public static bool TdUnitResponse(TdDpsManager tddps, int stno, int group, int addr, string text)
        {
            Syslog.Info($"TdUnitResponse:: stno[{stno}], group[{group}], addr[{addr}], text[{text}]");

            TdAddrBase? addrdata;
            tddps.GetTdAddrAll(out addrdata, stno, group, addr);

            if (addr == 999 && text != "ACK")
            {
                // 接続失敗
                Syslog.Info($"TdUnitRcv::ポート接続エラー group[{stno}]");
                return false;
            }

            if (addrdata == null)
            {
                Syslog.Info($"TdUnitRcv::存在しない表示器 stno[{stno}], group[{group}], addr[{addr}]");
                return false;
            }

            if (text == "TIM")
            {
                Syslog.Info($"TdUnitRcv::TIM stno[{stno}], group[{group}], addr[{addr}]");
                throw new Exception($"表示器タイムアウト\nport[{stno}], group[{group}], addr[{addr}]");
            }

            if (text == "LOW")
            {
                Syslog.Info($"TdUnitRcv::LOW stno[{stno}], group[{group}], addr[{addr}]");
                throw new Exception($"表示器を充電して下さい\nport[{stno}], group[{group}], addr[{addr}]");
            }
            if (text == "NAK")
            {
                Syslog.Info($"TdUnitRcv::NAK stno[{stno}], group[{group}], addr[{addr}]");
                if (addrdata != null)
                {
                    throw new Exception($"機器異常\n論理:[{addrdata.TdUnitAddrCode}] port[{stno}], group[{group}], addr[{addr}]");
                }
                else
                {
                    throw new Exception($"機器異常\nport[{stno}], group[{group}], addr[{addr}]");
                }
            }

            return true;
        }
    }
}
