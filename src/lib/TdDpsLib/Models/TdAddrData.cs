using ImTools;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Windows.Navigation;
using TdDpsLib.Defs;

namespace TdDpsLib.Models
{
    public enum DISPTYPE
    {
        DISPNO,
        ADDR,
        ALL8,
    }

    /// <summary>
    /// 表示器LED色
    /// </summary>
    public enum TdLedColor
    {
        Red = 1,    // 赤
        Yellow,     // 黄
        Green,      // 緑
        White,      // 白
        Blue,       // 青
        Claim,      // クレーム
        BigLed,     // 大きいLED
        Sensor,     // センサー
        LedMax,     // 最終
    }

    // 表示器アドレス基本クラス
    public class TdAddrBase : BindableBase
    {
        public const string END_DISPLAY = "   END";

        public string TdUnitCode { get; set; } = "";               // 表示器CD(表示器ユニークキー)
        public string TdUnitAddrCode { get; set; } = "";           // 論理アドレス
        public int TddGroup { get; set; } = 0;              // 分岐アドレス
        public int TddAddr { get; set; } = 0;               // 物理アドレス
        public int TddButton { get; set; } = 0;            // ボタン数
        public int TdUnitPortCode { get; set; } = 0;            // ポートCD
        public string TdPortCom { get; set; } = "";          // ポートCOM
        public int Stno { get; set; } = 0;               // 有線分岐No
        public int TextLen { get; set; } = 6;           // 表示桁数
        public string Type { get; set; } = "";            // 表示ﾀｲﾌﾟ 表示器 スタートBOX　配順表示器等

        public int? TdUnitAreaCode { get; set; } = null;           // エリア表示灯CD
        public string TdUnitAreaName { get; set; } = "";            // エリア表示灯名称
        public int TdUsageid { get; set; }
        public int TdUnitSeq { get; set; }

        public string Physics                       // 分岐-物理
        {
            get
            {
                string str;
                if (TdUnitPortType == TdControllerType.Wired)
                {
                    str = string.Format($"{TddGroup:00}{TddAddr:00}");
                }
                else
                {
                    str = string.Format($"{TddGroup:00}{TddAddr:000}");
                }
                return str;
            }
        }

        // ステーションNo+物理アドレス
        public string StnoPhysics                       // Stno+分岐-物理
        {
            get
            {
                string str;
                if (TdUnitPortType == TdControllerType.Wired)
                {
                    str = string.Format($"{Stno:00}-{TddGroup:00}{TddAddr:00}");
                }
                else
                {
                    str = string.Format($"{Stno:00}-{TddGroup:00}{TddAddr:000}");
                }
                return str;
            }
        }

        public TdControllerType TdUnitPortType { get; set; } = TdControllerType.None;

        // 表示器状態
        public TdDisplayUnitStatus DStatus { get; set; } = TdDisplayUnitStatus.Ready;

        // バッテリー状態
        public TdDisplayUnitStatus BatteryInfo { get; set; } = TdDisplayUnitStatus.Ready;


        //
        //　表示器ドライバのレスポンスTEXTに対応した表示器状態設定
        //
        public TdDisplayUnitStatus TdResponseToUpdateStatus(string text)
        {
            switch (text)
            {
                case "TIM":
                    DStatus = TdDisplayUnitStatus.Error;
                    break;

                case "NAK":
                    DStatus = TdDisplayUnitStatus.Nak;
                    break;
                case "OFF":
                    DStatus = TdDisplayUnitStatus.Off;
                    break;

                case "HIG":
                    BatteryInfo = TdDisplayUnitStatus.High;
                    break;
                case "MID":
                    BatteryInfo = TdDisplayUnitStatus.Mid;
                    break;
                case "LOW":
                    BatteryInfo = TdDisplayUnitStatus.Low;
                    break;

                case "ACK":
                    DStatus = TdDisplayUnitStatus.Ack;
                    break;

                case "RCN":
                    DStatus = TdDisplayUnitStatus.Rcn;
                    break;

                default:
                    break;
            }
            return DStatus;
        }
    };

