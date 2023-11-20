using DbLib.Defs;
using FastExpressionCompiler.LightExpression;
using ImTools;
using LogLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using TdDpsLib.Defs;

namespace TdDpsLib.Models
{
    // 表示器操作クラス
    public class TdDpsManager
    {
        public List<TdUnit> TdUnits = new List<TdUnit>();

        private List<TdPortData> _tdunitports = new List<TdPortData>();
        public List<TdPortData>? TdPorts { get; set; } = null;
        private List<TdAddrData> _tdunitaddraddrs = new List<TdAddrData>();
        public List<TdAddrData>? TdAddrs { get; set; } = null;
        private List<TdAreaData> _tdunitareaaddrs = new List<TdAreaData>();
        public List<TdAreaData>? TdAreas { get; set; } = null;
        public List<TdAreaData>? TdZoneBtns { get; set; } = null;
        private bool _bIdle { get; set; } = false;
        public int Tdunittype { get; set; } = (int)TdUnitType.TdRack;

        public TdDpsManager()
        {
            // 表示器レスポンスイベント登録
            SetTdResponseEvent(
                (stno, group, addr, text) =>
                {
                    TdAddrData? addrdata;
                    addrdata = TdAddrs != null ? TdAddrs.Find(p => p.TddGroup == group && p.TddAddr == addr && p.Stno == stno) : null; ;

                    if (addr == 999)
                    {
                        // 接続失敗
                        TdPortData? port = TdPorts != null ? TdPorts.Where(x => x.Stno == stno).FirstOrDefault() : null;
                        if (port != null)
                        {
                            port.PortStatus = TdPortStatus.Error;
                        }
                    }


                    if (addrdata != null)
                    {
                        // 状態設定
                        addrdata.TdResponseToUpdateStatus(text);
                    }

                    return;
                });
        }

        public bool Init()
        {
            Syslog.Info(@"TdDriverManager:Init");
            try
            {
                // 天吊,棚取得
                Tdunittype = TdLoader.LoadTdUnitType();


                TdPorts = TdLoader.LoadTdComs();
                TdAddrs = TdLoader.LoadTdAddrs(Tdunittype);
                TdAreas = TdLoader.LoadTdAreas(TdAddrs);

                if (TdPorts != null && TdAddrs != null)
                {
                    Syslog.Info($"TdDriverManager:Init ポート数:{TdPorts.Count} 表示器数:{TdAddrs.Count}");
                }
                else
                {
                    Syslog.Info($"TdDriverManager:Init失敗");
                }
            }
            catch (Exception)
            {
                throw;
            }

            return true;
        }
        public void Tarm()
        {
            Syslog.Info(@"TdDriverManager:Tarm");

            Close();

            TdPorts?.Clear();
            TdAddrs?.Clear();
            TdAreas?.Clear();

        }


        public bool Open()
        {
            if (TdAddrs == null)
            {
                throw new Exception($"表示器アドレスが未設定です。表示器を登録して下さい。");
            }
            if (TdPorts == null)
            {
                throw new Exception($"表示器ポートが、未設定です。表示器ポートを登録して下さい。");
            }

            if (TdAddrs.Count == 0)
            {
                throw new Exception($"表示器が未設定です。表示器マスタで表示器を登録して下さい。");
            }

            // DPSドライバクリア
            TdUnits.Clear();
            for (int i = 0; i < TdPorts.Count; i++)
            {
                TdUnit unit = new TdUnit();
                // キュークリア
                unit.TdCmdQue.Clear();
                unit.TdCmdQue.ItemEnqueued += unit.QueEnqueued;
                unit.TdCmdQue.ItemDequeued += unit.QueDequeued;
                unit.TdPort = TdPorts[i];

                TdUnits.Add(unit);
                if (unit.Open() == false)
                {
                    throw new Exception($"表示器の初期化に失敗しました\n\nポート:{TdPorts[i].TdPortCom} ポートCD:{TdPorts[i].TdUnitPortCode}");
                }
            }

            if (TdUnits.Count == 0)
            {
                throw new Exception($"表示器のCOMポート未設定です。ポートマスタでポートを登録して下さい。");
            }

            return true;
        }
        public void Close()
        {
            foreach (var unit in TdUnits)
            {
                if (unit.TdPort != null)
                {
                    Syslog.Info($"TdDriverManager:Close ポート:{unit.TdPort.TdPortCom} ポートCD:{unit.TdPort.TdUnitPortCode}");
                }

                unit.TdCmdQue.ItemEnqueued -= unit.QueEnqueued;
                unit.TdCmdQue.ItemDequeued -= unit.QueDequeued;
                unit.Close();
            }
            TdUnits.Clear();
            Syslog.Info($"TdDispManager Close All End");
        }

