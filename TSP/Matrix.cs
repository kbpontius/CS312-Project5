using System;

namespace TSP
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

        private void PrintMatrix(double[,] matrix)
        {
            Console.WriteLine("------------------------------------");
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    Console.Write((double.IsPositiveInfinity(matrix[i, j]) ? "INF" : matrix[i, j].ToString()) + "\t");
                }

                Console.Write("\n");
            }
        }
    }
}
