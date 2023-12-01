using DbLib.DbEntities;
using ImportLib.Engines;
using ImportLib.Repositories;
using LogLib;
using System.IO;

namespace ImportLib
{
    public enum GetSameDistResult
    {
        NONE,
        EXIST,
        WORK,
    }

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

        public async Task<GetSameDistResult> CheckSameDist(IImportEngine engine, CancellationToken token)
        {
            try
            {
                await engine.SetSameDist(token);

                if (!engine.SameDistInfos.Any())
                {
                    return GetSameDistResult.NONE;
                }

                if(engine.SameDistInfos.Any(x => x.IsWork))
                {
                    return GetSameDistResult.WORK;
                }

                return GetSameDistResult.EXIST;
            }
            catch (OperationCanceledException)
            {
                InsertFailLog(engine, "中断されました");
                throw;
            }
            catch (Exception ex)
            {
                Syslog.Error($"DataImportController.PreImport: {ex}");
                InsertFailLog(engine, ex.Message);
                throw;
            }
        }

        public async Task Import(IImportEngine engine, CancellationToken token)
        {
            try
            {
                var results = await engine.ImportAsync(token);
                foreach (var result in results)
                {
                    InsertSuccessLog(engine, result);
                }

                // 取込後のデータを削除
                engine._targetImportFiles.Where(x => x.Selected).ToList().ForEach(x =>
                {
                    new FileInfo(x.FilePath!).Delete();
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

        private void InsertSuccessLog(IImportEngine engine, ImportResult result)
        {
            using (var repo = new ImportRepository())
            {
                var log = new InterfaceLogsEntity
                {
                    DataType = (short)engine.DataType,
                    RowCount = result.DataCount,
                    FileSize = (int)result.FileSize,
                    Name = engine.DataName,
                    Status = "正常",
                    //FilePath = engine.ImportFilePath,
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
