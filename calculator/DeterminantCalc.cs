using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Calculator;

namespace calculator
{
    public class DeterminantCalc
    {
        public int GetSign(int x, int y)
        {
            return ((x & 1) == 0 ? 1 : -1)
                   *
                   ((y & 1) == 0 ? 1 : -1);
        }

        public int Calc(SquareMatrix matrix)
        {
            if (matrix is null) throw new DeterminantCalcException();

            if (matrix.Size == 2) return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];

            var result = 0;

            for (var x = 0; x < matrix.Size; x++)
            {
                var item = matrix[x, 0];
                var sign = GetSign(x, 0);
                var subMatrix = matrix.Reduce(x, 0);

                result += item * sign * Calc(subMatrix);
            }

            return result;
        }

        public Task<int> CalcInTask(SquareMatrix matrix, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (matrix is null) throw new DeterminantCalcException();

            if (matrix.Size < 4) return Task.FromResult(Calc(matrix));

            var layersMap = Enumerable.Range(0, matrix.Size - 1)
                .Select(x => new CalcLayer(x + 2))
                .ToDictionary(x => x.Size);

            CalcItem createCalcItem(int item, int sign, SquareMatrix matrix)
            {
                var result = new CalcItem(matrix.Size, item, sign, matrix);
                layersMap[matrix.Size].Items.Add(result);

                if (matrix.Size > 2)
                    for (var i = 0; i < matrix.Size; i++)
                        result.SubItems.Add(createCalcItem(matrix[i, 0], GetSign(i, 0), matrix.Reduce(i, 0)));

                return result;
            }

            var rootItem = createCalcItem(1, 1, matrix);

            var layersAscending = layersMap.OrderBy(x => x.Key).Select(x => x.Value);

            foreach (var layer in layersAscending)
            foreach (var calcItem in layer.Items)
                if (layer.Size == 2)
                    calcItem.Result = Calc(calcItem.Matrix);
                else
                    calcItem.Result = calcItem.SubItems.Sum(x => x.Item * x.Sign * x.Result);

            return Task.FromResult(rootItem.Result);
        }

        public int[] Calc(SquareMatrix matrix1, params SquareMatrix[] matrices)
        {
            var tasks = new List<Task<int>>();

            Task<int> startCalc(SquareMatrix matrix)
            {
                return Task<int>.Factory.StartNew(
                    o => Calc((SquareMatrix) o),
                    matrix,
                    CancellationToken.None,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default);
            }

            tasks.AddRange(new[] {matrix1}.Concat(matrices).Select(startCalc));

            var resultTask = Task.Factory.ContinueWhenAll(
                tasks.ToArray(),
                ts => { return ts.Select(x => x.Result).ToArray(); });
            resultTask.Wait();
            return resultTask.Result;
        }

        public Task<int[]> CalcAsync(CancellationToken token, params SquareMatrix[] matrices)
        {
            Task<int> startCalc(SquareMatrix matrix)
            {
                return Task<int>.Factory.StartNew(
                    o => Calc((SquareMatrix) o),
                    matrix,
                    token);
            }

            var tasks = matrices.Select(startCalc).ToArray();

            var resultTask = Task.Factory.ContinueWhenAll(
                tasks.ToArray(),
                ts => { return ts.Select(x => x.Result).ToArray(); },
                token);
            return resultTask;
        }

        private class CalcItem
        {
            public CalcItem(int size, int item, int sign, SquareMatrix matrix)
            {
                Size = size;
                Item = item;
                Sign = sign;
                Matrix = matrix;
            }

            public int Size { get; }
            public int Item { get; }
            public int Sign { get; }
            public SquareMatrix Matrix { get; }

            public List<CalcItem> SubItems { get; } = new List<CalcItem>();

            public int Result { get; set; }
        }

        private class CalcLayer
        {
            public CalcLayer(int size)
            {
                Size = size;
            }

            public int Size { get; }
            public List<CalcItem> Items { get; } = new List<CalcItem>();

            public override string ToString()
            {
                return $"Size: {Size}, Items Count: {Items.Count}";
            }
        }
    }
}