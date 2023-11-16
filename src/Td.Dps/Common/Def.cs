using System;

namespace Td.Dps
{
    /// <summary>
    /// 共通定義
    /// </summary>
    internal class Def
    {
        #region 制御コード
        /// <summary>
        /// Start Of Text（テキスト開始）
        /// </summary>
        internal const byte STX = 0x02;

        /// <summary>
        /// End Of Text（テキスト終了）
        /// </summary>
        internal const byte ETX = 0x03;

        /// <summary>
        /// End Of Transmission（伝送終了）
        /// </summary>
        internal const byte EOT = 0x04;

        /// <summary>
        /// Enquery（問い合わせ）
        /// </summary>
        internal const byte ENQ = 0x05;

        /// <summary>
        /// Acknowledgement（肯定応答）
        /// </summary>
        internal const byte ACK = 0x06;

        /// <summary>
        /// Negative Acknowledgement（否定応答）
        /// </summary>
        internal const byte NAK = 0x15;
        #endregion

        #region 動作コード
        /// <summary>
        /// ボタン点灯（0）
        /// </summary>
        internal const byte BTN_ON = 0x30;

        /// <summary>
        /// ボタン点滅（@）
        /// </summary>
        internal const byte BTN_BLK = 0x40;

        /// <summary>
        /// ボタン消灯（P）
        /// </summary>
        internal const byte BTN_OFF = 0x50;

        /// <summary>
        /// セグメント点灯（7）
        /// </summary>
        internal const byte SEG_ON = 0x37;

        /// <summary>
        /// セグメント点滅（G）
        /// </summary>
        internal const byte SEG_BLK = 0x47;

        /// <summary>
        /// セグメント消灯（W）
        /// </summary>
        internal const byte SEG_OFF = 0x57;

        /// <summary>
        /// PICKINGランプ点灯（8）
        /// </summary>
        internal const byte LMP_ON = 0x38;

        /// <summary>
        /// PICKINGランプ点滅（H）
        /// </summary>
        internal const byte LMP_BLK = 0x48;

        /// <summary>
        /// PICKINGランプ消灯（X）
        /// </summary>
        internal const byte LMP_OFF = 0x58;
        #endregion

        #region スレッド
        /// <summary>
        /// 受信インターバル
        /// </summary>
        internal const int REC_INTERVAL = 5;
      //internal const int REC_INTERVAL = 5;

        /// <summary>
        /// 送信インターバル
        /// </summary>
         internal const int TRA_INTERVAL = 5;
       //internal const int TRA_INTERVAL = 25;
        #endregion

        #region エラーコード
        /// <summary>
        /// エラー定義
        /// </summary>
        internal enum Err : int
        {
            /// <summary>
            /// エラー無し
            /// </summary>
            None = 0,

            /// <summary>
            /// シリアルポートのエラー
            /// </summary>
            Port = 1,

            /// <summary>
            /// パラメータのエラー
            /// </summary>
            Id = 2,
        }
        #endregion

        #region イベント情報
        /// <summary>
        /// イベント情報
        /// </summary>
        internal struct EventInfo
        {
            /// <summary>
            /// 無線グループ or 分岐アドレス
            /// </summary>
            internal int Group;

            /// <summary>
            /// 表示器アドレス
            /// </summary>
            internal int Address;

            /// <summary>
            /// ボタン番号
            /// </summary>
            internal int Button;

            /// <summary>
            /// 応答内容
            /// </summary>
            internal string Data;

            /// <summary>
            /// CTLボタン
            /// </summary>
            internal bool Ctl;

            /// <summary>
            /// ボタン押下イベント
            /// </summary>
            internal Event.ButtonPushEventHandler ButtonPush;

            /// <summary>
            /// 表示器応答イベント
            /// </summary>
            internal Event.ResponseEventHandler Response;

            /// <summary>
            /// ログ通知イベント
            /// </summary>
            internal Event.LogEventHandler Log;

            /// <summary>
            /// ボタン完了イベント
            /// </summary>
            internal Event.ButtonDoneEventHandler ButtonDone;
            
            /// <summary>
            /// ボタン完了イベント
            /// </summary>
            internal Event.BarCoodeEventHandler BarCodeRcv;
        }
        #endregion
    }
}
