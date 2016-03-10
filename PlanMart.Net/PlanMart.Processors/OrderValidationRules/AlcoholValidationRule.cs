using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;

namespace PlanMart.Processors.OrderValidationRules
{
    public class AlcoholValidationRule : IOrderValidationRule
    {
        private static string[] _statesToWhichAlcoholCanNotBeShipped = new[] { "VA", "NC", "SC", "TN", "AK", "KY", "AL" };
        private const int _minimumDrinkingAgeInUS = 21;

        public ValidationRuleResult Validate(Order order)
        {
            if (ContainsAlcohol(order))
            {
                if (_statesToWhichAlcoholCanNotBeShipped.Contains(order.ShippingRegion))
                {
                    return new ValidationRuleResult(false, "Alcohol may not be shipped to VA, NC, SC, TN, AK, KY, AL");
                }

                //TODO: Is everything in LocalDates or UTC, or mix?
                var birthday = new LocalDate(order.Customer.BirthDate.Year, order.Customer.BirthDate.Month, order.Customer.BirthDate.Day);
                var today = new LocalDate();
                var period = Period.Between(birthday, today);
                if ((period.Years < _minimumDrinkingAgeInUS) && (order.ShippingRegion == "US"))
                {
                    return new ValidationRuleResult(false, "Alcohol may only be shipped to customers age 21 or over in the US");
                }
            }


            return new ValidationRuleResult(true);

        }


        private bool ContainsAlcohol(Order order)
        {
            return order.Items.Any(item => item.Product.Type == ProductType.Alcohol);
        }
    }
}
