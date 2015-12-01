namespace TSP
{
    class Matrix
    {
        private readonly double[,] _matrix;
        private double _reductionCost = -1;

        public Matrix(City[] cities)
        {
            _matrix = new double[cities.Length, cities.Length];

            for(int i = 0; i < cities.Length; i++)
            {
                for(int j = 0; j < cities.Length; j++)
                {
                    _matrix[i, j] = cities[j].costToGetTo(cities[i]);
                }
            }
        }

        public Matrix(Matrix matrix)
        {
            this._matrix = matrix.GetMatrix();
            this._reductionCost = matrix.GetReductionCost();
        }

        public double[,] GetMatrix()
        {
            return _matrix;
        }

        public double GetReductionCost()
        {
            return _reductionCost;
        }

        public void SetReductionCost(double newReductionCost)
        {
            _reductionCost = newReductionCost;
        }
    }
}
