using System;

namespace BoomaEcommerce.Core.Exceptions
{
    public class SupplyFailureException : Exception
    {
        public SupplyFailureException() : base("External payment system failure")
        {
            
        }
    }
}