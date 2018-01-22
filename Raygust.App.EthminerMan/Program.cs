using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Raygust.Core.Parser;

namespace Raygust.App.EthminerMan
{
    class Program
    {
        const double DEFAULT_THRESHOLD = 10.0; //Measure in MH/s.
        const int DEFAULT_THRESHOLD_DURATION = 300; //Measure in seconds wait.
        const int CHECK_INTERVAL = 1000; //Measure in milliseconds.
        static EthminerParser parser = new EthminerParser();

        static void Main(string[] args)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            ArgumentConfigurations argConfigs = new ArgumentConfigurations();
            ArgumentParser argParser = new ArgumentParser(args, argConfigs);
            argConfigs.Load(Properties.Resources.Parameters);
            double minSpeedThreshold = DEFAULT_THRESHOLD;
            int duration = DEFAULT_THRESHOLD_DURATION;

            argParser.GetParamAsDouble("-tlimit", ref minSpeedThreshold);
            parser.minSpeedThreshold = minSpeedThreshold;
            argParser.GetParamAsInt("-dlimit", ref duration);
            parser.duration = duration;

            info.Arguments = string.Join(" ", argParser.ExtractUnknownArguments());
            info.FileName = @"ethminer.exe";
            Process p = new Process();
            p.StartInfo = info;
            p.EnableRaisingEvents = true;
            p.OutputDataReceived += ProcessConsoleOutput;
            p.ErrorDataReceived += ProcessConsoleOutput;

            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            info.RedirectStandardInput = true;
            info.CreateNoWindow = true;

            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            while (!p.WaitForExit(CHECK_INTERVAL))
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

        private static void ProcessConsoleOutput(object sender, DataReceivedEventArgs e)
        {
            Monitor.Enter(parser);
            Console.ResetColor();
            Console.WriteLine(e.Data);
            Console.ForegroundColor = ConsoleColor.Green;
            parser.Parse(e.Data);
            parser.PrintSpeedMessage();
            Monitor.Exit(parser);
        }
    }
}
