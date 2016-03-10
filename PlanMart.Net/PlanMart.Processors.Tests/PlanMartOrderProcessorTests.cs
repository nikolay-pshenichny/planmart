using System;
using System.Linq;
using NUnit.Framework;

namespace PlanMart.Processors.Tests
{
    /// <summary>
    /// Note: This is the original UnitTest (was provided in the default implementation). No changes were made here. Additional tests are in separate files.
    /// </summary>
    [TestFixture]
    public class PlanMartOrderProcessorTests
    {
        [Test]
        public void SampleTest()
        {
            var order = new Order(
                new Customer(new DateTime(1988, 3, 4), false),
                "AZ",
                PaymentMethod.Visa,
                new DateTime(2015, 11, 27));
            order.Items.Add(new ProductOrder(new Product(20, 2, ProductType.Alcohol), 3));
            order.Items.Add(new ProductOrder(new Product(25, 3, ProductType.Food), 3));

            var processor = new PlanMart.Processors.PlanMartOrderProcessor();
            var isValid = processor.ProcessOrder(order);
            var tax = order.LineItems.SingleOrDefault(x => x.Type == LineItemType.Tax);
            var shipping = order.LineItems.SingleOrDefault(x => x.Type == LineItemType.Shipping);
            var rewardPoints = order.LineItems.SingleOrDefault(x => x.Type == LineItemType.RewardsPoints);

            Assert.IsTrue(isValid);
            Assert.IsNotNull(tax);
            Assert.AreEqual(10.80M, tax.Amount);
            Assert.IsNotNull(shipping);
            Assert.AreEqual(10, shipping.Amount);
            Assert.IsNotNull(rewardPoints);
            Assert.AreEqual(134, rewardPoints.Amount);
        }
    }
}