        // 表示器ログイベント一括設定
        public bool SetTdLogEvent(TdUnit.TdLogEventHandler logevent)
        {
            foreach (var p in TdUnits)
            {
                p.TdLogEvent += logevent;
            }
            return true;
        }
        // 表示器押下イベント一括設定
        public bool SetTdButtonPushEvent(TdUnit.TdButtonPushEventHandler btnevent)
        {
            foreach (var p in TdUnits)
            {
                p.TdButtonPushEvent += btnevent;
            }
            return true;
        }
        // 表示器レスポンスイベント一括設定
        public bool SetTdResponseEvent(TdUnit.TdResponseEventHandler revent)
        {
            foreach (var p in TdUnits)
            {
                p.TdResponseEvent += revent;
            }
            return true;
        }
        // 接続ポートレスポンスイベント一括設定
        public bool SetTdPortEvent(TdUnit.TdPortEventHandler revent)
        {
            foreach (var p in TdUnits)
            {
                p.TdPortEvent += revent;
            }
            return true;
        }

        public bool GetTdAddr(out TdAddrData? addrdata, string tdunitaddrcode)
        {
            addrdata = null;
            addrdata = TdAddrs?.Find(p => p.TdUnitAddrCode == tdunitaddrcode);

            if (addrdata == null)
            {
                Syslog.Info($"GetTdAddr 取得エラー  tdunitaddrcode:{tdunitaddrcode}");
            }

            return addrdata == null ? false : true;
        }

        public bool GetTdAddr(out TdAddrData? addrdata, int stno, int group, int addr)
        {
            addrdata = null;
            addrdata = TdAddrs?.Find(p => p.TddGroup == group && p.TddAddr == addr && p.Stno == stno);

            if (addrdata == null)
            {
                Syslog.Info($"GetTdAddr 取得エラー  Stno:{stno} Group:{group} Addr:{addr}");
            }

            return addrdata == null ? false : true;
        }

        public bool GetTdAddrAll(out TdAddrBase? addrdata, int stno, int group, int addr)
        {
            addrdata = null;
            addrdata = TdAddrs?.Find(p => p.TddGroup == group && p.TddAddr == addr && p.Stno == stno);

            if (addrdata == null)
            {
                addrdata = TdAreas?.Find(p => p.TddGroup == group && p.TddAddr == addr && p.Stno == stno);
                if (addrdata == null)
                {
                    Syslog.Info($"GetTdAddrAll 取得エラー  Stno:{stno} Group:{group} Addr:{addr}");
                }
            }

            return addrdata == null ? false : true;
        }


