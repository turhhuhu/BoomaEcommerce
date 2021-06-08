using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BoomaEcommerce.Services.DTO.Policies;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BoomaEcommerce.Services.DTO.Discounts
{
    public abstract class DiscountDto : BaseEntityDto
    {
        public abstract DiscountType Type { get; set; }
        public  int Percentage { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public PolicyDto Policy { get; set; }
        public string Description { get; set; }
    }
    public class ProductDiscountDto : DiscountDto
    {
        [JsonRequired] public override DiscountType Type { get; set; } = DiscountType.Product;

        public Guid ProductGuid { get; set; }
    }
    public class CategoryDiscountDto : DiscountDto
    {
        [JsonRequired] public override DiscountType Type { get; set; } = DiscountType.Category;

        public string Category { get; set; }
    }
    public class BasketDiscountDto : DiscountDto
    {
        [JsonRequired] public override DiscountType Type { get; set; } = DiscountType.Basket;
    }
    public class CompositeDiscountDto : DiscountDto
    {
        [JsonRequired] public override DiscountType Type { get; set; } = DiscountType.Composite;

        [JsonRequired]
        public OperatorTypeDiscount Operator { get; set; }

        public IEnumerable<DiscountDto> Discounts { get; set; }
    }
    public enum OperatorTypeDiscount
    {
        Max,
        Sum
    }

    public enum DiscountType
    {
        Composite,
        Basket,
        Category,
        Product
    }
}