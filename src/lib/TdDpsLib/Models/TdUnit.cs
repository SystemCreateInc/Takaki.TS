using LogLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using TdDpsLib.Defs;
using static System.Net.Mime.MediaTypeNames;


namespace TdDpsLib.Models
{
    // 表示器キュー
    public class QueCommand
    {
        static public int QueSeqno=0;

        public QueCommand(ref TdAddrData addrdata, bool on, bool blink, int color, string text, bool priority = false)
        {
            Addrdata = addrdata;
            On = on;
            Blink = blink;
            Color = color;

            // 表示桁数に変換
            Text = string.Format("{0,9}", text).Substring(9 - addrdata.TextLen, addrdata.TextLen);
            Priority = priority;

            On_Base = On;
            Blink_Base = Blink;
            Color_Base = Color;
            Text_Base = Text;
            QueSeq = QueSeqno++;
        }

        public QueCommand(QueCommand? que)
        {
            if (que != null)
            {
                Addrdata = que.Addrdata;
                On = que.On;
                Blink = que.Blink;
                Color = que.Color;
                Text = que.Text;
                Priority = que.Priority;

                On_Base = que.On_Base;
                Blink_Base = que.Blink_Base;
                Color_Base = que.Color_Base;
                Text_Base = que.Text_Base;

                QueSeq = que.QueSeq;
            }
        }

        // 表示器アドレス
        public TdAddrData Addrdata = new TdAddrData();
        // 点灯
        public bool On = false;
        // 点滅
        public bool Blink = false;
        // 色
        public int Color = 0;
        // 表示内容
        public string Text = "";
        // 優先度
        public bool Priority = false;

        public bool On_Base = false;
        // 点滅
        public bool Blink_Base = false;
        // 色
        public int Color_Base = 0;
        // 表示内容
        public string Text_Base = "";
        public int QueSeq = 0;
    }


    //
    // 表示器ドライバー操作クラス
    //
    public class TdUnit
    {
        public delegate void TdLogEventHandler(int stno, string log);
        public delegate void TdButtonPushEventHandler(int stno, int group, int addr, int button);
        public delegate void TdResponseEventHandler(int stno, int group, int addr, string data);
        public delegate void TdPortEventHandler(int stno, TdPortStatus status, string data);

        public Td.Dps.IDps? TdController = null;

        // ポート情報
        public TdPortData? TdPort = null;

        // オープン有無
        public bool IsOpend { get => TdController != null; }

        // WakeUp起動有無
        public bool IsWakeChecked = false;
        // バッテリーチェック有無
        public bool IsBatteryChecked = false;

        // メイン側で処理する場合に使用
        public event TdLogEventHandler? TdLogEvent = null;
        public event TdButtonPushEventHandler? TdButtonPushEvent = null;
        public event TdResponseEventHandler? TdResponseEvent = null;
        public event TdPortEventHandler? TdPortEvent = null;

        // キュー関係
        public EventfulConcurrentQueue<QueCommand> TdCmdQue = new EventfulConcurrentQueue<QueCommand>();
        public CancellationTokenSource? QueCancelSrc = new CancellationTokenSource();
        public bool bQueWorking = false;

        public const int DelaySec = 10;


        // ポートオープン
        public bool Open()
        {
            if (TdPort == null)
            {
                throw new Exception($"ポート情報未設定");
            }

            Syslog.Info($"TdUnit:Open ポート:{TdPort.TdPortCom} ボーレート:{TdPort.Baudrate} ポートCd:{TdPort.TdUnitPortCode}");

            TdPort.PortStatus = TdPortStatus.Ready;
            TdPortEvent?.Invoke(TdPort.Stno, TdPort.PortStatus, "");

            TdController = null;
            QueCancelSrc = null;
            QueCancelSrc = new CancellationTokenSource();

            switch (TdPort.TdUnitPortType)
            {
                // 有線
                case TdControllerType.Wired:
                    TdController = new Td.Dps.Wired();
                    IsWakeChecked = false;
                    IsBatteryChecked = false;
                    break;

                // 有線LAN
                case TdControllerType.Wired_Lan:
                    TdController = new Td.Dps.Wired2();
                    IsWakeChecked = false;
                    IsBatteryChecked = false;
                    break;

                // 無線
                case TdControllerType.Wireless:
                    TdController = new Td.Dps.Wireless();
                    IsWakeChecked = true;
                    IsBatteryChecked = true;
                    break;

                // 無線2
                case TdControllerType.Wireless_Nec:
                    TdController = new Td.Dps.Wireless2();
                    IsWakeChecked = true;
                    IsBatteryChecked = true;
                    break;

                default:
                    throw new Exception($"表示器コントローラタイプ不明{TdPort.TdUnitPortType}");
            }


            //if (TdController.Open(TdPort.TdPortCom,9600) != 0)
            if (TdController.Open(TdPort.TdPortCom) != 0)
            {
                TdController = null;
                TdPort.PortStatus = TdPortStatus.Error;
                TdPortEvent?.Invoke(TdPort.Stno, TdPort.PortStatus, "");
                return false;
            }

            TdPort.PortStatus = TdPortStatus.Connect;
            TdPortEvent?.Invoke(TdPort.Stno, TdPort.PortStatus, "");

            TdController.Response += TdResponse;
            TdController.ButtonPush += TdButtonPush;
            TdController.Log += TdLog;

            return true;
        }
        public void Close()
        {
            Syslog.Info($"TdUnit::Close");

            if (TdController == null)
                return;

            Syslog.Info($"TdUnit::QueCancel");

            QueCancel();

            Syslog.Info($"EventHandle Clear");
            TdController.Response -= TdResponse;
            TdController.ButtonPush -= TdButtonPush;
            TdController.Log -= TdLog;

            Syslog.Info($"TTdUnit::Controller Close");
            TdController.Close();
            if (TdPort != null)
            {
//                TdPort.PortStatus = TdPortStatus.DisConnect;
//                TdPortEvent?.Invoke(TdPort.Stno, TdPort.PortStatus, "");
            }

            Syslog.Info($"TdUnit TdController Null");
            TdController = null;
            Syslog.Info($"TdUnit Close Exit");
        }

