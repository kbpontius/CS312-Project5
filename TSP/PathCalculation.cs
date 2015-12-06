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

        public PathCalculation(City[] cities, double elapsedTime)
        {
            Cities = cities;
            ElapsedTime = elapsedTime;
        }
    }
}
