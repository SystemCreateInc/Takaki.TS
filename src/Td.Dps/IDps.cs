using System;

namespace Td.Dps
{
    /// <summary>
    /// インターフェースの登録
    /// </summary>
    public interface IDps
    {
        #region プロパティ
        /// <summary>
        /// ドライバ種別
        /// </summary>
        int TypeDriver { get; }

        /// <summary>
        /// ポート番号
        /// </summary>
        string Port { get; }
        #endregion

        #region イベント
        /// <summary>
        /// 表示器ボタン押下のイベントハンドラ
        /// </summary>
        event Event.ButtonPushEventHandler ButtonPush;

        /// <summary>
        /// 表示器応答のイベントハンドラ
        /// </summary>
        event Event.ResponseEventHandler Response;

        /// <summary>
        /// 通信ログ通知のイベントハンドラ
        /// </summary>
        event Event.LogEventHandler Log;
        #endregion

        #region メソッド
        /// <summary>
        /// ポートを開く
        /// </summary>
        int Open(string portName);

        /// <summary>
        /// ポートを開く
        /// </summary>
        int Open(string portName, int baudRate);

        /// <summary>
        /// ポートを閉じる
        /// </summary>
        int Close();

        /// <summary>
        /// 表示器点灯
        /// </summary>
        int DisplayOn(int group, int address, int button, string display, bool priority);

        /// <summary>
        /// 表示器消灯
        /// </summary>
        int DisplayOff(int group, int address, int button, string display, bool priority);

        /// <summary>
        /// 表示器点滅
        /// </summary>
        int DisplayBlink(int group, int address, int button, string display, bool priority);

        /// <summary>
        /// セグメントのみ表示
        /// </summary>
        int SegmentOn(int group, int address, string display, bool priority);

        /// <summary>
        /// セグメントのみ点滅
        /// </summary>
        int SegmentBlink(int group, int address, string display, bool priority);

        /// <summary>
        /// セグメントのみクリア
        /// </summary>
        int SegmentOff(int group, int address, bool priority);

        /// <summary>
        /// 送信待ちデータのクリア
        /// </summary>
        void WaitTransClear();

        /// <summary>
        /// 送信待ちデータのカウント
        /// </summary>
        int WaitTransCount();

        /// <summary>
        /// 応答待ちデータのカウント
        /// </summary>
        /// <returns></returns>
        int WaitRespoCount();
        #endregion
    }
}
