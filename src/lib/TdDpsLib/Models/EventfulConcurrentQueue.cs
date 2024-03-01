using LogLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Controls;

namespace TdDpsLib.Models
{
    /// <summary>
    /// ロックを呼ばれた順に処理する
    /// </summary>
    public class OrderingLockInstance
    {
        /// <summary>
        /// 
        /// </summary>
        protected string Identifier { get; set; } = string.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    internal class OrderingLockManager
    {
        /// <summary>
        /// 
        /// </summary>
        static protected Dictionary<OrderingLockInstance, Queue<OrderingLock>> lockQueue = new Dictionary<OrderingLockInstance, Queue<OrderingLock>>();

        /// <summary>
        /// 
        /// </summary>
        static protected OrderingLockManager? instance = null;

        /// <summary>
        /// 
        /// </summary>
        public static OrderingLockManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockQueue)
                    {
                        if (instance == null)
                        {
                            instance = new OrderingLockManager();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locker"></param>
        /// <param name="lockInstance"></param>
        public void EnQueue(OrderingLock locker, OrderingLockInstance lockInstance)
        {
            lock (lockQueue)
            {
                if (lockQueue.ContainsKey(lockInstance) == true)
                {
                    // 誰か忙しい人がいる
                    lockQueue[lockInstance].Enqueue(locker);
                }
                else
                {
                    lockQueue.Add(lockInstance, new Queue<OrderingLock>());

                    // 走りだせ!
                    locker.Ready.Set();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locker"></param>
        /// <param name="lockInstance"></param>
        public void Finished(OrderingLock locker, OrderingLockInstance lockInstance)
        {
            lock (lockQueue)
            {
                if (lockQueue.ContainsKey(lockInstance) == true)
                {
                    if (lockQueue[lockInstance].Count == 0)
                    {
                        // 終了
                        lockQueue.Remove(lockInstance);
                    }
                    else
                    {
                        // 次へ
                        lockQueue[lockInstance].Dequeue().Ready.Set();
                    }
                }
                else
                {
                    // ERROR!(あってはならないルート)
                    throw new InvalidOperationException("Method calling sequence error.");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected OrderingLockManager()
        {
            // NOP
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <example>
    /// このクラスの一般的な利用方法を次の例に示します。
    /// <code><![CDATA[
    /// OrderingLockInstance lockInstance = new OrderingLockInstance();
    ///
    /// ...
    /// 
    /// using (new OrderingLock(lockInstance))
    /// {
    ///     // Do something
    /// }
    /// ]]></code>
    /// </example>
    public class OrderingLock : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        protected AutoResetEvent ready = new AutoResetEvent(false);

        /// <summary>
        /// 
        /// </summary>
        protected OrderingLockInstance? lockInstance = null;

        /// <summary>
        /// 
        /// </summary>
        internal AutoResetEvent Ready
        {
            get
            {
                return ready;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lockInstance"></param>
        public OrderingLock(OrderingLockInstance lockInstance)
        {
            this.lockInstance = lockInstance;
            OrderingLockManager.Instance.EnQueue(this, lockInstance);

            ready.WaitOne();
        }

        /// <summary>
        /// 
        /// </summary>
        ~OrderingLock()
        {
            // using パターンを使わない人のために

            // 次の人へ
            if (lockInstance != null)
            {
                OrderingLockManager.Instance.Finished(this, lockInstance);
                lockInstance = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            // 次へ
            if (lockInstance != null)
            {
                OrderingLockManager.Instance.Finished(this, lockInstance);
                lockInstance = null;
            }
        }
    }

    public sealed class EventfulConcurrentQueue<T> : ConcurrentQueue<T>
    {
        private readonly ConcurrentQueue<T> Queue = new ConcurrentQueue<T>();

        public event EventHandler? ItemEnqueued = null;
        public event EventHandler<ItemEventArgs<T>>? ItemDequeued = null;

        OrderingLockInstance lockInstance = new OrderingLockInstance();

        public new void Enqueue(T item)
        {
            if (item == null)
                return;

            using (new OrderingLock(lockInstance))
            {
                System.Diagnostics.Debug.WriteLine("QueLock_Enqueue");

                QueCommand newitem = (QueCommand)(object)item;
                if (newitem == null)
                    return;

                System.Diagnostics.Debug.WriteLine("Queue.Enqueue start addr:" + newitem.Addrdata.TddAddr.ToString() + " color:" + newitem.Color.ToString() + " test:" + newitem.Text);

                if (newitem.Priority == true)
                {
                    System.Diagnostics.Debug.WriteLine("Queue.Enqueue priority addr:" + newitem.Addrdata.TddAddr.ToString() + " color:" + newitem.Color.ToString() + " test:" + newitem.Text);

                    // 優先順位を変える
                    //                lock (_lockObj)
                    {
                        var type = typeof(T);
                        if (type == typeof(QueCommand))
                        {
                            List<QueCommand> ques = new List<QueCommand>();


                            while (Queue.Any())
                            {
                                T? que;
                                if (Queue.TryDequeue(out que) == false)
                                {
                                    break;
                                }
                                if (que != null)
                                {
                                    QueCommand p = new QueCommand((object)que as QueCommand);
                                    if (p.Addrdata.Stno == newitem.Addrdata.Stno && p.Addrdata.TddGroup == newitem.Addrdata.TddGroup && p.Addrdata.TddAddr == newitem.Addrdata.TddAddr)
                                    {
                                        Syslog.Info($"EventfulConcurrentQueue置き換え::addr=[{p.Addrdata.TdUnitAddrCode}] text[{p.Text}]→[{newitem.Text}]  on[{newitem.On}]→[{p.On}]  text[{newitem.Blink}]→[{p.Blink}] color[{newitem.Color}]→[{p.Color}]");

                                        // 表示部コピー
                                        p.Text = newitem.Text;

                                        // 点灯状態を最新点灯状態へ変更
                                        if (p.Color == newitem.Color)
                                        {
                                            p.On = newitem.On;
                                            p.Blink = p.On == false ? false : newitem.Blink;
                                        }
                                    }

                                    ques.Add(p);
                                }
                            }
#if false
                            var sels = ques.Where(x => x.Addrdata.Group == newitem.Addrdata.Group && x.Addrdata.Addr == newitem.Addrdata.Addr);

                            // 表示部を今回のデータに置き換えする
                            if (sels != null)
                            {
                                foreach (var p in sels)
                                {
                                    // 表示部コピー
                                    p.Text = newitem.Text;

                                    // 点灯状態を最新点灯状態へ変更
                                    if (p.Color == newitem.Color)
                                    {
                                        p.On = newitem.On;
                                        p.Blink = p.On == false ? false : newitem.Blink;
                                    }
                                }
                            }
#endif
                            bool bAppend = false;

                            // queを戻す
                            foreach (var p in ques)
                            {
                                Queue.Enqueue((T)(object)p);

                                if (bAppend == false && p.Priority==false)
                                {
                                    bAppend = true;
                                    Queue.Enqueue(item);
                                }
                            }

                            if (bAppend == false)
                            {
                                Queue.Enqueue(item);
                            }
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Queue.Enqueue in addr:" + newitem.Addrdata.TddAddr.ToString() + " color:" + newitem.Color.ToString() + " test:" + newitem.Text);

                    Queue.Enqueue(item);
                }
            }
            System.Diagnostics.Debug.WriteLine("QueLock_Enqueue_exit");

            OnItemEnqueued();
        }

        public new int Count
        {
            get
            {
                using (new OrderingLock(lockInstance))
                {
                    System.Diagnostics.Debug.WriteLine("QueLock_cnt");
                    return Queue.Count;
                }
            }
        }


        public new bool TryDequeue(out T? result)
        {
            using (new OrderingLock(lockInstance))
            {
                System.Diagnostics.Debug.WriteLine("QueLock_TryDequeuet");
                var success = Queue.TryDequeue(out result);
                if (success)
                {
                    OnItemDequeued(result);
                }
                System.Diagnostics.Debug.WriteLine("QueLock_TryDequeuet_exit");
                return success;
            }
        }

        public new bool TryPeek(out T? result)
        {
            return Queue.TryPeek(out result);
        }


        public new void Clear()
        {
            using (new OrderingLock(lockInstance))
            {
                System.Diagnostics.Debug.WriteLine("QueLock_Clear");
                while (Queue.Any())
                {
                    if (Queue.TryDequeue(out _) == false)
                    {
                        break;
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine("QueLock_Clear_exit");
        }

        void OnItemEnqueued()
        {
            ItemEnqueued?.Invoke(this, EventArgs.Empty);

            // オーバーフローを起こすのでWait
            Thread.Sleep(1);
        }

        void OnItemDequeued(T? item)
        {
            if (item == null)
                return;

            ItemDequeued?.Invoke(this, new ItemEventArgs<T> { Item = item });

            // オーバーフローを起こすのでWait
            Thread.Sleep(1);
        }
    }

    public sealed class ItemEventArgs<T> : EventArgs
    {
        public T? Item { get; set; }
    }
}
