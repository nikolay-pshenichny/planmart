using System.Linq;

namespace PlanMart.Processors.TaxCalculators
{
    public class DefaultShippingCalculator : IShippingCalculator
    {
        private const decimal Tax = 0.08m; // 8%

        public decimal Calculate(Order order)
        {
            /*
                Orders to nonprofits are exempt from all tax and shipping
                Shipping is $10 for orders under 20 pounds in the continental US
                Shipping is $20 for orders 20 pounds or over in the continental US
                Shipping for orders to the non-continental US is $35
            */

            if (order.Customer.IsNonProfit)
            {
                return 0.0m;
            }

            decimal result = 0.0m;

            if (!IsContinentalUS(order.ShippingRegion))
            {
                result = 35m;
            }
            else
            {
                result = (CalculateTotalWeight(order) >= 20.0m) ? 20m : 10m;
            }

            return result;
        }

        private bool IsContinentalUS(string shippingRegion)
        {
            bool result = shippingRegion != "HI";
            return result;
        }

        private decimal CalculateTotalWeight(Order order)
        {
            decimal result = order.Items.Sum(item => (item.Quantity * item.Product.Weight));
            return result;
        }
    }
}
