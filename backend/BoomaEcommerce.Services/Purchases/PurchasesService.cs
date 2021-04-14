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
        private readonly ISupplyClient _supplyClient;
        private readonly IPurchaseUnitOfWork _purchaseUnitOfWork;

        public PurchasesService(
            IMapper mapper,
            ILogger<PurchasesService> logger,
            IPaymentClient paymentClient,
            IPurchaseUnitOfWork purchaseUnitOfWork,
            ISupplyClient supplyClient)
        {
            _mapper = mapper;
            _logger = logger;
            _paymentClient = paymentClient;
            _purchaseUnitOfWork = purchaseUnitOfWork;
            _supplyClient = supplyClient;
        }

        public async Task<bool> CreatePurchaseAsync(PurchaseDto purchaseDto)
        {
            try
            {
                var purchase = _mapper.Map<Purchase>(purchaseDto);
                
                purchase.Buyer = await _purchaseUnitOfWork.UserRepository.FindByIdAsync(purchase.Buyer.Guid.ToString());
                
                var purchaseProducts = purchase.StorePurchases
                    .SelectMany(storePurchase =>
                        storePurchase.PurchaseProducts, (_, purchaseProduct) => purchaseProduct);

                var taskList = purchaseProducts.Select(purchaseProduct => 
                    Task.Run(async () =>
                {
                    var product = purchaseProduct.Product;
                    purchaseProduct.Product = await _purchaseUnitOfWork.ProductRepository.FindByIdAsync(product.Guid);
                }));

                await Task.WhenAll(taskList);

                if (!await purchase.MakePurchase())
                {
                    return false;
                }

                await _paymentClient.MakeOrder(purchase);

                await _purchaseUnitOfWork.PurchaseRepository.InsertOneAsync(purchase);

                await _purchaseUnitOfWork.ShoppingCartRepository
                    .DeleteOneAsync(cart =>
                    cart.User.Guid == purchase.Buyer.Guid);

                await _supplyClient.NotifyOrder(purchase);

                await _purchaseUnitOfWork.SaveAsync();


                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to make purchase.");
                return false;
            }

        }

        public async Task<IReadOnlyCollection<PurchaseDto>> GetAllUserPurchaseHistoryAsync(Guid userGuid)
        {
            try
            {
                var purchaseHistory =
                    await _purchaseUnitOfWork.PurchaseRepository.FilterByAsync(purchase =>
                            purchase.Buyer.Guid == userGuid);
                var purchaseHistoryList = purchaseHistory.ToList();
                var purchaseHistoryDtoList = _mapper.Map<List<PurchaseDto>>(purchaseHistoryList);
                return purchaseHistoryDtoList;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "The following error occurred during when trying to get user purchase history for user: {userGuid}", userGuid);
                return null;
            }
            
        }

        // function not supported yet.
        public Task<PurchaseDto> GetPurchaseAsync(Guid purchaseGuid)
        {
            throw new NotSupportedException();
        }

        // function not supported yet.
        public Task DeletePurchaseAsync(Guid purchaseGuid)
        {
            throw new NotSupportedException();
        }

        // function not supported yet.
        public Task UpdatePurchaseAsync(PurchaseDto purchase)
        {
            throw new NotSupportedException();
        }
    }
}
