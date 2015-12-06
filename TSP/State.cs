using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSP
{
    class State
    {
        public  Matrix Matrix;
        public  double LowerBound;
        private int[] _cityFrom;
        private int[] _cityTo;
        private int _citiesInSolution;

        public State(Matrix matrix, double lowerBound, int[] cityFrom, int[] cityTo, int citiesInSolution)
        {
            Matrix = new Matrix(matrix);
            LowerBound = lowerBound;
            _cityFrom = (int[])cityFrom.Clone();
            _cityTo = (int[])cityTo.Clone();
            _citiesInSolution = citiesInSolution;
        }

        public State(State state)
        {
            Matrix = new Matrix(state.Matrix);
            LowerBound = state.LowerBound;
            _cityFrom = (int[])state._cityFrom.Clone();
            _cityTo = (int[])state._cityTo.Clone();
            _citiesInSolution = state._citiesInSolution;
        }

        public double GetLowerBound()
        {
            return LowerBound;
        }

        public int[] GetCityFrom()
        {
            return _cityFrom;
        }

        public int[] GetCityTo()
        {
            return _cityTo;
        }

        public int GetCitiesInSolution()
        {
            return _citiesInSolution;
        }

        public void SetCityFromTo(int cityFrom, int cityTo)
        {
            _cityFrom[cityTo] = cityFrom;
            _cityTo[cityFrom] = cityTo;
        }

        public void SetCitiesInSolution(int citiesInSolution)
        {
            _citiesInSolution = citiesInSolution;
        }
    }
}
