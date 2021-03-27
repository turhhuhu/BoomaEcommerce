using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;

namespace BoomaEcommerce.Services.Purchases
{
    public class PurchasesService : IPurchasesService
    {
        public Task CreatePurchaseAsync(PurchaseDto purchase)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<PurchaseDto>> GetAllUserPurchaseHistoryAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<PurchaseDto>> GetUserPurchaseHistoryAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<PurchaseDto>> GetStorePurchaseHistoryAsync(Guid storeGuid)
        {
            throw new NotImplementedException();
        }

        public Task<PurchaseDto> GetPurchaseAsync(Guid purchaseGuid)
        {
            throw new NotImplementedException();
        }

        public Task DeletePurchaseAsync(Guid purchaseGuid)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePurchaseAsync(PurchaseDto purchase)
        {
            throw new NotImplementedException();
        }
    }
}
