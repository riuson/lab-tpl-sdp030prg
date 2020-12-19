using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Calculator.Laucher {
    internal class Program {
        private static readonly Random _random = new Random();

        private static void Main(string[] args) {
            var size = 10;
            var sw = new Stopwatch();

            var matrices = Enumerable.Range(0, 20)
                .Select(_ => CreateRandomMatrix(size))
                .ToArray();

            var calc = new DeterminantCalc();

            Console.WriteLine("Starting async calculation...");

            sw.Start();
            var task = calc.CalcManyAsync(CancellationToken.None, matrices);
            task.Wait();
            sw.Stop();
            var timeOfTask = sw.Elapsed;

            Console.WriteLine($"Time async: {timeOfTask}");

            Console.WriteLine("Starting plain calculation...");

            sw.Restart();
            calc.CalcMany(matrices);
            sw.Stop();
            var timePlain = sw.Elapsed;

            Console.WriteLine($"Time plain:   {timePlain}");
        }

        private static SquareMatrix CreateRandomMatrix(int size) {
            var matrix = SquareMatrixFactory.Create(size);

            for (var x = 0; x < size; x++)
            for (var y = 0; y < size; y++) {
                matrix[x, y] = _random.Next(0, 8);
            }

            return matrix;
        }
    }
}
