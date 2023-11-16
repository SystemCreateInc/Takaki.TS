using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelLib
{
    public enum SatoFont
    {
        X21 = 1,
        X22,
        X23,
        X24,
        K16 = 6,
        K24,
        K16G,
        K24G,
    }

    public enum SatoVector
    {
        S0,
        S90,
        S180,
        S270,
    }

    public enum SatoBarcodeType
    {
        BAR_JAN8 = 10,
        BAR_JAN13,
        BAR_UPCA,
        BAR_NW7,
        BAR_NOT_USE_THIS_VALUE,
        BAR_CODE39,
        BAR_ITF,
        BAR_CODE128,
        BAR_CODE93,
        BAR_CUSTOMER,
    }

    public enum SatoBarcodeThickness
    {
        // 比率 1:2
        Thickness_1_2 = 1,

        // 比率	1:3
        Thickness_1_3,

        // 比率 2:5
        Thickness_2_5,

        // Jan文字付
        Thickness_Jan,
    }

    public class SplFormatter
    {
        private StringBuilder _formattedString = new StringBuilder();
        private SatoVector _vector;
        private int _pitch;
        private int _ext_ver = 1;
        private int _ext_hor = 1;
        private bool _bold = false;
        private bool _smooth = false;
        private int _offset_x = 0;
        private int _offset_y = 0;

        private const string ESC = "\x1b";
        private const string STX = "\x02";
        private const string ETX = "\x03";

        public string GetString()
        {
            return _formattedString.ToString();
        }

        public void Start()
        {
            _formattedString.Append(STX);
            _formattedString.Append(ESC);
            _formattedString.Append("A");
        }

        public void End(int count)
        {
            if (count > 0)
            {
                _formattedString.Append(ESC);
                _formattedString.Append("Q" + count);
            }

            _formattedString.Append(ESC);
            _formattedString.Append("Z");
            _formattedString.Append(ETX);
        }

        public void SetOffset(int x, int y)
        {
            _offset_x = x;
            _offset_y = y;
        }

        public void SetPaperSize(int ver, int hor)
        {
            _formattedString.Append(ESC);
            _formattedString.Append("A1");
            _formattedString.Append($"{ver:D04}{hor:D04}");
        }

        public void SetVector(SatoVector vector)
        {
            _vector = vector;
            _formattedString.Append(ESC);
            _formattedString.Append("%");
            _formattedString.Append(((int)_vector).ToString());
        }

        public void SetPitch(int pitch)
        {
            _pitch = pitch;
            _formattedString.Append(ESC);
            _formattedString.Append("P");
            _formattedString.AppendFormat("{0:D2}", _pitch);
        }

        public void SetPPitch(string v)
        {
            _formattedString.Append(ESC);
            _formattedString.Append(v);
        }

        public void SetBold(bool bold)
        {
            _bold = bold;
        }

        public void SetHor(int x)
        {
            _ext_hor = x;
            SetScale(_ext_hor, _ext_ver);
        }

        public void SetVer(int y)
        {
            _ext_ver = y;
            SetScale(_ext_hor, _ext_ver);
        }

        public void SetScale(int x, int y)
        {
            _ext_hor = x;
            _ext_ver = y;
            _formattedString.Append(ESC);
            _formattedString.Append("L");
            _formattedString.AppendFormat("{0:D2}", _ext_hor);
            _formattedString.AppendFormat("{0:D2}", _ext_ver);
        }

        private void SetCoordinate(int x, int y)
        {
            _formattedString.Append(ESC);
            _formattedString.Append("V");
            _formattedString.Append(_offset_y + y);
            _formattedString.Append(ESC);
            _formattedString.Append("H");
            _formattedString.Append(_offset_x + x);
        }

        public void String(int x, int y, SatoFont font, string str)
        {
            SetCoordinate(x, y);

            _formattedString.Append(ESC);

            switch (font)
            {
                default:
                case SatoFont.X21:
                    _formattedString.Append("X21,");
                    break;
                case SatoFont.X22:
                    _formattedString.Append("X22,");
                    break;
                case SatoFont.X23:
                    _formattedString.Append("X23,0");
                    break;
                case SatoFont.X24:
                    _formattedString.Append("X24,0");
                    break;
                case SatoFont.K16:
                    _formattedString.Append("K8B");
                    break;
                case SatoFont.K24:
                    if (_bold && _smooth)
                    {
                        _formattedString.Append("K9E");
                    }
                    else if (_bold)
                    {
                        _formattedString.Append("K9D");
                    }
                    else if (_smooth)
                    {
                        _formattedString.Append("K9C");
                    }
                    else
                    {
                        _formattedString.Append("K9B");
                    }

                    break;
            }

            _formattedString.Append(str);
        }

        public void Barcode(int x, int y, SatoBarcodeType type, SatoBarcodeThickness thickness, int iWidth, int iHeight, string data)
        {
            SetCoordinate(x, y);

            _formattedString.Append(ESC);

            switch (thickness)
            {
                default:
                case SatoBarcodeThickness.Thickness_1_2:
                    _formattedString.Append("D");
                    break;
                case SatoBarcodeThickness.Thickness_1_3:
                    _formattedString.Append("B");
                    break;
                case SatoBarcodeThickness.Thickness_2_5:
                case SatoBarcodeThickness.Thickness_Jan:
                    _formattedString.Append("BD");
                    break;
            }

            switch (type)
            {
                case SatoBarcodeType.BAR_JAN8:
                    _formattedString.Append("4");
                    break;
                case SatoBarcodeType.BAR_JAN13:
                    _formattedString.Append("3");
                    break;
                case SatoBarcodeType.BAR_UPCA:
                    _formattedString.Append("H");
                    break;
                case SatoBarcodeType.BAR_NW7:
                    _formattedString.Append("0");
                    break;
                case SatoBarcodeType.BAR_ITF:
                    _formattedString.Append("2");
                    break;
                case SatoBarcodeType.BAR_CODE39:
                    _formattedString.Append("1");
                    break;
                case SatoBarcodeType.BAR_CODE128:
                    _formattedString.Append("G");
                    break;
                case SatoBarcodeType.BAR_CUSTOMER:
                    _formattedString.Append("Z");
                    break;
                case SatoBarcodeType.BAR_CODE93:
                    _formattedString.Append("C");
                    break;
            }

            _formattedString.AppendFormat("{0:D2}", iWidth);
            _formattedString.AppendFormat("{0:D3}", iHeight);
            _formattedString.Append(data);
        }

        public void Reverse(int x, int y, int cx, int cy)
        {
            SetCoordinate(x, y);

            _formattedString.Append(ESC);
            _formattedString.Append("(");
            _formattedString.Append(cx);
            _formattedString.Append(",");
            _formattedString.Append(cy);
        }

        public void Line(int x, int y, int thickness, SatoVector vector, int ilength)
        {
            SetCoordinate(x, y);

            _formattedString.Append(ESC);
            _formattedString.Append("FW");
            _formattedString.AppendFormat("{0:D2}", thickness);

            if (vector == SatoVector.S0 || vector == SatoVector.S180)
            {
                _formattedString.Append("H");
            }
            else
            {
                _formattedString.Append("V");
            }

            _formattedString.AppendFormat("{0:D4}", ilength);
        }

        public const string QR_ERROR_COLLECTION_LEVEL_7 = "L";
        public const string QR_ERROR_COLLECTION_LEVEL_15 = "M";
        public const string QR_ERROR_COLLECTION_LEVEL_25 = "Q";
        public const string QR_ERROR_COLLECTION_LEVEL_30 = "H";

        public void QR2(int x, int y, string data, string errorCollectionLevel, int cellSize)
        {
            Debug.Assert(cellSize >= 1 && cellSize <= 32);

            SetCoordinate(x, y);

            _formattedString.Append(ESC);
            _formattedString.Append("2D30");
            _formattedString.AppendFormat($",{errorCollectionLevel},{cellSize:D02},1,0");

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var sjisText = Encoding.GetEncoding("SJIS").GetBytes(data);
            _formattedString.Append(ESC);
            _formattedString.Append($"DN{sjisText.Length},{data}");

        }
    }
}
