using System.Threading.Tasks;
using BoomaEcommerce.Domain;

namespace BoomaEcommerce.Services.External
{
    public interface ISupplyClient
    {
        Task NotifyOrder(Purchase purchase);
    }
}