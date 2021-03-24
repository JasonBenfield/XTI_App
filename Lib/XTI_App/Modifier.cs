using MainDB.Entities;
using System.Threading.Tasks;
using XTI_App.Abstractions;

namespace XTI_App
{
    public sealed class Modifier
    {
        private readonly AppFactory factory;
        private readonly ModifierRecord record;

        internal Modifier(AppFactory factory, ModifierRecord record)
        {
            this.factory = factory;
            this.record = record ?? new ModifierRecord();
            ID = new EntityID(this.record.ID);
        }

        public EntityID ID { get; }
        public ModifierKey ModKey() => new ModifierKey(record.ModKey);
        public string TargetKey { get => record.TargetKey; }
        public int TargetID() => int.Parse(TargetKey);

        public bool IsForCategory(ModifierCategory modCategory) => modCategory.ID.Value == record.CategoryID;

        public Task SetDisplayText(string displayText)
        {
            return factory.DB.Modifiers.Update(record, r =>
            {
                r.DisplayText = displayText;
            });
        }

        public ModifierModel ToModel() => new ModifierModel
        {
            ID = ID.Value,
            CategoryID = record.CategoryID,
            ModKey = ModKey().DisplayText,
            TargetKey = TargetKey,
            DisplayText = record.DisplayText
        };

        public override string ToString() => $"{nameof(Modifier)} {ID.Value}";
    }
}
