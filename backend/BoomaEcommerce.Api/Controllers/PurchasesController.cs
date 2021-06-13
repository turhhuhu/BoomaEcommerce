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

        /*
        [Authorize]
        [HttpPost("{storeGuid}/offers")]
        public async Task<IActionResult> CreateProductOffer(Guid userGuid, ProductDto productDto, decimal price)
        {
            var createdOffer = await _purchaseService.CreateProductOffer(userGuid, productDto, price);


            if (createdOffer == null)
            {
                return NotFound();
            }
            var locationUrl = $"{this.GetBaseUrl()}/{createdOffer.Guid}";

            return Created(locationUrl, createdOffer);
        }

        [Authorize]
        [HttpGet("{storeGuid}/offers")]
        public async Task<IActionResult> GetProductOffers(Guid storeGuid, Guid userGuid)
        {
            var createdOffers = await _purchaseService.GetAllProductOffers(storeGuid, userGuid);

            if (createdOffers== null)
            {
                return NotFound();
            }
            var locationUrl = $"{this.GetBaseUrl()}/offers";

            return Created(locationUrl, createdOffers);
        }

        [Authorize]
        [HttpGet("{storeGuid}/offers/{offerGuid}")]
        public async Task<IActionResult> GetProductOffer(Guid storeGuid, Guid userGuid, Guid offerGuid)
        {
            var createdOffer = await _purchaseService.GetProductOffer(storeGuid, userGuid, offerGuid);

            if (createdOffer == null)
            {
                return NotFound();
            }
            var locationUrl = $"{this.GetBaseUrl()}/offers";

            return Created(locationUrl, createdOffer);
        }
        */
        //Todo Controller for Update - UPDATE {storeGuid}/offers (approve / decline / counter offer)

    }
}