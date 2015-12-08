using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSP
{
    class PathCalculation
    {
        public City[] Cities;
        public double ElapsedTime;
        public int NumberOfBSSFUpdates;

        public PathCalculation(City[] cities, double elapsedTime, int bssfUpdates)
        {
            Cities = cities;
            ElapsedTime = elapsedTime;
            NumberOfBSSFUpdates = bssfUpdates;
        }
    }
}