    // 表示器ボタンクラス
    public class TdLedButton : BindableBase
    {
        // 色
        public TdLedColor LedColor { get; set; } = TdLedColor.Red;
        // 表示内容
        public string? Text { get; set; } = "";
        // 点灯状態
        public bool IsLight { get; set; } = false;
        public bool IsBlink { get; set; } = false;
        // 点灯日時
        public DateTime? LightTime { get; set; } = null;

        public void Set(bool bOn, bool bBlink, string text)
        {
            IsLight = bOn;
            IsBlink = bBlink;
            Text = text;
            LightTime = DateTime.Now;
        }
        public void SetButtonOnOff(bool bOn, bool bBlink)
        {
            IsLight = bOn;
            IsBlink = bBlink;
            LightTime = DateTime.Now;
        }
        public void SetText(string text)
        {
            Text = text;
        }
    };

    public class TdAddrData : TdAddrBase
    {
        public TdAddrData()
        {
            for (int i = (int)TdLedColor.Red; i < (int)TdLedColor.LedMax; i++)
            {
                LedColors.Add(new TdLedButton
                {
                    LedColor = (TdLedColor)i,
                    Text = "",
                    IsLight = false,
                    IsBlink = false,
                    LightTime = null,
                });
            }
        }

        public List<TdLedButton> LedColors = new List<TdLedButton>();

        // 表示器状態

        public TdLedButton? GetLedButton(int color)
        {
            return LedColors.Find(x => (int)x.LedColor == color);
        }

        // ボタン点灯有無
        public bool IsLedButtonLight(int color)
        {
            return (LedColors.Find(x => (int)x.LedColor == color)?.IsLight ?? false);
        }
        // 該当ボタン以外で点灯あり？
        public bool IsLedButtonOtherLight(int color)
        {
            return LedColors.Find(x => x.IsLight == true && (int)x.LedColor != color) == null ? false : true;
        }
        public int GetBlinkButton()
        {
            // 点灯は使用しないので点灯を参照するように変更
//          int color = LedColors.FindIndex(x => x.IsBlink == true);
            int color = LedColors.FindIndex(x => x.IsLight == true);
            return color == -1 ? -1 : color + 1;
        }
        public string GetNowDisplay()
        {
            string display="";
            int blinkcolor = GetBlinkButton();
            if (blinkcolor != -1)
            {
                TdLedButton? led = GetLedButton(blinkcolor);
                if (led!=null)
                {
                    display = led.Text==null ? "" : led.Text;
                }
            }
            if (EndDispTime.IsRunning)
            {
                display = END_DISPLAY;
            }
            return display;
        }
        public void EndDisplay(bool bOn)
        {
            if (bOn)
            {
                EndDispTime.Reset();
                EndDispTime.Start();
            }
            else
            {
                EndDispTime.Stop();
            }
        }
        public bool IsEndTimeOver()
        {
            if (EndDispTime.IsRunning)
            {
                if (EndDispTime.ElapsedMilliseconds >= 3000)
                {
                    EndDispTime.Reset();
                    return true;
                }
            }

            return false;
        }
        public bool IsZoneBtn()
        {
            return TdUsageid == 4 ? true : false;
        }

        public string nowDisplay = "";
        public System.Diagnostics.Stopwatch EndDispTime = new System.Diagnostics.Stopwatch();
    }

    // エリア表示灯制御
    public class TdAreaData : TdAddrBase
    {
        public List<string> RangeTdUnitCode = new List<string>();
        public List<int> LedCount = new List<int>();
        private static object Lock = new object();

        public TdAreaData()
        {
            for (int i = (int)TdLedColor.Red; i < (int)TdLedColor.LedMax; i++)
            {
                LedCount.Add(0);
            }
        }

        //
        // // 論理アドレスチェック
        //
        public bool AreaCheck(string tdunitcode)
        {
            return RangeTdUnitCode.Find(x => x == tdunitcode) !=null ? true : false;
        }

        //
        // 点灯カウント
        //
        public bool CountUpDown(TdAddrData addrdata, bool on, int color)
        {
            bool result = false;

            lock (Lock)
            {
                LedCount[color] += on ? 1 : -1;

                if (on)
                {
                    result = LedCount[color] == 1 ? true : false;
                }
                else
                {
                    result = LedCount[color] == 0 ? true : false;
                }
            }
            return result;
        }
    }
}
