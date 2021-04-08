using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Data;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.External;
using Microsoft.Extensions.Logging;

namespace BoomaEcommerce.Services.Purchases
{
    public class PurchasesService : IPurchasesService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<PurchasesService> _logger;
        private readonly IPaymentClient _paymentClient;
        private readonly IPurchaseUnitOfWork _purchaseUnitOfWork;
        public PurchasesService(IMapper mapper, ILogger<PurchasesService> logger,
            IPaymentClient paymentClient, IPurchaseUnitOfWork purchaseUnitOfWork)
        {
            _mapper = mapper;
            _logger = logger;
            _paymentClient = paymentClient;
            _purchaseUnitOfWork = purchaseUnitOfWork;
        }
        public async Task CreatePurchaseAsync(PurchaseDto purchaseDto)
        {
            try
            {
                // TODO: might need to validate purchaseDto
                var purchase = _mapper.Map<Purchase>(purchaseDto);
                
                purchase.Buyer = await _purchaseUnitOfWork.UserRepository.FindByIdAsync(purchase.Buyer.Guid);
                
                var purchaseProducts = purchase.StorePurchases
                    .SelectMany(x =>
                        x.ProductsPurchases, (_, purchaseProduct) => purchaseProduct);
                
                foreach (var purchaseProduct in purchaseProducts)
                {
                    var product = purchaseProduct.Product;
                    purchaseProduct.Product = await _purchaseUnitOfWork.ProductRepository.FindByIdAsync(product.Guid);
                }

                if (!await purchase.MakePurchase())
                {
                    //TODO: CancelTransaction
                }

                await _paymentClient.MakeOrder(purchase);
                await _purchaseUnitOfWork.PurchaseRepository.InsertOneAsync(purchase);
                _purchaseUnitOfWork.Save();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public Task<IReadOnlyCollection<PurchaseDto>> GetAllUserPurchaseHistoryAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyCollection<PurchaseDto>> GetUserPurchaseHistoryAsync(Guid userId)
        {
            try
            {
                var purchaseHistory =
                    await _purchaseUnitOfWork.PurchaseRepository.FilterByAsync(purchase => purchase.Buyer.Guid == userId);
                var purchaseHistoryDtoList = _mapper.Map<List<PurchaseDto>>(purchaseHistory);
                return purchaseHistoryDtoList;
            }
            catch (Exception e)
            {
                _logger.LogError("the following error occured during \"Get purchase history method\": {Error}", e.Message);
                return null;
            }
            
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
