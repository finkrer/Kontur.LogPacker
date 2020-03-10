using System.Collections.Generic;
using static Kontur.LogPacker.Constants;

namespace Kontur.LogPacker
{
    internal class LineCompressor
    {
        private readonly List<byte> line;
        private readonly List<byte> previousLine;
        private int lineIndex;
        private int prevIndex;
        private int sameCharCounter;
        private List<byte> result;
        
        public LineCompressor(List<byte> line, List<byte> previousLine)
        {
            this.line = line;
            this.previousLine = previousLine;
        }

        public byte[] Compress()
        {
            result ??= CompressImpl();
            return result.ToArray();
        }

        private List<byte> CompressImpl()
        {
            result = new List<byte>(line.Capacity);
            
            while (lineIndex < line.Count)
            {
                switch (line[lineIndex])
                {
                    case (byte) '\n':
                        FinishLine();
                        break;
                    
                    case byte b when b == previousLine[prevIndex]:
                        MoveLinesToNextSymbol();
                        break;
                    
                    default:
                        AddCounterOrValue();
                        MoveLinesToNextWord();
                        if (prevIndex >= previousLine.Count)
                            AddRemainingSymbols();
                        break;
                }
            }

            return result;
        }

        private void AddAndEscape(byte value)
        {
            if (value >= EscapeValue)
                result.Add(EscapeValue);
            result.Add(value);
        }

        private void AddCounterOrValue()
        {
            if (sameCharCounter <= 0)
                return;
            if (sameCharCounter == 1)
                AddAndEscape(line[lineIndex - 1]);
            else
                result.Add((byte) (EscapeValue + sameCharCounter));
            sameCharCounter = 0;
        }

        private void FinishLine()
        {
            AddCounterOrValue();
            result.Add(line[lineIndex++]);
        }

        private void MoveLinesToNextSymbol()
        {
            lineIndex++;
            prevIndex++;
            sameCharCounter++;
        }

        private void MoveLineToNextWord()
        {
            while (lineIndex < line.Count && line[lineIndex] != 32) 
                AddAndEscape(line[lineIndex++]);
            while (lineIndex < line.Count && line[lineIndex] == 32) 
                result.Add(line[lineIndex++]);
        }

        private void MovePrevLineToNextWord()
        {
            while (prevIndex < previousLine.Count && previousLine[prevIndex] != 32)
                prevIndex++;
            while (prevIndex < previousLine.Count && previousLine[prevIndex] == 32)
                prevIndex++;
        }

        private void MoveLinesToNextWord()
        {
            MoveLineToNextWord();
            MovePrevLineToNextWord();
        }

        private void AddRemainingSymbols()
        {
            while (lineIndex < line.Count)
                AddAndEscape(line[lineIndex++]);
        }
    }
}