using System;

namespace BoomaEcommerce.Core.Exceptions
{
    public class PaymentFailureException : Exception
    {
        public PaymentFailureException() : base("Payment failed")
        {
            
        }
    }
}