namespace BoomaEcommerce.Domain.Policies
{
    public interface IStorePolicy
    {
        public bool CheckPolicy(IPurchaseType purchaseType);
    }
}