using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Calculator {
    public class DeterminantCalc {
        public int GetSign(int x, int y) =>
            ((x & 1) == 0 ? 1 : -1)
            *
            ((y & 1) == 0 ? 1 : -1);

        public long CalcOne(SquareMatrix matrix) => this.CalcPrivate(CancellationToken.None, matrix);

        public Task<long> CalcOneAsync(CancellationToken token, SquareMatrix matrix) =>
            Task<long>.Factory.StartNew(
                o => this.CalcPrivate(token, (SquareMatrix) o),
                matrix,
                token);


        public long[] CalcMany(params SquareMatrix[] matrices) =>
            matrices
                .Select(x => this.CalcPrivate(CancellationToken.None, x))
                .ToArray();

        public Task<long[]> CalcManyAsync(CancellationToken token, params SquareMatrix[] matrices) {
            var tasks = new List<Task<long>>();

            Task<long> startCalcTask(SquareMatrix matrix) {
                return Task<long>.Factory.StartNew(
                    o => {
                        //Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                        return this.CalcPrivate(token, (SquareMatrix) o);
                    },
                    matrix,
                    token);
            }

            Task<long> startCalcThread(SquareMatrix matrix) {
                return Task<long>.Factory.StartNew(
                    o => {
                        //Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                        return this.CalcPrivate(token, (SquareMatrix) o);
                    },
                    matrix,
                    token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default);
            }

            tasks.AddRange(matrices.Select(startCalcTask));

            var resultTask = Task.Factory.ContinueWhenAll(
                tasks.ToArray(),
                ts => { return ts.Select(x => x.Result).ToArray(); },
                token);
            return resultTask;
        }

        private long CalcPrivate(CancellationToken token, SquareMatrix matrix) {
            token.ThrowIfCancellationRequested();

            if (matrix is null) {
                throw new DeterminantCalcException();
            }

            var layersMap = Enumerable.Range(0, matrix.Size - 1)
                .Select(x => new CalcLayer(x + 2))
                .ToDictionary(x => x.Size);

            CalcItem createCalcItem(long item, int sign, SquareMatrix matrix) {
                token.ThrowIfCancellationRequested();
                var result = new CalcItem(matrix.Size, item, sign, matrix);
                layersMap[matrix.Size].Items.Add(result);

                if (matrix.Size > 2) {
                    for (var i = 0; i < matrix.Size; i++) {
                        token.ThrowIfCancellationRequested();
                        result.SubItems.Add(createCalcItem(matrix[i, 0], this.GetSign(i, 0), matrix.Reduce(i, 0)));
                    }
                }

                return result;
            }

            token.ThrowIfCancellationRequested();
            var rootItem = createCalcItem(1, 1, matrix);

            token.ThrowIfCancellationRequested();
            var layersAscending = layersMap.OrderBy(x => x.Key).Select(x => x.Value);

            foreach (var layer in layersAscending) {
                token.ThrowIfCancellationRequested();

                foreach (var calcItem in layer.Items) {
                    token.ThrowIfCancellationRequested();

                    if (layer.Size == 2) {
                        calcItem.Result = calcItem.Matrix[0, 0] * calcItem.Matrix[1, 1] -
                                          calcItem.Matrix[0, 1] * calcItem.Matrix[1, 0];
                    } else {
                        calcItem.Result = calcItem.SubItems.Sum(x => x.Item * x.Sign * x.Result);
                    }
                }
            }

            return rootItem.Result;
        }

        private class CalcItem {
            public CalcItem(int size, long item, int sign, SquareMatrix matrix) {
                this.Size = size;
                this.Item = item;
                this.Sign = sign;
                this.Matrix = matrix;
            }

            public int Size { get; }
            public long Item { get; }
            public int Sign { get; }
            public SquareMatrix Matrix { get; }

            public List<CalcItem> SubItems { get; } = new List<CalcItem>();

            public long Result { get; set; }
        }

        private class CalcLayer {
            public CalcLayer(int size) => this.Size = size;

            public int Size { get; }
            public List<CalcItem> Items { get; } = new List<CalcItem>();

            public override string ToString() => $"Size: {this.Size}, Items Count: {this.Items.Count}";
        }
    }
}
