using System.Linq;

namespace PlanMart.Processors.TaxCalculators
{
    public class DefaultTaxCalculator : ITaxCalculator
    {
        private const decimal Tax = 0.08m; // 8%

        public decimal Calculate(Order order)
        {
            decimal result = 0.0m;

            /*
                All items are taxed at 8% unless exempt
                The following types of items are exempt from tax:
                    Food items shipped to CA, NY
                    Clothing items shipped to CT
                Orders to nonprofits are exempt from all tax and shipping
            */

            foreach (var item in order.Items)
            {
                if (!ItemIsExempt(item, order.ShippingRegion, order.Customer.IsNonProfit))
                {
                    result += (item.Product.Price * item.Quantity) * Tax;
                }
            }

            return result;
        }

        private bool ItemIsExempt(ProductOrder item, string shippingRegion, bool isNonProfit)
        {
            bool result = isNonProfit;
            /*
                The following types of items are exempt from tax:
                    Food items shipped to CA, NY
                    Clothing items shipped to CT
                Orders to nonprofits are exempt from all tax and shipping
            */
            result |= ((item.Product.Type == ProductType.Food) && (new[] { "CA", "NY" }.Contains(shippingRegion)));

            result |= ((item.Product.Type == ProductType.Clothing) && (shippingRegion == "CT"));

            return result;
        }
    }
}
