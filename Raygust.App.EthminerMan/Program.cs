using System;
using System.Diagnostics;
using System.Threading;

namespace Raygust.App.EthminerMan
{
    class Program
    {
        #region constants
        const int CHECK_INTERVAL = 1000; //Measure in milliseconds.
        const string ETHMINERMAN_FILENAME = "EthminerMan.exe";
        #endregion

        static bool endProcess = false;
        static EthminerManager manager;

        static void Main(string[] args)
        {
            manager = new EthminerManager(args);
            manager.UnknownErrorScript = Properties.Settings.Default.Restart_Path;
            manager.UnknownErrorScriptPath = Properties.Settings.Default.Restart_Start_Path;

            Console.CancelKeyPress += Console_CancelKeyPress;

            manager.Start(true);

            while(!manager.minerProcess.WaitForExit(CHECK_INTERVAL))
            {
                manager.Process(ref endProcess);
            }

            if(!endProcess)
            {
                ProcessStartInfo newStartInfo = new ProcessStartInfo();
                newStartInfo.Arguments = String.Join(" ", args);
                newStartInfo.FileName = ETHMINERMAN_FILENAME;
                Process newProcess = new Process();
                newProcess.StartInfo = newStartInfo;
                Thread.Sleep(manager.RestartDelay);
                newProcess.Start();
            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            endProcess = true;
            manager.Stop();
        }
    }
}
