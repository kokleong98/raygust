using System;
using System.Collections.Generic;
using System.Text;

namespace Raygust.App.EthminerMan
{
    internal class MinerData
    {
        public double totalspeed = 0.0;
        public GpuInfo[] Gpus = { };
        public DateTime lastUpdate = DateTime.Now;
        public int accepted = 0;
        public int rejected = 0;
        public int failed = 0;
    }
}
