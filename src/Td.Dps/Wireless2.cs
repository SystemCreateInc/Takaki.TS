#undef SANQ
#define TIM

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Td.Dps
{
    /// <summary>
    /// ピッキング無線表示器2ドライバ
    /// </summary>
    public class Wireless2 : IDps
    {
        #region 定義
        #region コマンド
        /// <summary>
        /// 送信コマンド：従来式：LED・セグメント
        /// </summary>
        private const string T_TXD1 = "TXD1";

        /// <summary>
        /// 送信コマンド：従来式：LEDのみ
        /// </summary>
        private const string T_TXD2 = "TXD2";

        /// <summary>
        /// 送信コマンド：従来式：セグメントのみ
        /// </summary>
        private const string T_TXD3 = "TXD3";

        /// <summary>
        /// 送信コマンド：自走式パターン1
        /// </summary>
        private const string T_SET1 = "SET1";

        /// <summary>
        /// 送信コマンド：自走式パターン2
        /// </summary>
        private const string T_SET2 = "SET2";

        /// <summary>
        /// 送信コマンド：自走式パターン3
        /// </summary>
        private const string T_SET3 = "SET3";

        /// <summary>
        /// 送信コマンド：自走式：リセット
        /// </summary>
        private const string T_RESET = "REST";

        /// <summary>
        /// 送信コマンド：自走式：リセットLED1
        /// </summary>
        private const string T_RST1 = "RST1";

        /// <summary>
        /// 送信コマンド：自走式：リセットLED2
        /// </summary>
        private const string T_RST2 = "RST2";

        /// <summary>
        /// 送信コマンド：自走式：リセットLED3
        /// </summary>
        private const string T_RST3 = "RST3";

        /// <summary>
        /// 送信コマンド：自走式：リセットLED4
        /// </summary>
        private const string T_RST4 = "RST4";

        /// <summary>
        /// 送信コマンド：自走式：リセットLED5
        /// </summary>
        private const string T_RST5 = "RST5";

        /// <summary>
        /// 送信コマンド：自走式：END表示
        /// </summary>
        private const string T_END = "PEND";

        /// <summary>
        /// 送信コマンド：バッテリ情報要求
        /// </summary>
        private const string T_BATTERY = "LINK";

        /// <summary>
        /// 送信コマンド：ウェークアップ
        /// </summary>
        private const string T_WAKEUP = "WKUP";

        /// <summary>
        /// 送信コマンド：スリープ
        /// </summary>
        private const string T_SLEEP = "SLEP";

        /// <summary>
        /// 送信コマンド：バージョン
        /// </summary>
        private const string T_VER = "VERS";

        /// <summary>
        /// 受信コマンド：表示器応答
        /// </summary>
        private const string R_RES = "RES";

        /// <summary>
        /// 受信コマンド：従来式：ボタン押下
        /// </summary>
        private const string R_RXD = "RXD";

        /// <summary>
        /// 受信コマンド：自走式：ボタン完了
        /// </summary>
        private const string R_RET = "RET";

        /// <summary>
        /// 受信コマンド：自走式：CTLボタン完了
        /// </summary>
        private const string R_CTL = "CTL";

        /// <summary>
        /// 受信コマンド：バージョン
        /// </summary>
        private const string R_VER = "RVE";
        #endregion

        #region 定数
        /// <summary>
        /// 排他アクセス送信
        /// </summary>
        private readonly object lockObj = new object();

        /// <summary>
        /// タイムアウト時間（ミリ秒）
        /// </summary>
        private const int TIMEOUT = 3000;

        /// <summary>
        /// 応答待ちデータのカウント
        /// </summary>
        private const int RES_CNT = 10;

        /// <summary>
        /// 全ボタン
        /// </summary>
        private const int BTN_ALL = 9;

        /// <summary>
        /// 改行コード
        /// </summary>
        private const string NEW_LINE = "\n";

        /// <summary>
        /// 文字列：ACK
        /// </summary>
        private const string STR_ACK = "ACK";

        /// <summary>
        /// 文字列：NAK
        /// </summary>
        private const string STR_NAK = "NAK";

        /// <summary>
        /// 文字列：TIM
        /// </summary>
        private const string STR_TIM = "TIM";
        #endregion

        #region 変数
        /// <summary>
        /// イベント実行処理
        /// </summary>
        private EventWorker worker;

        /// <summary>
        /// シリアルポート
        /// </summary>
        private ComPort comPort;

#if !TIM
        /// <summary>
        /// タイマー
        /// </summary>
        private System.Timers.Timer timer;
#else
        /// <summary>
        /// タイムアウトカウント
        /// </summary>
        private int TimeoutCnt;
#endif

        /// <summary>
        /// ドライバ種別
        /// </summary>
        private int typeDriver;

        /// <summary>
        /// ポート番号
        /// </summary>
        private string portName;

        /// <summary>
        /// 送信中のデータ
        /// </summary>
        private CommandList transData;

        /// <summary>
        /// タイマー
        /// </summary>
        private System.Timers.Timer Dsp_timer;

        /// <summary>
        /// 送信データの格納
        /// </summary>
        private List<CommandList> transList;

        /// <summary>
        /// 受信データの格納
        /// </summary>
        private StringBuilder recData;

        /// <summary>
        /// 応答待ちのIDリスト
        /// </summary>
        private List<string> waitIdList;

        /// <summary>
        /// 受信スレッド
        /// </summary>
        private Thread receiveThread;

        /// <summary>
        /// 送信スレッド
        /// </summary>
        private Thread transThread;

        /// <summary>
        /// 受信スレッドの停止フラグ
        /// </summary>
        private bool receiveStop;

        /// <summary>
        /// 送信スレッドの停止フラグ
        /// </summary>
        private bool transStop;
        
#if SANQ
        /// <summary>
        /// GetLog用StringList
        /// </summary>
        private List<KeyValuePair<DateTime, string>> logList;
#endif
        #endregion

        #region プロパティ
        /// <summary>
        /// ドライバ種別
        /// </summary>
        public int TypeDriver
        {
            get { return this.typeDriver; }
        }

        /// <summary>
        /// ポート番号
        /// </summary>
        public string Port
        {
            get { return this.portName; }
        }
        #endregion

        #region イベント
        /// <summary>
        /// 表示器ボタン押下のイベントハンドラ
        /// </summary>
        public event Event.ButtonPushEventHandler ButtonPush;

        /// <summary>
        /// 表示器ボタン完了のイベントハンドラ
        /// </summary>
        public event Event.ButtonDoneEventHandler ButtonDone;

        /// <summary>
        /// 表示器応答のイベントハンドラ
        /// </summary>
        public event Event.ResponseEventHandler Response;

        /// <summary>
        /// 通信ログ通知のイベントハンドラ
        /// </summary>
        public event Event.LogEventHandler Log;

        /// <summary>
        /// バージョン表示のイベントハンドラ
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public event Event.ResponseEventHandler Version;
        #endregion
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Wireless2()
            : this(false)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="async">イベントの非同期</param>
        public Wireless2(bool async)
        {
            this.worker = new EventWorker(async);
            this.Init();
        }
        #endregion

        #region イベント
#if !TIM
        /// <summary>
        /// 接続エラーのタイマーイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.TimerStop();
            Def.EventInfo info = new Def.EventInfo();

            string id = this.transData.Id;
            this.waitIdList.Remove(id);

            info.Group = Convert.ToInt32(id.Substring(0, 1), 16);
            info.Address = 999;
            if (id.Length == 4)
            {
                info.Address = Convert.ToInt32(id.Substring(1, 3));
            }

            this.transData = null;

            // イベント発生
            info.Data = STR_TIM;
#if SANQ
            this.logList.Add(new KeyValuePair<DateTime, string>(DateTime.Now, "Connection Error"));
#endif
            this.LogEvent("Connection Error");
            this.ResponseEvent(info);
        }
#endif
        #endregion

        #region イベント
        /// <summary>
        /// 接続エラーのタイマーイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dsp_Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // 何も送っていない場合は、何もしない
            if (this.transData == null) {
                return;
            }
            for (int i = 0; i<this.transList.Count; i++) {
				if ( --this.transList[i].Timeout <= 0 ) {
            		Def.EventInfo info = new Def.EventInfo();
            		string id = this.transList[i].Id;
            		this.waitIdList.Remove(id);
            		info.Group = Convert.ToInt32(id.Substring(0, 1), 16);
            		info.Address = 999;
            		if (id.Length == 4) {
                		info.Address = Convert.ToInt32(id.Substring(1, 3));
            		}
                    this.transList.RemoveAt(i);
                    // イベント発生
                    info.Data = STR_TIM;
                    this.LogEvent("Connection Error");
                    this.ResponseEvent(info);
                    break;
				}
			}
        }
        #endregion
        
        
        #region メソッド
        /// <summary>
        /// 初期化
        /// </summary>
        private void Init()
        {
            // プロパティの初期化
            this.typeDriver = 2;
            this.portName = string.Empty;

            // リストの初期化
            this.transList = new List<CommandList>();
            this.recData = new StringBuilder();
            this.waitIdList = new List<string>();
#if SANQ
            this.logList = new List<KeyValuePair<DateTime, string>>();
#endif

            // データのクリア
            this.DataClear();
#if !TIM
            // タイマーの初期化
            this.TimerInit();
#endif
            // タイマーの初期化
            this.Dsp_timer_init();
        }

        /// <summary>
        /// データのクリア
        /// </summary>
        private void DataClear()
        {
            // リストのクリア
            this.transList.Clear();
            this.recData.Length = 0;
            this.waitIdList.Clear();
        }
        #endregion

        #region データの送信・受信
        #region データの送信
        /// <summary>
        /// 送信スレッドの開始
        /// </summary>
        private void TransThreadStart()
        {
            // スレッドが起動中
            if ((this.transThread != null) && (this.transThread.ThreadState == ThreadState.Running))
            {
                return;
            }

            this.transStop = false;                                     // 送信スレッド停止フラグのクリア
            this.transThread = new Thread(this.TransThreadWork);        // 送信スレッドの生成
            this.transThread.Priority = ThreadPriority.BelowNormal;     // 送信スレッドの優先順位を下げる
            this.transThread.Start();                                   // 送信スレッドの開始
            if (this.transThread.IsAlive == false)
            {
                Thread.Sleep(1);                                        // 送信スレッドの実行を待つ
            }
        }

        /// <summary>
        /// 送信スレッドの終了
        /// </summary>
        private void TransThreadTerminate()
        {
            this.transStop = true;                                      // 送信スレッド停止フラグのセット

            // スレッドが起動中
            if ((this.transThread != null) && (this.transThread.ThreadState == ThreadState.Running))
            {
                this.transThread.Abort();                               // 送信スレッドの終了
                this.transThread.Join(300);                             // 送信スレッドが終了するまで待つ
            }
        }

        /// <summary>
        /// 送信スレッド処理
        /// </summary>
        private void TransThreadWork()
        {
            // 止められるまで繰り返す
            while (this.transStop == false)
            {
                lock (lockObj)
                {
                    // 送信したいデータがある かつ ACK/NAK待ちのデータがない
                    if (this.transList.Count > 0 && this.transData == null)
                    {
                        // 送信可能なデータを調べる
                        int index = this.TransListChk();
                        if (index >= 0)
                        {
                            // 送信可能なデータがあれば、送信する
                            this.transData = this.transList[index];
                            this.Write(this.transData);
                            this.transList.RemoveAt(index);
                        }
                    }
                }

                // 指定時間待つ
                Thread.Sleep( ( (this.transList.Count<=0 ) ? Def.TRA_INTERVAL : 1 ));
            }
        }

        /// <summary>
        /// 送信可能なデータのチェック
        /// </summary>
        /// <returns>送信データのインデックス</returns>
        private int TransListChk()
        {
            int index = -1;

            // 応答待ちデータが多い場合は、送信しない
            if (this.waitIdList.Count > RES_CNT)
            {
                return index;
            }

            for (int i = 0; i < this.transList.Count; i++)
            {
                // IDの取得
                string id = this.transList[i].Id;

                // 応答待ちデータがない送信データのインデックスを取得
                if (this.waitIdList.Contains(id) == false)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        /// <summary>
        /// コマンド送信
        /// </summary>
        /// <param name="command"></param>
        private void Write(CommandList command)
        {
            // 送信文字列を取得
            string str = command.Command;

            // BCCの取得
            string bcc = this.BccCalc(str);

            // イベント発生
#if SANQ
            this.logList.Add(new KeyValuePair<DateTime, string>(DateTime.Now, "T:" + str + bcc));
#endif
            this.LogEvent("T:" + str + bcc);

            // BCCと改行コードを付加し送信
            this.comPort.Write(str + bcc + NEW_LINE);
            this.waitIdList.Add(command.Id);

#if !TIM
            // タイマーの開始
            this.TimerStart();
#else
            // タイムアウト値初期化
            this.TimeoutCnt = 0;
#endif
        }

        /// <summary>
        /// コマンド送信（ACK/NAK）
        /// </summary>
        /// <param name="command"></param>
        private void Write(string command)
        {
            // イベント発生
#if SANQ
            this.logList.Add(new KeyValuePair<DateTime, string>(DateTime.Now, "T:" + command));
#endif
            this.LogEvent("T:" + command);

            // 改行コードを付加し送信
            this.comPort.Write(command + NEW_LINE);
        }
        #endregion

        #region データの受信
        /// <summary>
        /// 受信スレッドの開始
        /// </summary>
        private void ReceiveThreadStart()
        {
            // スレッドが起動中
            if ((this.receiveThread != null) && (this.receiveThread.ThreadState == ThreadState.Running))
            {
                return;
            }

            this.receiveStop = false;                                   // 受信スレッド停止フラグのクリア
            this.receiveThread = new Thread(this.ReceiveThreadWork);    // 受信スレッドの生成
            this.receiveThread.Priority = ThreadPriority.AboveNormal;   // 受信スレッドの優先順位を上げる
            this.receiveThread.Start();                                 // 受信スレッドの開始
            while (this.receiveThread.IsAlive == false)
            {
                Thread.Sleep(1);                                        // 受信スレッドの実行を待つ
            }
        }

        /// <summary>
        /// 受信スレッドの終了
        /// </summary>
        private void ReceiveThreadTerminate()
        {
            this.receiveStop = true;                                    // 受信スレッド停止フラグのクリア

            // スレッドが起動中
            if (this.receiveThread.ThreadState == ThreadState.Running)
            {
                this.receiveThread.Abort();                             // 受信スレッドの終了
                this.receiveThread.Join(300);                           // 受信スレッドが終了するまで待つ
            }
        }

        /// <summary>
        /// 受信処理
        /// </summary>
        private void ReceiveThreadWork()
        {
            // 停止されるまで繰り返す
            while (this.receiveStop == false)
            {
                this.recData.Append(Encoding.ASCII.GetString(this.comPort.Read()));

                lock (lockObj)
                {
                    // データがあれば解析する
                    if (this.recData.Length >= 4)
                    {
                        this.DataAnalyze();
                    }
                }

#if TIM
                // タイムアウトを調べる
                this.TimeOutChk();
#endif

                // 指定時間待つ
                Thread.Sleep( (( this.recData.Length <= 0 ) ? Def.REC_INTERVAL : 1));
            }
        }

        /// <summary>
        /// データの解析
        /// </summary>
        private void DataAnalyze()
        {
            string str = string.Empty;
            int index = -1;
            
            try
            {
                // データの区切りを探す
                index = this.recData.ToString().IndexOf(NEW_LINE);

                // 区切りがある場合
                if (index > -1)
                {
                    // データを取り出し、削除
                    str = this.recData.ToString(0, index);
                    this.recData.Remove(0, index + NEW_LINE.Length);
                }

                // 受信データがある場合（ゴミ含）
                if (string.IsNullOrEmpty(str) == false)
                {
                    // イベント発生
#if SANQ
                    this.logList.Add(new KeyValuePair<DateTime, string>(DateTime.Now, "R:" + str));
#endif
                    this.LogEvent("R:" + str);
                }

                // ゴミデータは以降の処理をしない
                if (index < 3)
                {
                    return;
                }

                // BCC異常は以降の処理をしない
                if (this.BccCheck(str) == false)
                {
                    return;
                }

                // コマンドを取得
                string cmd = str.Substring(0, 3);

                switch (cmd)
                {
                    case STR_ACK:
                    case STR_NAK:
                        this.DataAnalyzeACK(cmd);
                        break;

                    case R_RES:
                        this.DataAnalyzeResponse(str);
                        break;

                    case R_RXD:
                        this.DataAnalyzeButtonPush(str);
                        break;

                    case R_RET:
                        this.DataAnalyzeButtonDone(str, false);
                        break;

                    case R_CTL:
                        this.DataAnalyzeButtonDone(str, true);
                        break;

                    case R_VER:
                        this.DataAnalyzeVersion(str);
                        break;

                    default:
                        this.Write(STR_NAK);
                        break;
                }
            }
            catch (Exception ex)
            {
                this.Write(STR_NAK);
#if SANQ
                this.logList.Add(new KeyValuePair<DateTime, string>(DateTime.Now, "Exception:" + ex.ToString()));
#endif
                this.LogEvent("Exception:" + ex.ToString());
            }
        }

        /// <summary>
        /// データ解析(ACK/NAKの応答)
        /// </summary>
        /// <param name="cmd">ACK or NAK</param>
        private void DataAnalyzeACK(string cmd)
        {
            try
            {
                // 何も送っていない場合は、何もしない
                if (this.transData == null)
                {
                    return;
                }

                Def.EventInfo info = new Def.EventInfo();
                string head = this.transData.Command.Substring(0, 4);
                string id = this.transData.Id;
                info.Group = Convert.ToInt32(id.Substring(0, 1), 16);
                info.Address = 999;
                if (id.Length == 4)
                {
                    info.Address = Convert.ToInt32(id.Substring(1, 3));
                }

                // 応答待ちデータの削除
                // WKUP or SLEP の場合のみ。その他は「RES」応答で削除
                if (head == T_WAKEUP || head == T_SLEEP)
                {
                    this.waitIdList.Remove(id);
                }

                // 再送データをセットする
                if (cmd == STR_NAK)
                {
                    this.transList.Insert(0, this.transData);
                    this.waitIdList.Remove(id);
                }

                // 送信中データのクリア
                this.transData = null;

#if !TIM
                // タイマーの停止
                this.TimerStop();
#endif
                // WKUP以外 and SLEP以外 or NAK再送の場合は、イベント無し
                if ((head != T_WAKEUP && head != T_SLEEP) || cmd == STR_NAK)
                {
                    return;
                }

                // イベント発生
                info.Data = cmd;
                this.ResponseEvent(info);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// データ解析(表示器の応答)
        /// </summary>
        /// <param name="data">受信データ</param>
        private void DataAnalyzeResponse(string data)
        {
            Def.EventInfo info = new Def.EventInfo();

            try
            {
                // データ分解
                string id = data.Substring(4, 4);
                info.Data = data.Substring(8, 3);
                info.Group = Convert.ToInt32(data.Substring(4, 1), 16);
                info.Address = Convert.ToInt32(data.Substring(5, 3));

                // 応答待ちデータの削除
                this.waitIdList.Remove(id);

                // イベント発生
                this.ResponseEvent(info);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// データ解析（ボタン押下）
        /// </summary>
        /// <param name="data">受信データ</param>
        private void DataAnalyzeButtonPush(string data)
        {
            Def.EventInfo info = new Def.EventInfo();

            try
            {
                // データ分解
                string id = data.Substring(4, 4);
                info.Button = Convert.ToInt32(data.Substring(3, 1));
                info.Group = Convert.ToInt32(data.Substring(4, 1), 16);
                info.Address = Convert.ToInt32(data.Substring(5, 3));
                info.Data = data;

                // イベント発生
                this.ButtonPushEvent(info);
                // 応答待ちデータの削除
                //this.waitIdList.Remove(id);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// データ解析（ボタン完了）
        /// </summary>
        /// <param name="data">受信データ</param>
        /// <param name="ctl">CTLボタン</param>
        private void DataAnalyzeButtonDone(string data, bool ctl)
        {
            Def.EventInfo info = new Def.EventInfo();

            try
            {
                // データ分解
                info.Button = Convert.ToInt32(data.Substring(3, 1));
                info.Group = Convert.ToInt32(data.Substring(4, 1), 16);
                info.Address = Convert.ToInt32(data.Substring(5, 3));
                info.Ctl = ctl;

                // イベント発生
                this.ButtonDoneEvent(info);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// データ解析（バージョン）
        /// </summary>
        /// <param name="data">受信データ</param>
        private void DataAnalyzeVersion(string data)
        {
            Def.EventInfo info = new Def.EventInfo();

            try
            {
                // データ分解
                string id = data.Substring(4, 4);
                info.Group = Convert.ToInt32(data.Substring(4, 1), 16);
                info.Address = Convert.ToInt32(data.Substring(5, 3));
                info.Data = data.Substring(8, 20);

                // 応答待ちデータの削除
                this.waitIdList.Remove(id);

                // イベント発生
                this.VersionEvent(info);
            }
            catch
            {
                throw;
            }
        }

#if TIM
        /// <summary>
        /// タイムアウト監視
        /// </summary>
        private void TimeOutChk()
        {
            // 何も送っていない場合は、何もしない
            if (this.transData == null)
            {
                return;
            }

            // タイムアウト値を超えた
            if (this.TimeoutCnt > TIMEOUT / Def.REC_INTERVAL)
            {
                Def.EventInfo info = new Def.EventInfo();

                string id = this.transData.Id;
                this.waitIdList.Remove(id);

                info.Group = Convert.ToInt32(id.Substring(0, 1), 16);
                info.Address = 999;
                if (id.Length == 4)
                {
                    info.Address = Convert.ToInt32(id.Substring(1, 3));
                }

                this.transData = null;

                // イベント発生
                info.Data = STR_TIM;
#if SANQ
                this.logList.Add(new KeyValuePair<DateTime, string>(DateTime.Now, "Connection Error"));
#endif
                this.LogEvent("Connection Error");
                this.ResponseEvent(info);
            }
            else
            {
                // タイムアウトカウントのインクリメント
                this.TimeoutCnt++;
            }
        }
#endif
        #endregion

        #region 計算
        /// <summary>
        /// BCCの計算
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string BccCalc(string text)
        {
            try
            {
                // バイト配列に変換
                byte[] dat = Encoding.ASCII.GetBytes(text);

                // xorの計算
                byte sum = 0;
                foreach (byte b in dat)
                {
                    sum ^= b;
                }

                // BCCを取得
                byte bcc = (byte)((sum & 0x0f) | '0');

                return ((char)bcc).ToString();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// BCCチェック
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool BccCheck(string text)
        {
            if (text.Length == 3)
            {
                return true;
            }

            // BCC計算
            string bcc = text.Substring(text.Length - 1);
            if (bcc == this.BccCalc(text.Substring(0, text.Length - 1)))
            {
                this.Write(STR_ACK);
                return true;
            }
            else
            {
                this.Write(STR_NAK);
                return false;
            }
        }
        #endregion
        #endregion

        #region シリアルポートの接続・切断
        /// <summary>
        /// シリアルポートの接続
        /// </summary>
        /// <param name="portName">使用するポート</param>
        /// <returns>ステータス</returns>
        public int Open(string portName)
        {
            return this.Open(portName, 115200);
        }

        /// <summary>
        /// シリアルポートの接続
        /// </summary>
        /// <param name="portName">使用するポート</param>
        /// <param name="baudRate">ボーレート</param>
        /// <returns>ステータス</returns>
        public int Open(string portName, int baudRate)
        {
            int ret = 0;

            try
            {
                // シリアル通信の開始
                this.comPort = new ComPort(portName, baudRate, System.IO.Ports.Parity.None);

                // 送受信データのクリア
                this.DataClear();

                // スレッドの開始
                this.ReceiveThreadStart();
                this.TransThreadStart();

                // ポート番号をセット
                this.portName = portName;
            }
            catch (ArgumentException)
            {   // ポート名が "COM" で始まっていません。
                ret = 1;
            }
            catch (UnauthorizedAccessException)
            {   // ポートへのアクセスが拒否されています。 or 指定した COM ポートを既に開いています。
                ret = 2;
            }
            catch (System.IO.IOException)
            {   // ポートが無効状態です。
                ret = 3;
            }
            catch (Exception)
            {   // その他の例外
                ret = 4;
            }

            return ret;
        }

        /// <summary>
        /// シリアルポートを切断
        /// </summary>
        /// <returns>ステータス</returns>
        public int Close()
        {
            int ret = 0;

            try
            {
	            // データのクリア
	            this.DataClear();
                // スレッドの終了
                this.ReceiveThreadTerminate();
                this.TransThreadTerminate();

                // シリアル通信の終了
                this.comPort.Close();
                this.portName = string.Empty;
            }
            catch (Exception)
            {   // その他の例外
                ret = 1;
            }

            return ret;
        }
        #endregion

        #region 表示器制御のコマンド
        #region LED+セグメント
        /// <summary>
        /// 表示器ボタン（個別ボタン） 共通
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="address">表示器アドレス（1～100）</param>
        /// <param name="button">ボタン番号（1：赤、2：黄、3：緑、4：白、5：青）</param>
        /// <param name="mode">動作（点灯、点滅、消灯）</param>
        /// <param name="display">セグメント表示（6文字）</param>
        /// <param name="priority">優先（true：する、false：しない）</param>
        /// <returns>ステータス</returns>
        private int DisplayCommon(int group, int address, int button, byte mode, string display, bool priority)
        {
            // シリアルポートがオープンされていない
            if (string.IsNullOrEmpty(this.portName))
            {
                return (int)Def.Err.Port;
            }

            // IDの判定
            string id = this.IdHantei(group, address);
            if (string.IsNullOrEmpty(id))
            {
                return (int)Def.Err.Id;
            }

            // ボタンの判定
            // ボタンの判定
            string btn = this.BtnHantei(button, mode);
            if (string.IsNullOrEmpty(btn))
            {
            	mode =	mode == Def.BTN_ON ? Def.SEG_ON :
            			mode == Def.BTN_BLK ? Def.SEG_BLK :
            			Def.SEG_OFF;
            		
                return this.SegmentCommon(group, address, mode, display, priority);
            }

            // 表示文字の判定
            string disp = this.DispHantei(display);

            // 送信コマンドの追加
            this.CommandAdd(T_TXD1, id, btn, disp, priority);

            return (int)Def.Err.None;
        }

        /// <summary>
        /// 表示器ボタン点灯（個別ボタン）
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="address">表示器アドレス（1～100）</param>
        /// <param name="button">ボタン番号（1：赤、2：黄、3：緑、4：白、5：青）</param>
        /// <param name="display">セグメント表示（6文字）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int DisplayOn(int group, int address, int button, string display, bool priority = false)
        {
            return this.DisplayCommon(group, address, button, Def.BTN_ON, display, priority);
        }

        /// <summary>
        /// 表示器ボタン点灯（全ボタン）
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="address">表示器アドレス（1～100）</param>
        /// <param name="display">セグメント表示（6文字）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int DisplayOn(int group, int address, string display, bool priority = false)
        {
            return this.DisplayCommon(group, address, BTN_ALL, Def.BTN_ON, display, priority);
        }

        /// <summary>
        /// 表示器ボタン点滅（個別ボタン）
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="address">表示器アドレス（1～100）</param>
        /// <param name="button">ボタン番号（1：赤、2：黄、3：緑、4：白、5：青）</param>
        /// <param name="display">セグメント表示（6文字）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int DisplayBlink(int group, int address, int button, string display, bool priority = false)
        {
            return this.DisplayCommon(group, address, button, Def.BTN_BLK, display, priority);
        }

        /// <summary>
        /// 表示器ボタン点滅（全ボタン）
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="address">表示器アドレス（1～100）</param>
        /// <param name="display">セグメント表示（6文字）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int DisplayBlink(int group, int address, string display, bool priority = false)
        {
            return this.DisplayCommon(group, address, BTN_ALL, Def.BTN_BLK, display, priority);
        }

        /// <summary>
        /// 表示器ボタン消灯（個別ボタン）
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="address">表示器アドレス（1～100）</param>
        /// <param name="button">ボタン番号（1：赤、2：黄、3：緑、4：白、5：青）</param>
        /// <param name="display">セグメント表示（6文字）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int DisplayOff(int group, int address, int button, string display, bool priority = false)
        {
            return this.DisplayCommon(group, address, button, Def.BTN_OFF, display, priority);
        }

        /// <summary>
        /// 表示器ボタン消灯（全ボタン）
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="address">表示器アドレス（1～100）</param>
        /// <param name="display">セグメント表示（6文字）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int DisplayOff(int group, int address, string display, bool priority = false)
        {
            return this.DisplayCommon(group, address, BTN_ALL, Def.BTN_OFF, display, priority);
        }
        #endregion

        #region LEDのみ
        /// <summary>
        /// LEDのみ 共通
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="address">表示器アドレス（1～100）</param>
        /// <param name="button">ボタン番号（1：赤、2：黄、3：緑、4：白、5：青）</param>
        /// <param name="mode">動作（点灯、点滅、消灯）</param>
        /// <param name="priority">優先（true：する、false：しない）</param>
        /// <returns>ステータス</returns>
        private int LedCommon(int group, int address, int button, byte mode, bool priority)
        {
            // シリアルポートがオープンされていない
            if (string.IsNullOrEmpty(this.portName))
            {
                return (int)Def.Err.Port;
            }

            // IDの判定
            string id = this.IdHantei(group, address);
            if (string.IsNullOrEmpty(id))
            {
                return (int)Def.Err.Id;
            }

            // ボタンの判定
            string btn = this.BtnHantei(button, mode);
            if (string.IsNullOrEmpty(btn))
            {
                return (int)Def.Err.Id;
            }

            // 送信コマンドの追加
            this.CommandAdd(T_TXD2, id, btn, "      ", priority);

            return (int)Def.Err.None;
        }

        /// <summary>
        /// LEDのみ点灯（個別ボタン）
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="address">表示器アドレス（1～100）</param>
        /// <param name="button">ボタン番号（1：赤、2：黄、3：緑、4：白、5：青）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int LedOn(int group, int address, int button, bool priority = false)
        {
            return this.LedCommon(group, address, button, Def.BTN_ON, priority);
        }

        /// <summary>
        /// LEDのみ点灯（全ボタン）
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="address">表示器アドレス（1～100）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int LedOn(int group, int address, bool priority = false)
        {
            return this.LedCommon(group, address, BTN_ALL, Def.BTN_ON, priority);
        }

        /// <summary>
        /// LEDのみ点滅（個別ボタン）
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="address">表示器アドレス（1～100）</param>
        /// <param name="button">ボタン番号（1：赤、2：黄、3：緑、4：白、5：青）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int LedBlink(int group, int address, int button, bool priority = false)
        {
            return this.LedCommon(group, address, button, Def.BTN_BLK, priority);
        }

        /// <summary>
        /// LEDのみ点滅（全ボタン）
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="address">表示器アドレス（1～100）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int LedBlink(int group, int address, bool priority = false)
        {
            return this.LedCommon(group, address, BTN_ALL, Def.BTN_BLK, priority);
        }

        /// <summary>
        /// LEDのみ消灯（個別ボタン）
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="address">表示器アドレス（1～100）</param>
        /// <param name="button">ボタン番号（1：赤、2：黄、3：緑、4：白、5：青）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int LedOff(int group, int address, int button, bool priority = false)
        {
            return this.LedCommon(group, address, button, Def.BTN_OFF, priority);
        }

        /// <summary>
        /// LEDのみ消灯（全ボタン）
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="address">表示器アドレス（1～100）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int LedOff(int group, int address, bool priority = false)
        {
            return this.LedCommon(group, address, BTN_ALL, Def.BTN_OFF, priority);
        }
        #endregion

        #region セグメントのみ
        /// <summary>
        /// セグメントのみ 共通
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="address">表示器アドレス（1～100）</param>
        /// <param name="mode">動作（表示、点滅、クリア）</param>
        /// <param name="display">セグメント表示（6文字）</param>
        /// <param name="priority">優先（true：する、false：しない）</param>
        /// <returns>ステータス</returns>
        private int SegmentCommon(int group, int address, byte mode, string display, bool priority)
        {
            // シリアルポートがオープンされていない
            if (string.IsNullOrEmpty(this.portName))
            {
                return (int)Def.Err.Port;
            }

            // IDの判定
            string id = this.IdHantei(group, address);
            if (string.IsNullOrEmpty(id))
            {
                return (int)Def.Err.Id;
            }

            // ボタンの設定
            string btn = ((char)mode).ToString();

            // 表示文字の判定
            string disp = this.DispHantei(display);

            // 送信コマンドの追加
            this.CommandAdd(T_TXD3, id, btn, disp, priority);

            return (int)Def.Err.None;
        }

        /// <summary>
        /// セグメントのみ表示
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="address">表示器アドレス（1～100）</param>
        /// <param name="display">セグメント表示（6文字）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int SegmentOn(int group, int address, string display, bool priority = false)
        {
            return this.SegmentCommon(group, address, Def.SEG_OFF, display, priority);
        }

        /// <summary>
        /// セグメントのみ点滅
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="address">表示器アドレス（1～100）</param>
        /// <param name="display">セグメント表示（6文字）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int SegmentBlink(int group, int address, string display, bool priority = false)
        {
            return this.SegmentCommon(group, address, Def.SEG_BLK, display, priority);
        }

        /// <summary>
        /// セグメントのみクリア
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="address">表示器アドレス（1～100）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int SegmentOff(int group, int address, bool priority = false)
        {
            return this.SegmentCommon(group, address, Def.SEG_OFF, "      ", priority);
        }
        #endregion

        #region バッテリーチェック・バージョン表示
        /// <summary>
        /// バッテリチェック
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="address">表示器アドレス（1～100）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int BatteryCheck(int group, int address, bool priority = false)
        {
            // シリアルポートがオープンされていない
            if (string.IsNullOrEmpty(this.portName))
            {
                return (int)Def.Err.Port;
            }

            // IDの判定
            string id = this.IdHantei(group, address);
            if (string.IsNullOrEmpty(id))
            {
                return (int)Def.Err.Id;
            }

            // 送信コマンドの追加
            this.CommandAdd(T_BATTERY, id, string.Empty, string.Empty, priority);
            
            Thread.Sleep(100);                                // 送信スレッドの実行を待つ

            return (int)Def.Err.None;
        }

        /// <summary>
        /// バージョン表示
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="address">表示器アドレス（1～100）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public int VersionDisp(int group, int address, bool priority = false)
        {
            // シリアルポートがオープンされていない
            if (string.IsNullOrEmpty(this.portName))
            {
                return (int)Def.Err.Port;
            }

            // IDの判定
            string id = this.IdHantei(group, address);
            if (string.IsNullOrEmpty(id))
            {
                return (int)Def.Err.Id;
            }

            // 送信コマンドの追加
            this.CommandAdd(T_VER, id, string.Empty, string.Empty, priority);

            return (int)Def.Err.None;
        }
        #endregion

        #region ウェイクアップ・スリープ
        /// <summary>
        /// ウェークアップ
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int WakeUp(int group, bool priority = false)
        {
            // シリアルポートがオープンされていない
            if (string.IsNullOrEmpty(this.portName))
            {
                return (int)Def.Err.Port;
            }

            // IDの判定
            string id = this.IdHantei(group);
            if (string.IsNullOrEmpty(id))
            {
                return (int)Def.Err.Id;
            }

            // 送信コマンドの追加
            this.CommandAdd(T_WAKEUP, id, string.Empty, string.Empty, priority);

            return (int)Def.Err.None;
        }

        /// <summary>
        /// スリープ
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int Sleep(int group, bool priority = false)
        {
            // シリアルポートがオープンされていない
            if (string.IsNullOrEmpty(this.portName))
            {
                return (int)Def.Err.Port;
            }

            // IDの判定
            string id = this.IdHantei(group);
            if (string.IsNullOrEmpty(id))
            {
                return (int)Def.Err.Id;
            }

            // 送信コマンドの追加
            this.CommandAdd(T_SLEEP, id, string.Empty, string.Empty, priority);

            return (int)Def.Err.None;
        }
        #endregion

        #region 自走式
        /// <summary>
        /// 表示器に点灯指示をセット
        /// </summary>
        /// <param name="pattern">動作パターン</param>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="address">表示器アドレス（1～100）</param>
        /// <param name="button">ボタン番号（1：赤、2：黄、3：緑、4：白、5：青）</param>
        /// <param name="display">セグメント表示（6文字）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int SelfSet(int pattern, int group, int address, int button, string display, bool priority = false)
        {
            // シリアルポートがオープンされていない
            if (string.IsNullOrEmpty(this.portName))
            {
                return (int)Def.Err.Port;
            }

            // IDの判定
            string id = this.IdHantei(group, address);
            if (string.IsNullOrEmpty(id))
            {
                return (int)Def.Err.Id;
            }

            // ボタンの判定
            string btn = this.BtnHantei(button);
            if (string.IsNullOrEmpty(btn))
            {
                return (int)Def.Err.Id;
            }

            // 表示文字の判定
            string disp = this.DispHantei(display);

            // 送信コマンドの追加
            switch (pattern)
            {
                case 1:
                    this.CommandAdd(T_SET1, id, btn, disp, priority);
                    break;

                case 2:
                    this.CommandAdd(T_SET2, id, btn, disp, priority);
                    break;

                case 3:
                    this.CommandAdd(T_SET3, id, btn, disp, priority);
                    break;

                default:
                    return (int)Def.Err.Id;
            }

            return (int)Def.Err.None;
        }

        /// <summary>
        /// SETコマンドの解除
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="address">表示器アドレス（1～100）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int SelfReset(int group, int address, int button, bool priority = false)
        {
            // シリアルポートがオープンされていない
            if (string.IsNullOrEmpty(this.portName))
            {
                return (int)Def.Err.Port;
            }

            // IDの判定
            string id = this.IdHantei(group, address);
            if (string.IsNullOrEmpty(id))
            {
                return (int)Def.Err.Id;
            }

            // ボタンの判定
            string btn = this.BtnHantei(button);
            if (string.IsNullOrEmpty(btn))
            {
                return (int)Def.Err.Id;
            }

            // 送信コマンドの追加
            switch (button)
            {
                case 1:
                    this.CommandAdd(T_RST1, id, btn, string.Empty, priority);
                    break;

                case 2:
                    this.CommandAdd(T_RST2, id, btn, string.Empty, priority);
                    break;

                case 3:
                    this.CommandAdd(T_RST3, id, btn, string.Empty, priority);
                    break;

                case 4:
                    this.CommandAdd(T_RST4, id, btn, string.Empty, priority);
                    break;

                case 5:
                    this.CommandAdd(T_RST5, id, btn, string.Empty, priority);
                    break;

                default:
                    return (int)Def.Err.Id;
            }

            return (int)Def.Err.None;
        }

        /// <summary>
        /// SETコマンドの解除
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="address">表示器アドレス（1～100）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int SelfReset(int group, int address, bool priority = false)
        {
            // シリアルポートがオープンされていない
            if (string.IsNullOrEmpty(this.portName))
            {
                return (int)Def.Err.Port;
            }

            // IDの判定
            string id = this.IdHantei(group, address);
            if (string.IsNullOrEmpty(id))
            {
                return (int)Def.Err.Id;
            }

            // 送信コマンドの追加
            this.CommandAdd(T_RESET, id, string.Empty, string.Empty, priority);

            return (int)Def.Err.None;
        }

        /// <summary>
        /// 表示器にEND表示
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="address">表示器アドレス（1～100）</param>
        /// <param name="time">表示時間（1～15）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int SelfEnd(int group, int address, int time, bool priority = false)
        {
            // シリアルポートがオープンされていない
            if (string.IsNullOrEmpty(this.portName))
            {
                return (int)Def.Err.Port;
            }

            // IDの判定
            string id = this.IdHantei(group, address);
            if (string.IsNullOrEmpty(id))
            {
                return (int)Def.Err.Id;
            }

            // 表示時間の判定
            string tm = this.TimeHantei(time);
            if (string.IsNullOrEmpty(tm))
            {
                return (int)Def.Err.Id;
            }

            // 送信コマンドの追加
            this.CommandAdd(T_END, id, tm, string.Empty, priority);

            return (int)Def.Err.None;
        }
        #endregion

        #region 判定
        /// <summary>
        /// IDの判定（個別表示器）
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <param name="address">表示器アドレス（1～100）</param>
        /// <returns>ID文字列</returns>
        private string IdHantei(int group, int address)
        {
            // 無線グループが範囲外
            if ((group < 0) || (group > 15))
            {
                return null;
            }

            // 表示器アドレスが範囲外
            if ((address < 1) || (address > 200))
            {
                return null;
            }

            return group.ToString("X") + address.ToString("000");
        }

        /// <summary>
        /// IDの判定(全表示器）
        /// </summary>
        /// <param name="group">無線グループ（0～15）</param>
        /// <returns>ID文字列</returns>
        private string IdHantei(int group)
        {
            // 無線グループが範囲外
            if ((group < 0) || (group > 15))
            {
                return null;
            }

            return group.ToString("X");
        }

        /// <summary>
        /// ボタンの判定
        /// </summary>
        /// <param name="button">ボタン番号</param>
        /// <param name="mode">点灯モード</param>
        /// <returns>ボタン文字列</returns>
        private string BtnHantei(int button, byte mode)
        {
            // ボタン番号が範囲外
            if ((button < 1 || button > 9) && button != BTN_ALL)
            {
                return null;
            }

            return ((char)(button | mode)).ToString();
        }

        /// <summary>
        /// ボタンの判定
        /// </summary>
        /// <param name="button">ボタン番号</param>
        /// <returns>ボタン文字列</returns>
        private string BtnHantei(int button)
        {
            // ボタン番号が範囲外
            if (button < 1 || button > 9)
            {
                return null;
            }

            return ((char)(button | Def.BTN_ON)).ToString();
        }

        /// <summary>
        /// 表示文字列の判定
        /// </summary>
        /// <param name="display">表示文字列</param>
        /// <returns>表示文字列</returns>
        private string DispHantei(string display)
        {
            if (display.Length <= 6)
            {
                return display.PadRight(6, ' ');
            }
            else
            {
                return display.Substring(0, 6);
            }
        }

        /// <summary>
        /// 表示時間の判定
        /// </summary>
        /// <param name="time">表示時間（1～15）</param>
        /// <returns>表示時間</returns>
        private string TimeHantei(int time)
        {
            // 表示時間が範囲外
            if ((time < 1) || (time > 15))
            {
                return null;
            }

            return time.ToString("X");
        }
        #endregion

        #region コマンド追加
        /// <summary>
        /// 送信コマンドの追加
        /// </summary>
        /// <param name="header">COM</param>
        /// <param name="id">ID</param>
        /// <param name="button">BUTTON</param>
        /// <param name="disp">表示</param>
        /// <param name="priority">優先</param>
        private void CommandAdd(string header, string id, string button, string disp, bool priority)
        {
            CommandList cmd = new CommandList(header, id, button, disp, priority);

            lock (lockObj)
            {
                if (priority)
                {
            		int index = this.transList.FindLastIndex(x => x.Id == id) + 1;
            		if ( index == 0 )
            		{
            			index = this.transList.FindLastIndex( x => x.Priority == true ) + 1;
            		}
                  //int index = this.transList.FindLastIndex(x => x.Priority == true) + 1;
                    this.transList.Insert(index, cmd);
                }
                else
                {
                    this.transList.Add(cmd);
                }
            }
        }
        #endregion
        #endregion

        #region 表示器状態
        /// <summary>
        /// 送信待ちデータのクリア
        /// </summary>
        public void WaitTransClear()
        {
            lock (lockObj)
            {
                this.transList.Clear();
            }
        }

        /// <summary>
        /// 送信待ちデータのカウント
        /// </summary>
        /// <returns>カウント数</returns>
        public int WaitTransCount()
        {
            lock (lockObj)
            {
                return this.transList.Count;
            }
        }

        /// <summary>
        /// 応答待ちデータのカウント
        /// </summary>
        /// <returns>カウント数</returns>
        public int WaitRespoCount()
        {
            return this.waitIdList.Count;
        }
        #endregion

        #region タイマー
#if !TIM
        /// <summary>
        /// タイマーの初期化
        /// </summary>
        private void TimerInit()
        {
            this.timer = new System.Timers.Timer();
            this.timer.Enabled = true;
            this.timer.Interval = TIMEOUT;
            this.timer.AutoReset = false;

            this.timer.Elapsed += this.Timer_Elapsed;
            this.TimerStop();
        }

        /// <summary>
        /// タイマーの開始
        /// </summary>
        private void TimerStart()
        {
            this.timer.Start();
        }

        /// <summary>
        /// タイマーの停止
        /// </summary>
        private void TimerStop()
        {
            this.timer.Stop();
        }
#endif
        #endregion

        #region タイマー
        /// <summary>
        /// タイマーの初期化
        /// </summary>
        private void Dsp_timer_init()
        {
            this.Dsp_timer = new System.Timers.Timer();
            this.Dsp_timer.Enabled = true;
            this.Dsp_timer.Interval = 100;
            this.Dsp_timer.AutoReset = true;

            this.Dsp_timer.Elapsed += this.Dsp_Timer_Elapsed;
            this.Dsp_timer_Start();
        }

        /// <summary>
        /// タイマーの開始
        /// </summary>
        private void Dsp_timer_Start()
        {
            this.Dsp_timer.Start();
        }

        /// <summary>
        /// タイマーの停止
        /// </summary>
        private void Dsp_timer_Stop()
        {
            this.Dsp_timer.Stop();
        }
        #endregion

        #region イベント呼び出し
        /// <summary>
        /// 表示器ボタン押下のイベント発生
        /// </summary>
        /// <param name="info">イベント情報</param>
        private void ButtonPushEvent(Def.EventInfo info)
        {
            if (this.ButtonPush != null)
            {
                info.ButtonPush = this.ButtonPush;
                this.worker.ButtonPushEvent(info);
#if SANQ
                if ( this.logList.Count > 10000 ) //	GetLog呼び出し無し対応
                    this.logList.Clear();
#endif
            }
        }

        /// <summary>
        /// 表示器ボタン完了のイベント発生
        /// </summary>
        /// <param name="info">イベント情報</param>
        private void ButtonDoneEvent(Def.EventInfo info)
        {
            if (this.ButtonDone != null)
            {
                info.ButtonDone = this.ButtonDone;
                this.worker.ButtonDoneEvent(info);
            }
        }

        /// <summary>
        /// 表示器応答のイベント発生
        /// </summary>
        /// <param name="info">イベント情報</param>
        private void ResponseEvent(Def.EventInfo info)
        {
            if (this.Response != null)
            {
                info.Response = this.Response;
                this.worker.ResponseEvent(info);
            }
        }

        /// <summary>
        /// 通信ログ通知のイベント発生
        /// </summary>
        /// <param name="str">文字列</param>
        private void LogEvent(string str)
        {
            if (this.Log != null)
            {
                Def.EventInfo info = new Def.EventInfo();
                info.Data = str;
                info.Log = this.Log;
                this.worker.LogEvent(info);
            }
        }

        /// <summary>
        /// バージョン取得のイベント発生
        /// </summary>
        /// <param name="info">イベント情報</param>
        private void VersionEvent(Def.EventInfo info)
        {
            if (this.Version != null)
            {
                info.Response = this.Version;
                this.worker.ResponseEvent(info);
            }
        }
        #endregion

#if SANQ
        /// <summary>
        /// ログリストの取得
        /// </summary>
        /// <param name="info">ログリストの取得</param>
        public List<KeyValuePair<DateTime, string>> GetLog()
        {
            List<KeyValuePair<DateTime, string>> lst=new List<KeyValuePair<DateTime,string>>();
            lst.AddRange(this.logList);
            this.logList.Clear();

            return lst;
        }
#endif
    }
}
