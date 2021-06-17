using System;
using System.Net.Http;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Services.DTO;
using BoomaEcommerce.Services.DTO.ProductOffer;
using BoomaEcommerce.Services.External.Payment;
using BoomaEcommerce.Services.Purchases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoomaEcommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchasesController : ControllerBase
    {
        private IPurchasesService _purchaseService;

        public PurchasesController(IPurchasesService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        [HttpPost]
        public async Task<IActionResult> PostPurchase([FromBody] PurchaseDetailsDto purchaseDetailsDto, [FromQuery]bool onlyReviewPrice = false)
        {
            if (onlyReviewPrice)
            {
                var totalPrice = await _purchaseService.GetPurchaseFinalPrice(purchaseDetailsDto.Purchase);
                return Ok(totalPrice);
            }

            var purchaseResult = await _purchaseService.CreatePurchaseAsync(purchaseDetailsDto);
            if (purchaseResult is null)
            {
                return BadRequest();
            }
            var locationUrl = $"{this.GetBaseUrl()}/purchases/{purchaseResult.Guid}";
            return Created(locationUrl, purchaseResult);
        }

    }
}