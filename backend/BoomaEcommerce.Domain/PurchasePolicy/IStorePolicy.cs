namespace BoomaEcommerce.Domain.PurchasePolicy
{
    public interface IStorePolicy
    {
        public bool CheckPolicy(IPurchaseType purchaseType);
    }
}