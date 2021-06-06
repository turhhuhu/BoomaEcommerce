using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using Microsoft.AspNetCore.Identity;

namespace BoomaEcommerce.Data
{
    public interface IUserUnitOfWork
    {
        IRepository<ShoppingBasket> ShoppingBasketRepo { get; set; }
        IRepository<ShoppingCart> ShoppingCartRepo { get; set; }
        UserManager<User> UserManager { get; set; }
        Task SaveAsync();
        void AttachNoChange<TEntity>(TEntity entity)
            where TEntity : class;
    }
}