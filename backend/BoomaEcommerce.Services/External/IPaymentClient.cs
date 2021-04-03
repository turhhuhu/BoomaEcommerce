using System.Threading.Tasks;
using BoomaEcommerce.Domain;

namespace BoomaEcommerce.Services.External
{
    public interface IPaymentClient
    {
        public Task<bool> Pay(Purchase purchase);
    }
}