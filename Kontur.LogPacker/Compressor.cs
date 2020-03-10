using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Kontur.LogPacker.Constants;

namespace Kontur.LogPacker
{
    internal class Compressor : IDisposable
    {
        private readonly Stream inputStream;
        private readonly Stream outputStream;
        private List<byte> previousLine;

        public Compressor(Stream inputStream, Stream outputStream)
        {
            this.inputStream = inputStream;
            this.outputStream = outputStream;
        }

        public void Compress()
        {
            using var logStreamReader = new LogStreamReader(inputStream);
            using var binaryWriter = new BinaryWriter(outputStream, Encoding.UTF8);
            previousLine = logStreamReader.ReadLine(MaxLineLength).ToList();
            binaryWriter.Write(previousLine.ToArray());

            while (!logStreamReader.ReachedEnd)
            {
                var line = logStreamReader.ReadLine(MaxLineLength).ToList();
                var compressedLine = new LineCompressor(line, previousLine).Compress();
                binaryWriter.Write(compressedLine);
                previousLine = line;
            }
        }

        public void Dispose()
        {
            inputStream?.Dispose();
            outputStream?.Dispose();
        }
    }
}