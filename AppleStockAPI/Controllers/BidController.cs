using AppleStockAPI.Models;

namespace AppleStockAPI.Controllers
{
    public class BidController {

		private readonly List<Bid> bids;

		public BidController() {
			bids = [];
		}

		/// <summary>
        /// Handles a placed bid.
		/// If bid price is not within +/-10% of last stock price, bid is not placed and an error message is sent in response.
		/// If bid is valid, it's added to the list of bids and a success message is sent in response.
		/// <param name="bid">Bid to handle</param>
		/// <param name="currentStockPrice">Current stock price that determines the range of acceptable prices</param>
        /// </summary>
		public Response HandleBid(Bid bid, double currentStockPrice) {

			Response response = new();

			bid.Price = Math.Truncate(bid.Price * 100) / 100;


			double lowestAccepted = Math.Round(currentStockPrice * 0.9, 2);
			double highestAccepted = Math.Round(currentStockPrice * 1.1, 2);

			if (bid.Quantity <= 0) {
				response.ErrorMessage = $"Bid quantity needs to be above 0.";
				response.Success = false;
			}
			else if (bid.Price < lowestAccepted) {
				response.ErrorMessage = $"Bid price is too low. Lowest accepted price at the moment is {lowestAccepted}.";
				response.Success = false;
			}
			else if (highestAccepted < bid.Price) {
				response.ErrorMessage = $"Bid price is too high. Highest accepted price at the moment is {highestAccepted}.";
				response.Success = false;
			} else {
				response.SuccessMessage = $"Bid placed succesfully with price {bid.Price} and quantity {bid.Quantity}.";
				response.Success = true;

				bids.Add(bid);
			}

			return response;
		}

		public List<Bid> GetBids() {
			return bids;
		}

		/// <summary>
		/// Returns a bid that matches given Guid id
		/// </summary>
		/// <param name="id">Bid id</param>
		/// <returns>A bid, if bids has a bid with given Guid id</returns>
		public Bid? GetBidWithId(Guid id) {
			return bids.Find(bid => bid.Id == id);
		}

		/// <summary>
		/// Removes a bid that matches the given id.
		/// </summary>
		/// <param name="id">Id of the bid that is wanted to be removed</param>
		/// <returns>An error message, if a bid with the give id was not found. If no errors returns void</returns>
		public string? RemoveBidWithId(Guid id) {
			Bid? targetBid = GetBidWithId(id);
			if (targetBid == null) {
				return $"Bid with id {id} was not found.";
			}
			bids.Remove(targetBid);
			return null;
		}

		public void ClearBids() {
			bids.Clear();
		}
	}
}