using System;
using System.ServiceProcess;

namespace Raygust.App.EthminerMan
{
    partial class EthminerManSvc : ServiceBase
    {
        EthminerManager manager = null;
        bool endProcess = false;
        public EthminerManSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            manager = new EthminerManager(args);
            manager.Start(false);
            tmProcess.Start();
        }

        protected override void OnStop()
        {
            tmProcess.Stop();
            manager.Stop();
            base.OnStop();
        }

        private void tmProcess_Tick(object sender, EventArgs e)
        {
            manager.Process(ref endProcess);
        }
    }
}
