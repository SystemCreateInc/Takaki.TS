using BackendLib;
using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace ProcessorLib
{
    public class ProcessorAdapter<T> : IProcessorAdapter where T : ICommandProcessor
    {
        private ICommandProcessor _processor;
        private IDictionary<string, MethodInfo> _processorMethods = new Dictionary<string, MethodInfo>();

        public string Capability { get; set; }

        public ProcessorAdapter(string capability, T processor)
        {
            Capability = capability;
            _processor = processor;

            foreach (var method in typeof(T).GetMethods())
            {
                var name = method.Name;
                if (method != null && name != null && name != "Object")
                {
                    _processorMethods.Add(name.ToUpper(), method);
                }
            }
        }

        public string? Invoke(string[] ar, MessageHandler handler)
        {
            var hook = _processor as IMessageHook;
            if (hook != null)
            {
                if (hook.Hook(ar, handler))
                {
                    return null;
                }
            }

            if (ar.Length < 5)
            {
                throw new Exception("処理できないコマンドです");
            }

            var terminal = ar[0].ToUpper();
            // var seq = int.Parse(ar[1]);
            var command = ar[3].ToUpper();
            var json = ar[4];
            if (!_processorMethods.ContainsKey(command))
            {
                throw new Exception("実装されていないコマンドです");
            }

            var method = _processorMethods[command];
            var methodPrms = method.GetParameters();
            var prms = new List<object>();

            //  JSONデコード
            if (!string.IsNullOrEmpty(json))
            {
                if (Json.Deserialize(json, methodPrms[0].ParameterType) is object obj)
                {
                    prms.Add(obj);
                }
            };

            //  2番目のパラメーターにアドレスをセット
            if (methodPrms.Length > prms.Count && methodPrms[prms.Count].ParameterType == typeof(string))
            {
                prms.Add(terminal);
            }

            //  3番目のパラメーターにハンドラーセット
            if (methodPrms.Length > prms.Count && methodPrms[prms.Count].ParameterType == typeof(MessageHandler))
            {
                prms.Add(handler);
            }

            //  4番目のパラメーターにリクエストデータ
            if (methodPrms.Length > prms.Count && methodPrms[prms.Count].ParameterType == typeof(string[]))
            {
                prms.Add(ar);
            }

            var result = method.Invoke(_processor, prms.ToArray());

            //  戻り値がresultはnullだがvoidの時はOKだけ返す
            if (method.ReturnType.Name == "Void")
            {
                return "";
            }

            if (result is null)
            {
                return null;
            }

            return result is string 
                ? (string)result
                : Json.Serialize(result);
        }
    }
}
