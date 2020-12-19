namespace Calculator {
    public abstract class SquareMatrix {
        internal SquareMatrix(int size) => this.Size = size;

        public abstract int this[int x, int y] { get; set; }

        public int Size { get; }

        public abstract SquareMatrix Reduce(int removeX, int removeY);

        public override bool Equals(object? obj) {
            if (!(obj is SquareMatrix sqmb)) {
                return false;
            }

            return this == sqmb;
        }

        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(SquareMatrix matrix1, SquareMatrix matrix2) {
            if (matrix1 is null || matrix2 is null) {
                return false;
            }

            if (ReferenceEquals(matrix1, matrix2)) {
                return true;
            }

            if (matrix1.Size != matrix2.Size) {
                return false;
            }

            for (var x = 0; x < matrix1.Size; x++)
            for (var y = 0; y < matrix1.Size; y++) {
                if (matrix1[x, y] != matrix2[x, y]) {
                    return false;
                }
            }

            return true;
        }

        public static bool operator !=(SquareMatrix matrix1, SquareMatrix matrix2) => !(matrix1 == matrix2);

        public override string ToString() => $"{this.Size} x {this.Size}";
    }

    internal class SquareMatrixSource : SquareMatrix {
        private readonly int[,] _array;

        public SquareMatrixSource(int size) : base(size) {
            if (size < 2) {
                throw new SquareMatrixException();
            }

            this._array = new int[size, size];
        }

        public override int this[int x, int y] {
            get {
                this.ThrowOnInvalidOffset(x);
                this.ThrowOnInvalidOffset(y);
                return this._array[x, y];
            }
            set {
                this.ThrowOnInvalidOffset(x);
                this.ThrowOnInvalidOffset(y);
                this._array[x, y] = value;
            }
        }

        public override SquareMatrix Reduce(int removeX, int removeY) =>
            new SquareMatrixReducedProxy(this, removeX, removeY);

        private void ThrowOnInvalidOffset(int offset) {
            if (offset < 0 || offset >= this.Size) {
                throw new SquareMatrixException();
            }
        }
    }

    internal class SquareMatrixReducedProxy : SquareMatrix {
        private readonly int _removeX;
        private readonly int _removeY;
        private readonly SquareMatrix _source;

        public SquareMatrixReducedProxy(SquareMatrix source, int removeX, int removeY) : base(source.Size - 1) {
            this._source = source;
            this._removeX = removeX;
            this._removeY = removeY;
        }

        public override int this[int x, int y] {
            get {
                var sourceX = x < this._removeX ? x : x + 1;
                var sourceY = y < this._removeY ? y : y + 1;
                return this._source[sourceX, sourceY];
            }
            set {
                var sourceX = x < this._removeX ? x : x + 1;
                var sourceY = y < this._removeY ? y : y + 1;
                this._source[sourceX, sourceY] = value;
            }
        }

        public override SquareMatrix Reduce(int removeX, int removeY) =>
            new SquareMatrixReducedProxy(this, removeX, removeY);
    }

    public static class SquareMatrixFactory {
        public static SquareMatrix Create(int size) => new SquareMatrixSource(size);

        public static SquareMatrix Create(int[,] array) {
            if (array is null) {
                throw new SquareMatrixException();
            }

            if (array.Rank != 2) {
                throw new SquareMatrixException();
            }

            var size1 = array.GetLength(0);
            var size2 = array.GetLength(1);

            if (size1 != size2) {
                throw new SquareMatrixException();
            }

            if (size1 < 2) {
                throw new SquareMatrixException();
            }

            var result = new SquareMatrixSource(array.GetLength(0));

            for (var x = 0; x < size1; x++)
            for (var y = 0; y < size1; y++) {
                result[x, y] = array[y, x];
            }

            return result;
        }
    }
}
