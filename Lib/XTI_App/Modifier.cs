using MainDB.Entities;
using System.Linq;
using System.Threading.Tasks;
using XTI_App.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace XTI_App
{
    public sealed class Modifier : IModifier
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

        async Task<IApp> IModifier.App() => await App();

        public Task<App> App()
        {
            var appIDs = factory.DB
                .ModifierCategories
                .Retrieve()
                .Where(modCat => modCat.ID == record.CategoryID)
                .Select(modCat => modCat.AppID);
            return factory.DB
                .Apps
                .Retrieve()
                .Where(a => appIDs.Any(id => a.ID == id))
                .Select(a => factory.App(a))
                .FirstAsync();
        }

        async Task<IModifier> IModifier.DefaultModifier() => await DefaultModifier();

        public async Task<Modifier> DefaultModifier()
        {
            Modifier defaultModifier;
            if (ModKey().Equals(ModifierKey.Default))
            {
                defaultModifier = this;
            }
            else
            {
                var appID = factory.DB
                    .ModifierCategories
                    .Retrieve()
                    .Where(modCat => modCat.ID == record.CategoryID)
                    .Select(modCat => modCat.AppID);
                var defaultCategoryID = factory.DB
                    .ModifierCategories
                    .Retrieve()
                    .Where(modCat => appID.Any(id => modCat.AppID == id))
                    .Select(modCat => modCat.ID);
                var defaultModifierRecord = await factory.DB
                    .Modifiers
                    .Retrieve()
                    .Where(m => defaultCategoryID.Any(id => id == m.CategoryID) && m.ModKey == ModifierKey.Default.Value)
                    .FirstOrDefaultAsync();
                defaultModifier = factory.Modifier(defaultModifierRecord);
            }
            return defaultModifier;
        }

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
