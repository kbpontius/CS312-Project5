namespace TSP
{
    class Matrix
    {
        // FIX: Remember to add LB of parent.
        private readonly double[,] _matrix;
        private readonly double _reductionCost = -1;
        private readonly double _parentCost;

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
            this._matrix = matrix.GetMatrix();
            this._reductionCost = matrix.GetReductionCost();
            this._parentCost = matrix._parentCost;
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
    }
}
