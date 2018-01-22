using System;
using System.Globalization;

namespace Raygust.App.EthminerMan
{
    internal class EthminerParser
    {
        private string data;
        public MinerData minerInfo = new MinerData();
        public string source;
        public DateTime time;
        public string identifier;
        public string message;
        public double minSpeedThreshold = 10.0;
        public DateTime lastspeedupdate;
        public int accepted = 0;
        public int rejected = 0;
        public int failed = 0;
        public int duration = 300;

        public bool Parse(string input)
        {
            bool result = false;

            if (string.IsNullOrEmpty(input) || input.Length < 25 || !input.StartsWith("  m"))
            {
                return false;
            }

            data = input;
            source = data.Substring(0, 4).Trim();
            time = DateTime.ParseExact(data.Substring(4, 9).Trim(), "HH:mm:ss", new CultureInfo("en-US"));
            DateTime current = DateTime.Now;
            if (time.Hour > current.Hour)
            {
                //copy date portion
                time.AddDays(-1);
            }
            identifier = data.Substring(14, 10).Trim();
            message = data.Substring(24);
            ProcessSpeedMessage(message);

            return result;
        }

        private void ProcessSpeedMessage(string message)
        {
            if (!message.StartsWith("Speed", StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }
            double tmpValue = 0.0;
            double.TryParse(message.Substring(5, 7), out tmpValue);
            minerInfo.totalspeed = tmpValue;

            string section = message.Substring(21, message.IndexOf('[') - 21);
            string[] gpuspeeds = section.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int size = gpuspeeds.Length / 2;
            if (minerInfo.Gpus.Length < size)
            {
                Array.Resize<GpuInfo>(ref minerInfo.Gpus, size);
                minerInfo.Gpus[size - 1] = new GpuInfo();
                minerInfo.Gpus[size - 1].minThreshold = minSpeedThreshold;
                minerInfo.Gpus[size - 1].minThresholdMetTime = DateTime.Now;
            }
            for (int i = 0; i < gpuspeeds.Length; i += 2)
            {
                double.TryParse(gpuspeeds[i + 1], out minerInfo.Gpus[i / 2].speed);
                if (minerInfo.Gpus[i / 2].minThresholdMet())
                {
                    minerInfo.Gpus[size - 1].minThresholdMetTime = DateTime.Now;
                }
            }

            #region process overall share status
            int startpos = message.IndexOf('[');
            int endpos = message.IndexOf(']');
            section = message.Substring(startpos + 1, endpos - startpos - 1);
            string[] tokens = section.Split(':');
            int.TryParse(tokens[0].Substring(1, tokens[0].IndexOf('+') - 1), out minerInfo.accepted);
            int.TryParse(tokens[1].Substring(1, tokens[1].IndexOf('+') - 1), out minerInfo.rejected);
            int.TryParse(tokens[2].Substring(1), out minerInfo.failed);
            #endregion

            lastspeedupdate = DateTime.Now;
        }

        public bool Break()
        {
            bool bResult = false;
            for (int i = 0; i < minerInfo.Gpus.Length; i++)
            {
                if (minerInfo.Gpus[i].elapsedThresholdMetSeconds() >= duration)
                {
                    bResult = true;
                    break;
                }
            }
            return bResult;
        }

        public void PrintSpeedMessage()
        {
            Console.Write(string.Format("total speed: {0}", minerInfo.totalspeed));
            for (int i = 0; i < minerInfo.Gpus.Length; i++)
            {
                Console.Write(string.Format(", GPU{0}:{1}, {2}, {3}", i, 
                    minerInfo.Gpus[i].speed, 
                    minerInfo.Gpus[i].minThresholdMet(), 
                    minerInfo.Gpus[i].elapsedThresholdMetSeconds()));
            }
            Console.WriteLine(string.Format(", A:{0} R:{1} F:{2}", minerInfo.accepted,
                minerInfo.rejected,
                minerInfo.failed));
        }
    }
}
