using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BoomaEcommerce.Core;
using BoomaEcommerce.Core.Exceptions;
using BoomaEcommerce.Data;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Domain.ProductOffer;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.DTO.ProductOffer;
using BoomaEcommerce.Services.External;
using BoomaEcommerce.Services.External.Payment;
using BoomaEcommerce.Services.External.Supply;
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

        public async Task<PurchaseDto> CreatePurchaseAsync(PurchaseDetailsDto purchaseDetailsDto)
        {
            int? paymentTransactionId = null;
            int? supplyTransactionId = null;

            try
            {
                await using var transaction = await _purchaseUnitOfWork.BeginTransaction();
                var purchase = _mapper.Map<Purchase>(purchaseDetailsDto.Purchase);

                await CreateOrGetBuyerInfo(purchase, purchaseDetailsDto.Purchase.UserBuyerGuid);
                var purchaseProducts = purchase.StorePurchases
                    .SelectMany(storePurchase =>
                        storePurchase.PurchaseProducts, (_, purchaseProduct) => purchaseProduct);


                await purchaseProducts.Select(async purchaseProduct =>
                {
                    var product = purchaseProduct.Product;
                    purchaseProduct.Product = await _purchaseUnitOfWork.ProductRepository.FindByIdAsync(product.Guid);
                }).WhenAllAwaitEach();

                await purchase.StorePurchases.Select(async storePurchase =>
                {
                    storePurchase.Store =
                        await _purchaseUnitOfWork.StoresRepository.FindByIdAsync(storePurchase.Store.Guid);
                    storePurchase.Buyer = purchase.Buyer;
                }).WhenAllAwaitEach();

                var purchaseResult = await purchase.MakePurchase();
                if (purchaseResult.IsPolicyFailure)
                {
                    throw new PolicyValidationException(purchaseResult.Errors);
                }

                if (!purchaseResult.Success)
                {
                    return null;
                }

                paymentTransactionId = await _paymentClient.MakePayment(purchaseDetailsDto.PaymentDetails);

                await _purchaseUnitOfWork.PurchaseRepository.InsertOneAsync(purchase);

                if (purchaseDetailsDto.Purchase.UserBuyerGuid.HasValue)
                {
                    await _purchaseUnitOfWork.ShoppingCartRepository
                        .DeleteOneAsync(cart =>
                            cart.User.Guid == purchaseDetailsDto.Purchase.UserBuyerGuid);
                }

                if (purchaseDetailsDto.SupplyDetails is not null)
                {
                    supplyTransactionId = await _supplyClient.MakeOrder(purchaseDetailsDto.SupplyDetails);
                }

                await NotifyOnPurchase(purchase);
                await _purchaseUnitOfWork.SaveAsync();

                await transaction.CommitAsync();

                return _mapper.Map<PurchaseDto>(purchase);
            }
            catch (PolicyValidationException e)
            {
                _logger.LogError(e, "Store policies for purchase failed.");
                throw;
            }
            catch (PaymentFailureException e)
            {
                RollbackTransactions(paymentTransactionId, supplyTransactionId);
                _logger.LogError(e, "External payment system failure");
                throw;
            }
            catch (SupplyFailureException e)
            {
                RollbackTransactions(paymentTransactionId, supplyTransactionId);
                _logger.LogError(e, "External supply system failure");
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to make purchase.");
                return null;
            }

        }

        private async Task CreateOrGetBuyerInfo(Purchase purchase, Guid? userBuyerGuid)
        {
            if (userBuyerGuid.HasValue)
            {
                purchase.Buyer = await _purchaseUnitOfWork.UserRepository.FindByIdAsync(userBuyerGuid.ToString());
            }
            else
            {
                purchase.Buyer.UserName = "GUEST" +  Guid.NewGuid();
                var res = await _purchaseUnitOfWork.UserRepository.CreateAsync(purchase.Buyer, Guid.NewGuid().ToString());
                if (!res.Succeeded)
                {
                    throw new UserCreationException("Failed to create guest user for the purchase.");
                }

                purchase.Buyer = await _purchaseUnitOfWork.UserRepository.FindByNameAsync(purchase.Buyer.UserName);
            }
        }


        private void RollbackTransactions(int? paymentTransactionId, int? supplyTransactionId)
        {
            if (paymentTransactionId.HasValue)
            {
                _paymentClient.CancelPayment(paymentTransactionId.Value);
            }

            if (supplyTransactionId.HasValue)
            {
                _supplyClient.CancelOrder(supplyTransactionId.Value);
            }
        }

        private async Task NotifyOnPurchase(Purchase purchaseDto)
        {
            foreach (var storePurchase in purchaseDto.StorePurchases)
            {
                await NotifyStoreSellersOnPurchase(storePurchase);
            }
        }


        private async Task NotifyStoreSellersOnPurchase(StorePurchase storePurchase)
        {
            var ownerships =
                (await _purchaseUnitOfWork.StoreOwnershipRepository.FilterByAsync(ownership =>
                    ownership.Store.Guid == storePurchase.Store.Guid)).ToList();


            var notifications = new List<(Guid, NotificationDto)>();
            foreach (var ownership in ownerships)
            {
                var notification = new StorePurchaseNotification(storePurchase.Buyer, storePurchase.Guid, storePurchase.Store);

                ownership.User.Notifications.Add(notification);
                notifications.Add((ownership.User.Guid, _mapper.Map<StorePurchaseNotificationDto>(notification)));
                _purchaseUnitOfWork.Attach(notification);
            }

            var notifyTask =
                _notificationPublisher.NotifyManyAsync(notifications);

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

        public async Task<decimal> GetPurchaseFinalPrice(PurchaseDto purchaseDto)
        {
            var purchase = _mapper.Map<Purchase>(purchaseDto);
            var purchaseProducts = purchase.StorePurchases
                .SelectMany(storePurchase =>
                    storePurchase.PurchaseProducts, (_, purchaseProduct) => purchaseProduct);


            await purchaseProducts.Select(async purchaseProduct =>
            {
                var product = purchaseProduct.Product;
                purchaseProduct.Product = await _purchaseUnitOfWork.ProductRepository.FindByIdAsync(product.Guid);
            }).WhenAllAwaitEach();

            await purchase.StorePurchases.Select(async storePurchase =>
            {
                storePurchase.Store = await _purchaseUnitOfWork.StoresRepository.FindByIdAsync(storePurchase.Store.Guid);
                storePurchase.Buyer = purchase.Buyer;
            }).WhenAllAwaitEach();
            var purchaseResult = purchase.CalculatePurchaseFinalPrice();
            if (purchaseResult.IsPolicyFailure)
            {
                throw new PolicyValidationException(purchaseResult.Errors);
            }
            return purchaseResult.Price;
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
