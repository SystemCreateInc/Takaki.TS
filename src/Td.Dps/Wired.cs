using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Td.Dps
{
    /// <summary>
    /// ピッキング有線表示器ドライバ
    /// </summary>
    public class Wired : IDps
    {
        #region 定義
        #region 定数
        /// <summary>
        /// 排他アクセス
        /// </summary>
        private readonly object lockObj = new object();

        /// <summary>
        /// 排他アクセス
        /// </summary>
        private readonly object lockObj_log = new object();

        /// <summary>
        /// 接続エラーのタイムアウト値(ミリ秒)
        /// </summary>
        private const int TIMEOUT = 3000;
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
        /// タイマー
        /// </summary>
        private System.Timers.Timer timer;

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
        /// 送信データの格納
        /// </summary>
        private List<CommandList> transList;

        /// <summary>
        /// 受信データの格納
        /// </summary>
        private List<byte> recData;

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

        /// <summary>
        /// 送信許可のフラグ
        /// </summary>
        private bool transEnable;

        /// <summary>
        /// 受信許可のフラグ
        /// </summary>
        private bool recEnable;

        /// <summary>
        /// 送信要求(ENQ)の送信フラグ
        /// </summary>
        private bool enqTrans;

        /// <summary>
        /// データの送信フラグ
        /// </summary>
        private bool datTrans;

        /// <summary>
        /// 接続エラーのカウント
        /// </summary>
        private int errCnt;
        
        /// <summary>
        /// GetLog用StringList
        /// </summary>
        private List<KeyValuePair<DateTime, string>> logList;
        
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
        public Wired()
            : this(false)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="async">イベントの非同期</param>
        public Wired(bool async)
        {
            this.worker = new EventWorker(async);
            this.Init();
        }
        #endregion

        #region イベント
        /// <summary>
        /// 接続エラーのタイマーイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // エラーのカウントアップ
            this.errCnt++;

            // 3回エラーならエラー処理
            if (this.errCnt >= 3)
            {
                this.TimerStop();

                string str = this.transList[0].Command;
                Def.EventInfo info = new Def.EventInfo();
                info.Group = Convert.ToInt32(str.Substring(1, 2));
                info.Address = Convert.ToInt32(str.Substring(3, 3));
                info.Data = "TIM";

                lock( lockObj_log ) {
                    this.logList.Add(new KeyValuePair<DateTime, string>(DateTime.Now, "Connection Error"));
                }
                // イベント発生
                this.LogEvent("Connection Error");
                this.ResponseEvent(info);
            }
            else
            {
                this.TimerStart();
                this.enqTrans = false;
            }
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
            this.typeDriver = 1;
            this.portName = string.Empty;

            // リストの初期化
            this.transList = new List<CommandList>();
            this.recData = new List<byte>();
            this.logList = new List<KeyValuePair<DateTime, string>>();

            // データのクリア
            this.DataClear();

            // タイマーの初期化
            this.TimerInit();
        }

        /// <summary>
        /// データのクリア
        /// </summary>
        private void DataClear()
        {
            // リストのクリア
            this.transList.Clear();
            this.recData.Clear();

            // フラグのクリア
            this.transEnable = false;
            this.recEnable = false;
            this.enqTrans = false;
            this.datTrans = false;
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

            this.transStop = false;                                 // 送信スレッド停止フラグのクリア
            this.transThread = new Thread(this.TransThreadWork);    // 送信スレッドの生成
            this.transThread.Priority = ThreadPriority.BelowNormal; // 送信スレッドの優先順位を下げる
            this.transThread.Start();                               // 送信スレッドの開始
            if (this.transThread.IsAlive == false)
            {
                Thread.Sleep(1);                                    // 送信スレッドの実行を待つ
            }
        }

        /// <summary>
        /// 送信スレッドの終了
        /// </summary>
        private void TransThreadTerminate()
        {
            this.transStop = true;                                  // 送信スレッド停止フラグのセット

            // スレッドが起動中
            if ((this.transThread != null) && (this.transThread.ThreadState == ThreadState.Running))
            {
                this.transThread.Abort();                           // 送信スレッドの終了
                this.transThread.Join(300);                         // 送信スレッドが終了するまで待つ
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
                    // 送信データがある かつ 受信許可していない かつ 送信中でない
                    if (this.transList.Count > 0 && this.recEnable == false && this.datTrans == false)
                    {
                        // 送信許可（ENQ -> ACK）の場合
                        if (this.transEnable == true)
                        {
                            // 送信データを取得
                            this.transData = this.transList[0];
                            this.transList.RemoveAt(0);

                            // 送信フラグをセット、送信許可フラグをクリア
                            this.datTrans = true;
                            this.transEnable = false;

                            // 送信する
                            this.Write(this.transData);

                        } else {
                            // ENQが未送信なら送信する
                            if (this.enqTrans == false)
                            {
                                this.enqTrans = true;
                                this.WriteEnq();
                            }
                        }
                    }
                }

                Thread.Sleep(( ( this.transList.Count <= 0 ) ? Def.TRA_INTERVAL : 1 ));
            }
        }

        /// <summary>
        /// コマンド送信
        /// </summary>
        /// <param name="command"></param>
        private void Write(CommandList command)
        {
            // 送信文字列を取得
            string str = command.Command;

            // BCCを取得
            string bcc = this.BccCalc(str);

            // 制御コードとBCCを付加し送信
            this.comPort.Write((char)Def.STX + str + (char)Def.ETX + bcc);

            lock( lockObj_log ) {
                this.logList.Add(new KeyValuePair<DateTime, string>(DateTime.Now, "T:[STX]" + str + "[ETX]" + bcc));
            }
            // イベント発生
            this.LogEvent("T:[STX]" + str + "[ETX]" + bcc);
        }

        /// <summary>
        /// ENQ送信
        /// </summary>
        private void WriteEnq()
        {
            this.comPort.Write(Def.ENQ);

            lock( lockObj_log ) {
                this.logList.Add(new KeyValuePair<DateTime, string>(DateTime.Now, "T:[ENQ]"));
            }
            // イベント発生
            this.LogEvent("T:[ENQ]");

            // タイマーの開始
            this.TimerStart();
        }

        /// <summary>
        /// ACK送信
        /// </summary>
        private void WriteACK()
        {
            this.comPort.Write(Def.ACK);

            lock( lockObj_log ) {
                this.logList.Add(new KeyValuePair<DateTime, string>(DateTime.Now, "T:[ACK]"));
            }
            // イベント発生
            this.LogEvent("T:[ACK]");
        }

        /// <summary>
        /// NAK送信
        /// </summary>
        private void WriteNAK()
        {
            this.comPort.Write(Def.NAK);

            lock( lockObj_log ) {
                this.logList.Add(new KeyValuePair<DateTime, string>(DateTime.Now, "T:[NAK]"));
            }
            // イベント発生
            this.LogEvent("T:[NAK]");
        }

        /// <summary>
        /// EOT送信
        /// </summary>
        private void WriteEOT()
        {
            this.comPort.Write(Def.EOT);

            lock( lockObj_log ) {
                this.logList.Add(new KeyValuePair<DateTime, string>(DateTime.Now, "T:[EOT]"));
            }
            // イベント発生
            this.LogEvent("T:[EOT]");
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
        /// 受信スレッド処理
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

                    // データがあれば解析する
                    if (this.recData.Count > 0)
                    {
                        this.DataAnalyze();
                    }
                }

                // 指定時間待つ
                Thread.Sleep( (( this.recData.Count <= 0 ) ? Def.REC_INTERVAL : 1 ) );
            }
        }

        /// <summary>
        /// データの解析
        /// </summary>
        private void DataAnalyze()
        {
            try
            {
                // 先頭の1バイトで分岐する
                switch (this.recData[0])
                {
                    case Def.ACK:
                        this.RecDataRemove();
                        this.DataAnalyzeAck();
                        break;

                    case Def.NAK:
                        this.RecDataRemove();
                        this.DataAnalyzeNak();
                        break;

                    case Def.ENQ:
                        this.RecDataRemove();
                        this.DataAnalyzeEnq();
                        break;

                    case Def.STX:
                        this.DataAnalyzeStx();
                        break;

                    default:
                        this.RecDataRemove();
                        break;
                }
            }
            catch (Exception ex)
            {
                lock( lockObj_log ) {
                    this.logList.Add(new KeyValuePair<DateTime, string>(DateTime.Now, "[例外]" + ex.ToString()));
                }
                this.LogEvent("[例外]" + ex.ToString());
            }
        }

        /// <summary>
        /// 受信データの1バイト削除
        /// </summary>
        private void RecDataRemove()
        {
            try
            {
                lock (lockObj)
                {
                    this.recData.RemoveAt(0);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// データの解析（先頭がACK）
        /// </summary>
        private void DataAnalyzeAck()
        {
            try
            {
                lock( lockObj_log ) {
                    this.logList.Add(new KeyValuePair<DateTime, string>(DateTime.Now, "R:" + AsciiToString.ControlString[Def.ACK]));
                }
                // イベント発生
                this.LogEvent("R:" + AsciiToString.ControlString[Def.ACK]);

                if (this.enqTrans == true)
                {   // 送信要求を送信している場合
                    // 送信を許可し、要求フラグをクリア
                    this.transEnable = true;
                    this.enqTrans = false;

                    // タイマーの停止
                    this.TimerStop();
                }
                else if (this.datTrans == true)
                {   // データを送信している場合
                    // 送信データをクリアし、送信フラグをクリア
                    this.transData = null;
                    this.datTrans = false;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// データの解析（先頭がNAK）
        /// </summary>
        private void DataAnalyzeNak()
        {
            try
            {
                lock( lockObj_log ) {
                    this.logList.Add(new KeyValuePair<DateTime, string>(DateTime.Now, "R:" + AsciiToString.ControlString[Def.NAK]));
                }
                // イベント発生
                this.LogEvent("R:" + AsciiToString.ControlString[Def.NAK]);

                if (this.enqTrans == true)
                {   // 送信要求を送信している場合
                    // 要求フラグをクリア
                    this.enqTrans = false;

                    // タイマーの停止
                    this.TimerStop();
                }
                else if (this.datTrans == true)
                {   // データを送信している場合
                    // 再送し、送信フラグをクリア
                    this.transList.Insert(0, this.transData);
                    this.datTrans = false;
                }
                else if (this.recEnable)
                {   // 受信許可中（ENQを受信している）の場合
                    // EOTを送信
                    this.WriteEOT();
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// データの解析（先頭がENQ）
        /// </summary>
        private void DataAnalyzeEnq()
        {
            try
            {
                lock( lockObj_log ) {
                    this.logList.Add(new KeyValuePair<DateTime, string>(DateTime.Now, "R:" + AsciiToString.ControlString[Def.ENQ]));
                }
                // イベント発生
                this.LogEvent("R:" + AsciiToString.ControlString[Def.ENQ]);

                // データを送信している場合
                if (this.datTrans == true)
                {
                    // NAKを送信
                    this.WriteNAK();
                }
                else
                {
                    // 送信禁止、受信許可
                    this.enqTrans = false;
                    this.transEnable = false;
                    this.recEnable = true;

                    // ACKを送信
                    this.WriteACK();
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// データの解析（先頭がSTX）
        /// </summary>
        private void DataAnalyzeStx()
        {
            byte[] bytes;

            try
            {
                lock (lockObj)
                {
                    // BCCまで受信していない
                    if (this.recData.Count < 20)
                    {
                        return;
                    }

                    // データを取り出し、削除
                    bytes = bytes = this.recData.GetRange(0, 20).ToArray();
                    this.recData.RemoveRange(0, 20);
                }


                // 受信文字列とBCCに分解
                string str = Encoding.ASCII.GetString(bytes, 1, 16);
                string bcc = Encoding.ASCII.GetString(bytes, 18, 2);

                lock( lockObj_log ) {
                    this.logList.Add(new KeyValuePair<DateTime, string>(DateTime.Now, "R:[STX]" + str + "[ETX]" + bcc));
                }
                // イベント発生追加
                this.LogEvent("R:[STX]" + str + "[ETX]" + bcc);

                // BCCの計算
                if (this.BccCalc(str) == bcc)
                {
                    lock( lockObj_log ) {
                        this.logList.Add(new KeyValuePair<DateTime, string>(DateTime.Now, "bcc ok"));
                    }
                    // イベント発生
                    this.LogEvent("bcc ok");
                    this.WriteACK();
                    // 受信許可フラグを戻す
                    this.recEnable = false;
                } else {
                    lock( lockObj_log ) {
                        this.logList.Add(new KeyValuePair<DateTime, string>(DateTime.Now, "bcc ng"));
                    }
                    // イベント発生
                    this.LogEvent("bcc ng");
                    this.WriteNAK();
                    
                    // 受信許可フラグを戻す
                    this.recEnable = false;
                    return;
                }

                // 分岐アドレス・表示器アドレス・ボタン番号に分解
                Def.EventInfo info = new Def.EventInfo();
                info.Group = Convert.ToInt32(str.Substring(1, 2));
                info.Address = Convert.ToInt32(str.Substring(3, 3));
                info.Button = Convert.ToInt32(str.Substring(6, 1));

                // イベント発生
                switch (str[0])
                {
                    case 'R':   // ボタン押下
                        this.ButtonPushEvent(info);
                        break;

                    case 'E':   // エラーデータ
                        info.Data = "NAK";
                        this.ResponseEvent(info);
                        break;

                    default:
                        break;
                }
            }
            catch
            {
                throw;
            }
        }
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
                byte sum = Def.ETX;
                foreach (byte b in dat)
                {
                    sum ^= b;
                }

                // BCCを取得
                byte[] bcc = new byte[2];
                bcc[0] = (byte)((sum & 0x0f) | '0');
                bcc[1] = (byte)((sum >> 4) | '0');

                return Encoding.ASCII.GetString(bcc);
            }
            catch
            {
                throw;
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

            while (this.transData != null)
            {
                Thread.Sleep(10);
            }

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
        /// 表示器ボタン 共通
        /// </summary>
        /// <param name="group">分岐アドレス（1～20）</param>
        /// <param name="address">表示器アドレス（1～195）</param>
        /// <param name="button">ボタン番号（1：赤、2：黄、3：緑、4：白、5：青）</param>
        /// <param name="mode">動作（点灯、点滅、消灯）</param>
        /// <param name="display">セグメント表示（9文字）</param>
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
            		
                return this.SegmentCommon(group, address, mode, display, priority);
            }

            // 表示文字の判定
            string disp = this.DispHantei(display);

            // 送信コマンドの追加
            this.CommandAdd("C", id, btn, disp, priority);

            return (int)Def.Err.None;
        }

        /// <summary>
        /// 表示器ボタン点灯
        /// </summary>
        /// <param name="group">分岐アドレス（1～20）</param>
        /// <param name="address">表示器アドレス（1～195）</param>
        /// <param name="button">ボタン番号（1：赤、2：黄、3：緑、4：白、5：青）</param>
        /// <param name="display">セグメント表示（9文字）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int DisplayOn(int group, int address, int button, string display, bool priority = false)
        {
            return this.DisplayCommon(group, address, button, Def.BTN_ON, display, priority);
        }

        /// <summary>
        /// 表示器ボタン点滅
        /// </summary>
        /// <param name="group">分岐アドレス（1～20）</param>
        /// <param name="address">表示器アドレス（1～195）</param>
        /// <param name="button">ボタン番号（1：赤、2：黄、3：緑、4：白、5：青）</param>
        /// <param name="display">セグメント表示（9文字）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int DisplayBlink(int group, int address, int button, string display, bool priority = false)
        {
            return this.DisplayCommon(group, address, button, Def.BTN_BLK, display, priority);
        }

        /// <summary>
        /// 表示器ボタン消灯
        /// </summary>
        /// <param name="group">分岐アドレス（1～20）</param>
        /// <param name="address">表示器アドレス（1～195）</param>
        /// <param name="button">ボタン番号（1：赤、2：黄、3：緑、4：白、5：青）</param>
        /// <param name="display">セグメント表示（9文字）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int DisplayOff(int group, int address, int button, string display, bool priority = false)
        {
            return this.DisplayCommon(group, address, button, Def.BTN_OFF, display, priority);
        }
        #endregion

        #region セグメントのみ
        /// <summary>
        /// セグメントのみ 共通
        /// </summary>
        /// <param name="group">分岐アドレス（1～20）</param>
        /// <param name="address">表示器アドレス（1～195）</param>
        /// <param name="mode">動作（表示、点滅、クリア）</param>
        /// <param name="display">セグメント表示（9文字）</param>
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
            this.CommandAdd("C", id, btn, disp, priority);

            return (int)Def.Err.None;
        }

        /// <summary>
        /// セグメントのみ表示
        /// </summary>
        /// <param name="group">分岐アドレス（1～20）</param>
        /// <param name="address">表示器アドレス（1～195）</param>
        /// <param name="display">セグメント表示（9文字）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int SegmentOn(int group, int address, string display, bool priority = false)
        {
            return this.SegmentCommon(group, address, Def.SEG_ON, display, priority);
        }

        /// <summary>
        /// セグメントのみ点滅
        /// </summary>
        /// <param name="group">分岐アドレス（1～20）</param>
        /// <param name="address">表示器アドレス（1～195）</param>
        /// <param name="display">セグメント表示（9文字）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int SegmentBlink(int group, int address, string display, bool priority = false)
        {
            return this.SegmentCommon(group, address, Def.SEG_BLK, display, priority);
        }

        /// <summary>
        /// セグメントのみクリア
        /// </summary>
        /// <param name="group">分岐アドレス（1～20）</param>
        /// <param name="address">表示器アドレス（1～195）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int SegmentOff(int group, int address, bool priority = false)
        {
            return this.SegmentCommon(group, address, Def.SEG_OFF, string.Empty, priority);
        }
        #endregion

        #region PICKINGランプのみ
        /// <summary>
        /// PICKINGランプのみ 共通
        /// </summary>
        /// <param name="group">分岐アドレス（1～20）</param>
        /// <param name="address">表示器アドレス（1～195）</param>
        /// <param name="mode">動作（表示、点滅、クリア）</param>
        /// <param name="priority">優先（true：する、false：しない）</param>
        /// <returns>ステータス</returns>
        private int Lamp(int group, int address, byte mode, bool priority)
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
            string disp = this.DispHantei(string.Empty);

            // 送信コマンドの追加
            this.CommandAdd("C", id, btn, disp, priority);

            return (int)Def.Err.None;
        }

        /// <summary>
        /// PICKINGランプのみ点灯
        /// </summary>
        /// <param name="group">分岐アドレス（1～20）</param>
        /// <param name="address">表示器アドレス（1～195）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int LampOn(int group, int address, bool priority = false)
        {
            return this.Lamp(group, address, Def.LMP_ON, priority);
        }

        /// <summary>
        /// PICKINGランプのみ点滅
        /// </summary>
        /// <param name="group">分岐アドレス（1～20）</param>
        /// <param name="address">表示器アドレス（1～195）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int LampBlink(int group, int address, bool priority = false)
        {
            return this.Lamp(group, address, Def.LMP_BLK, priority);
        }

        /// <summary>
        /// PICKINGランプのみ消灯
        /// </summary>
        /// <param name="group">分岐アドレス（1～20）</param>
        /// <param name="address">表示器アドレス（1～195）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int LampOff(int group, int address, bool priority = false)
        {
            return this.Lamp(group, address, Def.LMP_OFF, priority);
        }
        #endregion

        #region LEDのみ
        /// <summary>
        /// LEDのみ 共通
        /// </summary>
        /// <param name="group">分岐アドレス（1～20）</param>
        /// <param name="address">表示器アドレス（1～195）</param>
        /// <param name="button">ボタン番号（1：赤、2：黄、3：緑、4：白、5：青）</param>
        /// <param name="mode">動作（点灯、点滅、消灯）</param>
        /// <param name="priority">優先（true：する、false：しない）</param>
        /// <returns>ステータス</returns>
        private int LedCommon(int group, int address, int button, byte mode,  bool priority)
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

            // 表示文字の判定
            string disp = this.DispHantei(string.Empty);

            // 送信コマンドの追加
            this.CommandAdd("L", id, btn, disp, priority);

            return (int)Def.Err.None;
        }

        /// <summary>
        /// LEDのみ点灯
        /// </summary>
        /// <param name="group">分岐アドレス（1～20）</param>
        /// <param name="address">表示器アドレス（1～195）</param>
        /// <param name="button">ボタン番号（1：赤、2：黄、3：緑、4：白、5：青）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int LedOn(int group, int address, int button, bool priority = false)
        {
            return this.LedCommon(group, address, button, Def.BTN_ON, priority);
        }

        /// <summary>
        /// LEDのみ点滅
        /// </summary>
        /// <param name="group">分岐アドレス（1～20）</param>
        /// <param name="address">表示器アドレス（1～195）</param>
        /// <param name="button">ボタン番号（1：赤、2：黄、3：緑、4：白、5：青）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int LedBlink(int group, int address, int button, bool priority = false)
        {
            return this.LedCommon(group, address, button, Def.BTN_BLK, priority);
        }

        /// <summary>
        /// LEDのみ消灯
        /// </summary>
        /// <param name="group">分岐アドレス（1～20）</param>
        /// <param name="address">表示器アドレス（1～195）</param>
        /// <param name="button">ボタン番号（1：赤、2：黄、3：緑、4：白、5：青）</param>
        /// <param name="priority">優先（true：する、false：しない、デフォルト：false）</param>
        /// <returns>ステータス</returns>
        public int LedOff(int group, int address, int button, bool priority = false)
        {
            return this.LedCommon(group, address, button, Def.BTN_OFF, priority);
        }
        #endregion
        #region 判定
        /// <summary>
        /// IDの判定
        /// </summary>
        /// <param name="group">分岐アドレス（1～20）</param>
        /// <param name="address">表示器アドレス（1～195）</param>
        /// <returns>ID文字列</returns>
        private string IdHantei(int group, int address)
        {
            // 分岐アドレスが範囲外
            if ((group < 1) || (group > 20))
            {
                return null;
            }

            // 表示器アドレスが範囲外
            if ((address < 1) || (address > 999))
            {
                return null;
            }

            return group.ToString("00") + address.ToString("000");
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
            if (button < 0 || button > 9)
            {
                return null;
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
            if (display.Length <= 9)
            {
                return display.PadRight(9, ' ');
            }
            else
            {
                return display.Substring(0, 9);
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

            while (this.transData != null)
            {
                Thread.Sleep(10);
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
            if (this.transData == null)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        #endregion

        #region タイマー
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
            this.errCnt = 0;
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
                lock( lockObj_log ) {
                    if ( this.logList.Count > 10000 ) //	GetLog呼び出し無し対応
                        this.logList.Clear();
                }
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
        /// ログリストの取得
        /// </summary>
        /// <param name="info">ログリストの取得</param>
        public List<KeyValuePair<DateTime, string>> GetLog()
        {
            List<KeyValuePair<DateTime, string>> lst=new List<KeyValuePair<DateTime,string>>();
            lock( lockObj_log ) {
                lst.AddRange(this.logList);
                this.logList.Clear();
            }

            return lst;
        }
        #endregion
        #endregion
    }
}
