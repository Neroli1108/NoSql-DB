/////////////////////////////////////////////////////////////////////////
// Testexcu.cs - Help to run the WPF and Server automatically          //
// ver 1.0                                                             //
// YundingLI, CSE681 - Software Modeling and Analysis, Project #4      //
/////////////////////////////////////////////////////////////////////////
/*
 * Maintenance History:
 * --------------------
 * ver 1.0 : 18 Nov 2015
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLauncher
{
    class Program
    {
        // automatically run the Wpf and Server
        static void Main(string[] args)
        {
            const string serverPath = @"..\..\..\Server\bin\Debug\Server.exe";
            const string readerPath = @"..\..\..\WpfReadClient\bin\Debug\WpfApplication1.exe";
            const string writerPath = @"..\..\..\WpfClient\bin\Debug\WpfApplication1.exe";
            var readerCount = 1;
            var writerCount = 1;
            // Parse parameters
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToUpperInvariant())
                {
                    case "-R":
                        readerCount = Convert.ToInt32(args[i + 1]);
                        i++;
                        break;
                    case "-W":
                        writerCount = Convert.ToInt32(args[i + 1]);
                        i++;
                        break;
                    default:
                        Console.WriteLine($"Unrecognized parameter: {args[i]}");
                        return;
                }
            }
            // Start processes
            Console.WriteLine("Starting server");
            LaunchApp(serverPath, "", 1);
            Console.WriteLine("Starting reader x{0}", readerCount);
            LaunchApp(Path.GetFullPath(readerPath), "", readerCount);
            Console.WriteLine("Starting writer x{0}", writerCount);
            LaunchApp(Path.GetFullPath(writerPath), "", writerCount);
            Console.WriteLine("Complete");
        }

        
        static void LaunchApp(string path, string arg, int count)
        {
            var fullPath = Path.GetFullPath(path);
            var parentDir = Path.GetDirectoryName(fullPath) ?? "";
            for (int i = 0; i < count; i++)
            {
                var proc = new Process {StartInfo = new ProcessStartInfo(path, arg) {WorkingDirectory = parentDir}};
                proc.Start();
            }
        }
    }
}
