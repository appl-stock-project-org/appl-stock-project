namespace AppleStockAPI.Utilities {
    public static class MathUtils
    {
        /// <summary>
        /// Round a double to two decimals by truncating, so e.g. 10.9999 -> 10.99
        /// </summary>
        /// <param name="num">Number to truncate</param>
        /// <returns>Truncated number upto 2 decimals</returns>
        public static double TruncateTo2Decimals(double num)
        {
            return Math.Truncate(num * 100) / 100;
        }
    }
}