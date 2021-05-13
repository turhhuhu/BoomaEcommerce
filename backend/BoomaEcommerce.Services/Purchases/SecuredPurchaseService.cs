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

        public Task<bool> CreatePurchaseAsync(PurchaseDto purchase)
        {
            ServiceUtilities.ValidateDto<PurchaseDto, PurchaseServiceValidators.CreatePurchaseAsync>(purchase);
            
            // Visitor purchase
            if (purchase.BuyerGuid == default) return _purchaseService.CreatePurchaseAsync(purchase);

            CheckAuthenticated();
            var userGuidInClaims = ClaimsPrincipal.GetUserGuid();

            // Different user than the buyer trying to make the purchase. (only if registered)
            if (userGuidInClaims != purchase.BuyerGuid)
            {
                throw new UnAuthorizedException($"User {userGuidInClaims} found in claims does not match user {purchase.BuyerGuid} found in purchase.");
            }

            return _purchaseService.CreatePurchaseAsync(purchase);
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
