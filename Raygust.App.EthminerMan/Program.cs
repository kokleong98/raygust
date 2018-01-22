using System;

namespace Raygust.App.EthminerMan
{
    class Program
    {
        #region constants
        const int CHECK_INTERVAL = 1000; //Measure in milliseconds.
        #endregion

        static void Main(string[] args)
        {
            EthminerManager manager = new EthminerManager(args);

            manager.Start(true);

            while(!manager.minerProcess.WaitForExit(CHECK_INTERVAL))
            {
                manager.Process();
            }
        }
    }
}
