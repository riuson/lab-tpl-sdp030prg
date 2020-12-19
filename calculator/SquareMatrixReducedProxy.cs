namespace Calculator {
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
}
