using LogLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace BackendLib
{
    public delegate string? BackendFunc(string[] args, MessageHandler handler);

    public class MessageHandler
    {
        public BackendFunc Func { get; }
        public BackendService Backend { get; }

        private Dictionary<string, int> processors = new Dictionary<string, int>();

        public MessageHandler(BackendService backend, BackendFunc func)
        {
            Backend = backend;
            Func = func;
        }

        public void Run()
        {
            Syslog.Debug("Start listenning backend message...");

            while (true)
            {
                try
                {
                    PreRunLoop();

                    var msg = Backend.GetMessage();
                    switch (msg.rc)
                    {
                        case BackendService.BE_ERROR_OK:
                            HandleMessage(msg);
                            break;

                        case BackendService.BE_ERROR_TIMEOUT:
                            break;

                        case BackendService.BE_ERROR_CANCELED:
                            goto exit;
                    }
                }
                catch (Exception e)
                {
                    Syslog.Error($"EXCEPTION: {e.Message}");
                }
            }

        exit:

            Syslog.Debug("BackendMessageListener: Exit task");
        }

        public bool TryGetProcessorId(string capability, out int id)
        {
            return processors.TryGetValue(capability, out id);
        }

        private bool requestedProcessorList = false;
        private void PreRunLoop()
        {
            if (!requestedProcessorList)
            {
                if (RequestProcessorList() == BackendService.BE_ERROR_OK)
                    requestedProcessorList = true;
            }
        }

        private int RequestProcessorList()
        {
            Syslog.Debug("Request processor list");
            return Backend.PostMessage(0, BackendService.BEM_LISTPROCESSORS, null, BackendService.BEMS_NEEDRESULT);
        }

        private void HandleMessage(BackendService.Message msg)
        {
            try
            {
                Backend.CurrentMessage = msg;

                switch (msg.message)
                {
                    case BackendService.BEM_REGISTERED:
                        OnRegist(msg);
                        break;

                    case BackendService.BEM_UNREGISTERED:
                        OnUnregist(msg);
                        break;

                    case BackendService.BEM_LISTCLIENTS:
                        OnListClients(msg);
                        break;

                    case BackendService.BEM_CORE:
                        OnCoreMessage(msg);
                        break;
                }
            }
            catch (Exception e)
            {
                Syslog.Error($"BackendMessageListener:Exception: ${e}");
            }
        }

        private void OnCoreMessage(BackendService.Message msg)
        {
            var sw = new Stopwatch();
            sw.Start();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var textData = Encoding.GetEncoding("SJIS").GetString(msg.data);
            Syslog.Debug($"BackendMessageListener:R:[{textData.Length}] {textData}");

            var ar = textData.Split('\n');
            var resultText = Func(ar, this);

            sw.Stop();
            Syslog.Debug($"BackendMessageListener:S:({sw.ElapsedMilliseconds}ms)[{resultText?.Length ?? 0}] {resultText}");

            if (resultText == null)
            {
                return;
            }

            var sendbuf = Encoding.GetEncoding("SJIS").GetBytes(resultText);
            Backend.PostMessage(msg.fromClientId, BackendService.BEM_CORE, sendbuf, BackendService.BEMS_RESULT);
        }

        private void OnListClients(BackendService.Message msg)
        {
            var br = new BinaryReader(new MemoryStream(msg.data));
            int count = br.ReadUInt16();
            int registSize = Marshal.SizeOf<be_regist>();

            for (int i = 0; i < count; ++i)
            {
                var bytes = br.ReadBytes(registSize);
                IntPtr ptr = Marshal.AllocCoTaskMem(registSize);
                Marshal.Copy(bytes, 0, ptr, registSize);
                var regist = Marshal.PtrToStructure<be_regist>(ptr);
                Marshal.FreeCoTaskMem(ptr);

                RegisterProcessor(regist);
            }
        }

        private void OnRegist(BackendService.Message msg)
        {
            var regist = GetRegistFromMessage(msg);
            RegisterProcessor(regist);
        }

        private void OnUnregist(BackendService.Message msg)
        {
            var regist = GetRegistFromMessage(msg);
            UnregisterProcessor(regist);
        }

        private void RegisterProcessor(be_regist regist)
        {
            if (!processors.ContainsKey(regist.capability))
            {
                processors.Add(regist.capability, regist.uid);
            }

            Syslog.Debug($"BackendMessageListener: Regist: {regist.capability}");
        }

        private void UnregisterProcessor(be_regist regist)
        {
            processors.Remove(regist.capability);
            Syslog.Debug($"BackendMessageListener: Unregist: {regist.capability}");
        }

        private be_regist GetRegistFromMessage(BackendService.Message msg)
        {
            int size = Marshal.SizeOf<be_regist>();
            IntPtr ptr = Marshal.AllocCoTaskMem(size);
            Marshal.Copy(msg.data, 0, ptr, size);
            var regist = Marshal.PtrToStructure<be_regist>(ptr);
            Marshal.FreeCoTaskMem(ptr);
            return regist;
        }
    }
}
