using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;

namespace TSP
{
    class BranchAndBound
    {
        private PriorityQueue<State> pq = new PriorityQueue<State>();
        private int bssfUpdates = 0;
        private int _createdStates = 0;
        private int _maxStatesStored = 0;
        private int _prunedSates = 0;
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

        public PathCalculation CalculatePath()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (pq.Count != 0)
            {
                if (stopwatch.Elapsed.Seconds >= 30)
                {
                    break;
                }

                State state = pq.RemoveMin();

                if (state.GetLowerBound() < _bssf)
                {
                    LbDifferenceResult greatestDifferenceResult = CalculateGreatestLBDifference(state);
                    State[] children = new State[2]
                    {greatestDifferenceResult.IncludeState, greatestDifferenceResult.ExcludeState};

                    foreach (State child in children)
                    {
                        if (IsSolution(child))
                        {
                            bssfUpdates++;
                            _bssf = child.GetLowerBound();
                            _bestStateSoFar = child;
                        }
                        else if (child.GetLowerBound() < _bssf)
                        {
                            // Dividing by the number cities in the solution is a great way to prioritize those solutions
                            // that are further along. This helps the algorithm "dig deeper" into the search.
                            pq.Add((int)child.GetLowerBound() / (child.GetCitiesInSolution() == 0 ? 1 : child.GetCitiesInSolution()), child);

                            if (pq.Count > _maxStatesStored)
                            {
                                _maxStatesStored = pq.Count;
                            }
                        }
                        else
                        {
                            _prunedSates++;
                        }
                    }
                }
                else
                {
                    _prunedSates++;
                }
            }

            stopwatch.Stop();

            Console.WriteLine("----------------------------------");
            Console.WriteLine("Elapsed Time: " + stopwatch.ElapsedMilliseconds);
            Console.WriteLine("Problem Size: " + _cities.Length);
            Console.WriteLine("Max States: " + _maxStatesStored);
            Console.WriteLine("Total Created: " + _createdStates);
            Console.WriteLine("States Pruned: " + _prunedSates);
            Console.WriteLine("----------------------------------");

            return GeneratePathResult(_bestStateSoFar, _cities, stopwatch.ElapsedMilliseconds);
        }

        // Simply used for generating the results of the solution.
        private PathCalculation GeneratePathResult(State solution, City[] cities, double elapsedTime)
        {
            if (solution == null)
            {
                return new PathCalculation(null, elapsedTime, bssfUpdates);
            }

            List<City> pathResult = new List<City>();

            int currentCity = solution.GetCityTo()[0];
            while (pathResult.Count < cities.Length)
            {
                pathResult.Add(cities[currentCity]);
                currentCity = solution.GetCityTo()[currentCity];
            }

            return new PathCalculation(pathResult.ToArray(), elapsedTime, bssfUpdates);
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
            LbDifferenceResult currentGreatestLbDiff = null;

            for (int i = 0; i < state.Matrix.GetMatrix().GetLength(0); i++)
            {
                for (int j = 0; j < state.Matrix.GetMatrix().GetLength(0); j++)
                {
                    if (state.Matrix.GetMatrix()[i, j] == 0)
                    {
                        LbDifferenceResult lbDifference = GetLbDifference(i, j, state);

                        if (currentGreatestLbDiff == null ||
                            lbDifference.LowerBoundDifference > currentGreatestLbDiff.LowerBoundDifference)
                        {
                            if (currentGreatestLbDiff != null)
                            {
                                _prunedSates += 2;
                            }

                            currentGreatestLbDiff = lbDifference;
                        }
                        else
                        {
                            _prunedSates += 2;
                        }
                    }
                }
            }

            return currentGreatestLbDiff;
        }

        // TODO: FIX THIS METHOD TO MATCH THE EDGE.ROW && EDGE.COLUMN!!
        private LbDifferenceResult GetLbDifference(int row, int col, State state)
        {
            State includeState = new State(state);
            State excludeState = new State(state);
            _createdStates += 2;

            includeState.Matrix = SetupIncludeMatrix(includeState.Matrix, row, col);
            excludeState.Matrix = SetupExcludeMatrix(excludeState.Matrix, row, col);

            includeState = DeleteEdges(includeState, row, col);

            includeState = GenerateReducedMatrix(includeState);
            excludeState = GenerateReducedMatrix(excludeState);

            double lbDifference = excludeState.GetLowerBound() - includeState.GetLowerBound();

            return new LbDifferenceResult(includeState, excludeState, lbDifference, row, col);
        }

        private State GenerateReducedMatrix(State state)
        {
            Matrix matrix = state.Matrix;

            // This is the total cost of reduction.
            double sumDifference = 0;

            // Reduce row by row
            for (int i = 0; i < matrix.GetMatrix().GetLength(0); i++)
            {
                // Only reduce the row if a cell in this row has NOT already been select as part of the path.
                if (state.GetCityTo()[i] == -1)
                {
                    double rowMin = double.PositiveInfinity;

                    for (int j = 0; j < matrix.GetMatrix().GetLength(0); j++)
                    {
                        if (matrix.GetMatrix()[i, j] < rowMin)
                        {
                            rowMin = matrix.GetMatrix()[i, j];
                        }
                    }

                    if (!double.IsInfinity(rowMin))
                    {
                        for (int j = 0; j < matrix.GetMatrix().GetLength(0); j++)
                        {
                            matrix.GetMatrix()[i, j] -= rowMin;
                        }
                    }

                    sumDifference += rowMin;
                }
            }

            // Reduce column by column
            for (int i = 0; i < matrix.GetMatrix().GetLength(0); i++)
            {
                // Only reduce the col if a cell in this col has NOT already been select as part of the path.
                if (state.GetCityFrom()[i] == -1)
                {
                    double colMin = double.PositiveInfinity;

                    for (int j = 0; j < matrix.GetMatrix().GetLength(0); j++)
                    {
                        if (matrix.GetMatrix()[j, i] < colMin)
                        {
                            colMin = matrix.GetMatrix()[j, i];
                        }
                    }

                    if (!double.IsInfinity(colMin))
                    {
                        for (int j = 0; j < matrix.GetMatrix().GetLength(0); j++)
                        {
                            matrix.GetMatrix()[j, i] -= colMin;
                        }
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
            state.SetCitiesInSolution(state.GetCitiesInSolution() + 1);

            if (state.GetCitiesInSolution() < _cities.Length - 1)
            {
                int start = row;
                int end = col;

                while (state.GetCityFrom()[start] != -1)
                {
                    start = state.GetCityFrom()[start];
                }

                while (state.GetCityTo()[end] != -1)
                {
                    end = state.GetCityTo()[end];
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

            matrix.GetMatrix()[col, row] = double.PositiveInfinity;

            return matrix;
        }

        private Matrix SetupExcludeMatrix(Matrix matrix, int row, int col)
        {
            // Exclude cell
            matrix.GetMatrix()[row, col] = double.PositiveInfinity;

            return matrix;
        }

        private static void Populate<T>(T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }
        }

        private void PrintMatrix(State state)
        {
            Console.WriteLine("------------------------------------");

            Matrix matrix = state.Matrix;

            for (int i = 0; i < state.GetCityTo().Length; i++)
            {
                Console.WriteLine(i + " --> " + state.GetCityTo()[i]);
            }

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
