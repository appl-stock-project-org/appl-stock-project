using static System.DateTime;

namespace AppleStockAPI.Models {
    /// <summary>
    /// Class for a bid
    /// </summary>
	public class Bid {
        /// <summary>
        /// Format for the id. Also needed to parse DateTime from id.
        /// </summary>
        public const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss:fff";


        public int Quantity { get; set; }
        public double Price { get; set; }
        /// <summary>
        /// Unique id for a bid. Because only one user, current time is always unique. Makes it easy to order bids by date
        /// </summary>
        public string Id { get; set; } = DateTime.Now.ToString(DATE_FORMAT);

        override public string ToString() {
            var message = "Id: " + this.Id + " | Turned into DateTime: " + this.getCreationDate().ToString();
            return message;
        }

        /// <summary>
        /// Parses and returns the date out of Id which is a formatted DateTime
        /// </summary>
        public DateTime? getCreationDate() {

            if (DateTime.TryParseExact(this.Id, DATE_FORMAT, null, System.Globalization.DateTimeStyles.None, out DateTime parsedDateTime))
            {
                return parsedDateTime;
            }
            
            return null;
        }
        
    }
}