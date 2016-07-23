using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PriceCheck.Exceptions
{
    class PriceContainsCommaException : Exception
    {
        public PriceContainsCommaException()
        {
        }

        public PriceContainsCommaException(string message)
        : base(message)
        {
        }

        public PriceContainsCommaException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
