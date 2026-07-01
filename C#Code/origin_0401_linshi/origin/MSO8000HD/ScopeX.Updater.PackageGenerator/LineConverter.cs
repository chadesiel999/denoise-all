using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Updater.PackageGenerator
{
    internal class LineConverter
    {
        public string[] allLines;
        public int lineCount;
        public int state;
        public int ByteCount;
        public int startLineNo;
        public int actualByteCount;
        public Byte[] byteContent;
        public void ExecConvert()
        {
            int byteIndex = 0;
            byte byteValue;
            int i;
            Int32 lineNo;
            string lineStr;
            string byteStr;
            Int32 binCharCount;
            int lineLength;
            state = 0;
            byteContent = new Byte[ByteCount];
            actualByteCount = 0;
            for (lineNo = 0; lineNo < lineCount; lineNo++)
            {
                lineStr = allLines[lineNo];

                lineLength = lineStr.Length;
                if (lineLength <= 11)
                    continue;
                if (lineStr[8] != '0')
                    continue;

                lineStr = lineStr.Substring(9, lineLength - 9 - 2);
                binCharCount = lineLength - 9 - 2;

                i = 0;
                do
                {
                    byteStr = lineStr.Substring(i, 2);
                    byteValue = Convert.ToByte(byteStr, 16);
                    byteContent[byteIndex++] = byteValue;
                    actualByteCount++;
                    i += 2;
                } while (i < binCharCount);
            }
            state = 1;
        }
    }
}
