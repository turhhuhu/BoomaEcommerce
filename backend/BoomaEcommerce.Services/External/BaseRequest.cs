namespace BoomaEcommerce.Services.External
{
    public class BaseRequest
    {
        public string action_type { get; set; }

        public BaseRequest(string actionType)
        {
            action_type = actionType;
        }
    }
}