using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanMart.Processors.Extensions
{
    public static class OrderExtensions
    {
        public static bool ContainsFood(this Order self)
        {
            return self.Items.Any(item => item.Product.Type == ProductType.Food);
        }
    }
}
