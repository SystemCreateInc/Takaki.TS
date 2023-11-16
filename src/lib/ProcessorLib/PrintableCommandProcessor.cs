using BackendLib;
using LogLib;
using ProcessorLib.Models;
using System.Text;

namespace ProcessorLib
{
    public abstract class PrintableCommandProcessor : ICommandProcessor, IMessageHook
    {
        private CommandRelay _commandRelay = new CommandRelay();

        public string GetLabelCapability(PrintLabelRequest request)
        {
            switch (GetPrinterType(request))
            {
                default:
                case PrinterType.Sato:
                    return "PROC_SATOPRINT";

                case PrinterType.Tec:
                    return "PROC_TECPRINT";

                case PrinterType.Zebra:
                    return "PROC_ZEBRAPRINT";
            }
        }

        public abstract IEnumerable<string> GetLabelData(PrintLabelRequest request);

        public abstract PrinterType GetPrinterType(PrintLabelRequest request);

        public bool Hook(string[] ar, MessageHandler handler)
        {
            handler.TryGetProcessorId("PROC_TECPRINT", out int tecSvrId);
            handler.TryGetProcessorId("PROC_ZEBRAPRINT", out int zebraSvrId);
            handler.TryGetProcessorId("PROC_SATOPRINT", out int satoSvrId);

            // プリントサーバーからの結果以外処理しない
            var msg = handler.Backend.CurrentMessage;
            if (msg.fromClientId != tecSvrId 
                && msg.fromClientId != zebraSvrId
                && msg.fromClientId != satoSvrId)
            {
                return false;
            }

            // プリントサーバーにリクエストしたクライアントを取得する
            var seq = int.Parse(ar[1]);
            if (!_commandRelay.TryGetRequestedClientInfo(seq, out CommandRelay.RequestInfo? clientInfo) || clientInfo == null)
            {
                Syslog.Warn($"Can not get request id {ar[1]}");
                return false;
            }

            // リクエスト時のSEQに置き換える    
            ar[0] = clientInfo.Terminal;
            ar[1] = clientInfo.RequestSeq.ToString();
            var sendText = string.Join("\n", ar);
            Syslog.Debug($"PrintableCommandProcessor:S:{sendText}");
            var sendBytes = Encoding.GetEncoding("SJIS").GetBytes(sendText);
            handler.Backend.PostMessage(clientInfo.ClientId, BackendService.BEM_CORE, sendBytes, BackendService.BEMS_RESULT | BackendService.BEMS_NONEEDRESULT);

            //  結果を送信したので処理は打ち切り
            return true;
        }

        //  PrintSvrに中継する
        public string? Print(PrintLabelRequest request, MessageHandler handler, string[] ar)
        {
            var labeldata = GetLabelData(request);
            var labelRequest = new LabelRequest
            {
                Address = request.Address,
                Datas = labeldata,
                LabelType = request.LabelType,
            };

            var terminal = ar[0];
            var seq = int.Parse(ar[1]);
            _commandRelay.Relay(handler, GetLabelCapability(request), terminal, seq, "print", labelRequest);
            return null;
        }

        //  PrintSvrに中継する
        public string? PrintStatus(PrintLabelRequest request, MessageHandler handler, string[] ar)
        {
            var labelRequest = new LabelRequest
            {
                Address = request.Address,
            };

            var terminal = ar[0];
            var seq = int.Parse(ar[1]);
            _commandRelay.Relay(handler, GetLabelCapability(request), terminal, seq, "status", labelRequest);
            return null;
        }

        public LabelResponse PrintData(PrintLabelRequest request)
        {
            var printerType = GetPrinterType(request);
            var labeldata = GetLabelData(request);
            return new LabelResponse
            {
                LabelData = labeldata,
                LabelType = request.LabelType,
                PrinterType = printerType,
            };
        }

        public LabelResponse PrintDeviceType(PrintLabelRequest request)
        {
            var printerType = GetPrinterType(request);
            return new LabelResponse
            {
                PrinterType = printerType,
            };
        }
    }
}
