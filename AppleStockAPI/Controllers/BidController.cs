using AppleStockAPI.Models;
using System;
using static System.DateTime;

namespace AppleStockAPI.Controllers {
	public class BidController {
		/// <summary>
        /// Handles a placed bid.
		/// If bid price is not within +/-10% of last stock price, bid is not placed and an error message is sent in response.
		/// If bid is valid, it's added to the list of bids and a success message is sent in response.
        /// </summary>
		public static Response handleBid(Bid bid, List<Bid> listOfBids) {

			Response response = new Response();

			bid.Price = Math.Round(bid.Price, 2);

			const double MOCK_STOCK_PRICE = 100;

			double lowestAccepted = Math.Round(MOCK_STOCK_PRICE - MOCK_STOCK_PRICE * 0.1, 2);
			double highestAccepted = Math.Round(MOCK_STOCK_PRICE + MOCK_STOCK_PRICE * 0.1, 2);

			if (bid.Price < lowestAccepted) {
				response.ErrorMessage = $"Bid price is too low. Lowest accepted price at the moment is {lowestAccepted}.";
				response.Success = false;
			}
			else if (highestAccepted < bid.Price) {
				response.ErrorMessage = $"Bid price is too high. Highest accepted price at the moment is {highestAccepted}.";
				response.Success = false;
			} else {
				response.SuccessMessage = $"Bid placed succesfully with price {bid.Price} and quantity {bid.Quantity}.";
				response.Success = true;

				listOfBids.Add(bid);
			}

			return response;
		}
	}
}