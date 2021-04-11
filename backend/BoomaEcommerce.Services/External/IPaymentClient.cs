using System.Threading.Tasks;
using BoomaEcommerce.Domain;

namespace BoomaEcommerce.Services.External
{
    public interface IPaymentClient
    {
        public Task MakeOrder(Purchase purchase);
    }
}