using System.Linq;
using PlanMart.Processors.Constants;

namespace PlanMart.Processors.TaxCalculators
{
    /// <summary>
    /// Implements the following Tax calculation logic:
    ///    All items are taxed at 8% unless exempt
    ///    The following types of items are exempt from tax:
    ///        Food items shipped to CA, NY
    ///        Clothing items shipped to CT
    ///    Orders to nonprofits are exempt from all tax and shipping
    /// </summary>
    public class DefaultTaxCalculator : ITaxCalculator
    {
        private const decimal Tax = 0.08m; // 8%

        public decimal Calculate(Order order)
        {
            decimal result = 0.0m;

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
            /*
                The following types of items are exempt from tax:
                    Food items shipped to CA, NY
                    Clothing items shipped to CT
                Orders to nonprofits are exempt from all tax and shipping
            */

            bool result = isNonProfit;

            result |= ((item.Product.Type == ProductType.Food) && (new[] { StateAbbreviations.California, StateAbbreviations.NewYork }.Contains(shippingRegion)));

            result |= ((item.Product.Type == ProductType.Clothing) && (shippingRegion == StateAbbreviations.Connecticut));

            return result;
        }
    }
}
