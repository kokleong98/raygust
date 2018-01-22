using System;
using System.Collections.Generic;
using System.Text;

namespace Raygust.App.EthminerMan
{
    internal class GpuInfo
    {
        public double speed;
        public double minThreshold = 10.0;
        public DateTime minThresholdMetTime = DateTime.Now;
        public bool minThresholdMet()
        {
            return (speed >= minThreshold);
        }
        public int elapsedThresholdMetSeconds()
        {
            return Convert.ToInt32(DateTime.Now.Subtract(minThresholdMetTime).Duration().TotalSeconds);
        }
    }
}
