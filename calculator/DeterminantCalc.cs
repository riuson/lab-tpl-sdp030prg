using Fractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Calculator {
    public class DeterminantCalc {
        public int GetSign(int x, int y) =>
            ((x & 1) == 0 ? 1 : -1)
            *
            ((y & 1) == 0 ? 1 : -1);

        public BigInteger CalcOne(SquareMatrix matrix) => this.CalcPrivate(CancellationToken.None, matrix);

        public Task<BigInteger> CalcOneAsync(CancellationToken token, SquareMatrix matrix) =>
            Task<BigInteger>.Factory.StartNew(
                o => this.CalcPrivate(token, (SquareMatrix) o),
                matrix,
                token);


        public BigInteger[] CalcMany(params SquareMatrix[] matrices) =>
            matrices
                .Select(x => this.CalcPrivate(CancellationToken.None, x))
                .ToArray();

        public Task<BigInteger[]> CalcManyAsync(CancellationToken token, params SquareMatrix[] matrices) {
            var tasks = new List<Task<BigInteger>>();

            Task<BigInteger> startCalcTask(SquareMatrix matrix) {
                return Task<BigInteger>.Factory.StartNew(
                    o => { return this.CalcPrivate(token, (SquareMatrix) o); },
                    matrix,
                    token);
            }

            tasks.AddRange(matrices.Select(startCalcTask));

            var resultTask = Task.Factory.ContinueWhenAll(
                tasks.ToArray(),
                ts => { return ts.Select(x => x.Result).ToArray(); },
                token);


            //var resultTask = new Task<BigInteger[]>(() => {
            //    return matrices.AsParallel()
            //        .Select(x => this.CalcPrivate(token, x))
            //        .ToArray();
            //}, token);
            //resultTask.Start();

            return resultTask;
        }

        private BigInteger CalcPrivate(CancellationToken token, SquareMatrix matrix) {
            token.ThrowIfCancellationRequested();

            if (matrix is null) {
                throw new DeterminantCalcException();
            }

            if (matrix.Size == 2) {
                return this.CalcMatrixOfSize2(matrix);
            }

            var triangleArray = this.MatrixToFractionalArray(matrix);

            if (!this.TryGetRowNonZero(triangleArray, 0, 0, out var rowNonZero)) {
                return 0;
            }

            var invert = false;

            token.ThrowIfCancellationRequested();

            try {
                this.MakeTriangle(token, triangleArray, ref invert);
            } catch (DivideByZeroException) {
                throw new DivideByZeroException();
            }

            token.ThrowIfCancellationRequested();

            var result = this.GetDeterminant(triangleArray);
            token.ThrowIfCancellationRequested();

            return result.ToBigInteger() * (invert ? -1 : 1);
        }

        private Fraction[,] MatrixToFractionalArray(SquareMatrix matrix) {
            var result = new Fraction[matrix.Size, matrix.Size];

            for (var x = 0; x < matrix.Size; x++) {
                for (var y = 0; y < matrix.Size; y++) {
                    result[x, y] = matrix[x, y];
                }
            }

            return result;
        }

        private void MakeTriangle(CancellationToken token, Fraction[,] triangleArray, ref bool invert) {
            this.TryGetRowNonZero(triangleArray, 0, 0, out var rowNonZero);

            if (rowNonZero != 0) {
                this.SwapRows(triangleArray, 0, rowNonZero);
                invert = !invert;
            }

            for (var xy = 0; xy < triangleArray.GetLength(0); xy++) {
                token.ThrowIfCancellationRequested();
                this.MakeZeroes(triangleArray, xy, ref invert);
            }
        }

        private void MakeZeroes(Fraction[,] triangleArray, int xy, ref bool invert) {
            for (var y = xy + 1; y < triangleArray.GetLength(0); y++) {
                if (triangleArray[xy, xy] == 0) {
                    if (!this.TryGetRowNonZero(triangleArray, xy, xy, out var validRow)) {
                        throw new DivideByZeroException();
                    }

                    this.SwapRows(triangleArray, xy, validRow);
                    invert = !invert;
                }

                var coefficient = triangleArray[xy, y] / triangleArray[xy, xy];

                for (var x = xy; x < triangleArray.GetLength(0); x++) {
                    triangleArray[x, y] = triangleArray[x, y] - triangleArray[x, xy] * coefficient;
                }
            }
        }

        private void SwapRows(Fraction[,] triangleArray, int from, int to) {
            for (var i = 0; i < triangleArray.GetLength(0); i++) {
                var item = triangleArray[i, from];
                triangleArray[i, from] = triangleArray[i, to];
                triangleArray[i, to] = item;
            }
        }

        private bool TryGetRowNonZero(Fraction[,] triangleArray, int x, int y, out int resultY) {
            for (resultY = y; resultY < triangleArray.GetLength(0); resultY++) {
                if (triangleArray[x, resultY] != 0) {
                    return true;
                }
            }

            return false;
        }

        private Fraction GetDeterminant(Fraction[,] triangleArray) {
            Fraction result = 1;

            for (var i = 0; i < triangleArray.GetLength(0); i++) {
                result *= triangleArray[i, i];
            }

            return result;
        }

        private BigInteger CalcMatrixOfSize2(SquareMatrix matrix) =>
            matrix[0, 0] * matrix[1, 1] -
            matrix[0, 1] * matrix[1, 0];
    }
}
