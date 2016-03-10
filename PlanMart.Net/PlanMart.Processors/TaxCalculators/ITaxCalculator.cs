namespace PlanMart.Processors.TaxCalculators
{
    public interface ITaxCalculator
    {   
        decimal Calculate(Order order);
    }
}
