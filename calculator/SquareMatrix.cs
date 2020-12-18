namespace Calculator
{
    public class SquareMatrix
    {
        private readonly int[,] _array;

        public SquareMatrix(int size)
        {
            if (size < 2) throw new SquareMatrixException();
            _array = new int[size, size];
        }

        public SquareMatrix(int[,] array)
        {
            if (array is null) throw new SquareMatrixException();

            if (array.GetLength(0) != array.GetLength(1)) throw new SquareMatrixException();

            if (array.GetLength(0) < 2) throw new SquareMatrixException();

            _array = array;
        }

        public int this[int x, int y]
        {
            get
            {
                ThrowOnInvalidOffset(x);
                ThrowOnInvalidOffset(y);
                return _array[y, x];
            }
            set
            {
                ThrowOnInvalidOffset(x);
                ThrowOnInvalidOffset(y);
                _array[y, x] = value;
            }
        }

        public int Size => _array.GetLength(0);

        private void ThrowOnInvalidOffset(int offset)
        {
            if (offset < 0 || offset >= Size) throw new SquareMatrixException();
        }
    }
}