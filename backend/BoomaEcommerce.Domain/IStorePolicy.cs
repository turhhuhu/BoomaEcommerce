namespace BoomaEcommerce.Domain
{
    public interface IStorePolicy
    {
        public bool CheckPolicy(IPurchaseType purchaseType);
    }
}