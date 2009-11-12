using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flowar.Tools
{
    public class MinMax
    {
        private float _min;
        private float _max;

        public MinMax(float min, float max)
        {
            this._min = min;
            this._max = max;
        }

        public float EvalF(Random rnd)
        {
            return _min + (float)rnd.NextDouble() * (_max - _min);
        }

        public int EvalI(Random rnd)
        {
            return rnd.Next((int)_min, (int)_max);
        }
    }
}
