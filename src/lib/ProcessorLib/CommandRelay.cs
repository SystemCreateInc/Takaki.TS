using BackendLib;
using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorLib
{
    public class CommandRelay
    {
        private int _seq = 0;
        private Dictionary<int, RequestInfo> _requestMap = new Dictionary<int, RequestInfo>();

        public class RequestInfo
        {
            public string Terminal { get; set; } = string.Empty;
            public int ClientId { get; set; }
            public int RequestSeq { get; set; }
        }

        public void Relay(MessageHandler handler, string capability, string terminal, int seq, string command, object data)
        {
            // 送信先取得
            if (!handler.TryGetProcessorId(capability, out int targetid))
            {
                Syslog.Warn($"Unknown capability {capability}");
                throw new Exception("準備が出来ていません確認してください");
            }

            // 送信データ作成
            var sendDatas = new[]
            {
                Environment.MachineName,
                (++_seq).ToString(),
                capability,
                command,
                Json.Serialize(data),
            };
            var sendText = string.Join("\n", sendDatas);
            var sendBytes = Encoding.GetEncoding("SJIS").GetBytes(sendText);

            Syslog.Debug($"CommandRelay: Relay original sender {terminal} seq {seq}: {sendText}");

            // 送信元記憶
            var msg = handler.Backend.CurrentMessage;
            _requestMap[_seq] = new RequestInfo { Terminal = terminal, ClientId = msg.fromClientId, RequestSeq = seq };

            if (_seq >= 1000000)
            {
                _seq = 0;
            }

            BackendService.ThrowIf(handler.Backend.PostMessage(targetid, BackendService.BEM_CORE, sendBytes));
        }

        public bool TryGetRequestedClientInfo(int id, out RequestInfo? info)
        {
            return _requestMap.TryGetValue(id, out info);
        }
    }
}
