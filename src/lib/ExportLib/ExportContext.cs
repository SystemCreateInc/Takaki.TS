using DbLib.Defs;
using System.Data;

namespace ExportLib
{
    public class ExportContext
    {
        public ExportService Service { get; internal set; }
        public IExportProcessor Processor { get; internal set; }
        public DataType DataType { get; internal set; }
        public string FileName { get; internal set; }
        public string HulftId { get; internal set; }
        public bool IsForce { get; internal set; }
        public int FileSize { get; internal set; }
        public int ExportedCount { get; internal set; }
        public CancellationToken CancellationToken { get; internal set; }

        private IDbTransaction? _dbTrasaction;
        public IDbTransaction Transaction => _dbTrasaction!;

        private StreamWriter? _streamWriter;
        public StreamWriter StreamWriter => _streamWriter!;

        public ExportContext(ExportService service, IExportProcessor processor, DataType dataType, string fileName, string hulftId, bool isForce, CancellationToken cancellationToken)
        {
            Service = service;
            Processor = processor;
            DataType = dataType;
            FileName = fileName;
            HulftId = hulftId;
            IsForce = isForce;
            CancellationToken = cancellationToken;
        }

        public void SetTransaction(IDbTransaction transaction)
        {
            _dbTrasaction = transaction;
        }

        public void SetStreamWriter(StreamWriter stream)
        {
            _streamWriter = stream;
        }
    }
}
