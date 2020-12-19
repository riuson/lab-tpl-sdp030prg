using System;
using System.Diagnostics;
using System.Threading;

namespace Calculator.Laucher {
    internal class Program {
        private static void Main(string[] args) {
            var size = 8;
            var random = new Random();
            var matrix = SquareMatrixFactory.Create(size);
            var sw = new Stopwatch();

            for (var x = 0; x < size; x++)
            for (var y = 0; y < size; y++) {
                matrix[x, y] = random.Next(0, 10);
            }

            var calc = new DeterminantCalc();

            sw.Start();
            var task = calc.CalcOneAsync(CancellationToken.None, matrix);
            task.Wait();
            sw.Stop();
            var timeOfTask = sw.Elapsed;

            sw.Restart();
            calc.CalcOne(matrix);
            sw.Stop();
            var timePlain = sw.Elapsed;

            Console.WriteLine($"Time plain:   {timePlain}");
            Console.WriteLine($"Time async: {timeOfTask}");
        }
    }
}
