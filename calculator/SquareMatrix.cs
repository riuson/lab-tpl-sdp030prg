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
}
