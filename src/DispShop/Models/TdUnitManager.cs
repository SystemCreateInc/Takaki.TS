using DbLib.Defs;
using ImTools;
using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TdDpsLib.Models;

namespace DispShop.Models
{
    internal class TdUnitManager
    {
        private static string DISPLAY_HAIFUN = "------";
        private static readonly object _tdLock = new object(); // ロック用のオブジェクト

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

                tddps.Light(ref addrdata, bOn, bBlink, color, display, bQuick);

                // endタイマークリア
                if (bOn == true && color != (int)TdLedColor.Claim)
                {
                    if (display != TdAddrBase.END_DISPLAY)
                    {
                        addrdata.EndDisplay(false,color);
                    }
                }
            }

            return isLight;
        }


        // レスポンス
        public static bool TdUnitResponse(TdDpsManager tddps, int stno, int group, int addr, string text)
        {
            Syslog.Info($"TdUnitResponse:: stno[{stno}], group[{group}], addr[{addr}], text[{text}]");

            TdAddrBase ?addrdata;
            tddps.GetTdAddrAll (out addrdata, stno, group, addr);

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
                if (addrdata!=null)
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

        public static bool TdUnitRcv(TdDpsManager tddps, int stno, int group, int addr, int color)
        {
            Syslog.Info($"TdUnitRcv:: stno[{stno}], group[{group}], addr[{addr}], color[{color}]");

            TdAddrData? addrdata;
            tddps.GetTdAddr(out addrdata, stno, group, addr);

            if (addrdata != null)
            {
                // 点灯してない表示は無視
                var led = addrdata.GetLedButton(color);
                if (led == null)
                {
                    Syslog.Info($"TdUnitRcv::color err1{color}");
                    return false;
                }

                // 点灯しているボタンがあれば押下ボタン色を変更
                if (!led.IsLight)
                {
                    int blinkcolor = addrdata.GetBlinkButton();
                    if (blinkcolor != -1)
                    {
                        color = blinkcolor;
                        led = addrdata.GetLedButton(color);
                        if (led == null)
                        {
                            Syslog.Info($"TdUnitRcv::color err2{color}");
                            return false;
                        }
                    }
                    else
                    {
                        Syslog.Info($"TdUnitRcv::color err3{color}");
                        return false;
                    }
                }

                if (color==(int)TdLedColor.Green)
                {
                    // ハイフン表示に切り替える
                    string text = "";
                    var oldled = addrdata.GetLedButton(color);
                    if (oldled != null)
                    {
                        text = oldled.Text!;
                    }
                    tddps.Light(ref addrdata, false, false, color, text, true);
                    tddps.Light(ref addrdata, true, false, (int)TdLedColor.Red, DISPLAY_HAIFUN, true);
                }
                else
                {
                    if (color == (int)TdLedColor.Red)
                    {
                        // 数量反転
                        var oldled = addrdata.GetLedButton(color);
                        if (oldled != null)
                        {
                            if (oldled.Text! == DISPLAY_HAIFUN)
                            {
                                var greenled = addrdata.GetLedButton((int)TdLedColor.Green);
                                // 数量表示
                                tddps.Light(ref addrdata, true, false, color, greenled?.Text!, true);
                            }
                            else
                            {
                                // ハイフン表示
                                tddps.Light(ref addrdata, true, false, color, DISPLAY_HAIFUN, true);
                            }
                        }
                    }
                }
            }
            else
                {
                Syslog.Info($"TdUnitRcv::存在しない表示器 stno[{stno}], group[{group}], addr[{addr}]");
            }
            return true;
        }

        public static void TdUnitOff(string tdunitaddrcode, TdDpsManager tddps)
        {
            TdAddrData? addrdata;
            tddps.GetTdAddr(out addrdata, tdunitaddrcode);

            // 表示器の現在の状態取得
            if (addrdata != null)
            {
                for (int color = (int)TdLedColor.Red; color <= (int)TdLedColor.Claim; color++)
                {
                    if (addrdata.IsLedButtonLight(color))
                    {
                        tddps.Light(ref addrdata, false, false, color, "", false);
                    }
                }
            }
        }

    }
}
