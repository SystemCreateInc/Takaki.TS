using LogLib;
using System.Windows.Media.Imaging;
using ZXing;

namespace PrintLib.Barcode
{
    public static class NW7
    {
        public static string CreateSvg(string code, int height, int width, int margin, bool barcodeOnly)
        {
            // コード空白時空画像表示(エラー回避)
            if (string.IsNullOrEmpty(code))
            {
                Syslog.Debug($"Barcode[NW7]Can Not Create Code={code}");
                return string.Empty;
            }

            var writer = new BarcodeWriterSvg
            {
                Format = BarcodeFormat.CODABAR,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = margin,
                    PureBarcode = barcodeOnly,
                },
            };

            return writer.Write(code).ToString();
        }
    }
}
