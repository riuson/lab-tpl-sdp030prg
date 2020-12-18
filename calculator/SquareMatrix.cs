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

        public SquareMatrix Reduce(int removeX, int removeY)
        {
            if (Size < 3) throw new SquareMatrixException();

            ThrowOnInvalidOffset(removeX);
            ThrowOnInvalidOffset(removeY);

            var result = new SquareMatrix(Size - 1);

            for (int y1 = 0, y2 = 0; y1 < Size; y1++)
            {
                if (y1 == removeY) continue;

                for (int x1 = 0, x2 = 0; x1 < Size; x1++)
                {
                    if (x1 == removeX) continue;

                    result[x2, y2] = this[x1, y1];

                    x2++;
                }

                y2++;
            }

            return result;
        }

        private void ThrowOnInvalidOffset(int offset)
        {
            if (offset < 0 || offset >= Size) throw new SquareMatrixException();
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is SquareMatrix sqm)) return false;

            return this == sqm;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(SquareMatrix matrix1, SquareMatrix matrix2)
        {
            if (matrix1 is null || matrix2 is null) return false;

            if (ReferenceEquals(matrix1, matrix2)) return true;

            if (matrix1.Size != matrix2.Size) return false;

            for (var x = 0; x < matrix1.Size; x++)
            for (var y = 0; y < matrix1.Size; y++)
                if (matrix1[x, y] != matrix2[x, y])
                    return false;

            return true;
        }

        public static bool operator !=(SquareMatrix matrix1, SquareMatrix matrix2)
        {
            return !(matrix1 == matrix2);
        }
    }
}