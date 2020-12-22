using System.Threading.Tasks;
using MainDB.Entities;
using XTI_Core;

namespace XTI_App
{
    public sealed class Modifier
    {
        private readonly DataRepository<ModifierRecord> repo;
        private readonly ModifierRecord record;

        internal Modifier(DataRepository<ModifierRecord> repo, ModifierRecord record)
        {
            this.repo = repo;
            this.record = record ?? new ModifierRecord();
            ID = new EntityID(this.record.ID);
        }

        public EntityID ID { get; }
        public ModifierKey ModKey() => new ModifierKey(record.ModKey);
        public string TargetKey { get => record.TargetKey; }
        public string DisplayText { get => record.DisplayText; }
        public int TargetID() => int.Parse(TargetKey);

        public bool IsForCategory(ModifierCategory modCategory) => modCategory.ID.Value == record.CategoryID;

        public Task SetDisplayText(string displayText)
        {
            return repo.Update(record, r =>
            {
                r.DisplayText = displayText;
            });
        }

        public override string ToString() => $"{nameof(Modifier)} {ID.Value}";
    }
}
