namespace Calculator {
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
}
