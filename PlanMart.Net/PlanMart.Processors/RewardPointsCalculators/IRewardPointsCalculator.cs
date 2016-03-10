namespace PlanMart.Processors.RewardPointsCalculators
{
    public interface IRewardPointsCalculator
    {   
        decimal Calculate(Order order);
    }
}
