using BackendLib;
using LogLib;

namespace ProcessorLib
{
    public class BackendMessageProcessorHandler
    {
        private CancellationToken _cancellationToken;
        private IProcessorAdapter _processorAdapter;

        public BackendMessageProcessorHandler(IProcessorAdapter adapter)
        {
            _processorAdapter = adapter;
        }

        public void Run(CancellationToken token)
        {
            Syslog.Debug("Start BackendMessageProcessorHandler");

            //  バックエンドメッセージ処理
            _cancellationToken = token;
            using (var bml = new BackendMessageListener(_processorAdapter.Capability, BackendProc))
            {
                _cancellationToken.WaitHandle.WaitOne();
            }

            Syslog.Debug("Exit BackendMessageProcessorHandler");
        }

        private string? BackendProc(string[] ar, MessageHandler handler)
        {
            var response = new List<string>();

            try
            {
                var result = _processorAdapter.Invoke(ar, handler);
                if (result == null)
                {
                    return null;
                }

                response.Add(ar[0]);  //  target
                response.Add(ar[1]);  //  no
                response.Add("OK");
                response.Add(result);
            }
            catch (Exception? e)
            {
                var message = e.InnerException != null ? e.InnerException.Message : e.Message;

                response.Add(ar[0]);  //  target
                response.Add(ar.Length > 1 ? ar[1] : "");  //  no
                response.Add("ERR");
                response.Add(message);

                while (e != null)
                {
                    Syslog.Warn($"EXCEPTION: {e.Message}");
                    Syslog.Warn($"StackTrace: {e.StackTrace}");
                    e = e.InnerException;
                }
            }


            var text = string.Join("\n", response);

            return text;
        }
    }
}
