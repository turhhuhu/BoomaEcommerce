using System;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;

namespace BoomaEcommerce.Services.External
{
    public interface ISupplyClient
    {
        
        public Task<long> MakeOrder(Purchase purchase);

        public Task<int> CancelOrder(Guid purchaseGuid);
        
        public Task<string> HandShake();
    }
}