namespace Calculator {
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
