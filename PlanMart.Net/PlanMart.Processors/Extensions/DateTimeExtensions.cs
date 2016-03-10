using System;
using System.Linq;

namespace PlanMart.Processors.Extensions
{
    /// <summary>
    /// DateTime extensions to calculate some US Holidays
    /// TODO: Do we to take into account Canadian/Western/Estern/etc holidays?
    /// TODO: There are a lot of LINQ here. Methods will be slow. Do we need to optimize them?
    /// TODO: IsGoodFriday() is not implemented and will return False by default
    /// TODO: Add unit tests for these extensions
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Annually on November 11
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        public static bool IsVeteransDay(this DateTime today)
        {
            return (today.Month == 11) && (today.Day == 11);
        }

        /// <summary>
        /// Every year on the last Monday of May
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        public static bool IsMemorialDay(this DateTime today)
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
        public static bool IsThanksgiving(this DateTime today)
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
        public static DateTime ThanksgivingDate(this DateTime today)
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
        public static bool IsBlackFridayShopping(this DateTime today)
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
        public static bool IsBlackFridayPartying(this DateTime today)
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
        public static bool IsGoodFriday(this DateTime today)
        {
            // TODO: Implement this
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
        public static bool IsRecurringBlackFriday(this DateTime today)
        {
            return IsBlackFridayPartying(today) || IsBlackFridayShopping(today) || IsGoodFriday(today);
        }

    }
}
