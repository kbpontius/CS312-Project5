using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSP
{
    class State
    {
        public  Matrix Matrix;
        private readonly double _lowerBound;
        private readonly int[] _cityFrom;
        private readonly int[] _cityTo;
        private readonly int _citiesInSolution;

        public State(Matrix matrix, double lowerBound, int[] cityFrom, int[] cityTo, int citiesInSolution)
        {
            Matrix = new Matrix(matrix);
            _lowerBound = lowerBound;
            _cityFrom = (int[])cityFrom.Clone();
            _cityTo = (int[])cityTo.Clone();
            _citiesInSolution = citiesInSolution;
        }

        public double GetLowerBound()
        {
            return _lowerBound;
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
    }
}
