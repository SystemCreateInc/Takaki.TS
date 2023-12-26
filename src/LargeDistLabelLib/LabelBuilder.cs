using LabelLib;

namespace LargeDistLabelLib
{
    public class LabelBuilder
    {
        public IEnumerable<string> Build(LargeDistLabel data)
        {
            var fmtr = new SplFormatter();
            fmtr.SetOffset(0, 15);
            fmtr.Start();
            fmtr.SetPaperSize(35 * 8, 60 * 8);
            fmtr.SetScale(1, 1);
            fmtr.SetPPitch("PS");
            fmtr.SetPitch(0);

            fmtr.String(10, 0, SatoFont.K16, "納品日:");
            fmtr.String(70, 0, SatoFont.X22, $"{data.DtDelivery}");

            fmtr.String(340, 0, SatoFont.K16, "受注便:");
            fmtr.String(410, 0, SatoFont.X22, $"{data.CdJuchuBin}");

            fmtr.SetScale(2, 2);
            fmtr.String(90, 25, SatoFont.K24, $"<{data.CdBlock}ブロック>");

            fmtr.SetScale(1, 1);
            //fmtr.String(10, 50, SatoFont.K24, "仕分G:");
            fmtr.String(10, 80, SatoFont.X22, $"{data.CdDistGroup}");
            fmtr.String(110, 80, SatoFont.K24, $"{data.NmDistGroup}");

            //fmtr.String(10, 100, SatoFont.K24, "出荷バッチ:");
            fmtr.String(10, 110, SatoFont.X22, $"{data.CdShukkaBatch}");
            fmtr.String(110, 110, SatoFont.K24, $"{data.NmShukkaBatch}");

            fmtr.String(10, 140, SatoFont.X22, $"{data.CdHimban} {data.CdJan}");

            fmtr.String(10, 170, SatoFont.K24, $"{CutStr(data.NmHinSeishikimei, 38)}");
            fmtr.String(380, 200, SatoFont.K16, "入数:");
            fmtr.String(430, 200, SatoFont.X21, $"{data.NuBoxUnit}");

            fmtr.String(10, 200, SatoFont.X22, string.Format("{0, 3}", data.BoxPs));
            fmtr.String(90, 200, SatoFont.K24, "箱");
            fmtr.String(130, 200, SatoFont.X22, string.Format("{0, 3}", data.BaraPs));
            fmtr.String(210, 200, SatoFont.K24, "個");
            fmtr.String(250, 200, SatoFont.X22, string.Format("({0, 3})", data.TotalPs));

            fmtr.Barcode(60, 230, SatoBarcodeType.BAR_JAN13, SatoBarcodeThickness.Thickness_JanNoGuide, 3, 40, data.CdJan);
            fmtr.End(1);
            return new[] { fmtr.GetString() };
        }

        private string CutStr(string str, int size)
        {
            if (str.Trim() == string.Empty)
            {
                return string.Empty;
            }

            var cutStr = str.ChunkBySjis(size).Take(1).FirstOrDefault();
            return string.IsNullOrEmpty(cutStr) ? string.Empty : cutStr;
        }
    }
}
