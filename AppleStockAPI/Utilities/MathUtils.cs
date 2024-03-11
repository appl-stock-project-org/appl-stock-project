namespace AppleStockAPI.Utilities {
    public static class MathUtils
    {
        // Example utility method 1
        public static double TruncateTo2Decimals(double num)
        {
            return Math.Truncate(num * 100) / 100;
        }
    }
}