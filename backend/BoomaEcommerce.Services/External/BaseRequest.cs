namespace BoomaEcommerce.Services.External
{
    public class BaseRequest
    {
        protected string action_type { get; set; }

        public BaseRequest(string actionType)
        {
            action_type = actionType;
        }
    }
}