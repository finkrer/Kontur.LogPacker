using System;
using System.IO;
using System.IO.Compression;
using Kontur.LogPackerGZip;

namespace Kontur.LogPacker
{
    internal static class EntryPoint
    {
        public static void Main(string[] args)
        {
			if (args.Length == 2)
	        {
		        var (inputFile, outputFile) = (args[0], args[1]);

		        if (File.Exists(inputFile))
		        {
			        Compress(inputFile, outputFile);
			        return;
		        }
	        }

	        if (args.Length == 3 && args[0] == "-d")
	        {
		        var (inputFile, outputFile) = (args[1], args[2]);
		        if (File.Exists(inputFile))
		        {
			        Decompress(inputFile, outputFile);
			        return;
		        }
	        }

	        ShowUsage();
		}

	    private static void ShowUsage()
	    {
		    Console.WriteLine("Usage:");
		    Console.WriteLine($"{AppDomain.CurrentDomain.FriendlyName} [-d] <inputFile> <outputFile>");
		    Console.WriteLine("\t-d flag turns on the decompression mode");
		    Console.WriteLine();
	    }

	    private static void Compress(string inputFile, string outputFile)
	    {
		    var temporaryFile = CreateUniqueFileName();
		    try
		    {
			    using (var inputStream = File.OpenRead(inputFile))
			    using (var temporaryStream = File.Open(temporaryFile, FileMode.Create, FileAccess.Write))
				    new LogCompressor().Compress(inputStream, temporaryStream);
		    
			    using (var outputStream = File.Open(outputFile, FileMode.Create, FileAccess.Write))
			    using (var gzipStream = new GZipStream(outputStream, CompressionLevel.Optimal, true))
			    using (var temporaryStream = File.OpenRead(temporaryFile))
				    temporaryStream.CopyTo(gzipStream);
		    }
		    finally
		    {
			    File.Delete(temporaryFile);
		    }

	    }

	    private static void Decompress(string inputFile, string outputFile)
	    {
		    var temporaryFile = CreateUniqueFileName();
		    try
		    {
			    {
					using var inputStream = File.OpenRead(inputFile);
				    using var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress, true);
				    using var temporaryStream = File.Open(temporaryFile, FileMode.Create, FileAccess.Write);
				    gzipStream.CopyTo(temporaryStream);
			    }

			    {
				    using var temporaryStream = File.OpenRead(temporaryFile);
				    using var outputStream = File.Open(outputFile, FileMode.Create, FileAccess.Write);
				    new LogCompressor().Decompress(temporaryStream, outputStream);
			    }
		    }
		    finally
		    {
			    File.Delete(temporaryFile);
		    }
	    }
	    
	    private static string CreateUniqueFileName()
	    {
		    string fileName;
		    do
		    {
			    fileName = Guid.NewGuid().ToString();
		    } while (File.Exists(fileName));

		    return fileName;
	    }
    }
}