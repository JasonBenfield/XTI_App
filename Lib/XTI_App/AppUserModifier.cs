using MainDB.Entities;
using System.Threading.Tasks;

namespace XTI_App
{
    public sealed class AppUserModifier
    {
        private readonly AppFactory factory;
        private readonly AppUserModifierRecord record;

        public AppUserModifier(AppFactory factory, AppUserModifierRecord record)
        {
            this.factory = factory;
            this.record = record ?? new AppUserModifierRecord();
        }

        internal Task<Modifier> Modifier() => factory.Modifiers().Modifier(record.ModifierID);

        public override string ToString() => $"{nameof(AppUserModifier)} {record.ID}";
    }
}
