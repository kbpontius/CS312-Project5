using System;
using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;

namespace TSP
{
    class BranchAndBound
    {
        List<Matrix> matrices = new List<Matrix>();
        private PQ pq = new PQ(0);
        private List<Matrix> visited = new List<Matrix>(); 

        public BranchAndBound(City[] cities)
        {
            matrices.Add(new Matrix(cities));
            ReduceMatrix(matrices[0]);

            while (!pq.IsEmpty())
            {
                HeapNode node = pq.PopMin();
                Matrix currentMatrix = node.matrix;

                // expansionEdge = greatestLowerBoundDifference()
                // children = reduceChildren(expansionEdge, matrix {state?})

                // for child in children {
                //      if isSolution(child) {
                //          BSSF = childLB
                //      }
                //      else if child.LB < BSSF {
                //          pq.Add(child)
                //      }
            }
        }

        private double CalculateGreatestLBDifference(Matrix matrix)
        {
            double currentGreatestLbDiff = double.NegativeInfinity;

            for (int i = 0; i < matrix.GetMatrix().Length; i++)
            {
                for (int j = 0; j < matrix.GetMatrix().Length; j++)
                {
                    if (matrix.GetMatrix()[i, j] == 0)
                    {
                        int lbDiff = getLbDiff(i,j, matrix);

                        if (lbDiff > currentGreatestLbDiff)
                        {
                            currentGreatestLbDiff = lbDiff;
                        }
                    }
                }
            }

            return currentGreatestLbDiff;
        }

        // TODO: FIX THIS METHOD TO MATCH THE EDGE.ROW && EDGE.COLUMN!!
        private Matrix[] ReduceChildren(int row, int col, Matrix matrix)
        {
            Matrix includeMatrix = new Matrix(matrix);

            for (int i = 0; i < includeMatrix.GetMatrix().Length; i++)
            {
                includeMatrix.GetMatrix()[row, i] = double.PositiveInfinity;
                includeMatrix.GetMatrix()[i, col] = double.PositiveInfinity;
            }

            Matrix excludeMatrix = new Matrix(matrix);

            excludeMatrix.GetMatrix()[row, col] = double.PositiveInfinity;

            Matrix[] includeExcludeArray = new Matrix[2];
            includeExcludeArray[0] = includeMatrix;
            includeExcludeArray[1] = excludeMatrix;

            return includeExcludeArray;
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
