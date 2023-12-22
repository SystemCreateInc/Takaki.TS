using DbLib;
using DbLib.DbEntities;
using ExportLib.Exceptions;
using InterfaceTimingLib;
using LogLib;
using System.Text;

namespace ExportLib
{
    public class ExportService
    {
        private readonly IExportRepositoryFactory _factory;
        private readonly ScopeLogger _logger = new ScopeLogger<ExportService>();

        public IList<IExportProcessor> Processors { get; } = new List<IExportProcessor>();

        public event Action<ExportStatus>? StatusChangedEvent;
        public event Action<ProgressInfo>? ProgressEvent;

        public ExportService(IExportRepositoryFactory factory)
        {
            _factory = factory;
        }

        public void AddProcessor(IExportProcessor proc)
        {
            UpdateProcessorInfo(proc);
            Processors.Add(proc);
        }

        public void UpdateAllProcessorInfo()
        {
            foreach (var proc in Processors)
            {
                UpdateProcessorInfo(proc);
            }
        }

        public void UpdateProcessorInfo(IExportProcessor proc)
        {
            using (var repo = _factory.Create())
            {
                var ifile = repo.GetInterfaceFile(proc.DataType)
                    ?? throw new NotDefinedInterfaceException($"インターフェース情報が設定されていません。[DataType: {proc.DataType}({(int)proc.DataType})]");
                repo.Commit();

                proc.Name = ifile.Name;
                proc.FileName = ifile.FileName;
                proc.EnableInterval = ifile.EnableInterval;
                proc.EnableTiming = ifile.EnableTiming;
                proc.IntervalSec = ifile.IntervalSec;
                proc.SpecifiedTimings = ifile.Timings;
                proc.ExpDays = ifile.ExpDays;
                TimingCalculator.InitializeTiming(proc);
            }
        }

        public void UpdateAvailavleExportCount()
        {
            try
            {
                using (var db = DbFactory.CreateConnection())
                {
                    var tr = db.BeginTransaction();

                    foreach (var proc in Processors)
                    {
                        proc.UpdateAvailableExportCount(tr);
                    }

                    tr.Commit();
                }
            }
            catch (Exception e)
            {
                _logger.Error($"EXCEPTION: {e}");
            }
        }

        public void UpdateNextExportTime()
        {
            foreach (var proc in Processors)
            {
                TimingCalculator.UpdateNextTiming(proc);
            }
        }

        public void Export(CancellationToken token)
        {
            foreach (var proc in Processors)
            {
                // 送信タイミングのデータのみ送信
                if (CanExport(proc))
                {
                    ExportByProcessor(proc, token);
                }
            }
        }

        public void ForceExport(CancellationToken token, IEnumerable<IExportProcessor> procs)
        {
            foreach (var proc in procs)
            {
                ExportByProcessor(proc, token, true);
            }
        }

        private bool CanExport(IExportProcessor proc)
        {
            return TimingCalculator.ItsTime(proc, DateTime.Now);
        }

        private void ExportByProcessor(IExportProcessor proc, CancellationToken token, bool force = false)
        {
            _logger.Debug($"Start Exporting {proc.Name}...");
            var ctxt = new ExportContext(
                this,
                proc,
                proc.DataType,
                proc.FileName,
                force,
                token);

            try
            {
                var tmpFileName = CreateTempFileName(ctxt);
                _logger.Debug($"Create tmp file {tmpFileName}");

                using (var repo = _factory.Create())
                using (var locker = repo.Lock(proc.LockKey))
                {
                    using (var fs = new FileStream(tmpFileName, FileMode.Create, FileAccess.Write, FileShare.None))
                    using (var sw = new StreamWriter(fs, Encoding.GetEncoding("shift_jis")))
                    {
                        ctxt.SetStreamWriter(sw);
                        ctxt.SetTransaction(repo.Transaction);

                        NotifyStatus(ctxt, "前処理中");
                        proc.PreExport(ctxt);

                        NotifyStatus(ctxt, "データ作成中");
                        proc.ExportData(ctxt);

                        NotifyStatus(ctxt, "後処理中");
                        proc.PostExport(ctxt);
                    }

                    if (ctxt.ExportedCount > 0)
                    {
                        _logger.Debug($"Append to target file {ctxt.FileName}");
                        AppendTargetFile(ctxt, tmpFileName);
                        Syslog.SLCopy(ctxt.FileName);
                    }

                    _logger.Debug($"Delete tmp file {tmpFileName}");
                    File.Delete(tmpFileName);

                    repo.Commit();
                    NotifyStatus(ctxt, "終了");
                }

                CreateSuccessLog(ctxt);
            }
            catch (Exception ex)
            {
                InsertFailLog(ctxt, ex.Message);
                throw;
            }
            finally
            {
                proc.LastExportedTime = DateTime.Now;
                TimingCalculator.UpdateNextTiming(proc);
            }
        }

        private string CreateTempFileName(ExportContext ctxt)
        {
            var dir = Path.GetDirectoryName(ctxt.FileName) ?? "";
            var fname = $"_tmp_{Path.GetFileName(ctxt.FileName)}";
            var path = Path.Combine(dir, fname);
            return path;
        }

        private void NotifyStatus(ExportContext ctxt, string description)
        {
            var status = new ExportStatus(ctxt.Processor.Name, description, ctxt.ExportedCount);

            StatusChangedEvent?.Invoke(status);
        }

        public void NotifyProgress(string message, int value, int maximum, int minimum)
        {
            ProgressEvent?.Invoke(new ProgressInfo(message, value, maximum, minimum));
        }

        private void AppendTargetFile(ExportContext ctxt, string srcFile)
        {
            var buf = new byte[1024 * 10];
            using (var srcFs = new FileStream(srcFile, FileMode.Open, FileAccess.Read, FileShare.None))
            using (var wfs = new FileStream(ctxt.FileName, FileMode.Append, FileAccess.Write, FileShare.None))
            {
                int readedBytes;
                while ((readedBytes = srcFs.Read(buf, 0, buf.Length)) > 0)
                {
                    wfs.Write(buf, 0, readedBytes);
                }
            }
        }

        private void CreateSuccessLog(ExportContext ctxt)
        {
            using (var repo = _factory.Create())
            {
                repo.InsertLog(new InterfaceLogsEntity
                {
                    DataType = (short)ctxt.DataType,
                    RowCount = ctxt.ExportedCount,
                    FileSize = ctxt.FileSize,
                    Name = ctxt.Processor.Name,
                    Status = "成功",
                    Comment = ctxt.Processor?.GetLogComment(ctxt),
                    SrcFile = ctxt.FileName,
                    Terminal = Environment.MachineName,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                });
                repo.Commit();
            }
        }

        private void InsertFailLog(ExportContext ctxt, string message)
        {
            using (var repo = _factory.Create())
            {
                repo.InsertLog(new InterfaceLogsEntity
                {
                    DataType = (short)ctxt.DataType,
                    FileSize = ctxt.FileSize,
                    Name = ctxt.Processor.Name,
                    Status = "失敗",
                    Comment = message,
                    SrcFile = ctxt.FileName,
                    Terminal = Environment.MachineName,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                });
                repo.Commit();
            }
        }
    }
}
