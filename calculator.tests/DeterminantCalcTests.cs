using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Calculator.Tests {
    internal class DeterminantCalcTests {
        private Random _random = new Random();

        [OneTimeSetUp]
        public void OneTimeSetUp() {
            this._random = new Random();
        }

        [TestCase(0, 0, ExpectedResult = 1)]
        [TestCase(1, 0, ExpectedResult = -1)]
        [TestCase(2, 0, ExpectedResult = 1)]
        [TestCase(3, 0, ExpectedResult = -1)]
        [TestCase(0, 1, ExpectedResult = -1)]
        [TestCase(1, 1, ExpectedResult = 1)]
        [TestCase(2, 1, ExpectedResult = -1)]
        [TestCase(3, 1, ExpectedResult = 1)]
        [TestCase(0, 2, ExpectedResult = 1)]
        [TestCase(1, 2, ExpectedResult = -1)]
        [TestCase(2, 2, ExpectedResult = 1)]
        [TestCase(3, 2, ExpectedResult = -1)]
        [TestCase(0, 3, ExpectedResult = -1)]
        [TestCase(1, 3, ExpectedResult = 1)]
        [TestCase(2, 3, ExpectedResult = -1)]
        [TestCase(3, 3, ExpectedResult = 1)]
        public int ShouldGetSign(int x, int y) {
            // Arrange.
            var calc = new DeterminantCalc();

            // Act & Assert.
            return calc.GetSign(x, y);
        }

        [Test]
        public void ShouldFailOnNull() {
            // Arrange.
            var calc = new DeterminantCalc();

            // Act & Assert.
            Assert.Throws<DeterminantCalcException>(() => calc.CalcOne(null));
        }

        [Test]
        public void ShouldTaskFailOnNull() {
            // Arrange.
            var calc = new DeterminantCalc();
            SquareMatrix matrix = null;

            // Act.
            var task = calc.CalcOneAsync(CancellationToken.None, matrix);

            // Assert.
            try {
                task.Wait();
            } catch (AggregateException ae) {
                Assert.That(ae.InnerExceptions.Any(x => x is DeterminantCalcException), Is.True);
            }
        }

        [Test]
        public void ShouldCalcDeterminantOfSize2() {
            // Arrange.
            int[,] array = {
                { 11, -3 },
                { -15, -2 }
            };
            var matrix = SquareMatrixFactory.Create(array);
            var calc = new DeterminantCalc();
            var expected = -67;

            // Act.
            var actual = calc.CalcOne(matrix);

            // Assert.
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldCalcDeterminantOfSize3() {
            // Arrange.
            int[,] array = {
                { 1, -2, 3 },
                { 4, 0, 6 },
                { -7, 8, 9 }
            };
            var matrix = SquareMatrixFactory.Create(array);
            var calc = new DeterminantCalc();
            var expected = 204;

            // Act.
            var actual = calc.CalcOne(matrix);

            // Assert.
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldCalcDeterminantOfSize3Async() {
            // Arrange.
            int[,] array = {
                { 1, -2, 3 },
                { 4, 0, 6 },
                { -7, 8, 9 }
            };
            var matrix = SquareMatrixFactory.Create(array);
            var calc = new DeterminantCalc();
            var expected = 204;

            // Act.
            var task = calc.CalcOneAsync(CancellationToken.None, matrix);
            task.Wait();
            var actual = task.Result;

            // Assert.
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(8)]
        public void ShouldCalcDeterminantAsync(int size) {
            // Arrange.
            var random = new Random();
            var matrix = SquareMatrixFactory.Create(size);

            for (var x = 0; x < size; x++)
            for (var y = 0; y < size; y++) {
                matrix[x, y] = random.Next(0, 10);
            }

            var calc = new DeterminantCalc();

            // Act.
            var task = calc.CalcOneAsync(CancellationToken.None, matrix);
            task.Wait();

            // Assert.
            Console.WriteLine(task.Result);
        }

        [TestCase(5)]
        [TestCase(8)]
        [TestCase(9)]
        public void CompareCalculationTime(int size) {
            // Arrange.
            var matrices = Enumerable.Range(0, 10)
                .Select(_ => this.CreateRandomMatrix(size))
                .ToArray();
            var calc = new DeterminantCalc();
            var sw = new Stopwatch();

            // Act.
            sw.Start();
            var task = calc.CalcManyAsync(CancellationToken.None, matrices);
            task.Wait();
            sw.Stop();
            var timeOfTask = sw.Elapsed;

            sw.Restart();
            calc.CalcMany(matrices);
            sw.Stop();
            var timePlain = sw.Elapsed;

            // Assert.
            Console.WriteLine($"Time plain: {timePlain}");
            Console.WriteLine($"Time async: {timeOfTask}");
        }

        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(9)]
        [TestCase(10)]
        [TestCase(11)]
        //[TestCase(12)]
        //[TestCase(13)]
        //[TestCase(14)]
        //[TestCase(15)]
        //[TestCase(16)]
        public void ShouldCalcDeterminantOfMoreSize(int size) {
            // Arrange.
            var random = new Random();
            var matrix = SquareMatrixFactory.Create(size);

            for (var x = 0; x < size; x++)
            for (var y = 0; y < size; y++) {
                matrix[x, y] = random.Next(0, 10);
            }

            var calc = new DeterminantCalc();

            // Act.
            var actual = calc.CalcOne(matrix);

            // Assert.
        }

        [Test]
        public void ShouldCalcMultiple() {
            // Arrange.
            int[,] array1 = {
                { 11, -3 },
                { -15, -2 }
            };
            int[,] array2 = {
                { 1, -2, 3 },
                { 4, 0, 6 },
                { -7, 8, 9 }
            };

            var matrix1 = SquareMatrixFactory.Create(array1);
            var matrix2 = SquareMatrixFactory.Create(array2);
            var calc = new DeterminantCalc();
            var expected = new[] {
                -67, 204
            };

            // Act.
            var actual = calc.CalcMany(matrix1, matrix2);

            // Assert.
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldCalcMultipleAsync() {
            // Arrange.
            int[,] array1 = {
                { 11, -3 },
                { -15, -2 }
            };
            int[,] array2 = {
                { 1, -2, 3 },
                { 4, 0, 6 },
                { -7, 8, 9 }
            };

            var matrix1 = SquareMatrixFactory.Create(array1);
            var matrix2 = SquareMatrixFactory.Create(array2);
            var calc = new DeterminantCalc();
            var expected = new[] {
                -67, 204
            };

            // Act.
            var actualTask = calc.CalcManyAsync(CancellationToken.None, matrix1, matrix2);
            actualTask.Wait();
            var actual = actualTask.Result;

            // Assert.
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldCalcMultipleAsyncAndCancel() {
            // Arrange.
            var matrices = Enumerable.Range(0, 1000)
                .Select(_ => this.CreateRandomMatrix(8))
                .ToArray();
            var calc = new DeterminantCalc();
            var tokenSource = new CancellationTokenSource();
            var sw = new Stopwatch();

            // Act.
            sw.Start();
            var actualTask = calc.CalcManyAsync(tokenSource.Token, matrices);
            Thread.Sleep(100);
            try {
                tokenSource.Cancel();
                actualTask.Wait();
            } catch (AggregateException ae) {
                Console.WriteLine(ae.Message);
            }

            sw.Stop();

            // Assert.
            Assert.That(sw.Elapsed, Is.LessThan(TimeSpan.FromSeconds(1)));
            Assert.That(actualTask.Status, Is.EqualTo(TaskStatus.Canceled));
        }

        private SquareMatrix CreateRandomMatrix(int size) {
            var matrix = SquareMatrixFactory.Create(size);

            for (var x = 0; x < size; x++)
            for (var y = 0; y < size; y++) {
                matrix[x, y] = this._random.Next(0, 10);
            }

            return matrix;
        }
    }
}
