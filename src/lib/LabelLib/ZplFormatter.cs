using System.Text;

namespace LabelLib
{
    public class ZplFormatter
    {
        //  使用可能フォント
        //* Z:A.FNT      6839  P A  
        //* Z:B.FNT      7746  P B  
        //* Z:D.FNT     10648  P CD 
        //* Z:E12.FNT     15691  P E  
        //* Z:E24.FNT     47056  P
        //* Z:E6.FNT      8520  P
        //* Z:E8.FNT     10893  P
        //* Z:F.FNT     13275  P F  
        //* Z:G.FNT     49663  P G  
        //* Z:GS.FNT      5470  P?
        //* Z:H12.FNT     11536  P H  
        //* Z:H24.FNT     30436  P
        //* Z:H6.FNT      7247  P
        //* Z:H8.FNT      7825  P
        //* Z:MONOBD15.FNT      5570  P
        //* Z:NJ20WGL4.FNT    359678  P
        //* Z:NJ26WGL4.FNT    566518  P
        //* Z:NK20WGL4.FNT    710064  P
        //* Z:NK26WGL4.FNT   1081154  P
        //* Z:NS20WGL4.FNT   1248664  P
        //* Z:NS26WGL4.FNT   2025530  P
        //* Z:NT20WGL4.FNT    847138  P
        //* Z:NT26WGL4.FNT   1351834  P
        //* Z:P.FNT       116  P P  
        //* Z:Q.FNT       116  P Q  
        //* Z:R.FNT       116  P R  
        //* Z:S.FNT       116  P S  
        //* Z:T.FNT       116  P T  
        //* Z:U.FNT       116  P U  
        //* Z:V.FNT       116  P V
        //* E:GOTHIC35.FNT    999760       J
        //* E:TT0003M_.TTF    169188          

        private StringBuilder _formattedString = new StringBuilder();
        private int _offsetHor = 0;   // オフセット
        private int _offsetVer = 0;   // オフセット 

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

        public void Start()
        {
            _formattedString.Append("^XA");
            _formattedString.Append($"^CI28\r\n");
        }

        public void End(int labelCount)
        {
            _formattedString.Append("^XZ\r\n");
        }

        public enum Orientation {
            R0,
            R90,
            R180,
            R270,
        }

        string GetOrientation(Orientation ori)
        {
            switch (ori)
            {
                case Orientation.R0:
                    return "N";

                case Orientation.R90:
                    return "R";

                case Orientation.R180:
                    return "I";

                case Orientation.R270:
                    return "B";
            }

            return "";
        }

        public void String(int x, int y, int height, int width, string font, string data, Orientation orientation = Orientation.R0)
        {
            if (font.Length == 1)
            {
                _formattedString.Append($"^A{font}{GetOrientation(orientation)},{height},{width}");
            }
            else
            {
                _formattedString.Append($"^A@{GetOrientation(orientation)},{height},{width},{font}");
            }

            _formattedString.AppendFormat($"^FO{x + _offsetHor},{y + _offsetVer}");
            _formattedString.AppendFormat($"^FD{data}^FS\r\n");
        }

        public enum BarcodeType
        {
            Code39 = 3,
            Code128 = 9
        }

        private string GetPrintInterpretation(bool on)
        {
            return on ? "Y" : "N";
        }

        ///  <summary>
        ///  Code128バーコード印字
        ///  
        ///  Start Code A: >9
        ///  Start Code B: >:
        ///  Start Code C: >;
        ///  </summary>
        ///  <param name="moduleWidth">1 to 10</param>
        ///  <param name="barRatio">2.0 to 3.0, 0.1 step</param>
        public void BarcodeCode128(int x, int y, int height, int moduleWidth, int barRatio, string data, bool printText, Orientation orientation = Orientation.R0)
        {
            _formattedString.AppendFormat($"^FO{x + _offsetHor},{y + _offsetVer}^BY{moduleWidth},{barRatio}");
            _formattedString.Append($"^BC{GetOrientation(orientation)},{height},{GetPrintInterpretation(printText)}");
            _formattedString.Append($"^FD{data}^FS");
        }

        ///  <summary>
        ///  EANバーコード印字
        ///  </summary>
        ///  <param name="moduleWidth">1 to 10</param>
        ///  <param name="barRatio">2.0 to 3.0, 0.1 step</param>
        public void BarcodeEAN(int x, int y, int height, int moduleWidth, float barRatio, string data, bool printText,
                               Orientation orientation = Orientation.R0)
        {
            var command = data.Length == 8 ? "B8" : "BE";
            _formattedString.AppendFormat($"^FO{x + _offsetHor},{y + _offsetVer}^BY{moduleWidth},{barRatio}");
            _formattedString.Append($"^{command}{GetOrientation(orientation)},{height},{GetPrintInterpretation(printText)}");
            _formattedString.Append($"^FD{data}^FS");
        }

        public void Box(int x, int y, int height, int width, int thickness, bool black = true, int round = 0)
        {
            var color = black ? "B" : "W";
            _formattedString.AppendFormat($"^FO{x + _offsetHor},{y + _offsetVer}");
            _formattedString.Append($"^GB{width},{height},{thickness},{color},{round}^FS");
        }

        public void Reverse(int x, int y, int height, int width)
        {
            _formattedString.AppendFormat($"^LRY");
            _formattedString.AppendFormat($"^FO{x + _offsetHor},{y + _offsetVer}");
            _formattedString.Append($"^GB{width},{height},{width}^FS");
        }
    }
}

