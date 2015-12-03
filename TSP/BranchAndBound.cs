using System.Collections.Generic;

namespace TSP
{
    class BranchAndBound
    {
        List<Matrix> matrices = new List<Matrix>();
        private PQ pq = new PQ(0);
        private List<State> visited = new List<State>();
        private double _bssf;

        public BranchAndBound(City[] cities, double bssf)
        {
            _bssf = bssf;
            matrices.Add(new Matrix(cities, 0));
            CalculatePath();
        }

        private City[] CalculatePath()
        {
            ReduceMatrix(matrices[0]);

            while (!pq.IsEmpty())
            {
                HeapNode node = pq.PopMin();
                State state = node.State;
                visited.Add(state);
                Matrix currentMatrix = state.GetMatrix();

                LbDifferenceResult greatestDifferenceResult = CalculateGreatestLBDifference(currentMatrix);
                Matrix[] children = new Matrix[2]
                {greatestDifferenceResult.IncludeMatrix, greatestDifferenceResult.ExcludeMatrix};

                foreach(Matrix child in children)
                {
                    
                }
                // for child in children {
                //      if isSolution(child) {
                //          BSSF = childLB
                //      }
                //      else if child.LB < BSSF {
                //          pq.Add(child)
                //      }
            }
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

            for (int i = 0; i < includeMatrix.GetMatrix().Length; i++)
            {
                includeMatrix.GetMatrix()[row, i] = double.PositiveInfinity;
                includeMatrix.GetMatrix()[i, col] = double.PositiveInfinity;
            }

            Matrix excludeMatrix = new Matrix(matrix);

            excludeMatrix.GetMatrix()[row, col] = double.PositiveInfinity;

            includeMatrix = ReduceMatrix(includeMatrix);
            excludeMatrix = ReduceMatrix(excludeMatrix);

            double excludeCost = excludeMatrix.GetParentCost() - excludeMatrix.GetReductionCost();
            double includeCost = includeMatrix.GetParentCost() - includeMatrix.GetReductionCost();

            double lbDifference = excludeCost - includeCost;

            if (lbDifference < BSSF)
            {
                return new LbDifferenceResult(includeMatrix, excludeMatrix, lbDifference);
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
