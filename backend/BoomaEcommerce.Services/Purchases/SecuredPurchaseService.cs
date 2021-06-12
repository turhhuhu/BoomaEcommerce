using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Core.Exceptions;
using BoomaEcommerce.Services.Products;
using Microsoft.AspNetCore.Authorization;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Domain.ProductOffer;
using BoomaEcommerce.Services.DTO.ProductOffer;

namespace BoomaEcommerce.Services.Purchases
{
    public class SecuredPurchaseService : SecuredServiceBase, IPurchasesService
    {
        private readonly IPurchasesService _purchaseService;
        public SecuredPurchaseService(ClaimsPrincipal claimsPrincipal, IPurchasesService purchasesService) : base(claimsPrincipal)
        {
            _purchaseService = purchasesService;
        }

        public SecuredPurchaseService(IPurchasesService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        public static bool CreateSecuredPurchaseService(string token, string secret, IPurchasesService next, out IPurchasesService purchaseService)
        {
            try
            {
                var claimsPrincipal = ValidateToken(token, secret);
                purchaseService = new SecuredPurchaseService(claimsPrincipal, next);
                return true;
            }
            catch
            {
                purchaseService = null;
                return false;
            }
        }

        public Task<PurchaseDto> CreatePurchaseAsync(PurchaseDetailsDto purchaseDetailsDto)
        {
            //TODO: might need to change to a new general validator for purchaseDetailsDto instead of just PurchaseDto
            ServiceUtilities.ValidateDto<PurchaseDto, PurchaseServiceValidators.CreatePurchaseAsync>(purchaseDetailsDto.Purchase);
            
            // Visitor purchase
            if (purchaseDetailsDto.Purchase.BuyerGuid == default) return _purchaseService.CreatePurchaseAsync(purchaseDetailsDto);

            CheckAuthenticated();
            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();

            // Different user than the buyer trying to make the purchase. (only if registered)
            if (userGuidInClaims != purchaseDetailsDto.Purchase.BuyerGuid)
            {
                throw new UnAuthorizedException($"User {userGuidInClaims} found in claims does not match user {purchaseDetailsDto.Purchase.BuyerGuid} found in purchase.");
            }

            return _purchaseService.CreatePurchaseAsync(purchaseDetailsDto);
        }

        public Task<IReadOnlyCollection<PurchaseDto>> GetAllUserPurchaseHistoryAsync(Guid userGuid)
        {
            CheckAuthenticated();

            if (CheckRoleAuthorized(UserRoles.AdminRole))
            {
                return _purchaseService.GetAllUserPurchaseHistoryAsync(userGuid);
            }

            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();
            if (userGuidInClaims == userGuid)
            {
                return _purchaseService.GetAllUserPurchaseHistoryAsync(userGuid);
            }

            throw new UnAuthorizedException($"User {userGuidInClaims} found in claims does not match user {userGuid} provided to get history for.");
        }

        public Task<decimal> GetPurchaseFinalPrice(PurchaseDto purchaseDto)
        {
            ServiceUtilities.ValidateDto<PurchaseDto, PurchaseServiceValidators.CreatePurchaseAsync>(purchaseDto);
            return _purchaseService.GetPurchaseFinalPrice(purchaseDto);
        }

        public Task DeletePurchaseAsync(Guid purchaseGuid)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePurchaseAsync(PurchaseDto purchase)
        {
            throw new NotImplementedException();
        }

        /*
         * Product Offers
         */

        public Task<ProductOfferDto> CreateProductOffer(Guid userGuid, ProductDto product, decimal price)
        {
            CheckAuthenticated();
            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();
            if (userGuidInClaims == userGuid)
            {
                return _purchaseService.CreateProductOffer(userGuid, product, price);
            }
            throw new UnAuthorizedException($"User {userGuidInClaims} found in claims does not match user {userGuid} provided to create an offer.");
        }

        public Task<ProductOfferState> ApproveOffer(Guid ownerGuid, Guid productOfferGuid, Guid storeGuid)
        {
            CheckAuthenticated();
            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();
            if (userGuidInClaims == ownerGuid)
            {
                return _purchaseService.ApproveOffer(ownerGuid, productOfferGuid, storeGuid);
            }
            throw new UnAuthorizedException($"User {userGuidInClaims} found in claims does not match owner {ownerGuid} provided to approve an offer.");
        }

        public Task DeclineOffer(Guid ownerGuid, Guid productOfferGuid)
        {
            CheckAuthenticated();
            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();
            if (userGuidInClaims == ownerGuid)
            {
                return _purchaseService.DeclineOffer(ownerGuid, productOfferGuid);
            }
            throw new UnAuthorizedException($"User {userGuidInClaims} found in claims does not match owner {ownerGuid} provided to Decline an offer.");
        }

        public Task<ProductOfferDto> MakeCounterOffer(Guid ownerGuid, decimal counterOfferPrice, Guid offerGuid)
        {
            CheckAuthenticated();
            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();
            if (userGuidInClaims == ownerGuid)
            {
                return _purchaseService.MakeCounterOffer(ownerGuid, counterOfferPrice, offerGuid);
            }
            throw new UnAuthorizedException($"User {userGuidInClaims} found in claims does not match owner {ownerGuid} provided to make a counter offer.");
        }

        public Task<ProductOfferDto> GetProductOffer(Guid storeGuid, Guid userGuid, Guid offerGuid)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductOfferDto>> GetAllProductOffers(Guid storeGuid, Guid userGuid)
        {
            throw new NotImplementedException();
        }
    }
}
