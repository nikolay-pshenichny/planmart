using System;
using NUnit.Framework;

namespace PlanMart.Processors.Tests.ShippingCalculators.DefaultShippingCalculator
{
    [TestFixture]
    public class CalculateShould
    {
        [Test]
        public void ExemptNonProfitsFromShipping()
        {
            // Arrange
            var calculator = new PlanMart.Processors.ShippingCalculators.DefaultShippingCalculator();
            var order = new Order(new Customer(DateTime.Now, true), "CA", PaymentMethod.Mastercard, DateTime.Now);
            order.Items.Add(new ProductOrder(new Product(11m, 22m, ProductType.Clothing), 33));

            // Act
            var result = calculator.Calculate(order);

            // Assert
            Assert.AreEqual(0.0m, result, "Orders to nonprofits are exempt from all tax and shipping");
        }

        [Test]
        public void Return35ForNonContinentalUs()
        {
            // Arrange
            var calculator = new PlanMart.Processors.ShippingCalculators.DefaultShippingCalculator();
            var order = new Order(new Customer(DateTime.Now, false), "HI", PaymentMethod.Mastercard, DateTime.Now);
            order.Items.Add(new ProductOrder(new Product(11m, 22m, ProductType.Clothing), 33));

            // Act
            var result = calculator.Calculate(order);

            // Assert
            Assert.AreEqual(35.0m, result, "Shipping for orders to the non-continental US is $35");
        }

        [Test]
        public void Return10ForContinentalUsAndWeightUnder20Pounds()
        {
            // Arrange
            var calculator = new PlanMart.Processors.ShippingCalculators.DefaultShippingCalculator();
            var order = new Order(new Customer(DateTime.Now, false), "CA", PaymentMethod.Mastercard, DateTime.Now);
            order.Items.Add(new ProductOrder(new Product(1m, 19.99m, ProductType.Clothing), 1));

            // Act
            var result = calculator.Calculate(order);

            // Assert
            Assert.AreEqual(10.0m, result, "Shipping is $10 for orders under 20 pounds in the continental US");
        }

        [Test]
        public void Return20ForContinentalUsAndWeightEqualTo20Pounds()
        {
            // Arrange
            var calculator = new PlanMart.Processors.ShippingCalculators.DefaultShippingCalculator();
            var order = new Order(new Customer(DateTime.Now, false), "CA", PaymentMethod.Mastercard, DateTime.Now);
            order.Items.Add(new ProductOrder(new Product(1m, 20.00m, ProductType.Clothing), 1));

            // Act
            var result = calculator.Calculate(order);

            // Assert
            Assert.AreEqual(20.0m, result, "Shipping is $20 for orders 20 pounds or over in the continental US");
        }

        [Test]
        public void Return20ForContinentalUsAndWeightAbove20Pounds()
        {
            // Arrange
            var calculator = new PlanMart.Processors.ShippingCalculators.DefaultShippingCalculator();
            var order = new Order(new Customer(DateTime.Now, false), "CA", PaymentMethod.Mastercard, DateTime.Now);
            order.Items.Add(new ProductOrder(new Product(1m, 20.01m, ProductType.Clothing), 1));

            // Act
            var result = calculator.Calculate(order);

            // Assert
            Assert.AreEqual(20.0m, result, "Shipping is $20 for orders 20 pounds or over in the continental US");
        }
    }
}
