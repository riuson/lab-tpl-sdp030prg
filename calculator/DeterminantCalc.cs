﻿using System;
using Calculator;

namespace calculator
{
    public class DeterminantCalc
    {
        public int GetSign(int x, int y)
        {
            return ((x & 1) == 0 ? 1 : -1)
                   *
                   ((y & 1) == 0 ? 1 : -1);
        }

        public int Calc(SquareMatrix matrix)
        {
            throw new NotImplementedException();
        }
    }
}