namespace Raygust.App.EthminerMan
{
    partial class EthminerManSvc
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tmProcess = new System.Windows.Forms.Timer(this.components);
            // 
            // tmProcess
            // 
            this.tmProcess.Interval = 1000;
            this.tmProcess.Tick += new System.EventHandler(this.tmProcess_Tick);
            // 
            // EthminerManSvc
            // 
            this.CanHandlePowerEvent = true;
            this.CanShutdown = true;
            this.ServiceName = "EthminerManSvc";

        }

        #endregion

        private System.Windows.Forms.Timer tmProcess;
    }
}
