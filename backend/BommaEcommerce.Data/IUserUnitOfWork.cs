using System.Threading.Tasks;
using BoomaEcommerce.Domain;

namespace BoomaEcommerce.Data
{
    public interface IUserUnitOfWork
    {
        IRepository<ShoppingBasket> ShoppingBasketRepo { get; set; }
        IRepository<ShoppingCart> ShoppingCartRepo { get; set; }
        IRepository<PurchaseProduct> PurchaseProductRepo { get; set; }
        Task SaveAsync();
    }
}