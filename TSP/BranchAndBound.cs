using System.Collections.Generic;

namespace TSP
{
    class BranchAndBound
    {
        private PriorityQueue<State> pq = new PriorityQueue<State>();
        private List<State> visited = new List<State>();
        private double _bssf;

        public BranchAndBound(City[] cities, double bssf)
        {
            _bssf = bssf;
            Matrix reducedMatrix = (new Matrix(cities, 0));
            pq.Add(0, new State(reducedMatrix, reducedMatrix.GetParentCost() + reducedMatrix.GetReductionCost(), -1, -1));
            CalculatePath();
        }

        private City[] CalculatePath()
        {
            while (pq.Count == 0)
            {
                State state = pq.RemoveMin();
                visited.Add(state);
                Matrix currentMatrix = state.GetMatrix();

                LbDifferenceResult greatestDifferenceResult = CalculateGreatestLBDifference(currentMatrix);
                State[] children = new State[2] {greatestDifferenceResult.IncludeState, greatestDifferenceResult.ExcludeState};

                // TODO: WHEN DO I ADD CHILDREN TO THE PQ?
                foreach(State child in children)
                {
                    if (IsSolution(child))
                    {
                        _bssf = child.GetLowerBound();
                    }

                    if (child.GetLowerBound() < _bssf)
                    {
                        pq.Add((int)child.GetLowerBound(), child);
                    }
                }
            }
        }

        private bool IsSolution(State child)
        {

            return false;
        }

        private LbDifferenceResult CalculateGreatestLBDifference(Matrix matrix)
        {
            LbDifferenceResult currentGreatestLbDiff = new LbDifferenceResult(null, null, 0);

            for (int i = 0; i < matrix.GetMatrix().Length; i++)
            {
                for (int j = 0; j < matrix.GetMatrix().Length; j++)
                {
                    if (matrix.GetMatrix()[i, j] == 0)
                    {
                        LbDifferenceResult lbDifference = GetLbDifference(i,j, matrix, _bssf);

                        if (lbDifference == null) { continue; }

                        if (lbDifference.LowerBoundDifference > currentGreatestLbDiff.LowerBoundDifference)
                        {
                            currentGreatestLbDiff = lbDifference;
                        }
                    }
                }
            }

            return currentGreatestLbDiff;
        }

        // TODO: FIX THIS METHOD TO MATCH THE EDGE.ROW && EDGE.COLUMN!!
        private LbDifferenceResult GetLbDifference(int row, int col, Matrix matrix, double BSSF)
        {
            Matrix includeMatrix = new Matrix(matrix);
            Matrix excludeMatrix = new Matrix(matrix);

            for (int i = 0; i < includeMatrix.GetMatrix().Length; i++)
            {
                includeMatrix.GetMatrix()[row, i] = double.PositiveInfinity;
                includeMatrix.GetMatrix()[i, col] = double.PositiveInfinity;
            }

            excludeMatrix.GetMatrix()[row, col] = double.PositiveInfinity;

            includeMatrix.GetMatrix()[col, row] = double.PositiveInfinity;
            excludeMatrix.GetMatrix()[col, row] = double.PositiveInfinity;

            includeMatrix = ReduceMatrix(includeMatrix);
            excludeMatrix = ReduceMatrix(excludeMatrix);

            double excludeCost = excludeMatrix.GetParentCost() - excludeMatrix.GetReductionCost();
            double includeCost = includeMatrix.GetParentCost() - includeMatrix.GetReductionCost();

            double lbDifference = excludeCost - includeCost;

            if (lbDifference < BSSF)
            {
                return new LbDifferenceResult(new State(includeMatrix, includeCost, row, col), new State(excludeMatrix, excludeCost, row, col), lbDifference);
            }

            return null;
        }

        private Matrix ReduceMatrix(Matrix matrix)
        {
            // This is the total cost of reduction.
            double sumDifference = 0;

            // Reduce row by row.
            for (int i = 0; i < matrix.GetMatrix().Length; i++)
            {
                double rowMin = double.MaxValue;

                for (int j = 0; j < matrix.GetMatrix().Length; j++)
                {
                    if (matrix.GetMatrix()[i, j] < rowMin)
                    {
                        rowMin = matrix.GetMatrix()[i, j];
                    }
                }

                if (rowMin >  0)
                {

                    for (int j = 0; j < matrix.GetMatrix().Length; j++)
                    {
                        matrix.GetMatrix()[i, j] -= rowMin;
                        sumDifference += rowMin;
                    }
                }
            }

            // Reduce column by column.
            for (int i = 0; i < matrix.GetMatrix().Length; i++)
            {
                double colMin = double.MaxValue;

                for (int j = 0; j < matrix.GetMatrix().Length; j++)
                {
                    if (matrix.GetMatrix()[i, j] < colMin)
                    {
                        colMin = matrix.GetMatrix()[i, j];
                    }
                }

                if (colMin > 0)
                {
                    for (int j = 0; j < matrix.GetMatrix().Length; j++)
                    {
                        matrix.GetMatrix()[i, j] -= colMin;
                        sumDifference += colMin;
                    }
                }
            }

            matrix.SetReductionCost(sumDifference);

            return matrix;
        }
    }
}
