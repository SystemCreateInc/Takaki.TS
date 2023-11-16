using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Td.Dps
{
    /// <summary>
    /// ピッキング無線表示器ドライバ
    /// </summary>
    public class Wireless : IDps
    {
        #region 定義
        #region 定数
        /// <summary>
        /// 排他アクセス
        /// </summary>
        private readonly object lockObj = new object();

        /// <summary>
        /// タイムアウト時間（ミリ秒）
        /// </summary>
        private const int TIMEOUT = 10000;

        /// <summary>
        /// 応答待ちデータのカウント
        /// </summary>
        private const int RES_CNT = 5;

        /// <summary>
        /// 全ボタン
        /// </summary>
        private const int BTN_ALL = 9;
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

        /// <summary>
        /// ドライバ種別
        /// </summary>
        private int typeDriver;

        /// <summary>
        /// ポート番号
        /// </summary>
        private string portName;

        /// <summary>
        /// 送信データの格納
        /// </summary>
        private List<CommandList> transList;

        /// <summary>
        /// 受信データの格納
        /// </summary>
        private List<byte> recData;

        /// <summary>
        /// 応答待ちデータの格納
        /// </summary>
        private Dictionary<string, int> waitDict;

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
        /// 表示器応答のイベントハンドラ
        /// </summary>
        public event Event.ResponseEventHandler Response;

        /// <summary>
        /// 通信ログ通知のイベントハンドラ
        /// </summary>
        public event Event.LogEventHandler Log;
        #endregion
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Wireless()
            : this(false)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="async">イベントの非同期</param>
        public Wireless(bool async)
        {
            this.worker = new EventWorker(async);
            this.Init();
        }
        #endregion

        #region メソッド
        #region 初期化
        /// <summary>
        /// 初期化
        /// </summary>
        private void Init()
        {
            // プロパティの初期化
            this.typeDriver = 0;
            this.portName = string.Empty;

            // リストの初期化
            this.transList = new List<CommandList>();
            this.recData = new List<byte>();
            this.waitDict = new Dictionary<string, int>();

            // データのクリア
            this.DataClear();
        }

        /// <summary>
        /// データのクリア
        /// </summary>
        private void DataClear()
        {
            // リストのクリア
            lock (lockObj)
            {
                this.transList.Clear();
                this.recData.Clear();
                this.waitDict.Clear();
            }
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
                    // 送信したいデータがある かつ 応答待ちデータが少ない場合
                    if (this.transList.Count > 0 && this.waitDict.Count < RES_CNT)
                    {
                        // 送信可能なデータを調べる
                        int index = this.TransListChk();
                        if (index >= 0)
                        {
                            // 送信可能なデータがあれば、送信する
                            this.Write(this.transList[index]);
                            this.transList.RemoveAt(index);
                        }
                    }
                }

                // 指定時間待つ
                Thread.Sleep(Def.TRA_INTERVAL);
            }
        }

        /// <summary>
        /// 送信可能なデータのチェック
        /// </summary>
        /// <returns>送信データのインデックス</returns>
        private int TransListChk()
        {
            int index = -1;

            for (int i = 0; i < this.transList.Count; i++)
            {
                // IDの取得
                string id = this.transList[i].Id;

                // 応答待ちデータがない送信データのインデックスを取得
                if (this.waitDict.ContainsKey(id) == false)
                {
                    index = i;
                    this.waitDict[id] = 0;

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

            // イベント発生
            this.LogEvent("T:[ENQ]" + str + "[EOT]");

            // 制御コードを付加し送信
            this.comPort.Write((char)Def.ENQ + str + (char)Def.EOT);
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
                // 受信データを取得する
                lock (lockObj)
                {
                    this.recData.AddRange(this.comPort.Read());
                }

                // データがあれば解析する
                if (this.recData.Count > 0)
                {
                    this.DataAnalyze();
                }

                // タイムアウトを調べる
                this.TimeOutChk();

                // 指定時間待つ
                Thread.Sleep(Def.REC_INTERVAL);
            }
        }

        /// <summary>
        /// データの解析
        /// </summary>
        private void DataAnalyze()
        {
            byte[] bytes;

            try
            {
                lock (lockObj)
                {
                    // コマンド終端
                    int eot = this.recData.IndexOf(Def.EOT);

                    if (eot == -1)
                    {
                        // 終端未検出
                        return;
                    }

                    // データを取り出し、削除
                    bytes = this.recData.GetRange(0, eot + 1).ToArray();
                    this.recData.RemoveRange(0, eot + 1);
                }

                // 文字列に変換
                string str = AsciiToString.ToString(bytes);

                // イベント発生
                this.LogEvent("R:" + str);

                // コマンド先頭
                int enq = str.LastIndexOf(AsciiToString.ControlString[Def.ENQ]);
                if (enq == -1)
                {
                    // 先頭未検出
                    return;
                }
                else
                {
                    // ゴミを取り除く
                    str = str.Substring(enq);
                }

                // 文字列置換
                str = str.Replace(AsciiToString.ControlString[Def.ENQ], string.Empty);
                str = str.Replace(AsciiToString.ControlString[Def.EOT], string.Empty);
                str = str.Replace(AsciiToString.ControlString[Def.ACK], "ACK");
                str = str.Replace(AsciiToString.ControlString[Def.NAK], "NAK");

                // ボタン押下
                if (str.Length == 4)
                {
                    Def.EventInfo info = new Def.EventInfo();

                    // イベント情報を取得
                    info.Group = Convert.ToInt32(str.Substring(0, 1));
                    info.Address = Convert.ToInt32(str.Substring(1, 2));
                    info.Button = Convert.ToInt32(str.Substring(3, 1));

                    // イベント発生
                    this.ButtonPushEvent(info);
                }
                else if (str.Length == 6)
                {
                    Def.EventInfo info = new Def.EventInfo();

                    // イベント情報を取得
                    info.Group = Convert.ToInt32(str.Substring(0, 1));
                    info.Address = Convert.ToInt32(str.Substring(1, 2));
                    info.Data = str.Substring(3);

                    // 応答待ちデータの削除
                    string id = str.Substring(0, 3);
                    lock (lockObj)
                    {
                        this.waitDict.Remove(id);
                    }

                    // イベント発生
                    this.ResponseEvent(info);
                }
            }
            catch (Exception ex)
            {
                this.LogEvent("[例外]" + ex.ToString());
            }
        }

        /// <summary>
        /// タイムアウト監視
        /// </summary>
        private void TimeOutChk()
        {
            lock (lockObj)
            {
                // 応答待ちデータが無ければ、何もしない
                if (this.waitDict.Count <= 0)
                {
                    return;
                }

                // カウント降順に並べ、最初の要素を取得
                KeyValuePair<string, int> pair = this.waitDict.OrderByDescending(x => x.Value).First();

                // タイムアウト値を超えた
                if (pair.Value > TIMEOUT / Def.REC_INTERVAL)
                {
                    Def.EventInfo info = new Def.EventInfo();

                    // 無線グループ・表示器アドレスを修得
                    info.Group = Convert.ToInt32(pair.Key.Substring(0, 1));
                    info.Address = Convert.ToInt32(pair.Key.Substring(1, 2));
                    info.Data = "TIM";

                    // イベント発生
                    this.LogEvent("E:" + pair.Key + "TIM");
                    this.ResponseEvent(info);

                    this.waitDict.Remove(pair.Key);
                }
                else
                {
                    // タイムアウトカウントの更新
                    this.waitDict = this.waitDict.ToDictionary(x => x.Key, y => y.Value + 1);
                }
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
            return this.Open(portName, 9600);
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
                this.comPort = new ComPort(portName, baudRate, System.IO.Ports.Parity.Odd);

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
        /// <param name="group">無線グループ（0～9）</param>
        /// <param name="address">表示器アドレス（1～50）</param>
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
            string btn = this.BtnHantei(button, mode);
            if (string.IsNullOrEmpty(btn))
            {
            	mode =	mode == Def.BTN_ON ? Def.SEG_ON :
            			mode == Def.BTN_BLK ? Def.SEG_BLK :
            			Def.SEG_OFF;
            		
                return this.Segment(group, address, mode, display, priority);
            }

            // 表示文字の判定
            string disp = this.DispHantei(display);

            // 送信コマンドの追加
            this.CommandAdd("C", id, btn, disp, priority);

            return (int)Def.Err.None;
        }

        /// <summary>
        /// 表示器ボタン点灯（個別ボタン）
        /// </summary>
        /// <param name="group">無線グループ（0～9）</param>
        /// <param name="address">表示器アドレス（1～50）</param>
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
        /// <param name="group">無線グループ（0～9）</param>
        /// <param name="address">表示器アドレス（1～50）</param>
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
        /// <param name="group">無線グループ（0～9）</param>
        /// <param name="address">表示器アドレス（1～50）</param>
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
        /// <param name="group">無線グループ（0～9）</param>
        /// <param name="address">表示器アドレス（1～50）</param>
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
        /// <param name="group">無線グループ（0～9）</param>
        /// <param name="address">表示器アドレス（1～50）</param>
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
        /// <param name="group">無線グループ（0～9）</param>
        /// <param name="address">表示器アドレス（1～50）</param>
        /// <param name="display">セグメント表示（6文字）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int DisplayOff(int group, int address, string display, bool priority = false)
        {
            return this.DisplayCommon(group, address, BTN_ALL, Def.BTN_OFF, display, priority);
        }
        #endregion

        #region セグメントのみ
        /// <summary>
        /// セグメントのみ 共通
        /// </summary>
        /// <param name="group">無線グループ（0～9）</param>
        /// <param name="address">表示器アドレス（1～50）</param>
        /// <param name="mode">動作（表示、点滅、クリア）</param>
        /// <param name="display">セグメント表示（6文字）</param>
        /// <param name="priority">優先（true：する、false：しない）</param>
        /// <returns>ステータス</returns>
        private int Segment(int group, int address, byte mode, string display, bool priority)
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
            this.CommandAdd("C", id, btn, disp, priority);

            return (int)Def.Err.None;
        }

        /// <summary>
        /// セグメントのみ表示
        /// </summary>
        /// <param name="group">無線グループ（0～9）</param>
        /// <param name="address">表示器アドレス（1～50）</param>
        /// <param name="display">セグメント表示（6文字）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int SegmentOn(int group, int address, string display, bool priority = false)
        {
            return this.Segment(group, address, Def.SEG_OFF, display, priority);
        }

        /// <summary>
        /// セグメントのみ点滅
        /// </summary>
        /// <param name="group">無線グループ（0～9）</param>
        /// <param name="address">表示器アドレス（1～50）</param>
        /// <param name="display">セグメント表示（6文字）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int SegmentBlink(int group, int address, string display, bool priority = false)
        {
            return this.Segment(group, address, Def.SEG_BLK, display, priority);
        }

        /// <summary>
        /// セグメントのみクリア
        /// </summary>
        /// <param name="group">無線グループ（0～9）</param>
        /// <param name="address">表示器アドレス（1～50）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int SegmentOff(int group, int address, bool priority = false)
        {
            return this.Segment(group, address, Def.SEG_OFF, "         ", priority);
        }
        #endregion

        #region バッテリーチェック・ウェークアップ・延長
        /// <summary>
        /// バッテリチェック
        /// </summary>
        /// <param name="group">無線グループ（0～9）</param>
        /// <param name="address">表示器アドレス（1～50）</param>
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
            this.CommandAdd(string.Empty, id, string.Empty, "LNK", priority);

            return (int)Def.Err.None;
        }

        /// <summary>
        /// ウェークアップ
        /// </summary>
        /// <param name="group">無線グループ（0～9）</param>
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
            this.CommandAdd(string.Empty, id, string.Empty, "WUP", priority);

            return (int)Def.Err.None;
        }

        /// <summary>
        /// 延長
        /// </summary>
        /// <param name="group">無線グループ（0～9）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int Extension(int group, bool priority = false)
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
            this.CommandAdd(string.Empty, id, string.Empty, "EXT", priority);

            return (int)Def.Err.None;
        }
        #endregion

        #region 判定
        /// <summary>
        /// IDの判定（個別表示器）
        /// </summary>
        /// <param name="group">無線グループ（0～9）</param>
        /// <param name="address">表示器アドレス（1～50）</param>
        /// <returns>ID文字列</returns>
        private string IdHantei(int group, int address)
        {
            // 無線グループが範囲外
            if ((group < 0) || (group > 9))
            {
                return null;
            }

            // 表示器アドレスが範囲外
            if ((address < 1) || (address > 50))
            {
                return null;
            }

            return group.ToString("0") + address.ToString("00");
        }

        /// <summary>
        /// IDの判定(全表示器）
        /// </summary>
        /// <param name="group">無線グループ（0～9）</param>
        /// <returns>ID文字列</returns>
        private string IdHantei(int group)
        {
            // 無線グループが範囲外
            if ((group < 0) || (group > 9))
            {
                return null;
            }

            return group.ToString("0") + "99";
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
            if ((button < 1 || button > 5) && button != BTN_ALL)
            {
                return ((char)Def.SEG_ON).ToString();
            }

            return ((char)(button | mode)).ToString();
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
            lock (lockObj)
            {
                return this.waitDict.Count;
            }
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
        #endregion
        #endregion
    }
}
