using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Kontur.LogPacker.Constants;

namespace Kontur.LogPacker
{
    internal class Decompressor : IDisposable
    {
        private readonly Stream inputStream;
        private readonly Stream outputStream;
        private List<byte> previousLine;

        public Decompressor(Stream inputStream, Stream outputStream)
        {
            this.inputStream = inputStream;
            this.outputStream = outputStream;
        }

        public void Decompress()
        {
            using var logStreamReader = new LogStreamReader(inputStream);
            using var binaryWriter = new BinaryWriter(outputStream, Encoding.UTF8);
            
            previousLine = logStreamReader.ReadLine(MaxLineLength).ToList();
            binaryWriter.Write(previousLine.ToArray());
                
            while (!logStreamReader.ReachedEnd)
            {
                var line = logStreamReader.ReadLine(MaxLineLength).ToList();
                var decompressedLine = new LineDecompressor(line, previousLine).Decompress();
                binaryWriter.Write(decompressedLine);
                previousLine = decompressedLine.ToList();
            }
        }

        public void Dispose()
        {
            inputStream?.Dispose();
            outputStream?.Dispose();
        }
    }
}