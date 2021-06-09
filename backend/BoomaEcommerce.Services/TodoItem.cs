namespace BoomaEcommerce.Services
{
    public class TodoItem
    {
        protected string action_type { get; set; }

        public TodoItem(string actionType)
        {
            action_type = actionType;
        }
    }
}