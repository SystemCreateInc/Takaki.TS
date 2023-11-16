using System;

namespace Td.Dps
{
    /// <summary>
    /// イベントのデリゲート宣言
    /// </summary>
    public class Event
    {
        /// <summary>
        /// 表示器ボタン押下のデリゲート宣言
        /// </summary>
        /// <param name="group">
        /// <para>Wireless  : 無線グループ（0～9）</para>
        /// <para>Wired     : 分岐アドレス（1～20）</para>
        /// <para>Wireless2 : 無線グループ（0～15）</para>
        /// </param>
        /// <param name="address">
        /// <para>Wireless  : 表示器 アドレス（1～50）</para>
        /// <para>Wired     : 表示器アドレス（1～195）</para>
        /// <para>Wireless2 : 表示器 アドレス（1～150）</para>
        /// </param>
        /// <param name="button">ボタン番号（1：赤、2：黄、3：緑、4：白、5：青、6：CTL）</param>
        public delegate void ButtonPushEventHandler(int group, int address, int button);

        /// <summary>
        /// 表示器ボタン完了のデリゲート宣言
        /// </summary>
        /// <param name="group">
        /// Wireless2 : 無線グループ（0～15）</param>
        /// <param name="address">
        /// Wireless2 : 表示器 アドレス（1～150）</param>
        /// <param name="button">ボタン番号（1：赤、2：黄、3：緑、4：白、5：青）</param>
        /// <param name="ctl">CTLボタン</param>
        public delegate void ButtonDoneEventHandler(int group, int address, int button, bool ctl);

        /// <summary>
        /// 表示器ボタン完了のデリゲート宣言
        /// </summary>
        /// <param name="group">
        /// Wireless2 : 無線グループ（0～15）</param>
        /// <param name="address">
        /// Wireless2 : 表示器 アドレス（1～150）</param>
        /// <param name="button">ボタン番号（1：赤、2：黄、3：緑、4：白、5：青）</param>
        /// <param name="ctl">CTLボタン</param>
        public delegate void BarCoodeEventHandler(int group, int address, int button, string data);

        /// <summary>
        /// 表示器応答のデリゲート宣言
        /// </summary>
        /// <param name="group">
        /// <para>Wireless  : 無線グループ（0～9）</para>
        /// <para>Wired     : 分岐アドレス（1～20）</para>
        /// <para>Wireless2 : 無線グループ（0～15）</para>
        /// </param>
        /// <param name="address">
        /// <para>Wireless  : 表示器 アドレス（1～50）</para>
        /// <para>Wired     : 表示器アドレス（1～195）</para>
        /// <para>Wireless2 : 表示器 アドレス（1～150）</para>
        /// </param>
        /// <param name="data">応答内容（ACK：送信成功、NAK：送信失敗、TIM:タイムアウト、
        /// HIG：容量大、MI D：容量中、LOW：容量小、OFF：容量無）</param>
        public delegate void ResponseEventHandler(int group, int address, string data);

        /// <summary>
        /// 通信ログ通知のデリゲート宣言
        /// </summary>
        /// <param name="log">通信ログ</param>
        public delegate void LogEventHandler(string log);
    }
}
