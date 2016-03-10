namespace PlanMart.Processors.ShippingCalculators
{
    public interface IShippingCalculator
    {   
        decimal Calculate(Order order);
    }
}
