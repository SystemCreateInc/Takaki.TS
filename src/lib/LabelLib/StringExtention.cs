using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelLib
{
    public static class StringExtention
    {
        // SJISの長さで文字列を分割する
        public static IEnumerable<string> ChunkBySjis(this string source, int size)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var sjisEnc = Encoding.GetEncoding("SJIS");

            string currentText = "";
            int currentSize = 0;
            foreach (var c in source)
            {
                var charSize = sjisEnc.GetByteCount(new string(c, 1));
                if ((currentSize + charSize) >= size)
                {
                    yield return currentText;
                    currentSize = 0;
                    currentText = "";
                }

                currentSize += charSize;
                currentText += c;
            }

            if (currentText != "")
            {
                yield return currentText;
            }
        }
    }
}
