namespace TSP
{
    class Matrix
    {
        // FIX: Remember to add LB of parent.
        private double[,] _matrix;
        private double _reductionCost = -1;
        private double _parentCost;

        double LowerBound => _reductionCost + _parentCost;

        public Matrix(City[] cities, double parentCost)
        {
            _parentCost = parentCost;
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
            _matrix = (double[,])matrix.GetMatrix().Clone();
            _reductionCost = matrix.GetReductionCost();
            _parentCost = matrix._parentCost;
        }

        public double[,] GetMatrix()
        {
            return _matrix;
        }

        public double GetReductionCost()
        {
            return _reductionCost;
        }

        public double GetParentCost()
        {
            return _parentCost;
        }

        public void SetReductionCost(double reductionCost)
        {
            _reductionCost = reductionCost;
        }
    }
}
