using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibPixz
{
    class ArraySlice<T>
    {
        private readonly T[,] arr;
        private int firstDimension;

        public ArraySlice(T[,] arr)
        {
            this.arr = arr;
        }

        public int FirstDimension
        {
            get { return firstDimension; }
            set { this.firstDimension = value; }
        }

        public T this[int index]
        {
            get { return arr[firstDimension, index]; }
            set { arr[firstDimension, index] = value; }
        }
    }
}
