using PlanMart.Processors.Extensions;

namespace PlanMart.Processors.OrderValidationRules
{
    public class ShippingValidationRule : IOrderValidationRule
    {
        public ValidationRuleResult Validate(Order order)
        {
            if (order.ContainsFood() && (order.ShippingRegion == "HI"))
            {
                return new ValidationRuleResult(false, "Food may not be shipped to HI");
            }

            return new ValidationRuleResult(true);
        }
    }
}