        // ワイヤレス表示器のWakeup
        public bool WakeUp()
        {
            if (TdController == null)
                return false;

            if (IsWakeChecked == false)
                return false;

            if (TdPort == null)
                return false;

            Syslog.Info($"TdUnit:WakeUp ポート:{TdPort.TdPortCom} Stno:{TdPort.Stno}");

            if (TdPort.PortStatus != TdPortStatus.Connect)
            {
                Syslog.Info($"TdUnit:WakeUp StatusErr:{TdPort.PortStatus}");
                TdResponseEvent?.Invoke(TdPort.Stno, 0, 0, "");
                return false;
            }

            if (TdController.TypeDriver == (int)TdControllerType.Wireless)
            {
                ((Td.Dps.Wireless)TdController).WakeUp(TdPort.Stno);
            }
            else if (TdController.TypeDriver == (int)TdControllerType.Wireless_Nec)
            {
                ((Td.Dps.Wireless2)TdController).WakeUp(TdPort.Stno);
            }

            return true;
        }

        public bool BatteryCheck(List<TdAddrData> tdunitaddrs)
        {
            if (TdController == null)
                return false;

            if (IsBatteryChecked == false)
                return false;

            if (TdPort==null)
                return false;

            Syslog.Info($"TdUnit:BatteryCheck ポート:{TdPort.TdPortCom}");

            if (TdPort.PortStatus != TdPortStatus.Connect)
            {
                Syslog.Info($"TdUnit:BatteryCheck StatusErr:{TdPort.PortStatus}");
                return false;
            }


            foreach (var p in tdunitaddrs)
            {
                if (TdController.TypeDriver == (int)TdControllerType.Wireless)
                {
                    ((Td.Dps.Wireless)TdController).BatteryCheck(p.TddGroup, p.TddAddr);
                }
                else if (TdController.TypeDriver == (int)TdControllerType.Wireless_Nec)
                {
                    ((Td.Dps.Wireless2)TdController).BatteryCheck(p.TddGroup, p.TddAddr);
                }
            }
            return true;
        }

        public bool Sleep()
        {
            if (TdController == null)
                return false;

            if (IsWakeChecked == false)
                return false;

            if (TdPort == null)
                return false;

            Syslog.Info($"TdUnit:Sleep ポート:{TdPort.TdPortCom} Stno:{TdPort.Stno}");

            if (TdPort.PortStatus != TdPortStatus.Connect)
            {
                Syslog.Info($"TdUnit:Sleep StatusErr:{TdPort.PortStatus}");
                TdResponseEvent?.Invoke(TdPort.Stno, 0, 0, "");
                return false;
            }

            if (TdController.TypeDriver == (int)TdControllerType.Wireless_Nec)
            {
                ((Td.Dps.Wireless2)TdController).Sleep(TdPort.Stno);
            }

            return true;
        }

        public void TdResponse(int group, int addr, string text)
        {
            if (TdPort == null)
                return;

            int stno = TdPort.Stno;
            Syslog.Info($"TdResponse Stno:{stno} group:{group} addr:{addr} text:{text}");
#if false
            // 接続に戻す
            if (TdPort.PortStatus != TdPortStatus.Connect)
            {
                TdPort.PortStatus = TdPortStatus.Connect;
                TdPortEvent?.Invoke(TdPort.Stno, TdPort.PortStatus, "");
            }
#endif
            TdResponseEvent?.Invoke(stno, group, addr, text);
            return;
        }

