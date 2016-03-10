using System.Linq;

namespace PlanMart.Processors.OrderValidationRules
{
    public class OrderCompletenessValidationRule : IOrderValidationRule
    {
        public ValidationRuleResult Validate(Order order)
        {
            if (order.Customer == null)
            {
                return new ValidationRuleResult(false, "Order must contain a Customer's information");
            }

            if ((order.Items == null) || (!order.Items.Any()))
            {
                // An Order should not be empty (the customer should be ordering something!)
                return new ValidationRuleResult(false, "Order must contain at least one item");
            }

            if (order.Items.Any(x => x.Product == null))
            {
                return new ValidationRuleResult(false, "Product information must be assigned to all items in the order");
            }

            if (order.Items.Any(x => x.Quantity <= 0))
            {
                return new ValidationRuleResult(false, "All order items must have a valid quantity");
            }

            return new ValidationRuleResult(true);
        }
    }
}
