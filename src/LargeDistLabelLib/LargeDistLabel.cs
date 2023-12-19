namespace LargeDistLabelLib
{
    public struct LargeDistLabel
    {
        // 納品日
        public string DtDelivery;

        // 受注便
        public string CdJuchuBin;

        // ブロック
        public string CdBlock;

        // 仕分グループコード
        public string CdDistGroup;

        // 仕分グループ名称
        public string NmDistGroup;

        // 出荷バッチコード
        public string CdShukkaBatch;

        // 出荷バッチ名称
        public string NmShukkaBatch;

        // 品番
        public string CdHimban;

        // JANコード
        public string CdJan;

        // 品名
        public string NmHinSeishikimei;

        // 箱入数
        public int NuBoxUnit;

        // 大仕分数(箱)
        public int BoxPs;

        // 大仕分数(バラ)
        public int BaraPs;

        // 大仕分数(総個数)
        public int TotalPs;

        // バーコード
        public string Barcode;
    }
}
