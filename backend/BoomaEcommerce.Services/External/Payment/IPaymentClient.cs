using System;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;

namespace BoomaEcommerce.Services.External
{
    public interface IPaymentClient
    {
        
        public Task<long> MakePayment(Purchase purchase);

        public Task<int> CancelPayment(Guid purchaseGuid);

        public Task<string> HandShake();
    }
}