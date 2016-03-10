using System.Linq;
using PlanMart.Processors.Constants;

namespace PlanMart.Processors.ShippingCalculators
{
    /// <summary>
    /// Implements the following Shipping Cost calculation logic:
    ///    Orders to nonprofits are exempt from all tax and shipping
    ///    Shipping is $10 for orders under 20 pounds in the continental US
    ///    Shipping is $20 for orders 20 pounds or over in the continental US
    ///    Shipping for orders to the non-continental US is $35
    /// </summary>
    public class DefaultShippingCalculator : IShippingCalculator
    {
        private const decimal WeightThreshold = 20.0m;
        private const decimal PriceForOrdersWithWeightOverOrEqualToThreshold = 20.0m;
        private const decimal PriceForOrdersWithWeightBelowThreshold = 10.0m;
        private const decimal PriceForOrdersToNonContinentalUS = 35.0m;

        public decimal Calculate(Order order)
        {
            decimal result = 0.0m;

            if (!order.Customer.IsNonProfit)
            {
                if (!IsContinentalUS(order.ShippingRegion))
                {
                    result = PriceForOrdersToNonContinentalUS;
                }
                else
                {
                    result = (CalculateTotalWeight(order) >= WeightThreshold)
                                ? PriceForOrdersWithWeightOverOrEqualToThreshold
                                : PriceForOrdersWithWeightBelowThreshold;
                }
            }

            return result;
        }

        private bool IsContinentalUS(string shippingRegion)
        {
            bool result = shippingRegion != StateAbbreviations.Hawaii;
            return result;
        }

        private decimal CalculateTotalWeight(Order order)
        {
            decimal result = order.Items.Sum(item => (item.Quantity * item.Product.Weight));
            return result;
        }
    }
}
