using System.Linq;
using System;
using PlanMart.Processors.Constants;
using PlanMart.Processors.Extensions;

namespace PlanMart.Processors.TaxCalculators
{
    /// <summary>
    /// Implements the following Reward Points calculation logic:
    ///    Orders get 1 reward point per $2 spent
    ///    Orders get double rewards points when:
    ///        Using PlanMart rewards credit card
    ///        Three of these criteria met:
    ///            Multiple different products in the same order
    ///            Orders over $200 shipped to US regions other than AZ
    ///            Orders over $100 shipped to AZ
    ///            Orders on:
    ///                Any of the 3 recurring Black Fridays
    ///                Memorial Day
    ///                Veteran’s Day
    /// </summary>
    public class DefaultRewardPointsCalculator : IRewardPointsCalculator
    {
        public decimal Calculate(Order order)
        {
            var totalOrderAmount = order.Items.Sum(item => item.Quantity * item.Product.Price);

            int points = Decimal.ToInt32(totalOrderAmount / 2);

            if (IsDoublePoints(order, totalOrderAmount))
            {
                points *= 2;
            }

            return points;
        }

        private bool IsDoublePoints(Order order, decimal totalOrderAmount)
        {
            const int requiredCriteriaCount = 3;

            Func<bool> multipleDifferentProducts = () => order.Items.Select(x => x.Product.Type).Distinct().Count() > 1;
            Func<bool> over200ShippedToUSExceptAZ = () => (totalOrderAmount > 200.0m) && (order.ShippingRegion != StateAbbreviations.Arizona);
            Func<bool> over100ShippedToAZ = () => (totalOrderAmount > 100.0m) && (order.ShippingRegion == StateAbbreviations.Arizona);
            Func<bool> placedOnHolidays = () => order.Placed.IsRecurringBlackFriday() || order.Placed.IsMemorialDay() || order.Placed.IsVeteransDay();

            Func<bool>[] criteria = new[] { multipleDifferentProducts, over200ShippedToUSExceptAZ, over100ShippedToAZ, placedOnHolidays };

            int criteriaMet = 0;
            foreach(var criterion in criteria)
            {
                if (criterion())
                {
                    criteriaMet++;
                }

                // Stop evaluating criteria if 3 (requiredCriteriaCount) of them were already met
                if (criteriaMet >= requiredCriteriaCount)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
