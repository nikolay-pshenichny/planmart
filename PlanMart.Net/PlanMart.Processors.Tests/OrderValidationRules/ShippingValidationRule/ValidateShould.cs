using System;
using NUnit.Framework;

namespace PlanMart.Processors.Tests.OrderValidationRules.ShippingValidationRule
{
    [TestFixture]
    public class ValidateShould
    {
        [Test]
        public void RequireQuantityForOrderItems()
        {
            // Arrange
            var rule = new PlanMart.Processors.OrderValidationRules.ShippingValidationRule();
            var order = new Order(new Customer(DateTime.Now, false), "HI", PaymentMethod.Mastercard, DateTime.Now);
            order.Items.Add(new ProductOrder(new Product(1m, 2m, ProductType.Food), 1));
            order.Items.Add(new ProductOrder(new Product(1m, 2m, ProductType.Alcohol), 1));

            // Act 
            var result = rule.Validate(order);

            // Assert
            Assert.AreEqual(false, result.Valid);
            Assert.AreEqual("Food may not be shipped to HI", result.Message);
        }
    }
}
