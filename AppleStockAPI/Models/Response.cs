using static System.DateTime;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace AppleStockAPI.Models {
    /// <summary>
    /// Class for structuring a response
    /// </summary>
	public record Response {
        [JsonPropertyName("errorMessage")]
        public string? ErrorMessage { get; set; }
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [JsonPropertyName("successMessage")]
        public string? SuccessMessage { get; set; }
    }
}