using AppleStockAPI.Models;

namespace AppleStockAPI.Controllers
{
    public class BidController {

		private readonly List<Bid> bids;

		public BidController() {
			bids = new List<Bid>();
		}

		/// <summary>
        /// Handles a placed bid.
		/// If bid price is not within +/-10% of last stock price, bid is not placed and an error message is sent in response.
		/// If bid is valid, it's added to the list of bids and a success message is sent in response.
		/// <param name="bid">Bid to handle</param>
		/// <param name="currentStockPrice">Current stock price that determines the range of acceptable prices</param>
        /// </summary>
		public void ValidateBid(Bid bid, double currentStockPrice) {

			bid.Price = Math.Truncate(bid.Price * 100) / 100;


			double lowestAccepted = Math.Round(currentStockPrice * 0.9, 2);
			double highestAccepted = Math.Round(currentStockPrice * 1.1, 2);

			if (bid.Quantity <= 0) {
				throw new Exception("Bid quantity needs to be above 0.");
			}
			else if (bid.Price < lowestAccepted) {
				throw new Exception($"Bid price is too low. Lowest accepted price at the moment is {lowestAccepted}.");
			}
			else if (highestAccepted < bid.Price) {
				throw new Exception($"Bid price is too high. Highest accepted price at the moment is {highestAccepted}.");
			}

			return;
		}

        /// <summary>
        /// Returns all of the bids that have a higher or same price as the given price limit
        /// </summary>
        /// <param name="price">Valid price limit</param>
        /// <returns>List of bids that have a higher price than the limit</returns>
        public List<Bid> GetValidBids(double price)
        {

            List<Bid> validBids = bids.FindAll(bid => bid.Price >= price);

            return validBids.OrderByDescending(bid => bid.Price).ThenBy(bid => bid.CreatedAt).ToList();

        }

        /// <summary>
        /// Adds a bid to the list of bids
        /// </summary>
        /// <param name="bid">Bid to addd to the bids list</param>
        public void AddBid(Bid bid)
		{
            bids.Add(bid);
        }

		/// <summary>
		/// Gets all current bids
		/// </summary>
		/// <returns></returns>
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

		/// <summary>
		/// Empties the bids list
		/// </summary>
		public void ClearBids() {
			bids.Clear();
		}
    }
}