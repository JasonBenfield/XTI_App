namespace XTI_App.Api
{
    public sealed class FriendlyNameFromActionName
    {
        public FriendlyNameFromActionName(string actionName)
        {
            Value = actionName;
        }

        public string Value { get; }
    }
}
