using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadAStar.Model
{
    public struct ThreadData
    {
        public Int32 ThreadId;
        public Int16 Duration;
        public UInt64 CPUMin;
        public UInt64 CPUMax;
        public UInt64 CPUAverage;
        public Int16 CountRefresh;
    }
}
