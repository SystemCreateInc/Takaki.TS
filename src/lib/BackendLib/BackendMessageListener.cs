using System;
using System.IO;
using System.Threading.Tasks;

namespace BackendLib
{
    public class BackendMessageListener : IDisposable
    {
        private BackendService? _backend;
        private Task? _task;

        public BackendMessageListener()
        {
        }

        public BackendMessageListener(string capability, BackendFunc func)
        {
            Start(capability, func);
        }

        public void Dispose()
        {
            Stop();
        }

        public Task Start(string capability, BackendFunc func)
        {
            var name = Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);
            _backend = new BackendService(ClientType.BE_CLIENTTYPE_PROCESSOR, name, capability, "root", "");
            _task = Task.Run(() =>
            {
                var mta = new MessageHandler(_backend, func);
                mta.Run();
            });

            return _task;
        }

        public void Stop()
        {
            if (_task == null)
                return;

            _backend?.Cancel();
            _task.Wait();
            _task = null;

            _backend?.Dispose();
        }
    }
}
