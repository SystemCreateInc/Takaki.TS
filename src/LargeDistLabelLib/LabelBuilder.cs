using LabelLib;

namespace LargeDistLabelLib
{
    public class LabelBuilder : ILabelBuilder
    {
        private ILabelRepository _labelRepository;

        public LabelBuilder(ILabelRepository labelRepository)
        {
            _labelRepository = labelRepository;
        }

        public IEnumerable<string> Build(ILabelBuilderParam param)
        {
            var data = _labelRepository.Get();

            var fmtr = new SplFormatter();
            fmtr.SetOffset(0, 15);
            fmtr.Start();
            fmtr.SetPaperSize(35 * 8, 60 * 8);
            fmtr.SetScale(1, 1);
            fmtr.SetPPitch("PS");
            fmtr.SetPitch(2);

            fmtr.String(10, 0, SatoFont.K16, "納品日:");
            fmtr.String(80, 0, SatoFont.X21, $"{data.DtDelivery}");

            fmtr.String(250, 0, SatoFont.K16, "受注便:");
            fmtr.String(320, 0, SatoFont.X21, $"{data.CdJuchuBin}");

            fmtr.String(120, 20, SatoFont.K24, $"<{data.CdBlock}ブロック>");

            fmtr.SetScale(1, 1);
            fmtr.String(10, 50, SatoFont.K16, "仕分G:");
            fmtr.String(70, 50, SatoFont.X21, $"{data.CdDistGroup}");
            fmtr.String(140, 50, SatoFont.K16, $"{data.NmDistGroup}");

            fmtr.String(10, 75, SatoFont.K16, "出荷バッチ:");
            fmtr.String(110, 75, SatoFont.X21, $"{data.CdShukkaBatch}");
            fmtr.String(180, 75, SatoFont.K16, $"{data.NmShukkaBatch}");

            fmtr.String(10, 100, SatoFont.X21, $"{data.CdHimban} {data.CdJan}");

            fmtr.String(10, 120, SatoFont.K16, $"{CutStr(data.NmHinSeishikimei, 38)}");
            fmtr.String(350, 120, SatoFont.K16, "入数:");
            fmtr.String(400, 120, SatoFont.X21, $"{data.NuBoxUnit}");

            fmtr.String(10, 140, SatoFont.K24, "総数:");
            fmtr.String(80, 140, SatoFont.X22, string.Format("{0, 3}", data.BoxPs));
            fmtr.String(140, 140, SatoFont.K24, "箱");
            fmtr.String(200, 140, SatoFont.X22, string.Format("{0, 3}", data.BaraPs));
            fmtr.String(260, 140, SatoFont.K24, "個");
            fmtr.String(340, 140, SatoFont.X22, string.Format("({0, 3})", data.TotalPs));

            fmtr.Barcode(60, 170, SatoBarcodeType.BAR_JAN13, SatoBarcodeThickness.Thickness_Jan, 3, 40, data.CdJan);
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
