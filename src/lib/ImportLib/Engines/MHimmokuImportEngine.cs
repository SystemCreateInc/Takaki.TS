
using CsvHelper;
using CsvHelper.Configuration;
using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DbLib.Defs.DbLib.Defs;
using ImportLib.CSVModels;
using ImportLib.Models;
using ImportLib.Repositories;
using LogLib;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Markup;

namespace ImportLib.Engines
{
    public class MHimmokuImportEngine : IImportEngine
    {
        public DataType DataType => _interfaceFile.DataType;

        public string DataName => _interfaceFile.Name;

        public string ImportFilePath { get; set; } = string.Empty;

        public List<ImportFileInfo> TargetImportFiles { get; private set; } = new List<ImportFileInfo>();

        private InterfaceFile _interfaceFile;
        private ScopeLogger _logger = new ScopeLogger<MHimmokuImportEngine>();

        public IEnumerable<SameDistInfo> SameDistInfos { get; set; } = Enumerable.Empty<SameDistInfo>();

        public MHimmokuImportEngine(InterfaceFile interfaceFile)
        {
            _interfaceFile = interfaceFile;
            ImportFilePath = _interfaceFile.FileName;
        }

        public void UpdateImportFileInfo()
        {
            try
            {
                var dir = Path.GetDirectoryName(ImportFilePath);
                var fileName = Path.GetFileName(ImportFilePath);
                if (dir.IsNullOrEmpty() || fileName.IsNullOrEmpty())
                {
                    return;
                }

                TargetImportFiles = Directory.EnumerateFiles(dir!, fileName).Select(x =>
                {
                    Syslog.Debug($"found file: {x}");
                    var fi = new FileInfo(x);
                    return new ImportFileInfo
                    {
                        Selected = fi.Length > 0,
                        Name = DataName,
                        FileSize = fi.Length,
                        LastWriteTime = fi.LastWriteTime,
                        FilePath = x,
                    };
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.Warn($"UpdateImportFileInfo: {ex}");
                TargetImportFiles.Clear();
            }
        }

        public IEnumerable<ImportResult> Import(DataImportController controller, CancellationToken token)
        {
            var importResults = new List<ImportResult>();

            using (var repo = new ImportRepository())
            {
                repo.DeleteExpiredHimmokuData();

                foreach (var targetFile in TargetImportFiles)
                {
                    controller.NotifyProgress("ファイル読み込み中");
                    var importDatas = ReadFile(token, targetFile.FilePath!);
                    Syslog.Debug($"Read {importDatas.Count()} lines");

                    var fname = CreateBulkInsertFile(controller, importDatas);
                    controller.NotifyProgress("コピー中");
                    fname = CopyToImportFile(fname, repo);

                    controller.NotifyProgress("取り込み中");
                    InsertData(fname, repo, token);
                    var importedCount = importDatas.Count();
                    importResults.Add(new ImportResult(true, targetFile.FilePath ?? "", (long)targetFile.FileSize!, importedCount));
                }

                repo.Commit();
                return importResults;
            }
        }

        private string CopyToImportFile(string srcFile, ImportRepository repo)
        {
            var dir = repo.GetBulkInsertFileDirectoryPath();
            if (string.IsNullOrEmpty(dir))
            {
                throw new Exception($"括取り込みディレクトリパスを設定してください。");
            }

            var fname = Path.Combine(dir, "HinmokuBulkImport.dat");
            File.Delete(fname);
            File.Copy(srcFile, fname);
            File.Delete(srcFile);

            return fname;
        }

        private string CreateBulkInsertFile(DataImportController controller, IEnumerable<string> importDatas)
        {
            var fname = Path.GetTempFileName();
            var options = new FileStreamOptions
            {
                Mode = FileMode.Create,
                Access = FileAccess.Write,
            };

            var dateline = $",\"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\",\"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\"";
            using (var ws =  new StreamWriter(fname, Encoding.GetEncoding("Shift_jis"), options))
            {
                var count = 0;
                foreach (var line in importDatas)
                {
                    ws.Write(line.Substring(0, line.Length - 2));
                    ws.WriteLine(dateline);

                    if ((++count % 133)  == 0)
                    {
                        controller.NotifyProgress($"{count}/{importDatas.Count()}");
                    }
                }
            }

            return fname;
        }

        public InterfaceFile GetInterfaceFile()
        {
            return _interfaceFile with { FileName = ImportFilePath };
        }

        public Task<bool> SetSameDist(CancellationToken token)
        {
            // 処理無し
            return Task.Run(() => true);
        }

        private void InsertData(string fname, ImportRepository repo, CancellationToken token)
        {
            var sql = Sql.Format<TBMHIMMOKUEntity>($"BULK INSERT {nameof(TBMHIMMOKUEntity):T} FROM '{fname}' WITH (FORMAT='CSV')");
            repo.Connection.Execute(sql, transaction: repo.Transaction, commandTimeout: 0);
        }

        private IEnumerable<string> ReadFile(CancellationToken token, string targetImportFilePath)
        {
            Syslog.Debug($"Read {DataName} file");
            var datas = new List<string>();

            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                };

                using (var reader = new StreamReader(targetImportFilePath, Encoding.GetEncoding("shift_jis")))
                using (var csv = new CsvReader(reader, config))
                {
                    while (csv.Read())
                    {
                        token.ThrowIfCancellationRequested();

                        try
                        {
                            var line = csv.GetRecord<HimmokuFileLine>();
                            datas.Add(csv.Parser.RawRecord);
                        }
                        catch (CsvHelper.MissingFieldException)
                        {
                            _logger.Warn($"MissingField Skip Row={csv.Parser.Row} Length={csv.Parser.Record?.Length ?? 0}");
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var message = $"CSVファイルの読み込み中にエラーが発生しました\n{ex.Message}";
                throw new Exception(message);
            }

            return datas;
        }
    }
}
