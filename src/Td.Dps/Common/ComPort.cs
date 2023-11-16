using System.IO.Ports;
using System.Text;

namespace Td.Dps
{
    /// <summary>
    /// RS232Cの制御
    /// </summary>
    internal class ComPort
    {
        #region 定義
        #region 変数
        /// <summary>
        /// シリアルポートのオブジェクト
        /// </summary>
        private SerialPort port;
        #endregion

        #region イベント
        /// <summary>
        /// データ受信のデリゲート宣言
        /// </summary>
        /// <param name="bytes">受信文字列</param>
        public delegate void ReceivedDataHandler(byte[] bytes);

        /// <summary>
        /// データ受信のイベントハンドラ
        /// </summary>
        public event ReceivedDataHandler ReceivedData;
        #endregion
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="portName">使用するポート</param>
        /// <param name="baudRate">ボーレート</param>
        /// <param name="parity">パリティ</param>
        internal ComPort(string portName, int baudRate, Parity parity)
        {
            try
            {
                // シリアルポートを開く
                this.port = new SerialPort(portName, baudRate, parity, 8, StopBits.One);
                this.port.Open();
            }
            catch
            {
                throw;
            }

            // バッファをクリア
            this.port.DiscardInBuffer();
            this.port.DiscardOutBuffer();

            // イベントの追加
            //this.port.DataReceived += this.Port_DataReceived;
        }
        #endregion

        #region イベント
        /// <summary>
        /// データ受信のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (this.ReceivedData != null)
            {
                // 受信イベントを発生
                this.ReceivedData(this.Read());
            }
        }
        #endregion

        #region メソッド
        /// <summary>
        /// 閉じる
        /// </summary>
        internal void Close()
        {
            // 開いていない場合、何もしない
            if (this.port.IsOpen == false)
            {
                return;
            }

            try
            {
                this.port.Close();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 受信
        /// </summary>
        /// <returns></returns>
        internal byte[] Read()
        {
            // 長さを取得
            int len = this.port.BytesToRead;

            // 受信データ取得
            byte[] buffer = new byte[len];
            try
            {
                this.port.Read(buffer, 0, len);
            }
            catch
            {
                throw;
            }

            return buffer;
        }

        /// <summary>
        /// 文字列送信
        /// </summary>
        /// <param name="text">文字列</param>
        internal void Write(string text)
        {
            this.port.Write(text);
        }

        /// <summary>
        /// 1バイト送信
        /// </summary>
        /// <param name="text">バイト文字</param>
        internal void Write(byte text)
        {
            byte[] bytes = { text };
            try
            {
                this.port.Write(bytes, 0, 1);
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
