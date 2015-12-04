using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TSP
{
    class BranchAndBound
    {
        private PriorityQueue<State> pq = new PriorityQueue<State>();
        private List<State> visited = new List<State>();
        private State _bestStateSoFar = null;
        private double _bssf;
        private City[] _cities;

        public BranchAndBound(City[] cities, double bssf)
        {
            _cities = cities;
            _bssf = bssf;
            int[] defaultArray = new int[_cities.Length];
            Populate(defaultArray, -1);
            State state = GenerateReducedMatrix(new Matrix(_cities), defaultArray, defaultArray);
            pq.Add(0, state);
        }

        public City[] CalculatePath()
        {
            while (pq.Count != 0)
            {
                State state = pq.RemoveMin();
                visited.Add(state); // TODO: Use this for the report metrics?

                LbDifferenceResult greatestDifferenceResult = CalculateGreatestLBDifference(state);
                State[] children = new State[2] {greatestDifferenceResult.IncludeState, greatestDifferenceResult.ExcludeState};

                // TODO: WHEN DO I ADD CHILDREN TO THE PQ?
                foreach(State child in children)
                {
                    if (child == null) { continue; }

                    if (IsSolution(child))
                    {
                        _bssf = child.GetLowerBound();
                        _bestStateSoFar = child;
                    }

                    if (child.GetLowerBound() < _bssf)
                    {
                        pq.Add((int)child.GetLowerBound(), child);
                    }
                }
            }

            return GeneratePathResult(_bestStateSoFar, _cities);
        }

        private City[] GeneratePathResult(State solution, City[] cities)
        {
            if (solution == null) { return null; }

            List<City> pathResult = new List<City>();

            int currentCity = solution.GetCityTo()[0];
            while (pathResult.Count < cities.Length)
            {
                pathResult.Add(cities[currentCity]);
                currentCity = solution.GetCityTo()[currentCity];
            }

            return pathResult.ToArray();
        }

        private bool IsSolution(State child)
        {
            if (child.GetCitiesInSolution() == _cities.Length)
            {
                return true;
            }

            if (child.GetCitiesInSolution() > _cities.Length)
            {
                throw new Exception("There should never be more cities in the solution than cities themselves.");
            }

            return false;
        }

        private LbDifferenceResult CalculateGreatestLBDifference(State state)
        {
            LbDifferenceResult currentGreatestLbDiff = new LbDifferenceResult(null, null, 0);

            for (int i = 0; i < state.Matrix.GetMatrix().GetLength(0); i++)
            {
                for (int j = 0; j < state.Matrix.GetMatrix().GetLength(0); j++)
                {
                    if (state.Matrix.GetMatrix()[i, j] == 0)
                    {
                        LbDifferenceResult lbDifference = GetLbDifference(i,j, state, _bssf);

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
        private LbDifferenceResult GetLbDifference(int row, int col, State state, double bssf)
        {
            Matrix includeMatrix = new Matrix(state.Matrix);
            Matrix excludeMatrix = new Matrix(state.Matrix);

            includeMatrix = SetupIncludeMatrix(includeMatrix, row, col);
            excludeMatrix = SetupExcludeMatrix(excludeMatrix, row, col);

            State includeState = GenerateReducedMatrix(includeMatrix, state.GetCityTo(), state.GetCityFrom());
            State excludeState = GenerateReducedMatrix(excludeMatrix, state.GetCityTo(), state.GetCityFrom());

            double lbDifference = Math.Abs(excludeState.GetLowerBound() - includeState.GetLowerBound());

            if (lbDifference < bssf)
            {
                includeState.SetCityFromTo(row, col);
                excludeState.SetCityFromTo(row, col);
                return new LbDifferenceResult(includeState, excludeState, lbDifference);
            }

            return null;
        }

        private State GenerateReducedMatrix(Matrix matrix, int[] citiesTo, int[] citiesFrom)
        {
            // This is the total cost of reduction.
            double sumDifference = 0;
            PrintMatrix(matrix);
            // Reduce row by row.
            for (int i = 0; i < matrix.GetMatrix().GetLength(0); i++)
            {
                double rowMin = double.PositiveInfinity;

                for (int j = 0; j < matrix.GetMatrix().GetLength(0); j++)
                {
                    if (matrix.GetMatrix()[i, j] < rowMin && !Double.IsInfinity(matrix.GetMatrix()[i, j]))
                    {
                        rowMin = matrix.GetMatrix()[i, j];
                    }
                }

                if (rowMin >  0 && !Double.IsInfinity(rowMin))
                {
                    for (int j = 0; j < matrix.GetMatrix().GetLength(0); j++)
                    {
                        matrix.GetMatrix()[i, j] -= rowMin;
                    }

                    sumDifference += rowMin;
                }
            }

            // Reduce column by column.
            for (int i = 0; i < matrix.GetMatrix().GetLength(0); i++)
            {
                double colMin = double.PositiveInfinity;

                for (int j = 0; j < matrix.GetMatrix().GetLength(0); j++)
                {
                    if (matrix.GetMatrix()[i, j] < colMin && !Double.IsInfinity(colMin))
                    {
                        colMin = matrix.GetMatrix()[i, j];
                    }
                }

                if (colMin > 0 && !Double.IsInfinity(colMin))
                {
                    for (int j = 0; j < matrix.GetMatrix().GetLength(0); j++)
                    {
                        matrix.GetMatrix()[i, j] -= colMin;
                    }

                    sumDifference += colMin;
                }
            }

            PrintMatrix(matrix);

            State state = new State(matrix, sumDifference, citiesTo, citiesFrom, -1);

            return state;
        }

        private Matrix SetupIncludeMatrix(Matrix matrix, int row, int col)
        {
            // Set row and col to infinity
            for (int i = 0; i < matrix.GetMatrix().GetLength(0); i++)
            {
                matrix.GetMatrix()[row, i] = double.PositiveInfinity;
                matrix.GetMatrix()[i, col] = double.PositiveInfinity;
            }

            // Prevent premature cycles, set inverse location to infinity
            matrix.GetMatrix()[col, row] = double.PositiveInfinity;

            return matrix;
        }

        private Matrix SetupExcludeMatrix(Matrix matrix, int row, int col)
        {
            // Exclude cell
            matrix.GetMatrix()[row, col] = double.PositiveInfinity;

            // Prevent premature cycles, set inverse location to infinity
            matrix.GetMatrix()[col, row] = double.PositiveInfinity;

            return matrix;
        }

        private static void Populate<T>(T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }
        }

        private void PrintMatrix(Matrix matrix)
        {
            for (int i = 0; i < matrix.GetMatrix().GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetMatrix().GetLength(0); j++)
                {
                    Console.Write(matrix.GetMatrix()[i, j] + "\t\t");
                }

                Console.Write("\n");
            }
        }
    }
}
