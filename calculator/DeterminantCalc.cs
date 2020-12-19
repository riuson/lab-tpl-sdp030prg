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
    }
}