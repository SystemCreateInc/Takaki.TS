using DbLib.DbEntities;
using DbLib.Defs;
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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
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

        public static bool TdLight(ref DistColor distcolor, DistGroupEx distgroup, TdDpsManager tddps)
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
                    if (ttl_ps == 0)
                        strps = "  ";

                    string tddisplay = string.Format(" {0:D1}{1,2}{2,2}"
                        , itemseq.InSeq!, strcs, strps
                    );
                    // 桁オーバーは０を設定
                    if (2<strcs.Count() || 2 < strps.Count())
                    {
                        tddisplay = string.Format(" {0:D1}0000"
                            , itemseq.InSeq!
                        );
                    }

                    if (ttlps != 0)
                    {
                        distcolor.Tdunitdisplay.Add(new TdUnitDisplay() { Tdunitaddrcode = tdunitaddr, Status = (int)DbLib.Defs.Status.Ready, InSeq = (int)itemseq.InSeq!, TdDisplay = tddisplay });
                    }

                    // 件数取得用
                    tmotdunit.Add(new TdUnitDisplay() { Tdunitaddrcode = tdunitaddr, Status = (int)DbLib.Defs.Status.Ready, InSeq = (int)itemseq.InSeq!, TdDisplay = tddisplay });
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


            foreach (var query in querys)
            {
                if (distgroup.IsDistWorkNormal)
                {
                    // 一斉仕分け
                    TdUnitLight(query.Tdunitaddrcode, tddps, true, true, distcolor.DistColor_code, query.TdDisplay, true);
                }
                else
                {
                    // 追いかけ
                    TdUnitLight(query.Tdunitaddrcode, tddps, true, true, distcolor.DistColor_code, query.TdDisplay, true);
                }
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
                        if (addrdata.IsLedButtonLight((int)TdLedColor.Blue))
                        {
                            // end表示中なら再点灯
                            if (addrdata.EndDispTime.IsRunning)
                            {
                                TdUnitLight(query.Tdunitaddrcode, tddps, true, true, (int)TdLedColor.Blue, "   NOT", true);
                            }
                        }
                        else
                        {
                            TdUnitLight(query.Tdunitaddrcode, tddps, false, false, (int)distcolor.DistColor_code, "", true);
                        }
                    }
                }
            }

            return true;
        }

        // 表示器点灯
        public static bool TdUnitLight(string tdunitaddrcode, TdDpsManager tddps, bool bOn, bool bBlink, int color, string display, bool bQuick)
        {
            TdAddrData? addrdata;
            tddps.GetTdAddr(out addrdata, tdunitaddrcode);

            // 表示器の現在の状態取得
            bool isLight = false;
            if (addrdata != null)
            {
                isLight = addrdata.IsLedButtonLight(color);

                // 別色が点灯中は点灯させない
                int blinkcolor = addrdata.GetBlinkButton();
                if (blinkcolor == -1)
                {
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
            }

            return isLight;
        }

        // 表示器押下
        public static bool TdUnitRcv(TdDpsManager tddps, DistColorInfo distcolorinfo, int stno, int group, int addr, int color)
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
                color = addrdata.GetBlinkButton();
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
                    TdUnitDisplay? now = distcolor.Tdunitdisplay.Find(x => x.Tdunitaddrcode == addrdata.TdUnitAddrCode && x.Status == (int)DbLib.Defs.Status.Ready);
                    if (now == null)
                    {
                        break;
                    }
                    bBreak = true;

                    if (led.IsBlink == true)
                    {
                        if (now != null)
                        {
                            now.Status = bLoss ? (int)DbLib.Defs.Status.Inprog : (int)DbLib.Defs.Status.Completed;

                            TdUnitDisplay? next = distcolor.Tdunitdisplay.Find(x => x.Tdunitaddrcode == addrdata.TdUnitAddrCode && x.Status == (int)DbLib.Defs.Status.Ready);

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
                                    // 次の商品表示
                                    string text = next != null ? next.TdDisplay : "";
                                    tddps.Light(ref addrdata, true, true, color, text, true);
                                }
                                else
                                {
                                    // 次に投入した色の数量があれば表示する
                                    var distnextcolor = distcolorinfo.GetNetDistSeq(distcolor.DistSeq, addrdata!.TdUnitAddrCode);
                                    if (distnextcolor==null)
                                    {
                                        // 消灯
                                        tddps.Light(ref addrdata, false, false, color, "", true);
                                    }
                                    else
                                    {
                                        next = distnextcolor.Tdunitdisplay.Find(x => x.Tdunitaddrcode == addrdata.TdUnitAddrCode);
                                        string text = next != null ? next.TdDisplay : "";
                                        tddps.Light(ref addrdata, false, false, color, text, true);
                                        tddps.Light(ref addrdata, true, true, distnextcolor.DistColor_code, text, true);
                                    }
                                }
                            }

                            TdUnitDisplay? end = distcolor.Tdunitdisplay.Find(x => x.Status == (int)DbLib.Defs.Status.Ready);
                            if (end == null)
                            {
                                // END表示
                                addrdata.EndDisplay(true);
                                tddps.Light(ref addrdata, false, false, (int)TdLedColor.Claim, addrdata.GetNowDisplay(), true);

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
                                                color = addrdata.GetBlinkButton();
                                                if (color==-1)
                                                {
                                                    tddps.Light(ref addrdata, false, false, (int)TdLedColor.Claim, "", true);
                                                }
                                                else
                                                {
                                                    tddps.Light(ref addrdata, true, true, color, addrdata.GetNowDisplay(), true);
                                                }
                                            }
                                        }
                                    }

                                });

                                // 作業報告書開始
                                distcolor.ReportEnd();
                                distcolorinfo.ReportUpdate(distcolor.ReportShain, distcolor.DistWorkMode);

                                // 終了処理へ
                                DistColorManager.DistUpdate(distcolor);

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
