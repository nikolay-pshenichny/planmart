using System;
using NUnit.Framework;

namespace PlanMart.Processors.Tests.TaxCalculators.DefaultTaxCalculator
{
    [TestFixture]
    public class CalculateShould
    {
        [Test]
        public void ExemptFoodItemsShippedToCA()
        {
            // Arrange
            var calculator = new PlanMart.Processors.TaxCalculators.DefaultTaxCalculator();
            var order = new Order(new Customer(DateTime.Now, false), "CA", PaymentMethod.Mastercard, DateTime.Now);
            order.Items.Add(new ProductOrder(new Product(11m, 22m, ProductType.Food), 33));

            // Act
            var result = calculator.Calculate(order);

            // Assert
            Assert.AreEqual(0.0m, result, "The following types of items are exempt from tax: Food items shipped to CA, NY");
        }

        [Test]
        public void ExemptFoodItemsShippedToNY()
        {
            // Arrange
            var calculator = new PlanMart.Processors.TaxCalculators.DefaultTaxCalculator();
            var order = new Order(new Customer(DateTime.Now, false), "NY", PaymentMethod.Mastercard, DateTime.Now);
            order.Items.Add(new ProductOrder(new Product(11m, 22m, ProductType.Food), 33));

            // Act
            var result = calculator.Calculate(order);

            // Assert
            Assert.AreEqual(0.0m, result, "The following types of items are exempt from tax: Food items shipped to CA, NY");
        }

        [Test]
        public void ExemptClothingItemsShippedToCT()
        {
            // Arrange
            var calculator = new PlanMart.Processors.TaxCalculators.DefaultTaxCalculator();
            var order = new Order(new Customer(DateTime.Now, false), "CT", PaymentMethod.Mastercard, DateTime.Now);
            order.Items.Add(new ProductOrder(new Product(11m, 22m, ProductType.Clothing), 33));

            // Act
            var result = calculator.Calculate(order);

            // Assert
            Assert.AreEqual(0.0m, result, "The following types of items are exempt from tax: Clothing items shipped to CT");
        }

        [Test]
        public void ExemptNonProfitsFromTaxes()
        {
            // Arrange
            var calculator = new PlanMart.Processors.TaxCalculators.DefaultTaxCalculator();
            var order = new Order(new Customer(DateTime.Now, true), "CA", PaymentMethod.Mastercard, DateTime.Now);
            order.Items.Add(new ProductOrder(new Product(11m, 22m, ProductType.Clothing), 33));

            // Act
            var result = calculator.Calculate(order);

            // Assert
            Assert.AreEqual(0.0m, result, "Orders to nonprofits are exempt from all tax and shipping");
        }

        [Test]
        public void Calculate8PercentTaxOnEachNonExemptItem()
        {
            // Arrange
            var calculator = new PlanMart.Processors.TaxCalculators.DefaultTaxCalculator();
            var order = new Order(new Customer(DateTime.Now, false), "CA", PaymentMethod.Mastercard, DateTime.Now);
            order.Items.Add(new ProductOrder(new Product(1m, 2m, ProductType.Clothing), 3));
            order.Items.Add(new ProductOrder(new Product(4m, 5m, ProductType.Alcohol), 6));
            order.Items.Add(new ProductOrder(new Product(999m, 999m, ProductType.Food), 999)); // THis item is exempt (by the Food to CA rule)

            // Act
            var result = calculator.Calculate(order);

            // Assert
            var expected = (((1m * 3) + (4m * 6)) / 100) * 8; // 8% from total order sum
            Assert.AreEqual(expected, result, "All items are taxed at 8% unless exempt");
        }

        
        [Ignore("Not yet implemented")]
        [Test]
        public void UseRoundHalfUpStrategyToRoundCalculatedTax()
        {
            // TODO: Sales tax should be rounded using the round half up strategy
        }
    }
}
