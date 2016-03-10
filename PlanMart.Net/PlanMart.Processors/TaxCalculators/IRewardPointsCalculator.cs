namespace PlanMart.Processors.TaxCalculators
{
    public interface IRewardPointsCalculator
    {   
        decimal Calculate(Order order);
    }
}
