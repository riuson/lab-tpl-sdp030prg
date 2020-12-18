using Calculator;
using NUnit.Framework;

namespace calculator.tests
{
    internal class DeterminantCalcTests
    {
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
        public int ShouldGetSign(int x, int y)
        {
            // Arrange.
            var calc = new DeterminantCalc();

            // Act & Assert.
            return calc.GetSign(x, y);
        }

        [Test]
        public void ShouldFailOnNull()
        {
            // Arrange.
            var calc = new DeterminantCalc();

            // Act & Assert.
            Assert.Throws<DeterminantCalcException>(() => calc.Calc(null));
        }

        [Test]
        public void ShouldCalcDeterminantOfSize2()
        {
            // Arrange.
            int[,] array =
            {
                {11, -3},
                {-15, -2}
            };
            var matrix = new SquareMatrix(array);
            var calc = new DeterminantCalc();
            var expected = -67;

            // Act.
            var actual = calc.Calc(matrix);

            // Assert.
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldCalcDeterminantOfSize3()
        {
            // Arrange.
            int[,] array =
            {
                {1, -2, 3},
                {4, 0, 6},
                {-7, 8, 9}
            };
            var matrix = new SquareMatrix(array);
            var calc = new DeterminantCalc();
            var expected = 204;

            // Act.
            var actual = calc.Calc(matrix);

            // Assert.
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}