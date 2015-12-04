﻿namespace TSP
{
    class Matrix
    {
        private double[,] _matrix;

        public Matrix(City[] cities)
        {
            _matrix = new double[cities.Length, cities.Length];

            for(int i = 0; i < cities.Length; i++)
            {
                for(int j = 0; j < cities.Length; j++)
                {
                    if (i == j)
                    {
                        _matrix[i, j] = double.PositiveInfinity;
                        continue;
                    }

                    _matrix[i, j] = cities[i].costToGetTo(cities[j]);
                }
            }
        }

        public Matrix(Matrix matrix)
        {
            _matrix = (double[,])matrix.GetMatrix().Clone();
        }

        public double[,] GetMatrix()
        {
            return _matrix;
        }
    }
}
