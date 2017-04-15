using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScoutingFileAnalyzer
{
    class Stats
    {
        public string TeamNumber { get; set; }

        public double NumberOfGearsInAutoAverage { get; set; }

        public double NumberOfGearsInTeleOpAverage { get; set; }

        public double RobotTurnedOnLightAverage { get; set; }

        public double Score
        {
            get
            {
                return (1.2 * (NumberOfGearsInTeleOpAverage / 4)) + RobotTurnedOnLightAverage + NumberOfGearsInAutoAverage;
            }
        }

        public string[] ToArray()
        {
            return new string[] { TeamNumber, NumberOfGearsInAutoAverage.ToString(), NumberOfGearsInTeleOpAverage.ToString(), RobotTurnedOnLightAverage.ToString(), Score.ToString() };
        }
    }
}
