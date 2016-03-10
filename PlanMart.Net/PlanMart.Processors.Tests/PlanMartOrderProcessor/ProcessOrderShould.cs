using System;
using NUnit.Framework;
using Moq;
using System.Linq;

namespace PlanMart.Processors.Tests.PlanMartOrderProcessor
{
    [TestFixture]
    public class ProcessOrderShould
    {
        private PlanMart.Processors.PlanMartOrderProcessor _processor;

        private Mock<PlanMart.Processors.OrderValidationRules.IOrderValidationRule> _validationRule1Mock;

        private Mock<PlanMart.Processors.OrderValidationRules.IOrderValidationRule> _validationRule2Mock;

        private Mock<PlanMart.Processors.TaxCalculators.ITaxCalculator> _taxCalculatorMock;

        private Mock<PlanMart.Processors.ShippingCalculators.IShippingCalculator> _shippingCalculatorMock;

        private Mock<PlanMart.Processors.RewardPointsCalculators.IRewardPointsCalculator> _rewardPointsCalculatorMock;

        private Order _order;

        [SetUp]
        public void Initialize()
        {
            _validationRule1Mock = new Mock<PlanMart.Processors.OrderValidationRules.IOrderValidationRule>(MockBehavior.Strict);
            _validationRule1Mock.Setup(x => x.Validate(It.IsAny<Order>())).Returns(new PlanMart.Processors.OrderValidationRules.ValidationRuleResult(true));

            _validationRule2Mock = new Mock<PlanMart.Processors.OrderValidationRules.IOrderValidationRule>(MockBehavior.Strict);
            _validationRule2Mock.Setup(x => x.Validate(It.IsAny<Order>())).Returns(new PlanMart.Processors.OrderValidationRules.ValidationRuleResult(true));

            _taxCalculatorMock = new Mock<PlanMart.Processors.TaxCalculators.ITaxCalculator>(MockBehavior.Strict);
            _taxCalculatorMock.Setup(x => x.Calculate(It.IsAny<Order>())).Returns(11m);

            _shippingCalculatorMock = new Mock< PlanMart.Processors.ShippingCalculators.IShippingCalculator>(MockBehavior.Strict);
            _shippingCalculatorMock.Setup(x => x.Calculate(It.IsAny<Order>())).Returns(22m);

            _rewardPointsCalculatorMock = new Mock < PlanMart.Processors.RewardPointsCalculators.IRewardPointsCalculator>(MockBehavior.Strict);
            _rewardPointsCalculatorMock.Setup(x => x.Calculate(It.IsAny<Order>())).Returns(33m);

            _processor = new PlanMart.Processors.PlanMartOrderProcessor(
                new PlanMart.Processors.OrderValidationRules.IOrderValidationRule[] { _validationRule1Mock.Object, _validationRule2Mock.Object },
                _taxCalculatorMock.Object,
                _shippingCalculatorMock.Object,
                _rewardPointsCalculatorMock.Object);


            _order = new Order(new Customer(DateTime.Now, false), "XX", PaymentMethod.Mastercard, DateTime.Now);
        }

        [Test]
        public void UseAllValidatorsToValidateOrder()
        {
            // Arrange
            //

            // Act
            var result = _processor.ProcessOrder(_order);

            // Assert
            Assert.IsTrue(result);
            _validationRule1Mock.Verify(x => x.Validate(_order), Times.Once);
            _validationRule2Mock.Verify(x => x.Validate(_order), Times.Once);
        }

        [Test]
        public void TerminateValidationIfPreviousValidatorFailed()
        {
            // Arrange
            _validationRule1Mock.Setup(x => x.Validate(It.IsAny<Order>())).Returns(new PlanMart.Processors.OrderValidationRules.ValidationRuleResult(false));

            // Act
            var result = _processor.ProcessOrder(_order);

            // Assert
            Assert.IsFalse(result);
            _validationRule1Mock.Verify(x => x.Validate(_order), Times.Once);
            _validationRule2Mock.Verify(x => x.Validate(_order), Times.Never, "Second validator should never be called if first validator failed");
        }

        [Test]
        public void UseTaxCalculatorToCalulateTax()
        {
            // Arrange

            // Act
            var result = _processor.ProcessOrder(_order);

            // Assert
            var tax = _order.LineItems.Single(x => x.Type == LineItemType.Tax);
            Assert.AreEqual(11m, tax.Amount);
            _taxCalculatorMock.Verify(x => x.Calculate(_order), Times.Once);
        }

        [Test]
        public void UseShippingCalculatorToCalulateShippingCost()
        {
            // Arrange

            // Act
            var result = _processor.ProcessOrder(_order);

            // Assert
            var shipping = _order.LineItems.Single(x => x.Type == LineItemType.Shipping);
            Assert.AreEqual(22m, shipping.Amount);
            _shippingCalculatorMock.Verify(x => x.Calculate(_order), Times.Once);
        }


        [Test]
        public void UseRewardsCalculatorToCalulateRewardPoints()
        {
            // Arrange

            // Act
            var result = _processor.ProcessOrder(_order);

            // Assert
            var shipping = _order.LineItems.Single(x => x.Type == LineItemType.RewardsPoints);
            Assert.AreEqual(33m, shipping.Amount);
            _rewardPointsCalculatorMock.Verify(x => x.Calculate(_order), Times.Once);
        }
    }
}
