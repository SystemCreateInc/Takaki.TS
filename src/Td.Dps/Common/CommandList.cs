using System.Text;

namespace Td.Dps
{
    internal class CommandList
    {
        #region 定義
        #region 変数
        /// <summary>
        /// コマンド
        /// </summary>
        private string command;

        /// <summary>
        /// ID
        /// </summary>
        private string id;

        /// <summary>
        /// 優先度
        /// </summary>
        private bool priority;

        /// <summary>
        /// タイムアウトカウント
        /// </summary>
        private int timeout;

        #endregion

        #region プロパティ
        /// <summary>
        /// コマンド
        /// </summary>
        internal string Command
        {
            get { return this.command; }
        }

        /// <summary>
        /// コマンドのID
        /// </summary>
        internal string Id
        {
            get { return this.id; }
        }

        /// <summary>
        /// 優先度
        /// </summary>
        internal bool Priority
        {
            get { return this.priority; }
        }

        /// <summary>
        /// タイムアウト時間
        /// </summary>
        internal int Timeout
        {
			set { this.timeout = value; }
            get { return this.timeout; }
        }
        #endregion
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="header">ヘッダ</param>
        /// <param name="id">ID</param>
        /// <param name="button">ボタン</param>
        /// <param name="disp">表示</param>
        /// <param name="priority">優先度</param>
        internal CommandList(string header, string id, string button, string disp, bool priority)
        {
            // 文字列の連結
            StringBuilder builder = new StringBuilder();
            builder.Append(header);
            builder.Append(id);
            builder.Append(button);
            builder.Append(disp);

            // プロパティ設定
            this.command = builder.ToString();
            this.id = id;
            this.priority = priority;
            this.timeout = 100;
        }
        #endregion
    }
}
