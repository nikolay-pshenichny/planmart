namespace PlanMart.Processors.OrderValidationRules
{
    public interface IOrderValidationRule
    {
        ValidationRuleResult Validate(Order order);
    }
}
