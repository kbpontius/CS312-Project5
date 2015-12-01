using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSP
{
    class Matrix
    {
        double[,] matrix;

        public Matrix(List<City> cities)
        {
            matrix = new double[cities.Count, cities.Count];

            for(int i = 0; i < cities.Count; i++)
            {
                for(int j = 0; j < cities.Count; j++)
                {
                    matrix[i, j] = cities[j].costToGetTo(cities[i]);
                }
            }
        }
    }
}
