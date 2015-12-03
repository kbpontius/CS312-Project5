using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSP
{
    class State
    {
        private readonly Matrix _matrix;
        private readonly double _lowerBound;
        private readonly int _cityFrom;
        private readonly int _cityTo;

        public State(Matrix matrix, double lowerBound, int cityFrom, int cityTo)
        {
            _matrix = matrix;
            _lowerBound = lowerBound;
            _cityFrom = cityFrom;
            _cityTo = cityTo;
        }

        public Matrix GetMatrix()
        {
            return _matrix;
        }

        public double GetLowerBound()
        {
            return _lowerBound;
        }

        public int GetCityFrom()
        {
            return _cityFrom;
        }

        public int GetCityTo()
        {
            return _cityTo;
        }
    }
}
