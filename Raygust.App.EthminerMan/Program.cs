using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace Raygust.App.EthminerMan
{
    class Program
    {
        static EthminerParser parser = new EthminerParser();

        static void Main(string[] args)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            string arg = String.Join(" ", args);
            int indexstart = -1;
            bool special = false;
            bool special2 = false;
            double minSpeedThreshold = 10.0;
            int duration = 300;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("-tlimit"))
                {
                    special = true;
                    continue;
                }
                if (special && args[i].StartsWith("-"))
                {
                    indexstart = i;
                    double.TryParse(args[i - 1], out minSpeedThreshold);
                    parser.minSpeedThreshold = minSpeedThreshold;
                    break;
                }
            }
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("-dlimit"))
                {
                    special2 = true;
                    continue;
                }
                if (special2 && args[i].StartsWith("-"))
                {
                    if (i > indexstart)
                        indexstart = i;
                    int.TryParse(args[i - 1], out duration);
                    parser.duration = duration;
                    break;
                }
            }

            if (special || special2)
            {
                arg = String.Join(" ", args, indexstart, args.Length - indexstart);
            }


            info.Arguments = arg;
            info.FileName = @"ethminer.exe";
            Process p = new Process();
            p.StartInfo = info;
            p.EnableRaisingEvents = true;
            p.OutputDataReceived += P_OutputDataReceived;
            p.ErrorDataReceived += P_ErrorDataReceived;

            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            info.RedirectStandardInput = true;
            info.CreateNoWindow = true;

            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            while (!p.WaitForExit(1000))
            {
                Monitor.Enter(parser);
                if (parser.Break())
                {
                    p.Kill();
                    ProcessStartInfo newStartInfo = new ProcessStartInfo();
                    newStartInfo.Arguments = String.Join(" ", args);
                    newStartInfo.FileName = "EthminerMan.exe";
                    Process newProcess = new Process();
                    newProcess.StartInfo = newStartInfo;
                    newProcess.Start();
                    break;
                }
                Monitor.Exit(parser);
            }

        }

        private static void P_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.ResetColor();
            Console.WriteLine(e.Data);
            Monitor.Enter(parser);
            Console.ForegroundColor = ConsoleColor.Green;
            parser.Parse(e.Data);
            parser.PrintSpeedMessage();
            Monitor.Exit(parser);
        }

        static void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Debug.Print("Out: " + e.Data);
        }
    }
}
