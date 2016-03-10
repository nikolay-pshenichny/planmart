namespace PlanMart.Processors.TaxCalculators
{
    public interface IShippingCalculator
    {   
        decimal Calculate(Order order);
    }
}
