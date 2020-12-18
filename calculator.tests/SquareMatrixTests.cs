using NUnit.Framework;

namespace Calculator.Tests
{
    internal class SquareMatrixTests
    {
        [Test]
        public void ShouldCreateMatrixWithSize()
        {
            // Arrange.
            var size = 4;

            // Act.
            var matrix = new SquareMatrix(size);

            // Assert.
            Assert.That(matrix.Size, Is.EqualTo(size));
        }

        [Test]
        public void ShouldCreateMatrixFromArray()
        {
            // Arrange.
            int[,] array =
            {
                {1, 2, 3},
                {4, 5, 6},
                {7, 8, 9}
            };
            var expectedSize = 3;

            // Act.
            var matrix = new SquareMatrix(array);

            // Assert.
            Assert.That(matrix.Size, Is.EqualTo(expectedSize));
        }

        [Test]
        public void ShouldFailCreateMatrixFromArrayNull()
        {
            // Arrange.
            int[,] array = null;

            // Act & Assert.
            Assert.Throws<SquareMatrixException>(() => new SquareMatrix(array));
        }

        [Test]
        public void ShouldFailCreateMatrixFromTooSmallSize()
        {
            // Arrange & Act & Assert.
            Assert.Throws<SquareMatrixException>(() => new SquareMatrix(1));
        }

        [Test]
        public void ShouldFailCreateMatrixFromTooSmallArray()
        {
            // Arrange.
            var array = new int[1, 1];

            // Act & Assert.
            Assert.Throws<SquareMatrixException>(() => new SquareMatrix(array));
        }

        [Test]
        public void ShouldFailCreateMatrixFromArrayNotSquare()
        {
            // Arrange.
            var array = new int[4, 6];

            // Act & Assert.
            Assert.Throws<SquareMatrixException>(() => new SquareMatrix(array));
        }

        [TestCase(-1, 0)]
        [TestCase(0, -1)]
        [TestCase(-1, -1)]
        [TestCase(100, 0)]
        [TestCase(0, 100)]
        [TestCase(100, 100)]
        public void ShouldFailOnInvalidOffsetGet(int x, int y)
        {
            // Arrange.
            var matrix = new SquareMatrix(3);

            // Act & Assert.
            Assert.Throws<SquareMatrixException>(() =>
            {
                var b = matrix[x, y];
            });
        }

        [TestCase(-1, 0)]
        [TestCase(0, -1)]
        [TestCase(-1, -1)]
        [TestCase(100, 0)]
        [TestCase(0, 100)]
        [TestCase(100, 100)]
        public void ShouldFailOnInvalidOffsetSet(int x, int y)
        {
            // Arrange.
            var matrix = new SquareMatrix(3);

            // Act & Assert.
            Assert.Throws<SquareMatrixException>(() => { matrix[x, y] = 0; });
        }

        [Test]
        public void ShouldGetItem()
        {
            // Arrange.
            int[,] array =
            {
                {1, 2, 3},
                {4, 5, 6},
                {7, 8, 9}
            };
            var matrix = new SquareMatrix(array);

            // Act & Assert.
            Assert.That(matrix[0, 0], Is.EqualTo(1));
            Assert.That(matrix[1, 0], Is.EqualTo(2));
            Assert.That(matrix[2, 0], Is.EqualTo(3));
            Assert.That(matrix[0, 1], Is.EqualTo(4));
            Assert.That(matrix[1, 1], Is.EqualTo(5));
            Assert.That(matrix[2, 1], Is.EqualTo(6));
            Assert.That(matrix[0, 2], Is.EqualTo(7));
            Assert.That(matrix[1, 2], Is.EqualTo(8));
            Assert.That(matrix[2, 2], Is.EqualTo(9));
        }

        [Test]
        public void ShouldSetItem()
        {
            // Arrange.
            int[,] array =
            {
                {1, 2, 3},
                {4, 5, 6},
                {7, 8, 9}
            };
            var matrix = new SquareMatrix(array);

            // Act.
            matrix[0, 2] = 100;

            // Assert.
            Assert.That(matrix[0, 2], Is.EqualTo(100));
        }

        [Test]
        public void ShouldCompareMatrixEquals()
        {
            // Arrange.
            int[,] array1 =
            {
                {1, 2, 3},
                {4, 5, 6},
                {7, 8, 9}
            };
            int[,] array2 =
            {
                {1, 2, 3},
                {4, 5, 6},
                {7, 8, 9}
            };
            var matrix1 = new SquareMatrix(array1);
            var matrix2 = new SquareMatrix(array2);
            var expected = true;

            // Act.
            var actual = matrix1 == matrix2;

            // Assert.
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldCompareMatrixNotEquals1()
        {
            // Arrange.
            int[,] array1 =
            {
                {1, 2, 3},
                {4, 5, 6},
                {7, 8, 9}
            };
            int[,] array2 =
            {
                {1, 2, 3},
                {4, 5, 6},
                {7, 8, 0}
            };
            var matrix1 = new SquareMatrix(array1);
            var matrix2 = new SquareMatrix(array2);
            var expected = false;

            // Act.
            var actual = matrix1 == matrix2;

            // Assert.
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldCompareMatrixNotEquals2()
        {
            // Arrange.
            int[,] array1 =
            {
                {1, 2},
                {4, 5}
            };
            int[,] array2 =
            {
                {1, 2, 3},
                {4, 5, 6},
                {7, 8, 9}
            };
            var matrix1 = new SquareMatrix(array1);
            var matrix2 = new SquareMatrix(array2);
            var expected = false;

            // Act.
            var actual = matrix1 == matrix2;

            // Assert.
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldCompareMatrixNotEquals3()
        {
            // Arrange.
            int[,] array2 =
            {
                {1, 2, 3},
                {4, 5, 6},
                {7, 8, 9}
            };
            SquareMatrix matrix1 = null;
            var matrix2 = new SquareMatrix(array2);
            var expected = false;

            // Act.
            var actual = matrix1 == matrix2;

            // Assert.
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldReduceMatrix()
        {
            // Arrange.
            int[,] array =
            {
                {1, 2, 3},
                {4, 5, 6},
                {7, 8, 9}
            };
            int[,] arrayReduced =
            {
                {1, 3},
                {7, 9}
            };
            var matrix = new SquareMatrix(array);
            var matrixExpected = new SquareMatrix(arrayReduced);

            // Act.
            var matrixActual = matrix.Reduce(1, 1);

            // Assert.
            Assert.That(matrixActual, Is.EqualTo(matrixExpected));
        }
    }
}