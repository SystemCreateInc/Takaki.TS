using DbLib.DbEntities;
using DbLib.Defs;
using DbLib.Extensions;
using FastExpressionCompiler.LightExpression;
using LogLib;
using Picking.Defs;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using TdDpsLib.Defs;
using TdDpsLib.Models;

namespace Picking.Models
{
    public class DistColor : DistBase
    {
        // 配分タイプ
        private int _distworkmode = 0;
        public int DistWorkMode
        {
            get => _distworkmode;
            set
            {
                SetProperty(ref _distworkmode, value);

                switch (value)
                {
                    case (int)Defs.DistWorkMode.Dist:
                        DistWorkMode_name = "仕";
                        break;
                    case (int)Defs.DistWorkMode.Check:
                        DistWorkMode_name = "検";
                        break;
                }
            }
        }

        private string _distworkmode_name = "";
        public string DistWorkMode_name
        {
            get => _distworkmode_name;
            set => SetProperty(ref _distworkmode_name, value);
        }

        public bool IsWorking()
        {
            return DistType == (int)DistTypeStatus.Ready ? false : true;
        }


        // 同時仕分アイテム数
        private int _distitem_cnt;
        public int Distitem_cnt
        {
            get => _distitem_cnt;
            set
            {
                SetProperty(ref _distitem_cnt, value);

                if (value == 0)
                {
                    DisplayDistitem_cnt = "";
                }
                else
                {
                    DisplayDistitem_cnt = value.ToString();
                }
            }
        }

        // 同時仕分アイテム数
        private string _displaydistitem_cnt = "";
        public string DisplayDistitem_cnt
        {
            get => _displaydistitem_cnt;
            set => SetProperty(ref _displaydistitem_cnt, value);
        }

        // 9商品
        private List<DistItemSeq> _itemseqs = new List<DistItemSeq>();

        public List<DistItemSeq> ItemSeqs
        {
            get => _itemseqs;
            set => SetProperty(ref _itemseqs, value);
        }

        private List<int> _distseqs = new List<int>() { 0, 0, 0 };
        public List<int> DistSeq
        {
            get => _distseqs;
            set => SetProperty(ref _distseqs, value);
        }

        // 表示器単位の表示するPS数
        public List<TdUnitDisplay> Tdunitdisplay { get; set; } = new List<TdUnitDisplay>();

        public DateTime DistStartTm { get; set; }

        public void DistColorClear()
        {
            Distitem_cnt = 0;
            DistType = (int)DistTypeStatus.Ready;
            ItemSeqs = new List<DistItemSeq>();
            Tdunitdisplay.Clear();
            DStatus = (int)Status.Ready;
            for (int i = 0; i < DistSeq.Count(); i++)
                DistSeq[i] = 0;
            Clear();
        }
        public override string QtyFieldSpace(int ps)
        {
            if (Distitem_cnt == 0)
                return string.Empty;
            else
                return ps.ToString();
        }

        public ReportShain ReportShain = new ReportShain();

        public void ReportStart(Shain shain, int itemcnt, int distworkmode)
        {
            // 追加
            ReportShain = new ReportShain
            {
                CdShain = shain.CdShain,
                NmShain = shain.NmShain,
                DtWorkStart = DateTime.Now,
                DtNowStart = DateTime.Now,
                DistWorkMode = distworkmode,
                ItemCnt = itemcnt,
            };
        }
        public void ReportCountUp(string tdunitaddrcode, int ps)
        {
            ReportShain.addrs.Add(tdunitaddrcode);
            ReportShain.DistCnt += ps;
        }

        public void ReportEnd()
        {
            if (ReportShain.DtNowStart != null)
            {
                TimeSpan sp = DateTime.Now.Subtract((DateTime)ReportShain.DtNowStart);
                ReportShain.WorkTime += sp.TotalSeconds;
                ReportShain.DtWorkEnd = DateTime.Now;
            }
            ReportShain.DtNowStart = null;
        }
    }
    public class TdUnitDisplay
    {
        public string Tdunitaddrcode { get; set; } = "";
        public string TdDisplay { get; set; } = "";
        public int Status { get; set; }
        public int InSeq { get; set; }
        public int Zone { get; set; }
        public int TdUnitSeq { get; set; }
        public bool bLight { get; set; } = false;
    }
    public class TdColor
    {
        static public SolidColorBrush GetColorText(int color)
        {
            Color col = Colors.Black;
            switch (color)
            {
                case 1: col = Colors.Black; break;
                case 2: col = Colors.Black; break;
                case 3: col = Colors.Black; break;
                case 4: col = Colors.Black; break;
                case 5: col = Colors.White; break;
            }
            return new SolidColorBrush(col);
        }
        static public SolidColorBrush GetColorBack(int color)
        {
            Color col = Colors.Gray;
            switch (color)
            {
                case 1: col = Colors.Red; break;
                case 2: col = Colors.Yellow; break;
                case 3: col = Colors.LightGreen; break;
                case 4: col = Colors.White; break;
                case 5: col = Colors.Blue; break;
            }
            return new SolidColorBrush(col);
        }
    }
}
