using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceCheck.Exceptions
{
    class PriceTooLongException : Exception
    {
        public PriceTooLongException()
        {
        }

        public PriceTooLongException(string message)
        : base(message)
        {
        }

        public PriceTooLongException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
