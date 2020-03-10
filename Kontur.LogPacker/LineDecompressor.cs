using System.Collections.Generic;
using static Kontur.LogPacker.Constants;

namespace Kontur.LogPacker
{
    internal class LineDecompressor
    {
        private readonly List<byte> line;
        private readonly List<byte> previousLine;
        private int lineIndex;
        private int prevIndex;
        private List<byte> result;
        
        public LineDecompressor(List<byte> line, List<byte> previousLine)
        {
            this.line = line;
            this.previousLine = previousLine;
        }
        
        public byte[] Decompress()
        {
            result ??= DecompressImpl();
            return result.ToArray();
        }

        private List<byte> DecompressImpl()
        {
            result = new List<byte>(line.Capacity);
            
            while (lineIndex < line.Count)
            {
                switch (line[lineIndex])
                {
                    case byte v when v > EscapeValue:
                        AddLinkedValues();
                        break;
                    
                    case EscapeValue:
                        SkipEscapedValue();
                        break;
                    
                    case (byte) ' ':
                        MoveLinesToNewWord();
                        break;
                    
                    default:
                        result.Add(line[lineIndex++]);
                        break;
                }
            }

            return result;
        }

        private void AddLinkedValues()
        {
            for (var targetIndex = line[lineIndex++] - EscapeValue + prevIndex; prevIndex < targetIndex; prevIndex++)
                result.Add(previousLine[prevIndex]);
        }

        private void SkipEscapedValue()
        {
            result.Add(line[++lineIndex]);
            lineIndex++;
        }

        private void MoveLinesToNewWord()
        {
            while (lineIndex < line.Count && line[lineIndex] == 32) 
                result.Add(line[lineIndex++]);
            while (prevIndex < previousLine.Count && previousLine[prevIndex] != 32)
                prevIndex++;
            while (prevIndex < previousLine.Count && previousLine[prevIndex] == 32)
                prevIndex++;
        }
    }
}
