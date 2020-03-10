using System.IO;

namespace Kontur.LogPacker
{
	internal interface ICompressor
	{
		void Compress(Stream inputStream, Stream outputStream);
		void Decompress(Stream inputStream, Stream outputStream);
	}
}