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
    }
}