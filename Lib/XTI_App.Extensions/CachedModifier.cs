using XTI_App.Abstractions;

namespace XTI_App.Extensions
{
    internal sealed class CachedModifier : IModifier
    {
        private readonly ModifierKey modKey;

        public CachedModifier(IModifier source)
        {
            ID = source.ID;
            modKey = source.ModKey();
        }

        public EntityID ID { get; }

        public ModifierKey ModKey() => modKey;
    }
}
