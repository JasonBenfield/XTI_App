using MainDB.Entities;

namespace XTI_App
{
    public sealed class AppUserModifier
    {
        private readonly AppUserModifierRecord record;

        public AppUserModifier(AppUserModifierRecord record)
        {
            this.record = record ?? new AppUserModifierRecord();
        }

        public override string ToString() => $"{nameof(AppUserModifier)} {record.ID}";
    }
}
