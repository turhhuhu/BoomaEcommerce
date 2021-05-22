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
        private readonly INotificationPublisher _notificationPublisher;

        public PurchasesService(
            IMapper mapper,
            ILogger<PurchasesService> logger,
            IPaymentClient paymentClient,
            IPurchaseUnitOfWork purchaseUnitOfWork,
            ISupplyClient supplyClient,
            INotificationPublisher notificationPublisher)
        {
            _mapper = mapper;
            _logger = logger;
            _paymentClient = paymentClient;
            _purchaseUnitOfWork = purchaseUnitOfWork;
            _supplyClient = supplyClient;
            _notificationPublisher = notificationPublisher;
        }

        public async Task<PurchaseResult> CreatePurchaseAsync(PurchaseDto purchaseDto)
        {
            try
            {
                var purchase = _mapper.Map<Purchase>(purchaseDto);

                purchase.Buyer = await _purchaseUnitOfWork.UserRepository.FindByIdAsync(purchase.Buyer.Guid.ToString());

                var purchaseProducts = purchase.StorePurchases
                    .SelectMany(storePurchase =>
                        storePurchase.PurchaseProducts, (_, purchaseProduct) => purchaseProduct);

                var productTasks = purchaseProducts.Select(purchaseProduct =>
                    Task.Run(async () =>
                    {
                        var product = purchaseProduct.Product;
                        purchaseProduct.Product = await _purchaseUnitOfWork.ProductRepository.FindByIdAsync(product.Guid);
                    }));

                var storeTasks = purchase.StorePurchases.Select(storePurchase =>
                    Task.Run(async () =>
                    {
                        storePurchase.Store = await _purchaseUnitOfWork.StoresRepository.FindByIdAsync(storePurchase.Store.Guid);
                    }));

                await Task.WhenAll(productTasks.Concat(storeTasks));

                var purchaseResult = await purchase.MakePurchase();
                if (!purchaseResult.Success)
                {
                    return purchaseResult;
                }

                await _paymentClient.MakeOrder(purchase);

                await _purchaseUnitOfWork.PurchaseRepository.InsertOneAsync(purchase);

                if (purchase.Buyer is not null)
                {
                    await _purchaseUnitOfWork.ShoppingCartRepository
                        .DeleteOneAsync(cart =>
                            cart.User.Guid == purchase.Buyer.Guid);
                }

                await _supplyClient.NotifyOrder(purchase);
                await NotifyOnPurchase(purchase);
                await _purchaseUnitOfWork.SaveAsync();

                return PurchaseResult.Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to make purchase.");
                return PurchaseResult.Fail();
            }

        }

        private Task NotifyOnPurchase(Purchase purchaseDto)
        {
            return Task.WhenAll(purchaseDto.StorePurchases.Select(NotifyStoreSellersOnPurchase));
        }

        private async Task NotifyStoreSellersOnPurchase(StorePurchase storePurchase)
        {
            var ownerships =
                (await _purchaseUnitOfWork.StoreOwnershipRepository.FilterByAsync(ownership =>
                    ownership.Store.Guid == storePurchase.Store.Guid)).ToList();

            var notification = new StorePurchaseNotification(storePurchase);

            foreach (var ownership in ownerships)
            {
                ownership.User.Notifications.Add(notification);
            }
            var notifyTask = _notificationPublisher.NotifyManyAsync(_mapper.Map<StorePurchaseNotificationDto>(notification),
                ownerships.Select(ownership => ownership.User.Guid));

            await Task.WhenAll(_purchaseUnitOfWork.SaveAsync(), notifyTask);
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
