using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrComp
{
    public class Program
    {
        private static int BufferSize { get; set; }

        private static void PrintHelp()
        {
            Console.WriteLine("OrComp - Oracle Compression");
            Console.WriteLine("Programmed by Johan Welgemoed (Original Java Implementation: 13/09/2004, .Net Port: 23/07/2016)");
            Console.WriteLine("Usage : ");
            Console.WriteLine("OrComp -[Options] filein fileout");
            Console.WriteLine("Options : ");
            Console.WriteLine("  -c : Use the file size as buffer size (max. compression)");
            Console.WriteLine("  -z <buffersize> : Specify buffer size (save memory)");
            Console.WriteLine("  -x : Decompress input file");
            Console.WriteLine("  -fibsize <fibosize> : Specify the number of Fibonacci numbers to generate [37 max].");
            Console.WriteLine("  -debug : Prints out debug information");
            Console.WriteLine("  -stat : Prints out additional automata information");
            Console.WriteLine("  -help : Prints this help information");
            Console.WriteLine("Examples : ");
            Console.WriteLine("OrComp -c readme.txt readme.orc");
            Console.WriteLine("OrComp -z 512000 readme.txt readme.orc");
            Console.WriteLine("OrComp -x readme.orc readme.txt");
            Console.WriteLine("OrComp -stat -c readme.txt readme.orc");
            Console.WriteLine("OrComp -debug -x readme.orc readme.txt");
            Console.WriteLine("OrComp -debug -stat -z 512000 readme.txt readme.orc");
            Console.WriteLine();
            Console.WriteLine("Note on Fibsize :");
            Console.WriteLine("Specifying a to low or to high number of Fibonacci numbers will result");
            Console.WriteLine("in incorrect compression, the max. number of Fib numbers is 37,");
            Console.WriteLine("this means that OrComp can handle a file of size 1667MB, given that");
            Console.WriteLine("the length of the longest repition is not more than 14930352.");
            Console.WriteLine("Making it to low will result that the lengths and the positions of");
            Console.WriteLine("repitions will not be represented properly, leading to unspecified results (ie. garbage).");
        }

        public static void Main(string[] args)
        {
            BufferSize = 12000000;
            int argcount = 0;

            //args = new string[3];
            //args[0] = "-x";
            //args[2] = "Test.txt";
            //args[1] = "test.orc";

            try
            {
                switch (args[argcount])
                {
                    case "-debug":
                        EnableDebugging();
                        argcount++;
                        break;
                    case "-stat":
                        EnableStatistics();
                        argcount++;
                        break;
                    case "-fibsize":
                        int fibonacciSize = int.Parse(args[argcount + 1]);
                        SetFibonacciSize(fibonacciSize);
                        argcount = argcount + 2;
                        break;
                    case "-z":
                        int bufferSize = int.Parse(args[argcount + 1]);

                        CompressFileWithSpecifiedBufferSize(bufferSize, args[argcount + 2], args[argcount + 3]);
                        argcount += 4;
                        break;
                    case "-c":
                        CompressFileWithFileBufferSize(args[argcount + 1], args[argcount + 2]);
                        argcount += 3;
                        break;
                    case "-x":
                        DecompressFile(args[argcount + 1], args[argcount + 2]);
                        argcount += 3;
                        break;
                    case "-help":
                    default:
                        PrintHelp();
                        break;

                };
            }
            catch (IndexOutOfRangeException)
            {
                PrintHelp();
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Input file not found!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred.");
                PrintHelp();
            }
        }

        private static void DecompressFile(string inputFileName, string outputFileName)
        {
            var timeSpan = TimedAction(() =>
             {

                 OracleDecoder _decoder = new OracleDecoder();

                 using (var inputStream = new FileStream(inputFileName, FileMode.Open))
                 {
                     using (var outputStream = new FileStream(outputFileName, FileMode.CreateNew))
                     {
                         using (var bitInputStream = new BitInputStream(inputStream))
                         {
                             _decoder.Decompress(bitInputStream, outputStream, BufferSize);
                         }
                     }
                 }
             });

            Console.WriteLine("Decompression Time : {0}", timeSpan.ToString("g"));
        }

        private static void CompressFileWithFileBufferSize(string inputFileName, string outputFileName)
        {
            var fileInfo = new FileInfo(inputFileName);
            CompressFile((int)fileInfo.Length, inputFileName, outputFileName);
        }

        private static void CompressFileWithSpecifiedBufferSize(int bufferSize, string inputFileName, string outputFileName)
        {
            CompressFile(bufferSize, inputFileName, outputFileName);
        }

        private static void CompressFile(int bufferSize, string inputFileName, string outputFileName)
        {
            var fileInfo = new FileInfo(inputFileName);
            BufferSize = bufferSize;

            Console.WriteLine("Compressing {0} to {1}", inputFileName, outputFileName);
            Console.WriteLine("Buffer Size : {0}", BufferSize);
            Console.WriteLine("File Size : {0}", fileInfo.Length);
            Console.WriteLine("Compressing");

            OrCalc orcalc = new OrCalc();

            var timespan = TimedAction(() =>
            {
                using (var inputStream = new FileStream(inputFileName, FileMode.Open))
                using (var outputStream = new FileStream(outputFileName, FileMode.CreateNew))
                {
                    orcalc.Compress(inputStream, outputStream, BufferSize);
                }
            });

            var compressedFileInfo = new FileInfo(outputFileName);

            float perc = ((float)(100 - (float)(compressedFileInfo.Length * 100) / fileInfo.Length));
            float bpc = ((float)(compressedFileInfo.Length * 8) / fileInfo.Length);

            Console.WriteLine("Compressed File Size : {0} ({1} %)", compressedFileInfo.Length, perc);
            Console.WriteLine("Bits per Character : {0}", bpc);
            Console.WriteLine("Encoding Time : {0}", timespan.ToString("g"));
            Console.WriteLine("Length of maximum repition : {0}", orcalc.MaxRepeat);
        }

        private static void SetFibonacciSize(int fibonacciSize)
        {
            Fibonacci.FibonacciSize = fibonacciSize;
        }

        private static void EnableStatistics()
        {
           // OrCalc.setSTAT(true);
        }

        private static void EnableDebugging()
        {
           // OrCalc.setDEBUG(true);
           // BitLevelDecoding.setDEBUG(true);
        }

        private static TimeSpan TimedAction(Action action)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            action.Invoke();

            stopwatch.Stop();

            return stopwatch.Elapsed;
        }
    }
}
