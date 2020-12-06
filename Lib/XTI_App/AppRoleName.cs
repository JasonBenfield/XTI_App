using System;
using System.Text.RegularExpressions;
using XTI_Core;

namespace XTI_App
{
    public sealed class AppRoleName : TextValue, IEquatable<AppRoleName>
    {
        public AppRoleName(string displayText)
            : base(whitespaceRegex.Replace(displayText?.Trim().ToLower() ?? "", "_"), displayText?.Trim() ?? "")
        {
            if (string.IsNullOrWhiteSpace(displayText))
            {
                throw new ArgumentException($"{nameof(displayText)} is required");
            }
        }

        private static readonly Regex whitespaceRegex = new Regex("\\s+");

        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();
        public bool Equals(AppRoleName other) => _Equals(other);
    }
}
