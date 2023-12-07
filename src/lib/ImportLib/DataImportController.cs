using DbLib.DbEntities;
using ImportLib.Engines;
using ImportLib.Repositories;
using LogLib;
using Prism.Services.Dialogs;
using System.IO;

namespace ImportLib
{
    public class DataImportController
    {
        public DataImportController()
        {
            using (var repo = new ImportRepository())
            {
                Syslog.Debug($"Delete expired logs");
                repo.DeleteExpiredLogs();

                repo.Commit();
            }
        }

        public event Func<string, string, ButtonResult>? RequestComfirm;

        public async Task Import(IImportEngine engine, CancellationToken token)
        {
            try
            {
                var results = await Task.Run(() =>
                {
                    Syslog.Debug($"Import start {engine.DataName}");

                    var results = engine.Import(this, token);

                    Syslog.Debug($"Import end {engine.DataName}");

                    return results;
                });

                foreach (var result in results)
                {
                    InsertSuccessLog(engine, result);
                }

                // 取込後のデータを削除
                engine.TargetImportFiles.Where(x => x.Selected).ToList().ForEach(x =>
                {
#if DEBUG
                    Syslog.Debug($"fake Delete file {x.FilePath!}");
#else
                    new FileInfo(x.FilePath!).Delete();
#endif
                });
            }
            catch (OperationCanceledException)
            {
                InsertFailLog(engine, "中断されました");
                throw;
            }
            catch (ImportException ex)
            {
                Syslog.Error($"DataImportController.Import: {ex}");
                InsertFailLog(engine, ex.Message, ex.Result);
                throw;
            }
            catch (Exception ex)
            {
                Syslog.Error($"DataImportController.Import: {ex}");
                InsertFailLog(engine, ex.Message);
                throw;
            }
        }

        public ButtonResult Confirm(string message, string caption)
        {
            return RequestComfirm?.Invoke(message, caption) ?? throw new InvalidOperationException(message);
        }

        private void InsertSuccessLog(IImportEngine engine, ImportResult result)
        {
            using (var repo = new ImportRepository())
            {
                var log = new InterfaceLogsEntity
                {
                    DataType = (short)engine.DataType,
                    RowCount = (int)result.DataCount,
                    FileSize = (int)result.FileSize,
                    Name = engine.DataName,
                    Status = "正常",
                    SrcFile = result.FileName,
                    Comment = "",
                    Terminal = Environment.MachineName,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                repo.Insert(log);
                repo.Commit();
            }
        }

        private void InsertFailLog(IImportEngine engine, string message, ImportResult? result = null)
        {
            using (var repo = new ImportRepository())
            {
                var log = new InterfaceLogsEntity
                {
                    DataType = (short)engine.DataType,
                    RowCount = 0,
                    FileSize = (int)(result?.FileSize ?? 0),
                    Name = engine.DataName,
                    Status = "失敗",
                    //FilePath = engine.ImportFilePath,
                    Comment = message,
                    Terminal = Environment.MachineName,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                repo.Insert(log);
                repo.Commit();
            }
        }
    }
}
