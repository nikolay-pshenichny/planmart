using System.Linq;
using System;

namespace PlanMart.Processors.TaxCalculators
{
    public class DefaultRewardPointsCalculator : IRewardPointsCalculator
    {
        public decimal Calculate(Order order)
        {
            /*
                Orders get 1 reward point per $2 spent
                Orders get double rewards points when:
                    Using PlanMart rewards credit card
                    Three of these criteria met:
                        Multiple different products in the same order
                        Orders over $200 shipped to US regions other than AZ
                        Orders over $100 shipped to AZ
                        Orders on:
                            Any of the 3 recurring Black Fridays
                            Memorial Day
                            Veteran’s Day
            */

            var totalOrderAmount = order.Items.Sum(item => item.Quantity * item.Product.Price);
            int points = (int)(totalOrderAmount / 2);

            if (IsDoublePoints(order, totalOrderAmount))
            {
                points = points * 2;
            }

            return points;
        }

        private bool IsDoublePoints(Order order, decimal totalOrderAmount)
        {
            const int requiredCriteriaCount = 3;

            Func<bool> multipleDifferentProducts = () => order.Items.Select(x => x.Product.Type).Distinct().Count() > 1;
            Func<bool> over200ShippedToUSExceptAZ = () => (totalOrderAmount > 200.0m) && (order.ShippingRegion != "AZ");
            Func<bool> over100ShippedToAZ = () => (totalOrderAmount > 100.0m) && (order.ShippingRegion == "AZ");
            Func<bool> placedOnHolidays = () => IsRecurringBlackFriday(order.Placed) || IsMemorialDay(order.Placed) || IsVeteransDay(order.Placed);

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



        /// <summary>
        /// Annually on November 11
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        private bool IsVeteransDay(DateTime today)
        {
            return (today.Month == 11) && (today.Day == 11);
        }

        /// <summary>
        /// Every year on the last Monday of May
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        private bool IsMemorialDay(DateTime today)
        {
            const int May = 5;

            // If it is not May yet, lets skip calculations
            if (today.Month != May)
            {
                return false;
            }
            else
            {
                int lastMonday = Enumerable.Range(31, 1).Reverse().First(d => new DateTime(today.Year, May, d).DayOfWeek == DayOfWeek.Monday);
                return (today.Day == lastMonday);
            }
        }

        /// <summary>
        ///  Fourth Thursday of November in the United States. 
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        private bool IsThanksgiving(DateTime today)
        {
            const int November = 11;

            if (today.Month != November)
            {
                return false;
            }
            else
            {
                int fourthThursday = Enumerable.Range(1, 30).Where(d => new DateTime(today.Year, November, d).DayOfWeek == DayOfWeek.Thursday).Skip(3).First();
                return (today.Day == fourthThursday);
            }
        }

        /// <summary>
        ///  Fourth Thursday of November in the United States. 
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        private DateTime ThanksgivingDate(DateTime today)
        {
            const int November = 11;

            var fourthThursday = Enumerable.Range(1, 30).Select(d => new DateTime(today.Year, November, d))
                                           .Where(d => d.DayOfWeek == DayOfWeek.Thursday).Skip(3).First();

            return fourthThursday;
        }

        /// <summary>
        /// Black Friday(shopping), the Friday after U.S.Thanksgiving Day
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        private bool IsBlackFridayShopping(DateTime today)
        {
            var thanksgiving = ThanksgivingDate(today);

            // +1 because Thanksgiving is Thursday
            return (thanksgiving.Month == today.Month) && ((thanksgiving.Day + 1) == today.Day);
        }

        /// <summary>
        /// Black Friday (partying), the last Friday before Christmas
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        private bool IsBlackFridayPartying(DateTime today)
        {
            const int December = 12;

            // Let's skip calculations if it is not December yet
            if (today.Month != December)
            {
                return false;
            }

            // Christmas == Dec, 25
            int fridayBeforeChristmas = Enumerable.Range(1, 24).Reverse().Where(d => new DateTime(today.Year, December, d).DayOfWeek == DayOfWeek.Friday).First();

            return (today.Day == fridayBeforeChristmas);
        }


        /// <summary>
        /// Good Friday or Black Friday, a Christian observance of Jesus' crucifixion
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        private bool IsGoodFriday(DateTime today)
        {
            // TODO
            return false;
        }


        /// <summary>
        /// Any of the 3 recurring Black Fridays
        ///   -Black Friday (partying), the last Friday before Christmas
        ///   -Black Friday(shopping), the Friday after U.S.Thanksgiving Day
        ///   -Good Friday or Black Friday, a Christian observance of Jesus' crucifixion
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        private bool IsRecurringBlackFriday(DateTime today)
        {
            return IsBlackFridayPartying(today) || IsBlackFridayShopping(today) || IsGoodFriday(today);
        }
    }
}
