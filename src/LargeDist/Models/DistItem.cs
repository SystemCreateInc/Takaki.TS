using DbLib.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LargeDist.Models
{
    public class DistItem
    {
        public DistItem(long iDDIST,
            string dTDELIVERY,
            string cDGTIN13,
            string cDHIMBAN,
            string cDJUCHUBIN,
            string cDCOURSE,
            int cDROUTE,
            string? nMHINSEISHIKIMEI,
            string? tdunitaddrcode,
            short sTBOXTYPE,
            int nUBOXUNIT,
            int nULOPS,
            int nULRPS,
            int nUDOPS,
            int nUDRPS,
            int fGDSTATUS,
            string cDTOKUISAKI,
            string? nMTOKUISAKI,
            string? cDBLOCK,
            string? cDDISTGROUP,
            string? nMDISTGROUP,
            string cDSHUKKABATCH,
            string? nMSHUKKABATCH)
        {
            Id = iDDIST;
            DtDelivery = dTDELIVERY;
            CdGtin13 = cDGTIN13;
            CdHimban = cDHIMBAN;
            CdJuchuBin = cDJUCHUBIN;
            CdCourse = cDCOURSE;
            CdRoute = cDROUTE;
            NmHinSeishikimei = nMHINSEISHIKIMEI;
            Address = tdunitaddrcode;
            StBoxType = sTBOXTYPE;
            NuBoxUnit = nUBOXUNIT;
            OrderPiece = nULOPS;
            ResultPiece = nULRPS;
            DistOrderPiece = nUDOPS;
            DistResultPiece = nUDRPS;
            FgDStatus = (Status)fGDSTATUS;
            CdTokuisaki = cDTOKUISAKI;
            NmTokuisaki = nMTOKUISAKI;
            CdBlock = cDBLOCK;
            CdDistGroup = cDDISTGROUP;
            NmDistGroup = nMDISTGROUP;
            CdShukkaBatch = cDSHUKKABATCH;
            NmShukkaBatch = nMSHUKKABATCH;
            InputPiece = OrderPiece - ResultPiece;
        }

        public int GridPosition { get; set; }
        public int? ScanOrder { get; set; }

        // 残数
        public int RemainPiece => OrderPiece - ResultPiece;

        // 入力数
        public int InputPiece { get; set; }
        // 更新数
        public int LastInputPiece { get; private set; }

        // 商品停止フラグ
        public bool IsStopped { get; set; }
        public bool IsCompleted => RemainPiece == 0;

        public long Id { get; }
        public string DtDelivery { get; }
        public string CdGtin13 { get; }
        public string CdHimban { get; }
        public string CdJuchuBin { get; }
        public string CdCourse { get; }
        public int CdRoute { get; }
        public string? NmHinSeishikimei { get; }
        public string? Address { get; }
        public short StBoxType { get; }
        public int NuBoxUnit { get; set; }
        public int OrderPiece { get; }
        public int ResultPiece { get; private set; }
        public int DistOrderPiece { get; }
        public int DistResultPiece { get; }
        public Status FgDStatus { get; }
        public string CdTokuisaki { get; }
        public string? NmTokuisaki { get; }
        public string? CdBlock { get; }
        public string? CdDistGroup { get; }
        public string? NmDistGroup { get; }
        public string CdShukkaBatch { get; }
        public string? NmShukkaBatch { get; }

        public bool IsSameGroup(DistItem other )
        {
            return other.CdHimban == CdHimban
                && other.CdJuchuBin == CdJuchuBin
                && other.CdShukkaBatch == CdShukkaBatch;
        }

        public void RefrectInputPiece()
        {
            LastInputPiece = InputPiece;
            ResultPiece += InputPiece;
            // 残数を入力数にする
            InputPiece = RemainPiece;
        }

        internal void Undo()
        {
            ResultPiece = 0;
            InputPiece = 0;
        }
    }
}