        // 表示器点灯 or 消灯
        public bool Light(ref TdAddrData tdunitaddr, bool on, bool blink, int color, string text, bool priority = false)
        {
            int stno = tdunitaddr.Stno;
            var tdunit = TdUnits.Find(x => x.TdPort != null && x.TdPort.Stno == stno);
            if (tdunit == null)
            {
                Syslog.Info($"Light 表示器ドライバ取得失敗  Stno:{tdunitaddr.Stno} Group:{tdunitaddr.TddGroup} Addr:{tdunitaddr.TddAddr}");
                return false;
            }

            if (tdunit.IsOpend == false)
            {
                Syslog.Info($"Light 表示器未オープン状態のため失敗  Stno:{tdunitaddr.Stno} Group:{tdunitaddr.TddGroup} Addr:{tdunitaddr.TddAddr}");
                return false;
            }

            bool oldLightbutton = tdunitaddr.IsLedButtonLight(color);

            // 状態更新
            tdunitaddr.GetLedButton(color)?.Set(on, blink, text);

            // キューへ設定
            tdunit.TdCmdQue.Enqueue(new QueCommand(ref tdunitaddr, on, blink, color, text, priority));

            // エリア表示灯制御
            if (oldLightbutton != tdunitaddr.IsLedButtonLight(color))
            {
                if (TdAreas != null)
                {
                    string tdunitcode = tdunitaddr.TdUnitCode;
                    var areas = TdAreas.Where(x => x.AreaCheck(tdunitcode) == true);

                    if (areas != null)
                    {
                        foreach (var area in areas)
                        {
                            if (area.CountUpDown(tdunitaddr, on, color) == true)
                            {
                                // キューへ設定
                                TdAddrData addr = new TdAddrData
                                {
                                    TdUnitCode = area.TdUnitCode,
                                    Stno = area.Stno,
                                    TddGroup = area.TddGroup,
                                    TddAddr = area.TddAddr,
                                };

                                // 該当Stno検索
                                tdunit = TdUnits.Find(x => x.TdPort != null && x.TdPort.Stno == area.Stno);
                                if (tdunit == null)
                                {
                                    Syslog.Info($"LightArea 表示器ドライバ取得失敗  Stno:{area.Stno} group:{area.TddGroup} addr:{area.TddAddr}");
                                }
                                else
                                {
                                    tdunit.TdCmdQue.Enqueue(new QueCommand(ref addr, on, false, color, "", priority));
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        // 点灯待機数カウント
        public int WaitTransCount()
        {
            int cnt = 0;
            TdUnits.ForEach(x => cnt += (x.TdController?.WaitTransCount() ?? 0) + x.TdCmdQue.Count);
            return cnt;
        }


        public bool IsWakeUp()
        {
            // WakeUp,バッテリーチェックなし?
            return TdUnits.Find(x => x.IsWakeChecked == true) == null ? false : true;
        }
        public bool IsBatteryCheck()
        {
            // WakeUp,バッテリーチェックなし?
            return TdUnits.Find(x => x.IsBatteryChecked == true) == null ? false : true;
        }


        // WakeUp開始
        public void StartWakeUp(CancellationTokenSource ct)
        {
            // WakeUp,バッテリーチェックなし?
            if (IsWakeUp())
            {
                foreach (var unit in TdUnits)
                {
                    // キャンセルされた？
                    if (ct == null || (ct != null && ct.IsCancellationRequested))
                    {
                        break;
                    }

                    if (unit.IsWakeChecked)
                    {
                        unit.WakeUp();
                    }
                }
            }
        }
        public void StartSleep(CancellationTokenSource ct)
        {
            // WakeUp,バッテリーチェックなし?
            if (IsWakeUp())
            {
                foreach (var unit in TdUnits)
                {
                    // キャンセルされた？
                    if (ct == null || (ct != null && ct.IsCancellationRequested))
                    {
                        break;
                    }

                    if (unit.IsWakeChecked)
                    {
                        unit.Sleep();
                    }
                }
            }
        }
        // 初期化 WakeUp,バッテリーチェックをする
        public void StartBatteryCheck(CancellationTokenSource ct)
        {
            if (IsBatteryCheck())
            {
                foreach (var unit in TdUnits)
                {
                    // キャンセルされた？
                    if (ct == null || (ct != null && ct.IsCancellationRequested))
                    {
                        break;
                    }

                    if (unit.TdPort != null)
                    {
                        var tdunitaddrs = TdAddrs == null ? null : TdAddrs.Where(x => x.Stno == unit.TdPort.Stno).ToList();

                        if (tdunitaddrs != null)
                        {
                            if (unit.IsBatteryChecked)
                            {
                                unit.BatteryCheck(tdunitaddrs);
                            }
                        }
                    }
                }
            }
        }
        public void StartIdle()
        {
            _bIdle = true;
        }
        public void EndIdle()
        {
            _bIdle = false;
        }
        public bool IsIdle()
        {
            return _bIdle;
        }
    }
}