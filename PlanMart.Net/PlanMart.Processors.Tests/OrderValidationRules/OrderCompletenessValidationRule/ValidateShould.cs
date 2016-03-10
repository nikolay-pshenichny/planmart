using System;
using NUnit.Framework;

namespace PlanMart.Processors.Tests.OrderValidationRules.OrderCompletenessValidationRule
{
    [TestFixture]
    public class ValidateShould
    {
        [Test]
        public void RequireAtLeastOneOrderItem()
        {
            // Arrange
            var rule = new PlanMart.Processors.OrderValidationRules.OrderCompletenessValidationRule();
            var order = new Order(new Customer(DateTime.Now, false), "CA", PaymentMethod.Mastercard, DateTime.Now);

            // Act 
            var result = rule.Validate(order);

            // Assert
            Assert.AreEqual(false, result.Valid, "An Order should not be empty (the customer should be ordering something!)");
            Assert.AreEqual("Order must contain at least one item", result.Message);
        }

        [Test]
        public void RequireProductDetailsForOrderItems()
        {
            // Arrange
            var rule = new PlanMart.Processors.OrderValidationRules.OrderCompletenessValidationRule();
            var order = new Order(new Customer(DateTime.Now, false), "CA", PaymentMethod.Mastercard, DateTime.Now);
            order.Items.Add(new ProductOrder(null, 1));

            // Act 
            var result = rule.Validate(order);

            // Assert
            Assert.AreEqual(false, result.Valid);
            Assert.AreEqual("Product information must be assigned to all items in the order", result.Message);
        }

        [Test]
        public void RequireCustomerInformation()
        {
            // Arrange
            var rule = new PlanMart.Processors.OrderValidationRules.OrderCompletenessValidationRule();
            var order = new Order(null, "CA", PaymentMethod.Mastercard, DateTime.Now);
            order.Items.Add(new ProductOrder(null, 1));

            // Act 
            var result = rule.Validate(order);

            // Assert
            Assert.AreEqual(false, result.Valid);
            Assert.AreEqual("Order must contain a Customer's information", result.Message);
        }

        [Test]
        public void RequireQuantityForOrderItems()
        {
            // Arrange
            var rule = new PlanMart.Processors.OrderValidationRules.OrderCompletenessValidationRule();
            var order = new Order(new Customer(DateTime.Now, false), "CA", PaymentMethod.Mastercard, DateTime.Now);
            order.Items.Add(new ProductOrder(new Product(1m, 2m, ProductType.Alcohol), 0));
            order.Items.Add(new ProductOrder(new Product(1m, 2m, ProductType.Alcohol), 1));

            // Act 
            var result = rule.Validate(order);

            // Assert
            Assert.AreEqual(false, result.Valid);
            Assert.AreEqual("All order items must have a valid quantity", result.Message);
        }
    }
}
