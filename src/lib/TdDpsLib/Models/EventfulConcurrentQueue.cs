using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TdDpsLib.Models
{
    public sealed class EventfulConcurrentQueue<T> : ConcurrentQueue<T>
    {
        private readonly ConcurrentQueue<T> Queue = new ConcurrentQueue<T>();
        private object _lockObj = new object();

        public event EventHandler? ItemEnqueued = null;
        public event EventHandler<ItemEventArgs<T>>? ItemDequeued = null;


        public new void Enqueue(T item)
        {
            if (item == null)
                return;

            lock (_lockObj)
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

                                if (bAppend == false)
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
                lock (_lockObj)
                {
                    System.Diagnostics.Debug.WriteLine("QueLock_cnt");
                    return Queue.Count;
                }
            }
        }


        public new bool TryDequeue(out T? result)
        {
            lock (_lockObj)
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
            lock (_lockObj)
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
            Thread.Sleep(10);
        }

        void OnItemDequeued(T? item)
        {
            if (item == null)
                return;

            ItemDequeued?.Invoke(this, new ItemEventArgs<T> { Item = item });

            // オーバーフローを起こすのでWait
            Thread.Sleep(10);
        }
    }

    public sealed class ItemEventArgs<T> : EventArgs
    {
        public T? Item { get; set; }
    }
}
