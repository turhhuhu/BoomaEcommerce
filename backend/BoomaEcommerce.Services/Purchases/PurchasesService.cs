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
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Purchase> _purchaseRepository;
        public PurchasesService(IMapper mapper, ILogger<PurchasesService> logger,
            IPaymentClient paymentClient, IRepository<User> userRepository, IRepository<Product> productRepository,
            IRepository<Purchase> purchaseRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _paymentClient = paymentClient;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _purchaseRepository = purchaseRepository;
        }
        public async Task CreatePurchaseAsync(PurchaseDto purchaseDto)
        {
            // TODO: might need to validate purchaseDto
            var purchase = _mapper.Map<Purchase>(purchaseDto);
            purchase.Buyer = await _userRepository.FindByIdAsync(purchase.Buyer.Guid);
            purchase.ProductsPurchases
                .ForEach(async x => x.Product = await _productRepository.FindByIdAsync(x.Product.Guid));
            await purchase.MakePurchase();
            await _paymentClient.Pay(purchase);
            await _purchaseRepository.InsertOneAsync(purchase);
        }

        public Task<IReadOnlyCollection<PurchaseDto>> GetAllUserPurchaseHistoryAsync(string userId)
        {
            throw new NotImplementedException();
        }

        async public Task<IReadOnlyCollection<PurchaseDto>> GetUserPurchaseHistoryAsync(string userId)
        {
            try
            {
                var purchaseHistory =
                    await _purchaseRepository.FilterByAsync(purchase => purchase.Buyer.Guid.Equals(userId));
                var purchaseHistoryDtoList = _mapper.Map<List<PurchaseDto>>(purchaseHistory);
                return purchaseHistoryDtoList;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
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
