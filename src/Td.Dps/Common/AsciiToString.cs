using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Td.Dps
{
    internal class AsciiToString
    {
        /// <summary>
        /// 制御コード置換文字列定義
        /// </summary>
        internal static string[] ControlString = 
        {
            "[NUL]", "[SOH]", "[STX]", "[ETX]", 
            "[EOT]", "[ENQ]", "[ACK]", "[BEL]", 
            "[BS]",  "[HT]",  "[LF]",  "[HM]", 
            "[CL]",  "[CR]",  "[SO]",  "[SI]", 
            "[DLE]", "[DC1]", "[DC2]", "[DC3]", 
            "[DC4]", "[NAK]", "[SYN]", "[ETB]", 
            "[CAN]", "[EM]",  "[SUB]", "[ESC]", 
            "[FS]",  "[GS]",  "[RS]",  "[US]", 
        };

        /// <summary>
        /// ASCIIの文字列変換
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        internal static string ToString(byte[] bytes)
        {
            string str = Encoding.ASCII.GetString(bytes);

            StringBuilder sb = new StringBuilder();
            //foreach (char c in bytes.Select(b => (char)b))
            foreach (char c in str)
            {
                if (c <= 0x1F)
                {
                    sb.Append(ControlString[c]);
                }
                else if (c == 0x7F)
                {
                    sb.Append("[DEL]");
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}
