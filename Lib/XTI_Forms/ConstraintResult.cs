namespace XTI_Forms
{
    public sealed class ConstraintResult
    {
        public static ConstraintResult Passed() => new ConstraintResult(true, "");
        public static ConstraintResult Failed(string errorMessage) => new ConstraintResult(false, errorMessage);

        private ConstraintResult(bool isValid, string errorMessage)
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
        }

        public bool IsValid { get; }
        public string ErrorMessage { get; }
    }
}
