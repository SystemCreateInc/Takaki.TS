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
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using TdDpsLib.Defs;
using TdDpsLib.Models;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Picking.Models
{
    internal class TdUnitManager
    {
        public class TdMaguchi
        {
            public string TbUnitAddrCode = string.Empty;
            public List<string> TbUnitAddrCodes = new List<string>();
        };

        public static int[] ZoneOrderIn = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        // 間口
        public static int LighttingColor = 0;

        public static List<TdMaguchi> TdMaguchis = new List<TdMaguchi>();


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
            distcolor.DistType = distcolor.DistWorkMode == (int)DistWorkMode.Dist ? (int)DistTypeStatus.DistWorking : distcolor.DistWorkMode == (int)DistWorkMode.Check ? (int)DistTypeStatus.CheckWorking : (int)DistTypeStatus.ExtractionWorking;

            List<TdUnitDisplay> tmotdunit = new List<TdUnitDisplay>();

            foreach (var itemseq in distcolor.ItemSeqs)
            {
                // データなしは処理しない
                if (itemseq.Details == null || itemseq.Details.Count == 0)
                {
                    continue;
                }
#if false
                // 検品残数再設定
                if (distcolor.DistWorkMode != (int)DistWorkMode.Dist)
                {
                    itemseq.Dops = itemseq.Drps;
                    itemseq.Drps = 0;
                    itemseq.Ddps = itemseq.Dops - itemseq.Drps;
                    itemseq.Remain_shop_cnt = itemseq.Result_shop_cnt;
                    itemseq.Result_shop_cnt = 0;
                }
#endif
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

                    // 表示なし
                    if (ttlps == 0)
                        continue;

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

                    int zone = 0, seq = 0, seqreverse = 0;
                    TdAddrData? addrdata;
                    tddps.GetTdAddr(out addrdata, tdunitaddr);
                    if (addrdata != null)
                    {
                        zone = addrdata.TdUnitZoneCode;
                        seq = addrdata.TdUnitSeq;
                        seqreverse = addrdata.TdUnitSeqReverse;
                    }

                    if (ttlps != 0)
                    {
                        distcolor.Tdunitdisplay.Add(new TdUnitDisplay() { Tdunitaddrcode = tdunitaddr, Status = (int)DbLib.Defs.Status.Ready, InSeq = (int)itemseq.InSeq!, TdDisplay = tddisplay, Zone = zone, TdUnitSeq = seq, TdUnitSeqReverse = seqreverse });
                    }

                    // 件数取得用
                    tmotdunit.Add(new TdUnitDisplay() { Tdunitaddrcode = tdunitaddr, Status = (int)DbLib.Defs.Status.Ready, InSeq = (int)itemseq.InSeq!, TdDisplay = tddisplay, Zone = zone });
                }
            }


            // 数量がある場合のみ処理する
            if (distcolor.ItemSeqs.Sum(x => x.Dops) != 0)
            {
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

                if (IsDistWorkNormal)
                {
                    // 一斉仕分け
                    LighttingColor = distcolor.DistColor_code;
                    foreach (var query in querys)
                    {
                        TdUnitLight(query.Tdunitaddrcode, tddps, true, false, distcolor.DistColor_code, query.TdDisplay, false);
                    }
                    LighttingColor = 0;
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
            }

            return true;
        }

        // 表示器消灯
        public static bool TdLightOff(ref DistColor distcolor, TdDpsManager tddps, DistColorInfo distcolorinfo, bool bAll = false)
        {
            // 点灯中の中断処理は点灯が完了するまで待つ
            while (LighttingColor == distcolor.DistColor_code)
            {
                Syslog.Info($"TdLightOff:消灯待機:色:{LighttingColor}");
                System.Threading.Thread.Sleep(500);
            }

            lock (_tdLock)
            {
                var querys = distcolor.Tdunitdisplay
                 .Where(x => x.Status == (int)DbLib.Defs.Status.Ready)
                 .GroupBy(x => new { x.Tdunitaddrcode, x.Status })
                 .Select(x => x.First());

                HashSet<int> zones = new HashSet<int>();
                foreach (var query in querys)
                {
                    // 点灯してない表示器は無視
                    if (distcolorinfo.IsDistWorkNormal == false && query.bLight== false)
                        continue;

                    TdAddrData? addrdata;
                    tddps.GetTdAddr(out addrdata, query.Tdunitaddrcode);

                    if (addrdata != null)
                    {
                        if (bAll == false)
                        {
                            int zone = addrdata.TdUnitZoneCode;

                            TdUnitDisplay? now =
                                distcolorinfo.IsDistWorkNormal == true ?
                                    distcolor.Tdunitdisplay.Find(x => x.Tdunitaddrcode == addrdata.TdUnitAddrCode && x.Status == (int)DbLib.Defs.Status.Ready)
                                    : distcolor.Tdunitdisplay.Find(x => x.Tdunitaddrcode == addrdata.TdUnitAddrCode && x.Status == (int)DbLib.Defs.Status.Ready && x.Zone == zone);

                            if (now!=null)
                                now.Status = (int)DbLib.Defs.Status.Inprog;

                            TdUnitDisplay? next =
                                distcolorinfo.IsDistWorkNormal == true ?
                                    distcolor.Tdunitdisplay.Find(x => x.Tdunitaddrcode == addrdata.TdUnitAddrCode && x.Status == (int)DbLib.Defs.Status.Ready)
                                    : distcolor.Tdunitdisplay.Find(x => x.Tdunitaddrcode == addrdata.TdUnitAddrCode && x.Status == (int)DbLib.Defs.Status.Ready && x.Zone == zone);

                            if (distcolorinfo.IsDistWorkNormal)
                            {
                                int color = (int)distcolor.DistColor_code;

                                if (addrdata.GetLightButton() == distcolor.DistColor_code)
                                {
                                    // 次に投入した色の数量があれば表示する
                                    var distnextcolor = distcolorinfo.GetNextDistSeq(distcolor.DistSeqs[zone], addrdata!.TdUnitAddrCode);
                                    if (distnextcolor == null)
                                    {
                                        // 消灯
                                        tddps.Light(ref addrdata, false, false, color, "", true);
                                        // 間口消灯
                                        TdMaguchiLight(addrdata.TdUnitAddrCode, tddps, false, true);
                                    }
                                    else
                                    {
                                        next = distnextcolor.Tdunitdisplay.Find(x => x.Tdunitaddrcode == addrdata.TdUnitAddrCode);

                                        var distnextnextcolor = distcolorinfo.GetNextDistSeq(distnextcolor.DistSeqs[zone], addrdata!.TdUnitAddrCode);
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
                                    color = addrdata.GetLightButton();
                                    // ダブりがあれば
                                    bool blink = false;
                                    if (next != null)
                                    {
                                        var basecolor = distcolorinfo.GetDistColor(color);

                                        if (basecolor != null)
                                        {
                                            var nextnextcolor = distcolorinfo.GetNextDistSeq(basecolor.DistSeqs[zone], addrdata!.TdUnitAddrCode);

                                            if (nextnextcolor == distcolor)
                                            {
                                                nextnextcolor = distcolorinfo.GetNextDistSeq(distcolor.DistSeqs[zone], addrdata!.TdUnitAddrCode);
                                            }

                                            if (nextnextcolor != null && nextnextcolor != distcolor)
                                            {
                                                blink = true;
                                            }
                                        }
                                    }
                                    tddps.Light(ref addrdata, true, blink, color, addrdata.GetNowDisplay(), true);
                                }

#if false



                                int color = (int)distcolor.DistColor_code;
                                if (addrdata.GetLightButton() == distcolor.DistColor_code)
                                {
                                    if (next != null)
                                    {
                                        // 次の商品表示(別色が待機中なら点滅表示)
                                        string text = next != null ? next.TdDisplay : "";
                                        var distnextcolor = distcolorinfo.GetNextDistSeq(distcolor.DistSeqs[zone], addrdata!.TdUnitAddrCode);
                                        bool blink = false;
                                        if (distnextcolor != null)
                                        {
                                            color = distnextcolor.DistColor_code;
                                        }
                                        //消灯
                                        tddps.Light(ref addrdata, false, blink, (int)distcolor.DistColor_code, "", true);
                                        if (distnextcolor != null)
                                        {
                                            var basecolor = distcolorinfo.GetDistColor(color);

                                            if (basecolor != null)
                                            {
                                                var nextnextcolor = distcolorinfo.GetNextDistSeq(basecolor.DistSeqs[zone], addrdata!.TdUnitAddrCode);

                                                if (nextnextcolor == distcolor)
                                                {
                                                    nextnextcolor = distcolorinfo.GetNextDistSeq(distcolor.DistSeqs[zone], addrdata!.TdUnitAddrCode);
                                                }

                                                if (nextnextcolor != null && nextnextcolor != distcolor)
                                                {
                                                    blink = true;
                                                }
                                            }

                                            // 次の表示器点灯
                                            tddps.Light(ref addrdata, true, blink, color, text, true);
                                            zones.Add(addrdata.TdUnitZoneCode);
                                        }
                                    }
                                    else
                                    {

                                        TdUnitLight(query.Tdunitaddrcode, tddps, false, false, (int)distcolor.DistColor_code, "", false);
                                        zones.Add(addrdata.TdUnitZoneCode);
                                    }
                                }
                                else
                                {
                                    color = addrdata.GetLightButton();
                                    // ダブりがあれば
                                    bool blink = false;
                                    if (next != null)
                                    {
                                        var basecolor = distcolorinfo.GetDistColor(color);

                                        if (basecolor != null)
                                        {
                                            var nextnextcolor = distcolorinfo.GetNextDistSeq(basecolor.DistSeqs[zone], addrdata!.TdUnitAddrCode);

                                            if (nextnextcolor == distcolor)
                                            {
                                                nextnextcolor = distcolorinfo.GetNextDistSeq(distcolor.DistSeqs[zone], addrdata!.TdUnitAddrCode);
                                            }

                                            if (nextnextcolor != null && nextnextcolor != distcolor)
                                            {
                                                blink = true;
                                            }
                                        }
                                    }
                                    tddps.Light(ref addrdata, true, blink, color, addrdata.GetNowDisplay(), true);
                                }
#endif
                                }
                            else
                            {
                                // 追駆けの消灯
                                int color = (int)distcolor.DistColor_code;
                                tddps.Light(ref addrdata, false, false, color, "", true);
                                // 間口消灯
                                TdMaguchiLight(addrdata.TdUnitAddrCode, tddps, false, true);

                                TdUnitChaseLight(ref distcolorinfo, tddps, color, zone, distcolor.DistSeqs[zone]);
                            }
                        }
                        else
                        {
                            TdUnitLight(query.Tdunitaddrcode, tddps, false, false, (int)distcolor.DistColor_code, "", false);
                            zones.Add(addrdata.TdUnitZoneCode);
                        }
                    }
                }

                // スタートＢＯＸの消灯
                TdUnitLightChaseStartBox(tddps, distcolor.Tdunitdisplay, StartBoxMode.Off, distcolor.DistColor_code);

                if (zones.Count == 0)
                {
                    var dists = distcolor.Tdunitdisplay
                     .GroupBy(x => x.Tdunitaddrcode);

                    foreach (var d in distcolor.Tdunitdisplay)
                    {
                        zones.Add(d.Zone);
                    }
                }

                foreach (var z in zones)
                {
                    if(ClearZoneOrderIn(tddps, z))
                    {
                        // スタートＢＯＸの消灯
                        TdUnitLightChaseStartBox(tddps, distcolor.Tdunitdisplay, StartBoxMode.Off, distcolor.DistColor_code);
                    }
                }
            }
            return true;
        }

        // 表示器点灯
        public static bool TdUnitLight(string tdunitaddrcode, TdDpsManager tddps, bool bOn, bool bBlink, int color, string display, bool bQuick)
        {
            lock (_tdLock)
            {
                TdAddrData? addrdata;
                tddps.GetTdAddr(out addrdata, tdunitaddrcode);

                // 表示器の現在の状態取得(点灯中の場合は点滅へ移行)
                if (addrdata != null)
                {
                    int lightbtn = (int)TdLedColor.Red;
                    for (lightbtn = (int)TdLedColor.Red; lightbtn < (int)TdLedColor.Claim; lightbtn++)
                    {
                        if (lightbtn == color)
                            continue;
                        if (addrdata.IsLedButtonLight(lightbtn))
                        {
                            // end表示ならクリア
                            var led = addrdata.GetLedButton(lightbtn);
                            if (led != null)
                            {
                                if (led.Text == TdAddrBase.END_DISPLAY)
                                {
                                    addrdata.EndDisplay(false, lightbtn);
                                    tddps.Light(ref addrdata, false, false, lightbtn, "", true);
                                    break;
                                }
                            }
                        }
                    }

                    lightbtn = addrdata.GetLightButton();
                    if (bOn && lightbtn != -1)
                    {
                        if (lightbtn != color)
                        {
                            var led = addrdata.GetLedButton(lightbtn);
                            if (led != null)
                            {
                                bBlink = true;
                                color = lightbtn;
                                display = led.Text!;
                            }
                        }
                    }
                    
                    if (display != TdAddrBase.END_DISPLAY)
                    {
                        Syslog.Info($"TdUnitLight:addr[{addrdata.TdUnitAddrCode}] text[{display}]");

                        tddps.Light(ref addrdata, bOn, bBlink, color, display, bQuick);
                        // 間口点灯
                        TdMaguchiLight(tdunitaddrcode, tddps, bOn, false);
                    }

#if false
                    // endタイマークリア
                    if (bOn == true && color != (int)TdLedColor.Claim)
                    {
                        if (display != TdAddrBase.END_DISPLAY)
                        {
                            addrdata.EndDisplay(false, color);

                        }
                    }
#endif
                }
            }
            return true;
        }

        // 表示器押下
        public static bool TdUnitRcv(TdDpsManager tddps, DistColorInfo distcolorinfo, int stno, int group, int addr, int color, ref bool bDistEnd)
        {
            Syslog.Info($"TdUnitRcv:: stno[{stno}], group[{group}], addr[{addr}], color[{color}]");
            // 休憩中は無視
            if (tddps.IsIdle())
            {
                Syslog.Info($"TdUnitRcv::休憩中");
                return true;
            }

            DateTime nowtm = DateTime.Now;

            TdAddrData? addrdata;
            tddps.GetTdAddr(out addrdata, stno, group, addr);

            if (addrdata != null)
            {
                // チャタリング
                if (addrdata._ChatteringTime > nowtm)
                {
                    Syslog.Info($"チャタリング回避");
                    return false;
                }
                addrdata._ChatteringTime = nowtm + new TimeSpan(0, 0, 0, 0, 500);

                if (addrdata.EndDispTime.IsRunning)
                {
                    Syslog.Info($"END中は無視");
                    return false;
                }

                // ゾーン取得
                int zone = addrdata.TdUnitZoneCode;

                // スタートＢＯＸ押下
                if (addrdata.IsUnitStartBox() == true)
                {
                    var ledbox = addrdata.GetLedButton(addrdata.TddButton);

                    if (ledbox != null)
                    {
                        if (ledbox.Text == EnumExtensions.GetDescription(StartBoxMode.Go))
                        {
                            // 表示器点灯へ
                            TdUnitChaseFirstLight(ref distcolorinfo, tddps, addrdata.TddButton, zone, addrdata.TdUnitFront);
                        }
                    }
                    return true;
                }

                bool bLoss = false;

                // 欠品処理
                if (color == (int)TdLedColor.Claim)
                {
                    int blinkcolor = addrdata.GetLightButton();
                    if (blinkcolor != -1)
                    {
                        // 欠品
                        bLoss = true;
                        color = blinkcolor;

                        Syslog.Info($"TdUnitRcv::欠品:color{blinkcolor}");
#if false
                        // END表示
                        addrdata.EndDisplay(true, color);
                        tddps.Light(ref addrdata, true, false, color, addrdata.GetNowDisplay(), true);
                        // 間口消灯
                        TdMaguchiLight(addrdata.TdUnitAddrCode, tddps, false, true);

                        // END表示タスク起動
                        Task.Run(() =>
                        {
                            while (addrdata.EndDispTime.IsRunning)
                            {
                                System.Threading.Thread.Sleep(500);

                                if (addrdata.IsEndTimeOver() == true)
                                {
                                    addrdata.EndDisplay(false, color);
                                    if (addrdata.nowDisplay == TdAddrBase.END_DISPLAY)
                                    {
                                        tddps.Light(ref addrdata, false, false, addrdata._endColor, "", true);

                                        color = addrdata.GetLightButton();
                                        if (color == -1)
                                        {
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


                        // 配分処理を中断する
                        DistColor? distcolor = distcolorinfo?.GetDistColor(color);
                        if (distcolor != null)
                        {
                            Task.Run(() =>
                            {
                                bool bRet = TdUnitManager.TdLightOff(ref distcolor, tddps, distcolorinfo!);

                                // 作業報告書開始
                                distcolor.ReportEnd();
                                distcolorinfo!.ReportUpdate(distcolor.ReportShain, distcolor.DistWorkMode);

                                DistColorManager.DistUpdate(distcolor);
                            });
                        }
                        return true;
#endif
                    }
                    else
                    {
                        Syslog.Info($"TdUnitRcv::欠品押下したが色が点滅していない{blinkcolor}");
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
                            : distcolor.Tdunitdisplay.Find(x => x.Tdunitaddrcode == addrdata.TdUnitAddrCode && x.Status == (int)DbLib.Defs.Status.Ready && x.Zone == zone);
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
                                        itemseq.Drps -= ps;
                                        itemseq.Ddps -= ps;
                                        if (detail.Tdunitzonecode==1)
                                            itemseq.Left_ps_cnt -= ps;
                                        if (detail.Tdunitzonecode == 2)
                                            itemseq.Right_ps_cnt -= ps;
                                        detail.Drps += ps;
                                        detail.Ddps -= ps;
                                        detail.TdUnitPushTm = nowtm;
                                        detail.DStatus = detail.Drps == detail.Dops ? (int)DbLib.Defs.Status.Completed : (int)DbLib.Defs.Status.Inprog;
                                        now.Status = (int)DbLib.Defs.Status.Completed;

                                        Syslog.Info($"CountDown::Dist:color:{color} inseq:{itemseq.InSeq} item:{itemseq.CdHimban} Ops{detail.Ops} Ddps:{detail.Ddps} Drps:{detail.Drps} itemseq.Drps:{itemseq.Drps}");


                                        // 作業報告書開始
                                        distcolor.ReportCountUp(addrdata.TdUnitAddrCode, ps);
                                    }
                                    else
                                    {
                                        // 検品
                                        int ps = detail.Dops;
                                        distcolor.Drps += ps;
                                        distcolor.Ddps -= ps;
                                        itemseq.Drps += ps;
                                        itemseq.Ddps -= ps;
                                        if (detail.Tdunitzonecode == 1)
                                            itemseq.Left_ps_cnt -= ps;
                                        if (detail.Tdunitzonecode == 2)
                                            itemseq.Right_ps_cnt -= ps;
                                        detail.Drps -= ps;
                                        detail.Ddps += ps;
                                        detail.TdUnitPushTm = nowtm;
                                        now.Status = (int)DbLib.Defs.Status.Completed;

                                        Syslog.Info($"CountDown::Check:color:{color} inseq:{itemseq.InSeq} item:{itemseq.CdHimban} Ops{detail.Ops} Ddps:{detail.Ddps} Drps:{detail.Drps}");
                                    }
                                    itemseq.Remain_shop_cnt--;
                                    itemseq.Result_shop_cnt++;

                                    Syslog.Info($"CountDown::All:color:{color} inseq:{itemseq.InSeq} item:{itemseq.CdHimban} itemseq.Ops:{itemseq.Ops} itemseq.Dops:{itemseq.Dops} itemseq.Drps:{itemseq.Drps} itemseq.Order_shop_cnt:{itemseq.Order_shop_cnt} itemseq.Remain_shop_cnt:{itemseq.Remain_shop_cnt} itemseq.Result_shop_cnt:{itemseq.Result_shop_cnt}");
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
                                    var distnextcolor = distcolorinfo.GetNextDistSeq(distcolor.DistSeqs[zone], addrdata!.TdUnitAddrCode);
                                    bool blink = false;
                                    if (distcolorinfo.IsDistWorkNormal && distnextcolor != null)
                                    {
                                        blink = true;
                                    }
                                    if (next != null)
                                    {
                                        next.bLight = true;
                                    }

                                    tddps.Light(ref addrdata, true, blink, color, text, true);
                                }
                                else
                                {
                                    if (distcolorinfo.IsDistWorkNormal)
                                    {
                                        // 次に投入した色の数量があれば表示する
                                        var distnextcolor = distcolorinfo.GetNextDistSeq(distcolor.DistSeqs[zone], addrdata!.TdUnitAddrCode);
                                        if (distnextcolor == null)
                                        {
                                            // 消灯
                                            tddps.Light(ref addrdata, false, false, color, "", true);
                                            // 間口消灯
                                            TdMaguchiLight(addrdata.TdUnitAddrCode, tddps, false, true);
                                        }
                                        else
                                        {
                                            TdUnitDisplay? isend =
                                                distcolorinfo.IsDistWorkNormal == true ?
                                                distcolor.Tdunitdisplay.Find(x => x.Status == (int)DbLib.Defs.Status.Ready)
                                                 : distcolor.Tdunitdisplay.Find(x => x.Status == (int)DbLib.Defs.Status.Ready && x.Zone == zone);
                                            if (isend != null || bLoss == false)
                                            {
                                                next = distnextcolor.Tdunitdisplay.Find(x => x.Tdunitaddrcode == addrdata.TdUnitAddrCode);

                                                var distnextnextcolor = distcolorinfo.GetNextDistSeq(distnextcolor.DistSeqs[zone], addrdata!.TdUnitAddrCode);
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
                                    }
                                    else
                                    {
                                        // 消灯
                                        tddps.Light(ref addrdata, false, false, color, "", true);
                                        // 間口消灯
                                        TdMaguchiLight(addrdata.TdUnitAddrCode, tddps, false, true);

                                        TdUnitChaseLight(ref distcolorinfo, tddps, color, zone, distcolor.DistSeqs[zone]);
                                    }
                                }
                            }

                            TdUnitDisplay? end =
                                distcolorinfo.IsDistWorkNormal == true ?
                                distcolor.Tdunitdisplay.Find(x => x.Status == (int)DbLib.Defs.Status.Ready)
                                 : distcolor.Tdunitdisplay.Find(x => x.Status == (int)DbLib.Defs.Status.Ready && x.Zone == zone);
                            if (end == null || bLoss == true)
                            {
                                bDistEnd = true;

                                // END表示
                                addrdata.EndDisplay(true, color);
                                tddps.Light(ref addrdata, true, false, color, addrdata.GetNowDisplay(), true);

                                // 追っかけのスタートＢＯＸ消灯
                                if (distcolorinfo.IsDistWorkNormal == false)
                                {
                                    TdUnitLightChaseStartBox(tddps, null, StartBoxMode.Off, distcolor.DistColor_code, zone);

                                    if (ClearZoneOrderIn(tddps, zone))
                                    {
                                        TdUnitLightChaseStartBox(tddps, null, StartBoxMode.Off, distcolor.DistColor_code, zone);
                                    }
                                }

                                // END表示タスク起動
                                Task.Run(() =>
                                {
                                    int distseq = distcolor.DistSeqs[zone];
                                    Syslog.Info($"END:: distseq={distseq}");

                                    while (addrdata.EndDispTime.IsRunning)
                                    {
                                        System.Threading.Thread.Sleep(500);

                                        if (addrdata.IsEndTimeOver() == true)
                                        {
                                            Syslog.Info($"END消灯::color:{color} addr=[{addrdata.TdUnitAddrCode}] distseq={distseq}");
                                            if (addrdata.nowDisplay == TdAddrBase.END_DISPLAY)
                                            {
                                                if (distcolorinfo.IsDistWorkNormal)
                                                {
#if false
                                                    // 次に投入した色の数量があれば表示する
                                                    var distnextcolor = distcolorinfo.GetNextDistSeq(distseq, addrdata!.TdUnitAddrCode);
                                                    if (distnextcolor == null)
                                                    {
                                                        Syslog.Info($"END消灯:クリア");
                                                        // 消灯
                                                        tddps.Light(ref addrdata, false, false, color, "", true);
                                                        // 間口消灯
                                                        TdMaguchiLight(addrdata.TdUnitAddrCode, tddps, false, true);
                                                    }
                                                    else
                                                    {
                                                        next = distnextcolor.Tdunitdisplay.Find(x => x.Tdunitaddrcode == addrdata.TdUnitAddrCode && x.Status== (int)DbLib.Defs.Status.Ready);

                                                        var distnextnextcolor = distcolorinfo.GetNextDistSeq(distnextcolor.DistSeqs[zone], addrdata!.TdUnitAddrCode);
                                                        bool blink = false;
                                                        if (distcolorinfo.IsDistWorkNormal && distnextnextcolor != null)
                                                        {
                                                            blink = true;
                                                        }

                                                        string text = next != null ? next.TdDisplay : "";
                                                        Syslog.Info($"END消灯:消灯:color:{color} addr=[{addrdata.TdUnitAddrCode}] text[{text}]");
                                                        tddps.Light(ref addrdata, false, false, color, text, true);
                                                        if (next != null)
                                                        {
                                                            Syslog.Info($"END消灯:次点灯:color:{distnextcolor.DistColor_code} addr=[{addrdata.TdUnitAddrCode}] text[{text}]");
                                                            tddps.Light(ref addrdata, true, blink, distnextcolor.DistColor_code, text, true);
                                                        }
                                                        else
                                                        {
                                                            Syslog.Info($"END消灯:nextなし color:{distnextcolor.DistColor_code} addr=[{addrdata.TdUnitAddrCode}] text[{text}]");
                                                        }
                                                    }
#else
                                                    Syslog.Info($"END消灯:消灯:color:{color} addr=[{addrdata.TdUnitAddrCode}]");
                                                    tddps.Light(ref addrdata, false, false, addrdata._endColor, "", true);

                                                    color = addrdata.GetLightButton();
                                                    if (color == -1)
                                                    {
                                                    }
                                                    else
                                                    {
                                                        var led = addrdata.GetLedButton(color);
                                                        Syslog.Info($"END消灯:次点灯:color:{color} addr=[{addrdata.TdUnitAddrCode}] text[{led!.Text}]");
                                                        var dc = distcolorinfo.GetDistColor(color);
                                                        if (dc!=null)
                                                        {
                                                            var distnextnextcolor = distcolorinfo.GetNextDistSeq(dc.DistSeqs[zone], addrdata!.TdUnitAddrCode);
                                                            bool blink = false;
                                                            if (distcolorinfo.IsDistWorkNormal && distnextnextcolor != null)
                                                            {
                                                                blink = true;
                                                            }

                                                            tddps.Light(ref addrdata, true, blink, color, led!.Text!, true);
                                                        }
                                                        else
                                                        {
                                                            Syslog.Info($"END消灯:dcnull");
                                                        }
                                                    }
#endif
                                                }
                                                else
                                                {
                                                    Syslog.Info($"END消灯:追いかけ：次点灯");

                                                    // 無条件にクリア
                                                    tddps.Light(ref addrdata, false, false, addrdata._endColor, "", true);

                                                    int lightbtn = (int)TdLedColor.Red;
                                                    for (lightbtn = (int)TdLedColor.Red; lightbtn < (int)TdLedColor.Claim; lightbtn++)
                                                    {
                                                        if (lightbtn == color)
                                                            continue;
                                                        if (addrdata.IsLedButtonLight(lightbtn))
                                                        {
                                                            var led = addrdata.GetLedButton(lightbtn);
                                                            tddps.Light(ref addrdata, true, led!.IsBlink, lightbtn, led!.Text!, true);
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                            Syslog.Info($"ENDTIMEクリア");
                                            addrdata.EndDisplay(false, color);
                                        }
                                    }

                                });

                                // 全ゾーン処理終了で書き込み
                                end = distcolor.Tdunitdisplay.Find(x => x.Status == (int)DbLib.Defs.Status.Ready);
                                if (end == null || bLoss == true)
                                {
                                    Syslog.Info($"作業完了::color:{color} inseq:{itemseq.InSeq} item:{itemseq.CdHimban} itemseq.Ops:{itemseq.Ops} itemseq.Dops:{itemseq.Dops} itemseq.Drps:{itemseq.Drps} itemseq.Order_shop_cnt:{itemseq.Order_shop_cnt} itemseq.Remain_shop_cnt:{itemseq.Remain_shop_cnt} itemseq.Result_shop_cnt:{itemseq.Result_shop_cnt}");

                                    if (bLoss)
                                    {
                                        DistColor? distcolorLoss = distcolorinfo?.GetDistColor(color);
                                        if (distcolorLoss!=null)
                                        {
                                            Syslog.Info($"欠品ボタン押下による消灯開始");
                                            TdUnitManager.TdLightOff(ref distcolorLoss, tddps, distcolorinfo!);
                                        }
                                    }

                                    // 作業報告書開始
                                    distcolor.ReportEnd();
                                    distcolorinfo!.ReportUpdate(distcolor.ReportShain, distcolor.DistWorkMode);

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
        public static bool TdUnitLightChaseStartBox(TdDpsManager tddps, List<TdUnitDisplay>? tdunits, StartBoxMode startboxmode, int color, int zone = 0)
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

                int zoneorderin = ZoneOrderIn[z];

                int topseq = 0, mintdunitseq = 0;

                // ゾーン先頭のＳＥＱを取得
                var topaddr = tddps.TdAddrs!
                    .Where(x => x.TdUnitZoneCode == z && x.IsUnitNormal() == true)
                    .OrderBy(x => x.GetTdUnitSeq(zoneorderin)).FirstOrDefault();
                topseq = topaddr != null ? topaddr.GetTdUnitSeq(zoneorderin) : 0;

                // 点灯してる最小ＳＥＱを取得
                var addr = tddps.TdAddrs!
                    .Where(x => x.TdUnitZoneCode == z && x.GetLightButton() != -1 && x.IsUnitNormal() == true)
                    .OrderBy(x => x.GetTdUnitSeq(zoneorderin)).FirstOrDefault();
                mintdunitseq = addr != null ? addr.GetTdUnitSeq(zoneorderin) - (int)StartBoxMode.SpaceCnt : 99999;

                Syslog.Info($"TdUnitLightChaseStartBox:topseq={topseq} mintdunitseq={mintdunitseq} zone={zone}");

                // 侵入順序取得

                // 棚が１つでも空いていれば入れる様にする
                string gotext = topseq < mintdunitseq ? START_TEXT_GO : START_TEXT_SPACE;

                // スタートＢＯＸ
                var startboxs = tddps.GetTdStartBoxs(z, color);

                for (int stidx = 0; stidx < startboxs.Count(); stidx++)
                {
                    var ad = startboxs[stidx];

                    if (bOn)
                    {
                        var led = ad.GetLedButton(color);
                        if (led != null)
                        {
                            string text = bBlink ? gotext : START_TEXT_IN;

                            // 侵入先のみ表示
                            if (zoneorderin != 0)
                            {
                                if (ad.TdUnitFront != zoneorderin)
                                {
                                    text = START_TEXT_SPACE;
                                }
                            }

                            // 点灯
                            if (led.Text != text || led.IsBlink != bBlink)
                            {
                                tddps.Light(ref ad, bOn, bBlink, color, text, true);
                            }
                        }
                    }
                    else
                    {
                        if (ad.IsLedButtonLight(color) == true)
                        {
                            // 消灯
                            tddps.Light(ref ad, bOn, bBlink, color, START_TEXT_SPACE, true);
                        }
                    }

                }

                // 別色の状態を変更
                for (int i = (int)TdLedColor.Red; i < (int)TdLedColor.Claim; i++)
                {
                    if (i == color)
                        continue;

                    var startboxcols = tddps.GetTdStartBoxs(z, i);

                    for (int stidx = 0; stidx < startboxcols.Count(); stidx++)
                    {
                        var ad = startboxcols[stidx];

                        // 空きあればGO なければ空白
                        if (ad != null)
                        {
                            var led = ad.GetLedButton(i);
                            if (led != null)
                            {
                                gotext = START_TEXT_IN;

                                // 点滅(GO)か？
                                if (ad.GetBlinkButton()!=-1)
                                {
                                    gotext = topseq < mintdunitseq ? START_TEXT_GO : START_TEXT_SPACE;

                                    if (zoneorderin != 0)
                                    {
                                        // 侵入先のみ表示
                                        if (ad.TdUnitFront != zoneorderin)
                                        {
                                            gotext = "";
                                        }
                                    }

                                    // minが１つでもあいていればＧＯを表示あいてなければ空白
                                    if (led.Text != gotext)
                                    {
                                        tddps.Light(ref ad, true, true, i, gotext, true);
                                    }
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
        public static bool TdUnitChaseFirstLight(ref DistColorInfo distcolorinfo, TdDpsManager tddps, int color, int zone, int front)
        {
            Syslog.Info($"TdUnitChaseFirstLight:Start:color={color} zone={zone} front={front}");

            int distseq = distcolorinfo.DistSeq;

            // 侵入できない
            if (!IsZoneOrderIn(zone, front))
                return false;

            SetZoneOrderIn(zone, front);

            // 該当ゾーンで点灯している最小のDistSeqを取得
            int zoneorderin = ZoneOrderIn[zone];

            int mintdunitseq = 0;
            // 点灯してる最小ＳＥＱを取得
            var addr = tddps.TdAddrs!
            .Where(x => x.TdUnitZoneCode == zone && x.GetLightButton() != -1 && x.IsUnitNormal() == true)
            .OrderBy(x => x.GetTdUnitSeq(zoneorderin)).FirstOrDefault();
            mintdunitseq = addr != null ? addr.GetTdUnitSeq(zoneorderin) - (int)StartBoxMode.SpaceCnt : 99999;

            Syslog.Info($"TdUnitChaseFirstLight:mintdunitseq={mintdunitseq} zone={zone}");



            // 最小のアドレスの１つ前まで点灯させる
            var distcolors = distcolorinfo.DistColors!.Where(x => x.DistColor_code == color).ToList();
            foreach (var distcolor in distcolors)
            {
                var querys =
                    distcolor.Tdunitdisplay
                     .Where(x => x.Status == (int)DbLib.Defs.Status.Ready && x.bLight == false && x.Zone == zone && x.GetTdUnitSeq(zoneorderin) < mintdunitseq)
                     .OrderBy(x => x.GetTdUnitSeq(zoneorderin))
                     .GroupBy(x => x.Tdunitaddrcode)
                     .Select(x => x.First());

                if (querys.Count() != 0)
                {
                    // INへ変更
                    TdUnitLightChaseStartBox(tddps, null, StartBoxMode.In, distcolor.DistColor_code, zone);

                    // 点灯
                    foreach (var query in querys)
                    {
                        TdAddrData? addrdata;
                        tddps.GetTdAddr(out addrdata, query.Tdunitaddrcode);

                        if (addrdata != null)
                        {
                            TdUnitLight(query.Tdunitaddrcode, tddps, true, false, distcolor.DistColor_code, query.TdDisplay, false);
                            query.bLight = true;
                        }
                    }
                }

                // スタートＢＯＸ表示を変更
                distcolor.DistSeqs[zone] = ++distcolorinfo.DistSeq;

                // INへ変更
                TdUnitLightChaseStartBox(tddps, null, StartBoxMode.In, distcolor.DistColor_code, zone);

            }

            Syslog.Info($"TdUnitChaseFirstLight:end:color={color} zone={zone}");

            return true;
        }

        // 追っ駆け配分で消灯した表示器より前の表示器を点灯させる
        public static bool TdUnitChaseLight(ref DistColorInfo distcolorinfo, TdDpsManager tddps, int color, int zone, int distseq)
        {
            Syslog.Info($"TdUnitChaseLight:Start:color={color} zone={zone} distseq={distseq} 昇順{ZoneOrderIn[zone]}");

            int zoneorderin = ZoneOrderIn[zone];
            int mintdunitseq = 99999;
            // 点灯対象のdistseqを取得
            var colors = distcolorinfo.DistColors!.Where(x => x.DistSeqs[zone] != 0 && x.DistSeqs[zone] <= distseq)
                .OrderBy(x => x.DistSeqs[zone])
                .Select(x => x.DistColor_code)
                .ToList();
            foreach (var col in colors)
            {
                // 点灯してる最小ＳＥＱを取得
                var addr =
                    tddps.TdAddrs!
                     .Where(x => x.TdUnitZoneCode == zone && x.GetLightButton() == col && x.IsUnitNormal() == true)
                        .OrderBy(x => x.GetTdUnitSeq(zoneorderin)).FirstOrDefault();
                if (addr != null)
                {
                    int seq = addr.GetTdUnitSeq(zoneorderin) - (int)StartBoxMode.SpaceCnt;

                    if (seq < mintdunitseq)
                    {
                        mintdunitseq = seq;
                    }
                }
            }

            Syslog.Info($"TdUnitChaseLight:mintdunitseq={mintdunitseq}");

            // 点灯対象のdistseqを取得
            var distcolors = distcolorinfo.DistColors!.Where(x => x.DistSeqs[zone] != 0 && distseq < x.DistSeqs[zone])
                .OrderBy(x => x.DistSeqs[zone]).ToList();

            foreach (var distcolor in distcolors)
            {
                var querys = distcolor.Tdunitdisplay
                        .Where(x => x.Status == (int)DbLib.Defs.Status.Ready && x.bLight == false && x.Zone == zone && x.GetTdUnitSeq(zoneorderin) < mintdunitseq)
                        .OrderBy(x => x.GetTdUnitSeq(zoneorderin))
                        .GroupBy(x => x.Tdunitaddrcode)
                        .Select(x => x.First());

                // 点灯
                foreach (var query in querys)
                {
                    TdAddrData? addrdata;
                    tddps.GetTdAddr(out addrdata, query.Tdunitaddrcode);

                    if (addrdata != null)
                    {
                        Syslog.Info($"TdUnitChaseLight:点灯 色:{distcolor.DistColor_code} tdunitaddrcode:{query.Tdunitaddrcode} text:{query.TdDisplay} tdunitseq:{query.GetTdUnitSeq(zoneorderin)}");

                        if (addrdata.GetLightButton() == -1)
                        {
                            TdUnitLight(query.Tdunitaddrcode, tddps, true, false, distcolor.DistColor_code, query.TdDisplay, false);
                            query.bLight = true;
                        }
                    }
                }
                var readys = distcolor.Tdunitdisplay
                        .Where(x => x.Status == (int)DbLib.Defs.Status.Ready && x.Zone == zone)
                        .OrderBy(x => x.GetTdUnitSeq(zoneorderin))
                        .GroupBy(x => x.Tdunitaddrcode)
                        .Select(x => x.First());

                foreach (var r in readys)
                {
                    if (r.GetTdUnitSeq(zoneorderin) < mintdunitseq)
                    {
                        mintdunitseq = r.GetTdUnitSeq(zoneorderin) - (int)StartBoxMode.SpaceCnt;
                        Syslog.Info($"TdUnitChaseLight:再設定:[{distcolor.DistColor_name}]:mintdunitseq={mintdunitseq}");
                        break;
                    }
                }
            }

            // INへ変更&別色が使用可能になったらＧＯを表示
            TdUnitLightChaseStartBox(tddps, null, StartBoxMode.In, color, zone);

            Syslog.Info($"TdUnitChaseLight:End:color={color} zone={zone} distseq={distseq}");

            return true;
        }

        // 侵入可能か？
        public static bool IsZoneOrderIn(int zone, int front)
        {
            if (ZoneOrderIn[zone] != 0 && ZoneOrderIn[zone] != front)
                return false;

            return true;
        }
        // 侵入
        public static void SetZoneOrderIn(int zone, int front)
        {
            ZoneOrderIn[zone] = front;
        }

        // 完了
        public static bool ClearZoneOrderIn(TdDpsManager tddps, int zone)
        {
            for (int i = (int)TdLedColor.Red; i < (int)TdLedColor.Claim; i++)
            {
                var startboxcols = tddps.GetTdStartBoxs(zone, i);
                foreach (var sd in startboxcols)
                {
                    // 点灯？
                    if (sd.GetLightButton() != -1)
                    {
                        return false;
                    }
                }
            }

            // 未侵入状態へ変更
            ZoneOrderIn[zone] = 0;
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

        public static void SetMaguchi(TdDpsManager tddps, List<DistBase> maguchis)
        {
            TdMaguchis = new List<TdMaguchi>();

            var addrs = tddps.TdAddrs!.OrderBy(x => x.TdUnitAddrCode).ToList();

            foreach (var maguchi in maguchis)
            {
                TdAddrData? tdaddr;
                tddps.GetTdAddr(out tdaddr, maguchi.Tdunitaddrcode);

                TdMaguchi p = new TdMaguchi();
                p.TbUnitAddrCode = maguchi.Tdunitaddrcode;

                if (tdaddr != null)
                {
                    TdAddrData? addrdata = addrs.Find(p => p.TdUnitAddrCode == maguchi.Tdunitaddrcode);
                    int idx = addrs.IndexOf(tdaddr);
                    for (int i = 1; i < maguchi.Maguchi; i++)
                    {
                        if ((idx + i) < addrs.Count())
                        {
                            p.TbUnitAddrCodes.Add(addrs[idx + i].TdUnitAddrCode);
                        }
                    }
                }
                TdMaguchis.Add(p);
            }
        }
        public static void TdMaguchiLight(string tdunitaddrcode, TdDpsManager tddps, bool bOn, bool bQuick)
        {
            TdAddrData? addrdata;
            tddps.GetTdAddr(out addrdata, tdunitaddrcode);

            if (addrdata != null)
            {
                // 間口がある場合はＺＺＺＺを点灯消灯
                var maguchi = TdMaguchis.Find(x => x.TbUnitAddrCode == addrdata.TdUnitAddrCode);
                if (maguchi != null)
                {
                    maguchi.TbUnitAddrCodes.ForEach(x =>
                    {
                        tddps.GetTdAddr(out addrdata, x);
                        string display = bOn ? "ZZZZZZ" : "";
                        if (addrdata != null)
                        {
                            tddps.Light(ref addrdata, false, false, (int)TdLedColor.Claim, display, bQuick);
                        }
                    });
                }
            }
        }

    }
}
