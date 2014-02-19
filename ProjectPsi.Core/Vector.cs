using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPsi.Core
{
    public struct Vector<TNumericType>
    {
        public TNumericType X { get; set; }
        public TNumericType Y { get; set; }

        public Vector(TNumericType x, TNumericType y) : this()
        {
            X = x;
            Y = y;
        }
    }
}
