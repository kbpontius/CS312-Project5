using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSP
{
    class Matrix
    {
        private double[,] matrix;
        private double reductionCost = -1;

        public Matrix(City[] cities)
        {
            matrix = new double[cities.Length, cities.Length];

            for(int i = 0; i < cities.Length; i++)
            {
                for(int j = 0; j < cities.Length; j++)
                {
                    matrix[i, j] = cities[j].costToGetTo(cities[i]);
                }
            }
        }

        public Matrix(Matrix matrix)
        {
            this.matrix = matrix.GetMatrix();
            this.reductionCost = matrix.GetReductionCost();
        }

        public double[,] GetMatrix()
        {
            return matrix;
        }

        public double GetReductionCost()
        {
            return reductionCost;
        }

        public void SetReductionCost(double newReductionCost)
        {
            reductionCost = newReductionCost;
        }
    }
}
