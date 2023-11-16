using System;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace Td.Dps
{
    internal class EventWorker
    {
        #region 定義
        #region 定数
        /// <summary>
        /// 同期
        /// </summary>
        private static readonly object sync = new object();
        #endregion

        #region 変数
        /// <summary>
        /// イベントの非同期
        /// </summary>
        private bool isAsync;
        #endregion
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="async"></param>
        internal EventWorker(bool async)
        {
            this.isAsync = async;
        }
        #endregion

        #region メソッド
        #region ボタン押下イベント
        /// <summary>
        /// ボタン押下イベント
        /// </summary>
        /// <param name="info"></param>
        internal void ButtonPushEvent(Def.EventInfo info)
        {
            if (this.isAsync == true)
            {
                // 非同期処理
                AsyncCallback callback = new AsyncCallback(this.ButtonPushAsync);
                IAsyncResult ar = info.ButtonPush.BeginInvoke(info.Group, info.Address, info.Button, callback, null);
            }
            else
            {
                // 同期処理
                WaitCallback callback = new WaitCallback(this.ButtonPushWait);
                ThreadPool.QueueUserWorkItem(callback, info);
            }
        }

        /// <summary>
        /// 表示器ボタン押下のコールバック（同期）
        /// </summary>
        /// <param name="obj"></param>
        private void ButtonPushWait(object obj)
        {
            lock (sync)
            {
                Def.EventInfo info = (Def.EventInfo)obj;
                info.ButtonPush.Invoke(info.Group, info.Address, info.Button);
            }
        }

        /// <summary>
        /// 表示器ボタン押下のコールバック（非同期）
        /// </summary>
        /// <param name="result"></param>
        private void ButtonPushAsync(IAsyncResult result)
        {
            AsyncResult asyncResult = (AsyncResult)result;
            Event.ButtonPushEventHandler res = (Event.ButtonPushEventHandler)asyncResult.AsyncDelegate;
            res.EndInvoke(result);
        }
        #endregion

        #region 表示器応答イベント
        /// <summary>
        /// 表示器応答イベント
        /// </summary>
        /// <param name="info"></param>
        internal void ResponseEvent(Def.EventInfo info)
        {
            if (this.isAsync == true)
            {
                // 非同期処理
                AsyncCallback callback = new AsyncCallback(this.ResponseAsync);
                IAsyncResult ar = info.Response.BeginInvoke(info.Group, info.Address, info.Data, callback, null);
            }
            else
            {
                // 同期処理
                WaitCallback callback = new WaitCallback(this.ResponseWait);
                ThreadPool.QueueUserWorkItem(callback, info);
            }
        }

        /// <summary>
        /// 表示器応答のコールバック（同期）
        /// </summary>
        /// <param name="obj"></param>
        private void ResponseWait(object obj)
        {
            lock (sync)
            {
                Def.EventInfo info = (Def.EventInfo)obj;
                info.Response.Invoke(info.Group, info.Address, info.Data);
            }
        }

        /// <summary>
        /// 表示器応答のコールバック（非同期）
        /// </summary>
        /// <param name="result"></param>
        private void ResponseAsync(IAsyncResult result)
        {
            AsyncResult asyncResult = (AsyncResult)result;
            Event.ResponseEventHandler res = (Event.ResponseEventHandler)asyncResult.AsyncDelegate;
            res.EndInvoke(result);
        }
        #endregion

        #region ログ通知イベント
        /// <summary>
        /// ログ通知イベント
        /// </summary>
        /// <param name="info"></param>
        internal void LogEvent(Def.EventInfo info)
        {
            ////if (this.isAsync == true)
            ////{
            ////    // 非同期処理
            ////    AsyncCallback callback = new AsyncCallback(this.LogAsync);
            ////    IAsyncResult ar = info.Log.BeginInvoke(info.Data, callback, null);
            ////}
            ////else
            ////{
            ////    // 同期処理
            ////    WaitCallback callback = new WaitCallback(this.LogWait);
            ////    ThreadPool.QueueUserWorkItem(callback, info);
            ////}

            // 非同期処理
            AsyncCallback callback = new AsyncCallback(this.LogAsync);
            IAsyncResult ar = info.Log.BeginInvoke(info.Data, callback, null);
        }

        /////// <summary>
        /////// ログ通知のコールバック（同期）
        /////// </summary>
        /////// <param name="obj"></param>
        ////private void LogWait(object obj)
        ////{
        ////    lock (sync)
        ////    {
        ////        Def.EventInfo info = (Def.EventInfo)obj;
        ////        info.Log.Invoke(info.Data);
        ////    }
        ////}

        /// <summary>
        /// ログ通知のコールバック（非同期）
        /// </summary>
        /// <param name="result"></param>
        private void LogAsync(IAsyncResult result)
        {
            AsyncResult asyncResult = (AsyncResult)result;
            Event.LogEventHandler res = (Event.LogEventHandler)asyncResult.AsyncDelegate;
            res.EndInvoke(result);
        }
        #endregion

        #region ボタン完了イベント
        /// <summary>
        /// ボタン完了イベント
        /// </summary>
        /// <param name="info"></param>
        internal void ButtonDoneEvent(Def.EventInfo info)
        {
            if (this.isAsync == true)
            {
                // 非同期処理
                AsyncCallback callback = new AsyncCallback(this.ButtonDoneAsync);
                IAsyncResult ar = info.ButtonDone.BeginInvoke(info.Group, info.Address, info.Button, info.Ctl, callback, null);
            }
            else
            {
                // 同期処理
                WaitCallback callback = new WaitCallback(this.ButtonDoneWait);
                ThreadPool.QueueUserWorkItem(callback, info);
            }
        }

        /// <summary>
        /// 表示器ボタン完了のコールバック（同期）
        /// </summary>
        /// <param name="obj"></param>
        private void ButtonDoneWait(object obj)
        {
            lock (sync)
            {
                Def.EventInfo info = (Def.EventInfo)obj;
                info.ButtonDone.Invoke(info.Group, info.Address, info.Button, info.Ctl);
            }
        }

        /// <summary>
        /// 表示器ボタン完了のコールバック（非同期）
        /// </summary>
        /// <param name="result"></param>
        private void ButtonDoneAsync(IAsyncResult result)
        {
            AsyncResult asyncResult = (AsyncResult)result;
            Event.ButtonDoneEventHandler res = (Event.ButtonDoneEventHandler)asyncResult.AsyncDelegate;
            res.EndInvoke(result);
        }
        #endregion

        #region BarCodeイベント
        /// <summary>
        /// 表示器応答イベント
        /// </summary>
        /// <param name="info"></param>
        internal void BarCoodeEvent(Def.EventInfo info)
        {
            if (this.isAsync == true)
            {
                // 非同期処理
                AsyncCallback callback = new AsyncCallback(this.BarCodeAsync);
                IAsyncResult ar = info.BarCodeRcv.BeginInvoke(info.Group, info.Address, info.Button, info.Data, callback, null);
            }
            else
            {
                // 同期処理
                WaitCallback callback = new WaitCallback(this.BarCodeWait);
                ThreadPool.QueueUserWorkItem(callback, info);
            }
        }

        /// <summary>
        /// 表示器応答のコールバック（同期）
        /// </summary>
        /// <param name="obj"></param>
        private void BarCodeWait(object obj)
        {
            lock (sync)
            {
                Def.EventInfo info = (Def.EventInfo)obj;
                info.BarCodeRcv.Invoke(info.Group, info.Address, info.Button, info.Data);
            }
        }

        /// <summary>
        /// 表示器応答のコールバック（非同期）
        /// </summary>
        /// <param name="result"></param>
        private void BarCodeAsync(IAsyncResult result)
        {
            AsyncResult asyncResult = (AsyncResult)result;
            Event.BarCoodeEventHandler res = (Event.BarCoodeEventHandler)asyncResult.AsyncDelegate;
            res.EndInvoke(result);
        }
        #endregion
        #endregion
    }
}
