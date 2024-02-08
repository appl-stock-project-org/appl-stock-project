using static System.DateTime;

namespace AppleStockAPI.Models {
    /// <summary>
    /// Class for structuring a response
    /// </summary>
	public class Response {
        public string? ErrorMessage { get; set; }
        public bool Success { get; set; }
        public string? SuccessMessage { get; set; }
    }
}