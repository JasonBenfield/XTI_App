using System;

namespace XTI_App.Api
{
    public sealed class ErrorModel : IEquatable<ErrorModel>
    {
        public ErrorModel(string message, string source = null)
        {
            Message = message;
            Source = source;
            value = $"{Message}|{Source}";
            hashCode = value.GetHashCode();
        }

        private readonly string value;
        private readonly int hashCode;

        public string Message { get; }
        public string Source { get; }

        public override bool Equals(object obj) => Equals(obj as ErrorModel);
        public bool Equals(ErrorModel other) => value == other?.value;
        public override int GetHashCode() => hashCode;

        public override string ToString() => $"{nameof(ErrorModel)} {value}";
    }
}
