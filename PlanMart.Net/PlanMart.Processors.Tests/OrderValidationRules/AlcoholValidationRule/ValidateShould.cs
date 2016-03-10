using System;
using NUnit.Framework;

namespace PlanMart.Processors.Tests.OrderValidationRules.AlcoholValidationRule
{
    [TestFixture]
    public class ValidateShould
    {
        [TestCase("1980-01-01", "1990-01-01", false)]
        [TestCase("1980-05-05", "2001-05-04", false)]
        [TestCase("1980-05-05", "2001-05-05", true)]
        [TestCase("1980-05-05", "2001-06-06", true)]
        [TestCase("1980-05-05", "2002-01-01", true)]
        public void RequireCustomerToBe21ToOrderAlcohol(string customerBirthDate, string orderDate, bool expectedResult)
        {
            // Arrange
            var rule = new PlanMart.Processors.OrderValidationRules.AlcoholValidationRule();
            var order = new Order(new Customer(DateTime.Parse(customerBirthDate), false), "HI", PaymentMethod.Mastercard, DateTime.Parse(orderDate));
            order.Items.Add(new ProductOrder(new Product(1m, 2m, ProductType.Alcohol), 1));

            // Act 
            var result = rule.Validate(order);

            // Assert
            // NOTE: Customer BirthDate and Order.Placed date should be used for age calculation
            Assert.AreEqual(expectedResult, result.Valid);
            if (!expectedResult)
            {
                Assert.AreEqual("Alcohol may only be shipped to customers age 21 or over in the US", result.Message);
            }
        }

        [TestCase("VA", false)]
        [TestCase("NC", false)]
        [TestCase("SC", false)]
        [TestCase("TN", false)]
        [TestCase("AK", false)]
        [TestCase("KY", false)]
        [TestCase("AL", false)]
        [TestCase("CA", true)]
        public void PhohibitAlcoholShipmentsToCertainStates(string state, bool expectedResult)
        {
            // Arrange
            var rule = new PlanMart.Processors.OrderValidationRules.AlcoholValidationRule();
            var order = new Order(new Customer(DateTime.Now.AddYears(-40), false), state, PaymentMethod.Mastercard, DateTime.Now);
            order.Items.Add(new ProductOrder(new Product(1m, 2m, ProductType.Alcohol), 1));

            // Act 
            var result = rule.Validate(order);

            // Assert
            Assert.AreEqual(expectedResult, result.Valid);
            if (!expectedResult)
            {
                Assert.AreEqual("Alcohol may not be shipped to VA, NC, SC, TN, AK, KY, AL", result.Message);
            }
        }
    }
}
