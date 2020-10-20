namespace XTI_App
{
    public struct EntityID
    {
        public EntityID(int value)
        {
            Value = value;
        }

        public int Value { get; }

        public bool IsValid() => Value > 0;
        public bool IsNotValid() => !IsValid();

        public override string ToString() => $"{nameof(EntityID)} {Value}";
    }
}
