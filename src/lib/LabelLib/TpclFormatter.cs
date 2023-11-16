using System.Text;

namespace LabelLib
{
    public class TpclFormatter
    {
        private StringBuilder _formattedString = new StringBuilder();
        private int _seq = 0;
        private int _olseq = 0;
        private int _bseq = 0;
        private int _angle = 0;       // 角度 0,90,180,270
        private int _offsetHor = 0;   // オフセット
        private int _offsetVer = 0;   // オフセット 
        private int _extHor = 1;      // 倍率
        private int _extVer = 1;      // 倍率

        private const string STARTMARK = "{";
        private const string ENDMARK = "|}\r";

        public string GetString()
        {
            return _formattedString.ToString();
        }

        public byte[] GetBytes()
        {
            return System.Text.Encoding.UTF8.GetBytes(_formattedString.ToString());
        }

        public void SetOffset(int cx, int cy)
        {
            _offsetHor = cx;
            _offsetVer = cy;
        }

        public void SetVector(int v)
        {
            _angle = v;
        }

        public void SetScale(int cx, int cy)
        {
            _extHor = cx;
            _extVer = cy;
        }

        public void Start(int hight, int width)
        {
            _formattedString.Append(STARTMARK + "D");
            _formattedString.AppendFormat("{0:D4},", width + 20);
            _formattedString.AppendFormat("{0:D4},", hight);
            _formattedString.AppendFormat("{0:D4}", width);
            _formattedString.Append(ENDMARK);
            _formattedString.Append(STARTMARK + "C" + ENDMARK);

            // シーケンスクリア
            _seq = 0;
        }

        public void End(int labelCount)
        {
            _formattedString.Append(STARTMARK + "XS;I,");
            _formattedString.AppendFormat("{0:D4},0002C4001", labelCount);
            _formattedString.Append(ENDMARK);
            _formattedString.Append("\r");
        }

        private int GetAngleValue(int angle)
        {
            int angleCommand = 0;
            switch (_angle)
            {
                case 0:
                    angleCommand = 0;
                    break;

                case 90:
                    angleCommand = 11;
                    break;

                case 180:
                    angleCommand = 22;
                    break;

                case 270:
                    angleCommand = 33;
                    break;
            }
            return angleCommand;
        }

        public enum DecorationType
        {
            B,  // 黒文字
            W,  // 白抜き文字
            F,  // 四角枠付き文字
        }

        public void String(int x, int y, string ctype, string data, DecorationType decoration = DecorationType.B, int align = 1, int spacing = 0)
        {
            _seq++;
            _formattedString.Append(STARTMARK + "PC");
            _formattedString.AppendFormat("{0:D3};", _seq);
            _formattedString.AppendFormat("{0:D4},", x + _offsetHor);
            _formattedString.AppendFormat("{0:D4},", y + _offsetVer);
            _formattedString.AppendFormat("{0},", _extHor);
            _formattedString.AppendFormat("{0},", _extVer);
            _formattedString.AppendFormat("{0},", ctype);
            _formattedString.AppendFormat("{0:+02},", spacing);
            _formattedString.AppendFormat("{0:D2},", GetAngleValue(_angle));

            switch (decoration)
            {
                case DecorationType.B:
                    _formattedString.Append("B,");
                    break;

                case DecorationType.W:
                    _formattedString.Append("W0505,");
                    break;

                case DecorationType.F:
                    _formattedString.Append("F0505,");
                    break;
            }

            _formattedString.AppendFormat("P{0}=", align);
            _formattedString.Append(data);
            _formattedString.Append(ENDMARK);
        }

        public void StringOL(int x, int y, int cx, int cy, string ctype, string data, bool reverse = false)
        {
            _olseq++;
            _formattedString.Append(STARTMARK + "PV");
            _formattedString.AppendFormat("{0:D2};", _olseq);
            _formattedString.AppendFormat("{0:D4},", x + _offsetHor);
            _formattedString.AppendFormat("{0:D4},", y + _offsetVer);
            _formattedString.AppendFormat("{0:D4},", cx);
            _formattedString.AppendFormat("{0:D4},", cy);
            _formattedString.AppendFormat("{0},", ctype);
            _formattedString.AppendFormat("{0:D2},", GetAngleValue(_angle));

            if (reverse)
            {
                _formattedString.Append("W1010");
            }
            else
            {
                _formattedString.Append("B");
            }

            _formattedString.Append("=");
            _formattedString.Append(data);
            _formattedString.Append(ENDMARK);
        }

        public enum BarcodeType
        {
            Code39 = 3,
            Code128 = 9
        }

        public enum CheckDigit
        {
            None,
            Chceck,
            Auto,
            Auto2,
            Auto3,
        }

        public void Barcode(int x, int y, BarcodeType barcodeType, CheckDigit cd, int modWidth, int rotate, int height, string data)
        {
            _formattedString.Append(STARTMARK);
            _formattedString.AppendFormat("XB{0:D2};", ++_bseq);
            _formattedString.AppendFormat("{0:D4},", x + _offsetHor);
            _formattedString.AppendFormat("{0:D4},", y + _offsetVer);
            _formattedString.AppendFormat("{0},", (int)barcodeType);
            _formattedString.AppendFormat("{0},", (int)cd);
            _formattedString.AppendFormat("{0:D2},", modWidth);
            _formattedString.AppendFormat("{0},", rotate);
            _formattedString.AppendFormat("{0:D4}", height);
            _formattedString.Append("=");
            _formattedString.Append(data.Trim());
            _formattedString.Append(ENDMARK);
        }

        public void BarcodeCode39(int x, int y, BarcodeType barcodeType, CheckDigit cd, int thinWidth, int thinSpace, int thickWidth,
            int thickSpace, int charSpace, int rotate, int height, string data)
        {
            _formattedString.Append(STARTMARK);
            _formattedString.AppendFormat("XB{0:D2};", ++_bseq);
            _formattedString.AppendFormat("{0:D4},", x + _offsetHor);
            _formattedString.AppendFormat("{0:D4},", y + _offsetVer);
            _formattedString.AppendFormat("{0},", (int)barcodeType);
            _formattedString.AppendFormat("{0},", (int)cd);
            _formattedString.AppendFormat("{0:D2},", thinWidth);
            _formattedString.AppendFormat("{0:D2},", thinSpace);
            _formattedString.AppendFormat("{0:D2},", thickWidth);
            _formattedString.AppendFormat("{0:D2},", thickSpace);
            _formattedString.AppendFormat("{0:D2},", charSpace);
            _formattedString.AppendFormat("{0},", rotate);
            _formattedString.AppendFormat("{0:D4}", height);
            _formattedString.Append("=");
            _formattedString.Append(data.Trim());
            _formattedString.Append(ENDMARK);
        }

        public enum LineType
        {
            Line = 0,       // ライン
            Square,         // 四角形
            ThinningLine,   // 間引きライン
            ThinningSquare  // 間引き四角形
        }

        public void Line(int startX, int stratY, int endX, int endY, LineType lineType, int lineWidth)
        {
            _formattedString.Append(STARTMARK);
            _formattedString.AppendFormat("LC;");
            _formattedString.AppendFormat("{0:D4},", startX);
            _formattedString.AppendFormat("{0:D4},", stratY);
            _formattedString.AppendFormat("{0:D4},", endX);
            _formattedString.AppendFormat("{0:D4},", endY);
            _formattedString.AppendFormat("{0},", (int)lineType);
            _formattedString.AppendFormat("{0:D2}", lineWidth);
            _formattedString.Append(ENDMARK);
        }
    }
}

