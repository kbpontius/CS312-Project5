using System;
using System.Collections.Generic;
using System.Globalization;
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
            State state = GenerateReducedMatrix(new State(new Matrix(_cities), -1, defaultArray, defaultArray, 0));
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
            LbDifferenceResult currentGreatestLbDiff = new LbDifferenceResult(null, null, double.NegativeInfinity, -1, -1);

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
                            currentGreatestLbDiff.IncludeState = DeleteEdges(currentGreatestLbDiff.IncludeState, i, j);
                        }
                    }
                }
            }

            return currentGreatestLbDiff;
        }

        // TODO: FIX THIS METHOD TO MATCH THE EDGE.ROW && EDGE.COLUMN!!
        private LbDifferenceResult GetLbDifference(int row, int col, State state, double bssf)
        {
            State includeState = new State(state);
            State excludeState = new State(state);

            includeState.Matrix = SetupIncludeMatrix(includeState.Matrix, row, col);
            excludeState.Matrix = SetupExcludeMatrix(excludeState.Matrix, row, col);

            includeState = GenerateReducedMatrix(includeState);
            excludeState = GenerateReducedMatrix(excludeState);

            double lbDifference = excludeState.GetLowerBound() - includeState.GetLowerBound();

            includeState.SetCitiesInSolution(includeState.GetCitiesInSolution() + 1);

            return new LbDifferenceResult(includeState, excludeState, lbDifference, row, col);
        }

        private State GenerateReducedMatrix(State state)
        {
            Matrix matrix = state.Matrix;

            // This is the total cost of reduction.
            double sumDifference = 0;

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

            state.LowerBound += sumDifference;
            state.Matrix = matrix;

            return state;
        }

        private State DeleteEdges(State state, int row, int col)
        {
            state.GetCityTo()[row] = col;
            state.GetCityFrom()[col] = row;

            state.Matrix.GetMatrix()[col, row] = double.PositiveInfinity;

            if (state.GetCitiesInSolution() < _cities.Length)
            {
                int start = row;
                int end = col;

                while (state.GetCityTo()[end] != -1)
                {
                    end = state.GetCityTo()[end];
                }

                while (state.GetCityFrom()[start] != -1)
                {
                    start = state.GetCityFrom()[start];
                }

                while (start != col)
                {
                    state.Matrix.GetMatrix()[end, start] = double.PositiveInfinity;
                    state.Matrix.GetMatrix()[col, start] = double.PositiveInfinity;
                    start = state.GetCityTo()[start];
                }
            }

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
            Console.WriteLine("------------------------------------");
            for (int i = 0; i < matrix.GetMatrix().GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetMatrix().GetLength(0); j++)
                {
                    Console.Write((Double.IsPositiveInfinity(matrix.GetMatrix()[i, j]) ? "INF" : matrix.GetMatrix()[i, j].ToString()) + "\t");
                }

                Console.Write("\n");
            }
        }
    }
}
