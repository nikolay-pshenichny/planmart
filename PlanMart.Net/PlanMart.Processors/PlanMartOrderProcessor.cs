using System.Collections.Generic;
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
            : this(new IOrderValidationRule[] { new AlcoholValidationRule(), new ShippingValidationRule() }, new DefaultTaxCalculator(), new DefaultShippingCalculator(), new DefaultRewardPointsCalculator())
        {
            //
        }

        public PlanMartOrderProcessor(IOrderValidationRule[] orderValidationRules, ITaxCalculator taxCalculator, IShippingCalculator shippingCalculator, IRewardPointsCalculator rewardPointsCalculator)
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

            //TODO: Consider moving this to ValidationRules
            if (order.Customer == null)
            {
                throw new InvalidOperationException("Order must contain Customer information");
            }

            if ((order.Items == null) || (!order.Items.Any()))
            {
                // An Order should not be empty (the customer should be ordering something!)
                throw new InvalidOperationException("Order must contain at least one item");
            }

            if (order.Items.Any(x => x.Product == null))
            {
                throw new InvalidOperationException("Product information must be assigned to all items in the order");
            }

            if (order.Items.Any(x=> x.Quantity <= 0))
            {
                throw new InvalidOperationException("All Items must have a valid quantity");
            }

            // Validate that we can process the order
            foreach (var validationRule in this._orderValidationRules)
            {
                var validationResult = validationRule.Validate(order);

                // Since we don't need to collect all failed rules, lets exist asap if one of the validation rules failed.
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