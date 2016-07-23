using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PriceCheck.Exceptions;
using PriceCheck.View;

namespace PriceCheck.Utility
{
    static class CheckPrice
    {
        //TODO Dette skal nok laves på en bedre måde.
        //TODO Forhindre at når der er en exeption, så skal vinduet ikke lukkes når der trykkes på kryds.
 
        public static double CheckPriceFormat(string price)
        {
            if (price.Length < 16)
            {
                if (!price.Contains(","))
                {
                    return Math.Round(Convert.ToDouble(price), 2);
                }
                throw new PriceContainsCommaException();
            }
            throw new PriceTooLongException();
        }
    }
}
