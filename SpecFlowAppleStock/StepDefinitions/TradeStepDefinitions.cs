using AppleStockAPI.Controllers;

namespace SpecFlowAppleStock.StepDefinitions
{
    [Binding]
    public sealed class TradeStepDefinitions
    {
        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef
        double price = 0;
        int qnt = 0;
        TradeController controller = new TradeController(); 
            
        [Given("the trade price is (.*)")]
        public void GivenTheTradePriceIs(double number)
        {


            //TODO: implement arrange (precondition) logic
            // For storing and retrieving scenario-specific data see https://go.specflow.org/doc-sharingdata
            // To use the multiline text or the table argument of the scenario,
            // additional string/Table parameters can be defined on the step definition
            // method. 

            price = number;
        }

        [Given("the trade quantity is (.*)")]
        public void GivenTheTradeQuantityIs(int number)
        {
            //TODO: implement arrange (precondition) logic

            qnt = number;
        }

        [When(@"the trade is added")]
        public void WhenTheTradeIsAdded()
        {
            controller.RecordTrade(price, qnt);
        }


        [Then("the result should be (.*)")]
        public void ThenTheResultShouldBe(int result)
        {
            //TODO: implement assert (verification) logic

            controller.GetTrades().Count.Should().Be(1);
        }
    }
}
