using System.Linq;
using PlanMart.Processors.OrderValidationRules;
using PlanMart.Processors.TaxCalculators;
using System;

namespace PlanMart.Processors
{
    /// <summary>
    /// Your implementation of IOrderProcessor should go here.
    /// </summary>
    public class PlanMartOrderProcessor : IOrderProcessor
    {
        private readonly IOrderValidationRule[] _orderValidationRules;

        private readonly ITaxCalculator _taxCalculator;
        private readonly IShippingCalculator _shippingCalculator;
        private readonly IRewardPointsCalculator _rewardPointsCalculator;


        public PlanMartOrderProcessor()
            // "Inject" default implementations of the required dependencies
            // TODO: consider using a DI framework
            : this(new IOrderValidationRule[] 
                        {
                            new OrderCompletenessValidationRule(),
                            new AlcoholValidationRule(),
                            new ShippingValidationRule()
                        }, 
                  new DefaultTaxCalculator(), 
                  new DefaultShippingCalculator(), 
                  new DefaultRewardPointsCalculator())
        {
            //
        }

        public PlanMartOrderProcessor(
            IOrderValidationRule[] orderValidationRules, 
            ITaxCalculator taxCalculator, 
            IShippingCalculator shippingCalculator, 
            IRewardPointsCalculator rewardPointsCalculator)
        {
            if ((orderValidationRules == null) || (!orderValidationRules.Any()))
            {
                throw new ArgumentException("At least one order validator is expected");
            }

            if (taxCalculator == null)
            {
                throw new ArgumentNullException("Tax calculator is required");
            }

            this._orderValidationRules = orderValidationRules;
            this._taxCalculator = taxCalculator;
            this._shippingCalculator = shippingCalculator;
            this._rewardPointsCalculator = rewardPointsCalculator;
        }

        public bool ProcessOrder(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException("order");
            }

            // Validate
            foreach (var validationRule in _orderValidationRules)
            {
                var validationResult = validationRule.Validate(order);

                // Since we don't need to collect information from all failed rules, lets exist ASAP if one of the validation rules failed.
                if (!validationResult.Valid)
                {
                    return false;
                }
            }
            
            // Tax
            order.LineItems.Add(new LineItem(LineItemType.Tax, this._taxCalculator.Calculate(order)));

            // Shipping
            order.LineItems.Add(new LineItem(LineItemType.Shipping, this._shippingCalculator.Calculate(order)));

            // Rewards
            order.LineItems.Add(new LineItem(LineItemType.RewardsPoints, this._rewardPointsCalculator.Calculate(order)));

            return true;
        }
    }
}