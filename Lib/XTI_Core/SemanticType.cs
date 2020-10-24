using System;

namespace XTI_Core
{
    public abstract class SemanticType<T> : IEquatable<T>
    {
        public static implicit operator T(SemanticType<T> semanticType) =>
            semanticType != null ? semanticType.Value : default;

        public static bool Equals(SemanticType<T> a, SemanticType<T> b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }
            if ((a == null) || (b == null))
            {
                return false;
            }
            return a.Equals(b);
        }

        protected SemanticType(T value) : this(value, null)
        {
        }

        protected SemanticType(T value, string displayText)
        {
            Value = value;
            DisplayText = getDisplayText(value, displayText);
            hashCode = Value?.GetHashCode() ?? 0;
        }

        private static string getDisplayText(T value, string displayText)
        {
            if (displayText == null)
            {
                displayText = value as string;
                if (displayText == null)
                {
                    displayText = value?.ToString();
                }
            }
            return displayText;
        }

        private readonly int hashCode;

        public T Value { get; }
        public string DisplayText { get; }

        public bool IsBlank() => string.IsNullOrWhiteSpace(DisplayText);

        public override string ToString() => $"{GetType().Name} {Value}: {DisplayText}";

        public override bool Equals(object obj)
        {
            bool isEqual;
            if (obj == null)
            {
                isEqual = false;
            }
            else if (obj.GetType() == typeof(T))
            {
                isEqual = Equals((T)obj);
            }
            else if (GetType() == obj.GetType())
            {
                isEqual = _Equals((SemanticType<T>)obj);
            }
            else
            {
                isEqual = false;
            }
            return isEqual;
        }

        public override int GetHashCode() => hashCode;

        public bool Equals(T other) => _Equals(other);
        protected virtual bool _Equals(T other) => Equals(Value, other);

        protected bool _Equals(SemanticType<T> other)
        {
            if (other == null) { return false; }
            return Equals(Value, other.Value);
        }
    }
}
