using System.Linq;
using PlanMart.Processors.Constants;

namespace PlanMart.Processors.OrderValidationRules
{
    public class ShippingValidationRule : IOrderValidationRule
    {
        public ValidationRuleResult Validate(Order order)
        {
            if (ContainsFood(order) && (order.ShippingRegion == StateAbbreviations.Hawaii))
            {
                return new ValidationRuleResult(false, "Food may not be shipped to HI");
            }

            return new ValidationRuleResult(true);
        }

        private bool ContainsFood(Order order)
        {
            return order.Items.Any(item => item.Product.Type == ProductType.Food);
        }
    }
}
