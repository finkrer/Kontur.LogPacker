using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kontur.LogPacker
{
    internal class LogStreamReader : IDisposable
    {
        private readonly BinaryReader reader;
        private readonly long length;
        private long position;
        public bool ReachedEnd => position >= length;

        public LogStreamReader(Stream stream)
        {
            reader = new BinaryReader(stream);
            length = reader.BaseStream.Length;
        }

        public IEnumerable<byte> ReadLine(int maxLength)
        {
            byte nextByte = 0;
            for (var i = 0; nextByte != '\n'; i++)
            {
                if (i > maxLength && nextByte != Constants.EscapeValue || ReachedEnd)
                    yield break;

                yield return nextByte = ReadByte();
            }
        }

        private byte ReadByte()
        {
            position++;
            return reader.ReadByte();
        }

        public void Dispose()
        {
            reader?.Dispose();
        }
    }
}