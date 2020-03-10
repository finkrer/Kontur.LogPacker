
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kontur.LogPacker
{
	internal class LogCompressor : ICompressor
	{
		public void Compress(Stream inputStream, Stream outputStream)
		{
			using var compressor = new Compressor(inputStream, outputStream);
			compressor.Compress();
		}

		public void Decompress(Stream inputStream, Stream outputStream)
		{
			using var decompressor = new Decompressor(inputStream, outputStream);
			decompressor.Decompress();
		}
	}
}
