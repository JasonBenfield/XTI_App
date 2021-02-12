using System;

namespace XTI_App.Abstractions
{
    public struct EntityID : IEquatable<EntityID>, IEquatable<int>
    {
        public EntityID(int value)
        {
            Value = value;
        }

        public int Value { get; }

        public bool IsValid() => Value > 0;
        public bool IsNotValid() => !IsValid();

        public override bool Equals(object obj)
        {
            if (obj is EntityID entityID)
            {
                return Equals(entityID);
            }
            if (obj is int id)
            {
                return Equals(id);
            }
            return base.Equals(obj);
        }
        public bool Equals(EntityID other) => Equals(other.Value);
        public bool Equals(int other) => Value == other;
        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => $"{nameof(EntityID)} {Value}";


    }
}
