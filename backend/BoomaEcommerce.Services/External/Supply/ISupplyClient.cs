using System;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;

namespace BoomaEcommerce.Services.External.Supply
{
    public interface ISupplyClient
    {
        
        public Task<int> MakeOrder(SupplyDetailsDto supplyDetails);

        public Task<int> CancelOrder(int transactionId);
    }
}