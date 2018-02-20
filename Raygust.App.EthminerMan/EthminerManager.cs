using System;
using System.Diagnostics;
using System.Threading;
using Raygust.Core.Parser;

namespace Raygust.App.EthminerMan
{
    internal class EthminerManager
    {
        #region constants
        const double DEFAULT_THRESHOLD = 10.0; //Measure in MH/s.
        const int DEFAULT_THRESHOLD_DURATION = 300; //Measure in seconds wait.
        const int CHECK_INTERVAL = 1000; //Measure in milliseconds.
        const string ETHMINER_FILENAME = "ethminer.exe";
        const string ETHMINERMAN_FILENAME = "EthminerMan.exe";
        #endregion

        #region variables
        string[] _args = null;
        bool _runningConsole = false;
        bool isDebug = false;
        EthminerParser parser = new EthminerParser();
        DateTime _lastErrorTime = DateTime.Now;
        int _lastErrorCount = 0;
        #endregion

        public Process minerProcess = new Process();
        public int RestartDelay = 15000;
        public string UnknownErrorScript = string.Empty;
        public string UnknownErrorScriptPath = string.Empty;


        public EthminerManager(string[] args)
        {
            _args = (string[])args.Clone();
        }

        public void Start(bool runningConsole)
        {
            _runningConsole = runningConsole;
            ProcessStartInfo info = new ProcessStartInfo();
            ArgumentConfigurations argConfigs = new ArgumentConfigurations();
            ArgumentParser argParser = new ArgumentParser(_args, argConfigs);
            argConfigs.Load(Properties.Resources.Parameters);
            double minSpeedThreshold = DEFAULT_THRESHOLD;
            int duration = DEFAULT_THRESHOLD_DURATION;

            #region EthminerMan Argument processing
            argParser.GetParamAsDouble("-tlimit", ref minSpeedThreshold);
            parser.minSpeedThreshold = minSpeedThreshold;
            argParser.GetParamAsInt("-dlimit", ref duration);
            parser.duration = duration;
            isDebug = argParser.ParamExist("-debug");
            argParser.GetParamAsInt("-dstart", ref RestartDelay);
            #endregion

            #region Ethminer Process Information settings
            info.Arguments = string.Join(" ", argParser.ExtractUnknownArguments());
            info.FileName = ETHMINER_FILENAME;
            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            info.RedirectStandardInput = true;
            info.CreateNoWindow = true;
            #endregion

            #region Ethminer process setup
            minerProcess.StartInfo = info;
            minerProcess.EnableRaisingEvents = true;
            minerProcess.OutputDataReceived += ProcessConsoleOutput;
            minerProcess.ErrorDataReceived += ProcessConsoleOutput;
            minerProcess.Start();
            minerProcess.BeginOutputReadLine();
            minerProcess.BeginErrorReadLine();
            #endregion
        }

        //Interval check on threshold condition
        public void Process(ref bool endProcess)
        {
            Monitor.Enter(parser);
            if (parser.Break())
            {
                minerProcess.Kill();
                if(_runningConsole)
                {
                    ProcessStartInfo newStartInfo = new ProcessStartInfo();
                    newStartInfo.Arguments = String.Join(" ", _args);
                    newStartInfo.FileName = ETHMINERMAN_FILENAME;
                    Process newProcess = new Process();
                    newProcess.StartInfo = newStartInfo;
                    Thread.Sleep(RestartDelay);
                    newProcess.Start();
                    endProcess = true;
                }
                else
                {
                    minerProcess = new Process();
                    Start(_runningConsole);
                }

            }
            Monitor.Exit(parser);
        }

        public void Stop()
        {
            if(!minerProcess.HasExited)
            {
                minerProcess.Kill();
            }
        }

        private void HandleRestartScript()
        {
            ProcessStartInfo newStartInfo = new ProcessStartInfo();
            newStartInfo.Arguments = String.Join(" ", _args);
            string[] parts = UnknownErrorScript.Split(' ');
            newStartInfo.FileName = parts[0];
            if (parts.Length > 1)
            {
                newStartInfo.Arguments = string.Join(" ", parts, 1, parts.Length - 1);
            }
            newStartInfo.WorkingDirectory = UnknownErrorScriptPath;
            Process newProcess = new Process();
            newProcess.StartInfo = newStartInfo;
            newProcess.Start();
        }

        private void ProcessConsoleOutput(object sender, DataReceivedEventArgs e)
        {
            Monitor.Enter(parser);
            if (isDebug && _runningConsole)
            {
                Console.ResetColor();
                Console.WriteLine(e.Data);
            }

            parser.Parse(e.Data);
            if (_runningConsole)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                parser.PrintSpeedMessage();
                if (parser.IsUnknownError(e.Data))
                {
                    if(string.IsNullOrEmpty(UnknownErrorScript))
                    {
                        HandleRestartScript();
                    }
                }
            }
            
            Monitor.Exit(parser);
        }
    }
}
