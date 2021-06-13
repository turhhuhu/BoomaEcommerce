using System.Threading.Tasks;
using BoomaEcommerce.Core;
using BoomaEcommerce.Domain;
using BoomaEcommerce.Domain.ProductOffer;
using Microsoft.AspNetCore.Identity;

namespace BoomaEcommerce.Data
{
    public interface IUserUnitOfWork
    {
        IRepository<ShoppingBasket> ShoppingBasketRepo { get; set; }
        IRepository<ShoppingCart> ShoppingCartRepo { get; set; }
        IRepository<ProductOffer> ProductOfferRepo { get; set; }
        IRepository<User> UserRepository { get; set; }
        IRepository<Product> ProductRepository { get; set; }

        IRepository<ApproverOwner> ApproversRepo { get; set; }
        Task SaveAsync();

        void AttachNoChange<TEntity>(TEntity entity)
            where TEntity : class, IBaseEntity;

        public void Attach<TEntity>(TEntity entity) 
            where TEntity : class, IBaseEntity;
    }
}