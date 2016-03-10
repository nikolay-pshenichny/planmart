using System;
using NUnit.Framework;

namespace PlanMart.Processors.Tests.RewardPointsCalculators.DefaultRewardPointsCalculator
{
    //TODO: Add tests for Double Points
    [TestFixture]
    public class CalculateShould
    {
        [TestCase(1, 0)]
        [TestCase(2, 1)]
        [TestCase(4, 2)]
        [TestCase(100, 50)]
        public void Return1PointForEach2DollarsSpent(decimal price, decimal expectedPoints)
        {
            // Arrange
            var calculator = new PlanMart.Processors.RewardPointsCalculators.DefaultRewardPointsCalculator();
            var order = new Order(new Customer(DateTime.Now, false), "CA", PaymentMethod.Mastercard, DateTime.Now);
            order.Items.Add(new ProductOrder(new Product(price, 22m, ProductType.Clothing), 1));

            // Act
            var result = calculator.Calculate(order);

            // Assert
            Assert.AreEqual(expectedPoints, result, "Orders get 1 reward point per $2 spent");
        }
    }
}
