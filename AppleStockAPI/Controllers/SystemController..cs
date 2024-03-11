using AppleStockAPI.Models;
using System.Text.Json;

namespace AppleStockAPI.Controllers
{
    /// <summary>
    /// Parent controller for handling offers, trades and bids
    /// </summary>
    public class SystemController
    {
        ExternalCallController? apiCaller { get; set; }
        readonly double MOCK_STOCK_PRICE;

        TradeController tradeController { get; set; }
        BidController bidController { get; set; }
        OfferController offerController { get; set; }


        public SystemController()
        {
            tradeController = new TradeController();
            bidController = new BidController();
            offerController = new OfferController();
            apiCaller = new ExternalCallController();
        }

        public SystemController(double mockPrice)
        {
            tradeController = new TradeController();
            bidController = new BidController();
            offerController = new OfferController();
            apiCaller = null;
            MOCK_STOCK_PRICE = mockPrice;
        }

        /// <summary>
        /// Function to get the current stock price, either from the API or a mock price
        /// </summary>
        /// <returns>Price of current AAPL stock</returns>
        public double GetCurrentStockPrice()
        {
            if (apiCaller != null)
            {
                return apiCaller.GetLastFetchedPrice();
            }
            else
            {
                return MOCK_STOCK_PRICE;
            }
        }

        /// <summary>
        /// Handles a placed offer.
        /// If offer price is not within +/-10% of last stock price, offer is not placed and an error message is sent in response.
        /// If offer is valid, it's matched against bids to see if a trade can be made and any excess quantity of the offer is added to the list of offers and a success message is sent in response.
        /// </summary>
        public Response HandleOffer(JsonElement obj)
        {
            Response response = new();
            Offer offer;

            try {
                string jsonString = obj.ToString();
                offer = JsonSerializer.Deserialize<Offer>(jsonString)!;
            }
            catch (Exception) {
                response.Success = false;
                response.ErrorMessage = "Request body didn't adhere to the structure of a valid offer.";
                return response;
            }

            
            int originalQuantity = offer.Quantity;

            try
            {
                offerController.ValidateOffer(offer, GetCurrentStockPrice());
            }
            catch (Exception e)
            {
                response.Success = false;
                response.ErrorMessage = e.Message;
                return response;
            }

            // Match offer to existing bids, bids should be matched in descending order of price and ascending order of date, meaning highes prices are matched first and oldest bids are matched first
            var bids = bidController.GetValidBids(offer.Price);
            string tradeMessage = "";
            foreach (var bid in bids)
            {
                if (offer.Quantity == bid.Quantity)
                {
                    tradeController.RecordTrade(bid.Price, offer.Quantity);
                    tradeMessage += ($" Trade made with bid {bid.Id} with price {bid.Price} and quantity {offer.Quantity}.");
                    bidController.RemoveBidWithId(bid.Id);
                    offer.Quantity = 0;
                    break;
                }
                else if (offer.Quantity < bid.Quantity)
                {
                    tradeController.RecordTrade(bid.Price, offer.Quantity);
                    tradeMessage += ($" Trade made with bid {bid.Id} with price {bid.Price} and quantity {offer.Quantity}.");
                    bid.Quantity -= offer.Quantity;
                    offer.Quantity = 0;
                    break;
                }
                else
                {
                    tradeController.RecordTrade(bid.Price, bid.Quantity);
                    tradeMessage += ($" Trade made with bid {bid.Id} with price {bid.Price} and quantity {bid.Quantity}.");
                    bidController.RemoveBidWithId(bid.Id);
                    offer.Quantity -= bid.Quantity;
                }
            }
            if (offer.Quantity > 0)
            {
                offerController.AddOffer(offer);
            }

            response.Success = true;
            response.SuccessMessage = $"Offer successfully placed with the price of {offer.Price} and quantity of {originalQuantity}." + tradeMessage;
            response.RecordId = offer.Id;
            return response;
        }

        /// <summary>
        /// Handles a placed bid.
		/// If bid price is not within +/-10% of last stock price, bid is not placed and an error message is sent in response.
		/// If the bid is valid, it's matched against offers to see if a trade can be made and any excess quantity of the bid is added to the list of bids and a success message is sent in response.
        /// </summary>
		public Response HandleBid(JsonElement obj)
        {

            Response response = new();
            Bid bid;

            try {
                string jsonString = obj.ToString();
                bid = JsonSerializer.Deserialize<Bid>(jsonString)!;
            }
            catch (Exception) {
                response.Success = false;
                response.ErrorMessage = "Request body didn't adhere to the structure of a valid bid.";
                return response;
            }


            
            int originalQuantity = bid.Quantity;

            try
            {
                bidController.ValidateBid(bid, GetCurrentStockPrice());
            }
            catch (Exception e)
            {
                response.Success = false;
                response.ErrorMessage = e.Message;
                return response;
            }

            // Match bid to existing offers, offers should be matched in ascending order of price and date, meaning chepeast offers are matched first and oldest offers are matched first
            var offers = offerController.GetValidOffers(bid.Price);
            string tradeMessage = "";
            foreach (var offer in offers)
            {
                if (bid.Quantity == offer.Quantity)
                {
                    tradeController.RecordTrade(offer.Price, bid.Quantity);
                    tradeMessage += ($" Trade made with offer {offer.Id} with price {offer.Price} and quantity {bid.Quantity}.");
                    offerController.RemoveOffer(offer.Id);
                    bid.Quantity = 0;
                    break;
                }
                else if (bid.Quantity < offer.Quantity)
                {
                    tradeController.RecordTrade(offer.Price, bid.Quantity);
                    tradeMessage += ($" Trade made with offer {offer.Id} with price {offer.Price} and quantity {bid.Quantity}.");
                    offer.Quantity -= bid.Quantity;
                    bid.Quantity = 0;
                    break;
                }
                else
                {
                    tradeController.RecordTrade(offer.Price, offer.Quantity);
                    tradeMessage += ($" Trade made with offer {offer.Id} with price {offer.Price} and quantity {offer.Quantity}.");
                    offerController.RemoveOffer(offer.Id);
                    bid.Quantity -= offer.Quantity;
                }
            }
            if (bid.Quantity > 0)
            {
                bidController.AddBid(bid);
            }

            response.Success = true;
            response.SuccessMessage = $"Bid successfully placed with the price of {bid.Price} and quantity of {originalQuantity}." + tradeMessage;
            response.RecordId = bid.Id;
            return response;
        }

        /// <summary>
        /// Function to list all recorded trades in ascending order of trade time
        /// </summary>
        /// <returns>List of tradecontrollers trades</returns>
        public List<Trade> ListTrades()
        {
            return tradeController.ListTrades();
        }

        /// <summary>
        /// Function to list all offers
        /// </summary>
        /// <returns>List of offers</returns>
        public List<Offer> ListOffers()
        {
            return offerController.GetOffers();
        }

        /// <summary>
        /// Function to list all bids
        /// </summary>
        /// <returns>List of bids</returns>
        public List<Bid> ListBids()
        {
            return bidController.GetBids();
        }

        /// <summary>
        /// Function to clear all the trades, bids and offers
        /// </summary>
        public void Reset()
        {
            tradeController.ClearTrades();
            bidController.ClearBids();
            offerController.ClearOffers();
        }

    }
}
