using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;

namespace BoomaEcommerce.Services.DTO.ProductOffer
{
    public class ProductOfferDto : BaseEntityDto
    {
        public Guid UserGuid { get; set; }
        public ProductDto Product { get; set; }
        public ProductOfferStateDto State { get; set; }
        public decimal OfferPrice { get; set; }
        public decimal? CounterOfferPrice { get; set; }
        //public List<StoreOwnershipDto> ApprovedOwners { get; set; }
    }

    public enum ProductOfferStateDto
    {
        Pending,
        Approved,
        Declined,
        CounterOfferReceived,
        Error
    }
}