        public void TdButtonPush(int group, int addr, int button)
        {
            if (TdPort == null)
                return;

            int stno = TdPort.Stno;
            int physics = TdPort.TdUnitPortType == 0
                ? (group * 100) + addr : (group * 1000) + addr;

            Syslog.Info($"TdButtonPush Stno:{stno} group:{group} addr:{addr} 色:{button} 物理:{physics}");

            TdButtonPushEvent?.Invoke(stno, group, addr, button);
        }
        public void TdLog(string log)
        {
            if (TdPort == null)
                return;

            if (log.StartsWith("R:ERR"))
            {
                log = log.Replace("R", "E");                                // RをEに置き換え(赤文字にする)
            }

            // Errorの文字が含まれる場合も赤文字に
            if (log.Contains("Error"))
            {
                log = "E:" + log;

                // エラーステータス設定
                TdPort.PortStatus = TdPortStatus.Error;
                TdPortEvent?.Invoke(TdPort.Stno, TdPort.PortStatus, log);
            }

            string stnomsg = " [Stno:" + TdPort.Stno.ToString() + "] ";
            string logmsg = stnomsg + log;

            Syslog.Info($"TdLog:{logmsg}");

            TdLogEvent?.Invoke(TdPort.Stno, logmsg);
        }

        // キュー挿入
        public void QueEnqueued(object? sender, EventArgs e)
        {
            if (sender == null)
                return;

            if (bQueWorking)
                return;

            if (QueCancelSrc == null)
                return;

            try
            {
                bQueWorking = true;

                var t = Task.Factory.StartNew(() =>
                {
                    while (TdCmdQue.Count != 0)
                    {
                        // キャンセルか？
                        if (QueCancelSrc.IsCancellationRequested)
                            return;

                        QueCommand ?que;
                        TdCmdQue.TryPeek(out que);

                        if (que != null)
                        {
                            if (TdController != null)
                            {
                                // 点灯へ
                                if (TdController.WaitTransCount() == 0)
                                {
                                    // 点灯へ
                                    TdCmdQue.TryDequeue(out que);
                                }
                                else
                                {
                                    // ちょっと待つ
                                    Task.Delay(DelaySec);
                                }
                            }
                        }
                    }

                }, QueCancelSrc.Token);

                t.Wait();

            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
            }

            bQueWorking = false;

            return;
        }

        // キュー完了
        public void QueDequeued(object? sender, ItemEventArgs<QueCommand> e)
        {
            if (sender == null)
                return;

            if (e == null || e.Item == null)
                return;

            if (TdController == null)
                return;

            if (QueCancelSrc!=null && QueCancelSrc.IsCancellationRequested)
                return;

            QueCommand que = e.Item;
            que.Priority = true; // 優先度はキューで設定するので強制でtrueを設定

            int rc = 0;
            if (que.On == true)
            {
                if (que.Blink == true)
                {
                    rc = TdController.DisplayBlink(que.Addrdata.TddGroup, que.Addrdata.TddAddr, que.Color, que.Text, que.Priority);
                }
                else
                {
                    rc = TdController.DisplayOn(que.Addrdata.TddGroup, que.Addrdata.TddAddr, que.Color, que.Text, que.Priority);
                }
            }
            else
            {
                rc = TdController.DisplayOff(que.Addrdata.TddGroup, que.Addrdata.TddAddr, que.Color, que.Text, que.Priority);
            }

            // 表示器情報設定
            if (rc == 0)
            {
                // 先に内部ステータスを更新する
                que.Addrdata.GetLedButton(que.Color)?.Set(que.On_Base, que.Blink_Base, que.Text_Base);
                if (que.Addrdata.QueSeq < que.QueSeq)
                {
                    que.Addrdata.nowDisplay = que.Text_Base;
                    Syslog.Info($"点灯1:color:{que.Color} addr=[{que.Addrdata.TdUnitAddrCode}] nowtext[{que.Addrdata.nowDisplay}] [{que.Text_Base}]  [{que.Text}]");
                }
                else
                {
                    Syslog.Info($"点灯2:color:{que.Color} addr=[{que.Addrdata.TdUnitAddrCode}] nowtext[{que.Addrdata.nowDisplay}] [{que.Text_Base}]  [{que.Text}]");
                }
                que.Addrdata.QueSeq = que.QueSeq;
#if true
                // バッファーがなくなるまで待つ（無限ループする？？）
                while (TdController.WaitTransCount() != 0)
                {
                    if (QueCancelSrc == null)
                        break;

                    if (QueCancelSrc.IsCancellationRequested)
                        break;

                    Task.Delay(DelaySec);
                }
#endif
            }
            return;
        }

        // 表示器処理中断
        public void QueCancel()
        {
            if (QueCancelSrc!=null)
                QueCancelSrc.Cancel();

            TdCmdQue.Clear();
        }
    }
}
