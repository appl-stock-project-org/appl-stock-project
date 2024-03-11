using AppleStockAPI.Utilities;

namespace AppleStockAPI.Unit.Tests
{
    public class MathUtilsTests
    {

        [TestCase(100, 100)]
        [TestCase(100.9, 100.9)]
        [TestCase(100.99, 100.99)]
        [TestCase(100.999, 100.99)]
        [TestCase(0.99999999, 0.99)]
        [TestCase(-100.999, -100.99)]
        public void TruncateTo2Decimals_Truncates_Correctly(double numToBeTruncated, double expectedResult)
        {
            double result = MathUtils.TruncateTo2Decimals(numToBeTruncated);
            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
