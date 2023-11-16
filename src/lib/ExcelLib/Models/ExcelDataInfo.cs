namespace ExcelLib.Models
{
    public class ExcelDataInfo
    {
        public ExcelDataInfo(object?[,] excelData)
        {
            ExcelData = excelData;
        }

        // Excel出力データ
        public object?[,] ExcelData;

        // 開始行、列（1始まり）
        public int StartRow = 1;
        public int StartColumn = 1;

        // 行挿入：データの行数分、開始位置から行挿入
        public bool Insert = false;

        // コピーした行の挿入：行挿入時に、開始位置の行をコピー
        public bool InsertCopy = false;

        // 既存行削除：最終データ行の次行を、データ記入後に削除
        public bool DeleteDefaultRow = false;

        // 合計行開始列：最終データ行の2行先へ、合計行開始列から最終列まで合計(SUM関数)出力。-1で合計記載無し ※セルの書式が「文字列」の列は記載しない。
        public int StartSum = -1;

        // 書式設定:データ１行目から変数型を参照し、書式設定を行う(現状はString「文字列」のみ。他は「標準」)
        public bool Format = true;

        // 出力データ内のヘッダー(カラム名)記載行数：Format=true時の書式取得列を指定値＋１から取得する(DataGridから出力時は常時1)
        public int Header = 1;

        // シート名称(複数シート作成時のみ。1件目のExcelDataInfoへシート名を記載)
        public string SheetName = "Sheet1";
    }
}
