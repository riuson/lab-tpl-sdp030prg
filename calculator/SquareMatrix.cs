using System;

namespace Calculator
{
    public class SquareMatrix
    {
        public SquareMatrix(int size)
        {
        }

        public SquareMatrix(int[,] array)
        {
            throw new NotImplementedException();
        }

        public int this[int x, int y]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public int Size => throw new NotImplementedException();
    }
}